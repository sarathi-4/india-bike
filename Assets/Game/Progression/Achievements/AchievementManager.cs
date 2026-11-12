using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.Progression.Rewards;
using UnityEngine;

namespace IndianBikeLife.Progression.Achievements
{
    /// <summary>
    /// Master achievement service implementing IAchievementManager, IGameService, and ISaveable.
    /// Tracks milestone progress, awards achievement rewards, and persists unlock status.
    /// </summary>
    public class AchievementManager : MonoBehaviour, IAchievementManager, ISaveable
    {
        private const string Tag = "AchievementManager";

        [SerializeField] private AchievementCatalogue _catalogue;

        private readonly Dictionary<string, float> _progressMap = new Dictionary<string, float>();
        private readonly Dictionary<string, bool> _unlockedMap = new Dictionary<string, bool>();
        private readonly List<AchievementData> _runtimeCatalogue = new List<AchievementData>();

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "AchievementManager";
        public int UnlockedAchievementCount
        {
            get
            {
                int count = 0;
                foreach (var kvp in _unlockedMap)
                {
                    if (kvp.Value) count++;
                }
                return count;
            }
        }

        public int TotalAchievementCount => _runtimeCatalogue.Count;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            BuildDefaultAchievements();

            ServiceLocator.Register<IAchievementManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            EventBus.Subscribe<MissionCompletedEvent>(OnMissionCompleted);
            EventBus.Subscribe<PassengerMountedEvent>(OnPassengerMounted);

            _isInitialized = true;
            GameLogger.I(Tag, $"AchievementManager initialized. Total achievements: {_runtimeCatalogue.Count}");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            EventBus.Unsubscribe<MissionCompletedEvent>(OnMissionCompleted);
            EventBus.Unsubscribe<PassengerMountedEvent>(OnPassengerMounted);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<IAchievementManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private void BuildDefaultAchievements()
        {
            _runtimeCatalogue.Clear();

            if (_catalogue != null && _catalogue.Achievements != null && _catalogue.Achievements.Count > 0)
            {
                for (int i = 0; i < _catalogue.Achievements.Count; i++)
                {
                    if (_catalogue.Achievements[i] != null)
                        _runtimeCatalogue.Add(_catalogue.Achievements[i]);
                }
            }
            else
            {
                // Procedural baseline definitions for the 8 required achievement categories
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_first_ride", "First Kickstart", "Start your engine and complete your first ride.", AchievementCategory.FirstRide, 1f, RewardData.CreateRuntimeReward("rew_ach_first_ride", 500, 2, 100)));
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_dist_10k", "Highway Cruiser 10K", "Accumulate 10 kilometers of total riding distance.", AchievementCategory.DistanceMilestone, 10f, RewardData.CreateRuntimeReward("rew_ach_dist_10k", 1000, 5, 250)));
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_dist_50k", "Highway Cruiser 50K", "Accumulate 50 kilometers of total riding distance.", AchievementCategory.DistanceMilestone, 50f, RewardData.CreateRuntimeReward("rew_ach_dist_50k", 2500, 10, 500)));
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_mission_10", "Mission Specialist", "Successfully complete 10 missions.", AchievementCategory.MissionCompletion, 10f, RewardData.CreateRuntimeReward("rew_ach_mission_10", 3000, 15, 600)));
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_ghat_ooty", "Ghat Conqueror", "Conquer the Nilgiris Ghat road hairpin bends to Ooty.", AchievementCategory.HillStationExploration, 1f, RewardData.CreateRuntimeReward("rew_ach_ghat_ooty", 2000, 10, 400)));
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_pillion_25k", "Pillion Companion", "Ride 25 kilometers with a passenger.", AchievementCategory.PassengerRides, 25f, RewardData.CreateRuntimeReward("rew_ach_pillion_25k", 2000, 8, 350)));
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_bike_collector", "Garage Enthusiast", "Own at least 3 motorcycles in your collection.", AchievementCategory.MotorcycleCollection, 3f, RewardData.CreateRuntimeReward("rew_ach_bike_collector", 5000, 25, 1000)));
                _runtimeCatalogue.Add(AchievementData.CreateRuntime("ach_iron_butt", "Iron Butt Rider", "Spend 5 hours total riding time on Indian roads.", AchievementCategory.RidingTimeMilestone, 5f, RewardData.CreateRuntimeReward("rew_ach_iron_butt", 4000, 20, 800)));
            }

            for (int i = 0; i < _runtimeCatalogue.Count; i++)
            {
                string id = _runtimeCatalogue[i].AchievementId;
                if (!_progressMap.ContainsKey(id)) _progressMap[id] = 0f;
                if (!_unlockedMap.ContainsKey(id)) _unlockedMap[id] = false;
            }
        }

        public IReadOnlyList<AchievementData> GetAllAchievements()
        {
            return _runtimeCatalogue;
        }

        public AchievementData GetAchievement(string achievementId)
        {
            if (string.IsNullOrEmpty(achievementId)) return null;
            return _runtimeCatalogue.Find(a => a.AchievementId == achievementId);
        }

        public bool IsAchievementUnlocked(string achievementId)
        {
            if (string.IsNullOrEmpty(achievementId)) return false;
            return _unlockedMap.TryGetValue(achievementId, out bool unlocked) && unlocked;
        }

        public void ReportProgress(string achievementId, float progressDelta)
        {
            if (string.IsNullOrEmpty(achievementId) || progressDelta <= 0f) return;

            float current = _progressMap.TryGetValue(achievementId, out float val) ? val : 0f;
            SetProgress(achievementId, current + progressDelta);
        }

        public void SetProgress(string achievementId, float absoluteProgress)
        {
            if (string.IsNullOrEmpty(achievementId)) return;

            var achievement = GetAchievement(achievementId);
            if (achievement == null) return;

            if (IsAchievementUnlocked(achievementId)) return; // Already unlocked

            float clamped = Mathf.Clamp(absoluteProgress, 0f, achievement.TargetProgress);
            _progressMap[achievementId] = clamped;

            EventBus.Publish(new AchievementProgressEvent(achievementId, clamped, achievement.TargetProgress));

            if (clamped >= achievement.TargetProgress)
            {
                UnlockAchievement(achievement);
            }
        }

        private void UnlockAchievement(AchievementData achievement)
        {
            string id = achievement.AchievementId;
            _unlockedMap[id] = true;
            _progressMap[id] = achievement.TargetProgress;

            GameLogger.I(Tag, $"🏆 ACHIEVEMENT UNLOCKED: '{achievement.Title}'! ({achievement.Description})");

            // Grant reward if attached
            if (achievement.Reward != null)
            {
                if (ServiceLocator.TryGet<RewardManager>(out var rewardManager))
                {
                    rewardManager.ClaimReward(achievement.Reward, enforceSingleClaim: true);
                }
            }

            EventBus.Publish(new AchievementUnlockedEvent(achievement));
        }

        private void OnMissionCompleted(MissionCompletedEvent evt)
        {
            ReportProgress("ach_first_ride", 1f);
            ReportProgress("ach_mission_10", 1f);
        }

        private void OnPassengerMounted(PassengerMountedEvent evt)
        {
            ReportProgress("ach_pillion_25k", 1f);
        }

        public void PopulateSaveData(SaveData data)
        {
            if (data == null) return;
            if (data.progression == null) data.progression = new ProgressionSaveData();

            data.progression.achievements = new List<AchievementProgressSaveData>();
            foreach (var kvp in _progressMap)
            {
                string id = kvp.Key;
                float prog = kvp.Value;
                bool unlocked = _unlockedMap.TryGetValue(id, out bool u) && u;
                data.progression.achievements.Add(new AchievementProgressSaveData(id, prog, unlocked));
            }
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data == null || data.progression == null || data.progression.achievements == null) return;

            for (int i = 0; i < data.progression.achievements.Count; i++)
            {
                var saved = data.progression.achievements[i];
                if (saved != null && !string.IsNullOrEmpty(saved.achievementId))
                {
                    _progressMap[saved.achievementId] = saved.progress;
                    _unlockedMap[saved.achievementId] = saved.isUnlocked;
                }
            }
        }
    }
}

