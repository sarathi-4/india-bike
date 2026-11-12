using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    /// <summary>
    /// Tracks active mission state, evaluates checkpoint proximity, monitors route adherence,
    /// and updates individual objective completion states.
    /// </summary>
    public class MissionProgressTracker
    {
        private const string Tag = "MissionProgressTracker";

        private MissionData _activeMission;
        private MissionDifficultySettings _difficultySettings;

        private int _currentCheckpointIndex = 0;
        private float _offRouteTimer = 0f;
        private float _currentMotorcycleDamage = 0f;
        private float _distanceCoveredMeters = 0f;
        private Vector3 _lastPlayerPosition;

        public event Action<int, int> OnCheckpointPassed; // current, total
        public event Action<string> OnFailureConditionMet;
        public event Action OnAllRequiredObjectivesCompleted;

        public int CurrentCheckpointIndex => _currentCheckpointIndex;
        public int TotalCheckpoints => _activeMission?.Checkpoints?.Count ?? 0;
        public float DistanceCoveredMeters => _distanceCoveredMeters;
        public float CurrentDamagePercent => _currentMotorcycleDamage;

        public void Initialize(MissionData mission, MissionDifficultySettings settings, Vector3 initialPlayerPos)
        {
            _activeMission = mission;
            _difficultySettings = settings ?? new MissionDifficultySettings();
            _currentCheckpointIndex = 0;
            _offRouteTimer = 0f;
            _currentMotorcycleDamage = 0f;
            _distanceCoveredMeters = 0f;
            _lastPlayerPosition = initialPlayerPos;

            // Reset all objectives to 0
            if (_activeMission?.Objectives != null)
            {
                for (int i = 0; i < _activeMission.Objectives.Count; i++)
                {
                    _activeMission.Objectives[i].ResetProgress();
                }
            }
        }

        public void Update(Vector3 playerPosition, float deltaTime, bool isOffRoute = false)
        {
            if (_activeMission == null || _activeMission.Status != MissionStatus.Active) return;

            // 1. Track distance
            float stepDist = Vector3.Distance(playerPosition, _lastPlayerPosition);
            if (stepDist > 0.05f && stepDist < 100f) // Ignore teleportation jumps
            {
                _distanceCoveredMeters += stepDist;
                _lastPlayerPosition = playerPosition;
            }

            // 2. Evaluate Checkpoint Proximity
            EvaluateCheckpoints(playerPosition);

            // 3. Evaluate Destination Proximity
            EvaluateDestination(playerPosition);

            // 4. Monitor Off-Route Failure Condition
            if (_activeMission.FailureConfig.failOnOffRoute)
            {
                if (isOffRoute)
                {
                    _offRouteTimer += deltaTime;
                    if (_offRouteTimer >= _activeMission.FailureConfig.offRouteTimeoutSeconds)
                    {
                        OnFailureConditionMet?.Invoke($"Left the designated route for {_offRouteTimer:0} seconds.");
                        return;
                    }
                }
                else
                {
                    _offRouteTimer = Mathf.Max(0f, _offRouteTimer - deltaTime * 2f);
                }
            }

            // 5. Evaluate Passenger Requirement
            if (_activeMission.FailureConfig.failOnPassengerDismount)
            {
                if (ServiceLocator.TryGet<IPassengerManager>(out var passengerManager))
                {
                    if (!passengerManager.HasPassenger)
                    {
                        OnFailureConditionMet?.Invoke("Passenger dismounted during a required passenger mission.");
                        return;
                    }
                }
            }

            // 6. Check overall objective completion
            if (_activeMission.AreAllRequiredObjectivesComplete())
            {
                OnAllRequiredObjectivesCompleted?.Invoke();
            }
        }

        private void EvaluateCheckpoints(Vector3 playerPosition)
        {
            if (_activeMission?.Checkpoints == null || _currentCheckpointIndex >= _activeMission.Checkpoints.Count) return;

            var cp = _activeMission.Checkpoints[_currentCheckpointIndex];
            float radius = _difficultySettings != null ? _difficultySettings.checkpointRadius : cp.triggerRadius;

            if ((playerPosition - cp.position).sqrMagnitude <= radius * radius)
            {
                int passedIdx = _currentCheckpointIndex;
                _currentCheckpointIndex++;

                // Update checkpoint objective if present
                UpdateObjectiveProgress(MissionObjectiveType.HitCheckpoints, 1f);

                OnCheckpointPassed?.Invoke(_currentCheckpointIndex, _activeMission.Checkpoints.Count);
                GameLogger.I(Tag, $"Checkpoint {_currentCheckpointIndex}/{_activeMission.Checkpoints.Count} reached.");
            }
        }

        private void EvaluateDestination(Vector3 playerPosition)
        {
            if (_activeMission == null) return;

            // Check if destination objective exists
            float destRadius = 15.0f;
            if ((playerPosition - _activeMission.DestinationPosition).sqrMagnitude <= destRadius * destRadius)
            {
                UpdateObjectiveProgress(MissionObjectiveType.ReachDestination, 1f);
            }
        }

        public void ReportDamage(float damageAmount)
        {
            _currentMotorcycleDamage += damageAmount;

            float maxDamage = _difficultySettings != null ? _difficultySettings.maxAllowedDamage : 100f;
            if (_activeMission != null && _activeMission.FailureConfig.failOnExcessiveDamage)
            {
                if (_currentMotorcycleDamage >= maxDamage)
                {
                    OnFailureConditionMet?.Invoke($"Motorcycle damage exceeded allowed threshold ({_currentMotorcycleDamage:0}% / {maxDamage:0}%).");
                }
            }
        }

        public void UpdateObjectiveProgress(MissionObjectiveType type, float progressDelta)
        {
            if (_activeMission?.Objectives == null) return;

            for (int i = 0; i < _activeMission.Objectives.Count; i++)
            {
                if (_activeMission.Objectives[i].type == type)
                {
                    _activeMission.Objectives[i].AddProgress(progressDelta);
                }
            }
        }

        public void ForceCompleteAllObjectives()
        {
            if (_activeMission?.Objectives == null) return;

            for (int i = 0; i < _activeMission.Objectives.Count; i++)
            {
                _activeMission.Objectives[i].SetProgress(_activeMission.Objectives[i].targetValue);
            }
        }
    }
}

