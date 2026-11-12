using System;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.NavigationSystem;
using IndianBikeLife.Progression.Rewards;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    /// <summary>
    /// Executes an active mission instance, integrates timer and progress tracker,
    /// evaluates success/failure states, and communicates with Navigation and Passenger systems.
    /// </summary>
    public class MissionController : MonoBehaviour
    {
        private const string Tag = "MissionController";

        [SerializeField] private Transform _playerMotorcycle;

        private MissionData _activeMission;
        private MissionDifficulty _difficulty = MissionDifficulty.Normal;
        private MissionDifficultySettings _difficultySettings;

        private MissionTimer _timer;
        private MissionProgressTracker _progressTracker;
        private MissionResultSummary _lastResult;

        public event Action<MissionResultSummary> OnMissionFinished;

        public MissionData ActiveMission => _activeMission;
        public MissionDifficulty Difficulty => _difficulty;
        public MissionTimer Timer => _timer;
        public MissionProgressTracker Tracker => _progressTracker;
        public MissionResultSummary LastResult => _lastResult;

        public void SetPlayerTarget(Transform target)
        {
            _playerMotorcycle = target;
        }

        public bool StartMission(MissionData missionAsset, MissionDifficulty difficulty = MissionDifficulty.Normal)
        {
            if (missionAsset == null)
            {
                GameLogger.E(Tag, "Cannot start null mission!");
                return false;
            }

            // Clean up any previously running mission
            if (_activeMission != null && _activeMission.Status == MissionStatus.Active)
            {
                CancelMission();
            }

            _activeMission = missionAsset.CloneRuntimeInstance();
            _difficulty = difficulty;
            _difficultySettings = MissionDifficultySettings.GetDefaultSettings(difficulty);

            Vector3 startPos = _playerMotorcycle != null ? _playerMotorcycle.position : _activeMission.StartPosition;

            // 1. Setup Timer
            _timer = new MissionTimer();
            float effectiveTime = _activeMission.TimeLimitSeconds > 0 
                ? _activeMission.TimeLimitSeconds * _difficultySettings.timeLimitMultiplier 
                : 0f;
            _timer.Start(effectiveTime);
            _timer.OnTimerExpired += HandleTimeExpired;

            // 2. Setup Progress Tracker
            _progressTracker = new MissionProgressTracker();
            _progressTracker.Initialize(_activeMission, _difficultySettings, startPos);
            _progressTracker.OnFailureConditionMet += HandleFailure;
            _progressTracker.OnAllRequiredObjectivesCompleted += HandleSuccess;

            _activeMission.Status = MissionStatus.Active;

            // 3. Connect to NavigationManager if active
            if (ServiceLocator.TryGet<INavigationManager>(out var navManager))
            {
                navManager.GenerateRoute(startPos, _activeMission.DestinationPosition, _activeMission.DestinationName);
            }

            GameLogger.I(Tag, $"Started Mission '{_activeMission.Title}' [{_activeMission.Category}] at difficulty {_difficulty}.");
            EventBus.Publish(new MissionStartedEvent(_activeMission, _difficulty));
            return true;
        }

        public void PauseMission()
        {
            if (_activeMission != null && _activeMission.Status == MissionStatus.Active)
            {
                _activeMission.Status = MissionStatus.Paused;
                _timer?.Pause();
                GameLogger.I(Tag, $"Mission '{_activeMission.Title}' paused.");
            }
        }

        public void ResumeMission()
        {
            if (_activeMission != null && _activeMission.Status == MissionStatus.Paused)
            {
                _activeMission.Status = MissionStatus.Active;
                _timer?.Resume();
                GameLogger.I(Tag, $"Mission '{_activeMission.Title}' resumed.");
            }
        }

        public void CompleteMission()
        {
            if (_activeMission == null || _activeMission.Status != MissionStatus.Active) return;

            _timer?.Stop();
            _activeMission.Status = MissionStatus.Completed;
            _activeMission.RuntimeElapsedTime = _timer?.ElapsedSeconds ?? 0f;

            _lastResult = MissionResultController.CalculateSuccessResult(
                _activeMission,
                _difficulty,
                _activeMission.RuntimeElapsedTime,
                _progressTracker?.DistanceCoveredMeters ?? 0f,
                _progressTracker?.CurrentDamagePercent ?? 0f
            );

            // Grant rewards
            if (ServiceLocator.TryGet<RewardManager>(out var rewardManager) && _lastResult.isSuccess)
            {
                var earnedReward = RewardData.CreateRuntimeReward(
                    $"mission_{_activeMission.MissionId}",
                    _lastResult.earnedSilver,
                    _lastResult.earnedGold,
                    _lastResult.earnedXP
                );
                rewardManager.ClaimReward(earnedReward, enforceSingleClaim: false);
            }

            GameLogger.I(Tag, $"🎉 MISSION COMPLETED: '{_activeMission.Title}'! Stars: {_lastResult.starRating}/3, Time: {_lastResult.timeTakenSeconds:0.0}s, Silver: +{_lastResult.earnedSilver}, XP: +{_lastResult.earnedXP}");

            EventBus.Publish(new MissionCompletedEvent(_activeMission, _lastResult.timeTakenSeconds, _activeMission.Reward));
            OnMissionFinished?.Invoke(_lastResult);
        }

        public void FailMission(string failureReason)
        {
            if (_activeMission == null || _activeMission.Status != MissionStatus.Active) return;

            _timer?.Stop();
            _activeMission.Status = MissionStatus.Failed;
            _activeMission.RuntimeElapsedTime = _timer?.ElapsedSeconds ?? 0f;

            _lastResult = MissionResultController.CalculateFailureResult(
                _activeMission,
                _activeMission.RuntimeElapsedTime,
                _progressTracker?.DistanceCoveredMeters ?? 0f,
                _progressTracker?.CurrentDamagePercent ?? 0f,
                failureReason
            );

            GameLogger.W(Tag, $"❌ MISSION FAILED: '{_activeMission.Title}'! Reason: {failureReason}");

            EventBus.Publish(new MissionFailedEvent(_activeMission, failureReason));
            OnMissionFinished?.Invoke(_lastResult);
        }

        public void RestartMission()
        {
            if (_activeMission == null) return;

            var missionAsset = _activeMission;
            var difficulty = _difficulty;

            CancelMission();
            StartMission(missionAsset, difficulty);

            EventBus.Publish(new MissionRestartedEvent(_activeMission));
            GameLogger.I(Tag, $"Mission '{_activeMission.Title}' restarted.");
        }

        public void CancelMission()
        {
            if (_activeMission == null) return;

            _timer?.Stop();
            _activeMission.Status = MissionStatus.Cancelled;

            if (ServiceLocator.TryGet<INavigationManager>(out var navManager))
            {
                navManager.CancelRoute();
            }

            EventBus.Publish(new MissionCancelledEvent(_activeMission));
            GameLogger.I(Tag, $"Mission '{_activeMission.Title}' cancelled.");
            _activeMission = null;
        }

        private void HandleTimeExpired()
        {
            FailMission("Time expired! You were unable to reach all objectives within the allotted time.");
        }

        private void HandleFailure(string reason)
        {
            FailMission(reason);
        }

        private void HandleSuccess()
        {
            CompleteMission();
        }

        private void Update()
        {
            if (_activeMission == null || _activeMission.Status != MissionStatus.Active) return;

            float dt = Time.deltaTime;
            _timer?.Update(dt);

            Vector3 pos = _playerMotorcycle != null ? _playerMotorcycle.position : transform.position;
            _progressTracker?.Update(pos, dt, isOffRoute: false);
        }
    }
}

