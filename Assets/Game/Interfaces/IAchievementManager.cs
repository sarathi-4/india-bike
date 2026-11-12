using System;
using System.Collections.Generic;
using IndianBikeLife.Progression.Achievements;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for player achievements tracking, milestone evaluation, and unlock rewards.
    /// </summary>
    public interface IAchievementManager : IGameService
    {
        IReadOnlyList<AchievementData> GetAllAchievements();
        AchievementData GetAchievement(string achievementId);
        bool IsAchievementUnlocked(string achievementId);
        void ReportProgress(string achievementId, float progressDelta);
        void SetProgress(string achievementId, float absoluteProgress);
        int UnlockedAchievementCount { get; }
        int TotalAchievementCount { get; }
    }
}

