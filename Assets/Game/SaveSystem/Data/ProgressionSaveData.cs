using System;
using System.Collections.Generic;
using IndianBikeLife.Progression.Economy;

namespace IndianBikeLife.Core.SaveSystem.Data
{
    [Serializable]
    public class AchievementProgressSaveData
    {
        public string achievementId;
        public float progress;
        public bool isUnlocked;
        public string unlockedTimestampUtc;

        public AchievementProgressSaveData() { }

        public AchievementProgressSaveData(string id, float prog, bool unlocked, string timestamp = "")
        {
            achievementId = id;
            progress = prog;
            isUnlocked = unlocked;
            unlockedTimestampUtc = timestamp;
        }
    }

    /// <summary>
    /// Dedicated persistence container for Stage 3 Player Progression, Missions, Economy, and Achievements.
    /// Backward-compatible with Stage 1 & 2 saves via SaveMigratorV1ToV2.
    /// </summary>
    [Serializable]
    public class ProgressionSaveData
    {
        public int silverCoins = 1000;
        public int goldCoins = 10;
        public List<TransactionRecord> transactionHistory = new List<TransactionRecord>();

        public int currentXP = 0;
        public int currentLevel = 1;

        public List<string> completedMissionIds = new List<string>();
        public List<string> claimedRewardIds = new List<string>();

        public List<AchievementProgressSaveData> achievements = new List<AchievementProgressSaveData>();

        // Daily login calendar persistence
        public string lastLoginDateUtc = "";
        public int consecutiveDaysLoggedIn = 0;
        public List<int> claimedCalendarDays = new List<int>();

        // Weekly challenge tracking
        public string currentWeekId = "";
        public int weeklyChallengePoints = 0;

        public static ProgressionSaveData CreateDefault()
        {
            return new ProgressionSaveData();
        }
    }
}

