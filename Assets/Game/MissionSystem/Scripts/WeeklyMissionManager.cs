using System;
using System.Collections.Generic;
using System.Globalization;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.Progression.Rewards;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    [Serializable]
    public class WeeklyChallenge
    {
        public string challengeId;
        public string title;
        public string description;
        public float targetValue;
        public float currentProgress;
        public bool isCompleted;
        public int pointsValue;

        public WeeklyChallenge() { }

        public WeeklyChallenge(string id, string tit, string desc, float target, int points)
        {
            challengeId = id;
            title = tit;
            description = desc;
            targetValue = target;
            currentProgress = 0f;
            isCompleted = false;
            pointsValue = points;
        }

        public void AddProgress(float val)
        {
            if (isCompleted || val <= 0f) return;
            currentProgress = Mathf.Min(targetValue, currentProgress + val);
            if (currentProgress >= targetValue) isCompleted = true;
        }
    }

    /// <summary>
    /// Coordinates multi-tier weekly challenges, calculates weekly leaderboard challenge points,
    /// and grants high-tier rewards for weekly completion.
    /// </summary>
    public class WeeklyMissionManager : MonoBehaviour, IGameService, ISaveable
    {
        private const string Tag = "WeeklyMissionManager";

        [SerializeField] private List<WeeklyChallenge> _weeklyChallenges = new List<WeeklyChallenge>();
        [SerializeField] private int _targetWeeklyPoints = 100;
        [SerializeField] private int _currentWeeklyPoints = 0;
        [SerializeField] private bool _isWeeklyRewardClaimed = false;

        private string _currentWeekId = "";
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public IReadOnlyList<WeeklyChallenge> WeeklyChallenges => _weeklyChallenges;
        public int CurrentWeeklyPoints => _currentWeeklyPoints;
        public int TargetWeeklyPoints => _targetWeeklyPoints;
        public bool IsWeeklyRewardClaimed => _isWeeklyRewardClaimed;
        public string SaveKey => "WeeklyMissionManager";

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            CheckWeeklyRollover();

            ServiceLocator.Register<WeeklyMissionManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, "WeeklyMissionManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<WeeklyMissionManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private string GetCurrentWeekId(DateTime date)
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            int weekNum = cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return $"{date.Year}-W{weekNum:D2}";
        }

        public void CheckWeeklyRollover()
        {
            DateTime nowUtc = DateTime.UtcNow;
            string thisWeek = GetCurrentWeekId(nowUtc);

            if (thisWeek != _currentWeekId)
            {
                _currentWeekId = thisWeek;
                _currentWeeklyPoints = 0;
                _isWeeklyRewardClaimed = false;
                GenerateWeeklyChallenges();
            }
        }

        private void GenerateWeeklyChallenges()
        {
            _weeklyChallenges.Clear();

            _weeklyChallenges.Add(new WeeklyChallenge("wk_distance_50km", "Bharat Explorer", "Ride a cumulative 50 kilometers.", 50000f, 40));
            _weeklyChallenges.Add(new WeeklyChallenge("wk_complete_10_missions", "Mission Veteran", "Complete 10 total missions across any category.", 10f, 35));
            _weeklyChallenges.Add(new WeeklyChallenge("wk_pillion_couple_10km", "Monsoon Couple Tour", "Ride 10 kilometers in Couple Riding mode.", 10000f, 25));

            GameLogger.I(Tag, $"Generated new weekly challenges for week {_currentWeekId}.");
        }

        public void ReportWeeklyProgress(string challengeId, float progressDelta)
        {
            var ch = _weeklyChallenges.Find(c => c.challengeId == challengeId);
            if (ch != null && !ch.isCompleted)
            {
                ch.AddProgress(progressDelta);
                if (ch.isCompleted)
                {
                    _currentWeeklyPoints += ch.pointsValue;
                    GameLogger.I(Tag, $"Weekly challenge completed: '{ch.title}'! (+{ch.pointsValue} Points, Total: {_currentWeeklyPoints})");
                }
            }
        }

        public bool ClaimWeeklyGrandReward()
        {
            if (_isWeeklyRewardClaimed || _currentWeeklyPoints < _targetWeeklyPoints) return false;

            _isWeeklyRewardClaimed = true;

            if (ServiceLocator.TryGet<RewardManager>(out var rewardManager))
            {
                var grandReward = RewardData.CreateRuntimeReward(
                    $"weekly_grand_{_currentWeekId}",
                    10000,
                    50,
                    2500
                );
                rewardManager.ClaimReward(grandReward, enforceSingleClaim: true);
            }

            GameLogger.I(Tag, $"Weekly Grand Reward claimed for week {_currentWeekId}!");
            return true;
        }

        public void PopulateSaveData(SaveData data)
        {
            if (data == null) return;
            if (data.progression == null) data.progression = new ProgressionSaveData();

            data.progression.currentWeekId = _currentWeekId;
            data.progression.weeklyChallengePoints = _currentWeeklyPoints;
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data == null || data.progression == null) return;

            _currentWeekId = data.progression.currentWeekId;
            _currentWeeklyPoints = data.progression.weeklyChallengePoints;
            CheckWeeklyRollover();
        }
    }
}
