using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.Progression.Rewards;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    [Serializable]
    public class DailyTask
    {
        public string taskId;
        public string title;
        public string description;
        public MissionObjectiveType objectiveType;
        public float targetValue;
        public float currentProgress;
        public bool isCompleted;
        public bool isRewardClaimed;
        public int rewardSilver;
        public int rewardXP;

        public DailyTask() { }

        public DailyTask(string id, string tit, string desc, MissionObjectiveType type, float target, int silver, int xp)
        {
            taskId = id;
            title = tit;
            description = desc;
            objectiveType = type;
            targetValue = target;
            currentProgress = 0f;
            isCompleted = false;
            isRewardClaimed = false;
            rewardSilver = silver;
            rewardXP = xp;
        }

        public void AddProgress(float amount)
        {
            if (isCompleted || amount <= 0f) return;
            currentProgress = Mathf.Min(targetValue, currentProgress + amount);
            if (currentProgress >= targetValue)
            {
                isCompleted = true;
            }
        }
    }

    /// <summary>
    /// Manages daily rotating mission objectives, UTC day-rollover, and cheat-resilient reset verification.
    /// </summary>
    public class DailyMissionManager : MonoBehaviour, IGameService, ISaveable
    {
        private const string Tag = "DailyMissionManager";

        [SerializeField] private List<DailyTask> _activeDailyTasks = new List<DailyTask>();

        private string _lastDailyResetDateUtc = "";
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public IReadOnlyList<DailyTask> ActiveDailyTasks => _activeDailyTasks;
        public string SaveKey => "DailyMissionManager";

        public event Action<DailyTask> OnDailyTaskCompleted;
        public event Action<DailyTask> OnDailyTaskRewardClaimed;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            CheckDailyRollover();

            ServiceLocator.Register<DailyMissionManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, "DailyMissionManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<DailyMissionManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public void CheckDailyRollover()
        {
            DateTime nowUtc = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(_lastDailyResetDateUtc) && DateTime.TryParse(_lastDailyResetDateUtc, out DateTime lastReset))
            {
                // Anti-cheat: device time set behind last recorded date
                if (nowUtc < lastReset)
                {
                    GameLogger.W(Tag, "System clock manipulation detected during daily mission check.");
                    return;
                }

                // If on same calendar day (UTC), no rollover needed
                if (nowUtc.Date == lastReset.Date && _activeDailyTasks.Count > 0)
                {
                    return;
                }
            }

            // Generate fresh set of 3 daily tasks
            GenerateDailyTasks(nowUtc);
        }

        private void GenerateDailyTasks(DateTime date)
        {
            _activeDailyTasks.Clear();
            string dateStr = date.ToString("yyyyMMdd");

            _activeDailyTasks.Add(new DailyTask(
                $"daily_km_{dateStr}",
                "Highway Patrol",
                "Ride a total of 5,000 meters across Indian roads.",
                MissionObjectiveType.ReachDestination,
                5000f,
                1000,
                200
            ));

            _activeDailyTasks.Add(new DailyTask(
                $"daily_cps_{dateStr}",
                "Precision Riding",
                "Successfully clear 10 route checkpoints.",
                MissionObjectiveType.HitCheckpoints,
                10f,
                1500,
                300
            ));

            _activeDailyTasks.Add(new DailyTask(
                $"daily_pillion_{dateStr}",
                "Pillion Touring",
                "Ride with a passenger for at least 2,000 meters.",
                MissionObjectiveType.TransportPassenger,
                2000f,
                2000,
                400
            ));

            _lastDailyResetDateUtc = date.ToString("o");
            GameLogger.I(Tag, $"Generated 3 new daily tasks for UTC date {date:yyyy-MM-dd}.");
        }

        public void ReportDailyProgress(MissionObjectiveType type, float delta)
        {
            for (int i = 0; i < _activeDailyTasks.Count; i++)
            {
                var task = _activeDailyTasks[i];
                if (task.objectiveType == type && !task.isCompleted)
                {
                    task.AddProgress(delta);
                    if (task.isCompleted)
                    {
                        GameLogger.I(Tag, $"Daily task completed: '{task.title}'!");
                        OnDailyTaskCompleted?.Invoke(task);
                    }
                }
            }
        }

        public bool ClaimDailyTaskReward(string taskId)
        {
            var task = _activeDailyTasks.Find(t => t.taskId == taskId);
            if (task == null || !task.isCompleted || task.isRewardClaimed)
            {
                return false;
            }

            task.isRewardClaimed = true;

            if (ServiceLocator.TryGet<RewardManager>(out var rewardManager))
            {
                var rew = RewardData.CreateRuntimeReward(
                    $"claim_{taskId}",
                    task.rewardSilver,
                    0,
                    task.rewardXP
                );
                rewardManager.ClaimReward(rew, enforceSingleClaim: false);
            }

            OnDailyTaskRewardClaimed?.Invoke(task);
            return true;
        }

        public void PopulateSaveData(SaveData data)
        {
            // State persisted through ProgressionSaveData or custom keys
        }

        public void RestoreFromSaveData(SaveData data)
        {
            CheckDailyRollover();
        }
    }
}
