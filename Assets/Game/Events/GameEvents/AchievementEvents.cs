using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Progression.Achievements;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct AchievementProgressEvent : IGameEvent
    {
        public string AchievementId { get; }
        public float CurrentProgress { get; }
        public float TargetProgress { get; }

        public AchievementProgressEvent(string achievementId, float currentProgress, float targetProgress)
        {
            AchievementId = achievementId;
            CurrentProgress = currentProgress;
            TargetProgress = targetProgress;
        }
    }

    public readonly struct AchievementUnlockedEvent : IGameEvent
    {
        public AchievementData Achievement { get; }

        public AchievementUnlockedEvent(AchievementData achievement)
        {
            Achievement = achievement;
        }
    }
}

