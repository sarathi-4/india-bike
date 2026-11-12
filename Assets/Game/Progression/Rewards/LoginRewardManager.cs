using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Progression.Rewards
{
    [Serializable]
    public class DailyCalendarReward
    {
        public int dayIndex; // 1 to 7
        public int silverCoins;
        public int goldCoins;
        public int xp;
        public string title;

        public DailyCalendarReward(int day, int silver, int gold, int exp, string desc)
        {
            dayIndex = day;
            silverCoins = silver;
            goldCoins = gold;
            xp = exp;
            title = desc;
        }
    }

    /// <summary>
    /// Manages the 7-day recurring daily login reward calendar, consecutive streaks,
    /// and guards against local system clock manipulation exploits.
    /// </summary>
    public class LoginRewardManager : MonoBehaviour, IGameService, ISaveable
    {
        private const string Tag = "LoginRewardManager";

        [SerializeField] private List<DailyCalendarReward> _calendarRewards = new List<DailyCalendarReward>();

        private string _lastLoginDateUtc = "";
        private int _consecutiveStreak = 0;
        private readonly List<int> _claimedCalendarDays = new List<int>();
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public int ConsecutiveStreak => _consecutiveStreak;
        public int CurrentCalendarDay => ((_consecutiveStreak - 1) % 7) + 1;
        public string SaveKey => "LoginRewardManager";

        public event Action<int, DailyCalendarReward> OnDailyRewardClaimed;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            if (_calendarRewards == null || _calendarRewards.Count == 0)
            {
                PopulateDefaultCalendar();
            }

            ServiceLocator.Register<LoginRewardManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, "LoginRewardManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<LoginRewardManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private void PopulateDefaultCalendar()
        {
            _calendarRewards = new List<DailyCalendarReward>
            {
                new DailyCalendarReward(1, 500, 0, 100, "Day 1 Welcome"),
                new DailyCalendarReward(2, 750, 2, 150, "Day 2 Fuel Boost"),
                new DailyCalendarReward(3, 1000, 5, 250, "Day 3 Cruising"),
                new DailyCalendarReward(4, 1500, 5, 400, "Day 4 Gear Up"),
                new DailyCalendarReward(5, 2000, 10, 600, "Day 5 Touring"),
                new DailyCalendarReward(6, 2500, 15, 800, "Day 6 Veteran Rider"),
                new DailyCalendarReward(7, 5000, 30, 1500, "Day 7 Grand Pillion Celebration")
            };
        }

        public bool CanClaimToday()
        {
            DateTime nowUtc = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(_lastLoginDateUtc))
            {
                if (DateTime.TryParse(_lastLoginDateUtc, out DateTime lastLogin))
                {
                    // Clock anti-cheat check: if device time is earlier than last recorded login
                    if (nowUtc < lastLogin)
                    {
                        GameLogger.W(Tag, "Clock manipulation detected: current time is earlier than last login date.");
                        return false;
                    }

                    // Already claimed today
                    if (nowUtc.Date == lastLogin.Date)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool ClaimDailyReward()
        {
            if (!CanClaimToday())
            {
                GameLogger.W(Tag, "Daily reward cannot be claimed today (already claimed or clock tamper detected).");
                return false;
            }

            DateTime nowUtc = DateTime.UtcNow;

            // Check streak continuity: did player miss a day?
            if (!string.IsNullOrEmpty(_lastLoginDateUtc) && DateTime.TryParse(_lastLoginDateUtc, out DateTime lastLogin))
            {
                int daysDifference = (int)(nowUtc.Date - lastLogin.Date).TotalDays;
                if (daysDifference == 1)
                {
                    _consecutiveStreak++;
                }
                else
                {
                    // Missed one or more days -> reset streak to 1
                    _consecutiveStreak = 1;
                    _claimedCalendarDays.Clear();
                }
            }
            else
            {
                _consecutiveStreak = 1;
                _claimedCalendarDays.Clear();
            }

            int dayIndex = CurrentCalendarDay;
            DailyCalendarReward reward = _calendarRewards.Find(r => r.dayIndex == dayIndex);

            if (reward != null)
            {
                if (ServiceLocator.TryGet<RewardManager>(out var rewardManager))
                {
                    var rewardData = RewardData.CreateRuntimeReward(
                        $"daily_login_day_{dayIndex}_{nowUtc:yyyyMMdd}",
                        reward.silverCoins,
                        reward.goldCoins,
                        reward.xp
                    );
                    rewardManager.ClaimReward(rewardData, enforceSingleClaim: false);
                }

                _claimedCalendarDays.Add(dayIndex);
                _lastLoginDateUtc = nowUtc.ToString("o");

                GameLogger.I(Tag, $"Claimed Daily Reward for Day {dayIndex}: {reward.silverCoins} Silver, {reward.goldCoins} Gold, {reward.xp} XP");
                OnDailyRewardClaimed?.Invoke(dayIndex, reward);
                return true;
            }

            return false;
        }

        public void PopulateSaveData(SaveData data)
        {
            if (data == null) return;
            if (data.progression == null) data.progression = new ProgressionSaveData();

            data.progression.lastLoginDateUtc = _lastLoginDateUtc;
            data.progression.consecutiveDaysLoggedIn = _consecutiveStreak;
            data.progression.claimedCalendarDays = new List<int>(_claimedCalendarDays);
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data == null || data.progression == null) return;

            _lastLoginDateUtc = data.progression.lastLoginDateUtc;
            _consecutiveStreak = data.progression.consecutiveDaysLoggedIn;
            _claimedCalendarDays.Clear();
            if (data.progression.claimedCalendarDays != null)
            {
                _claimedCalendarDays.AddRange(data.progression.claimedCalendarDays);
            }
        }
    }
}

