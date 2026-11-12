using IndianBikeLife.Core.Events.Core;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct XPEarnedEvent : IGameEvent
    {
        public int AmountEarned { get; }
        public int TotalXP { get; }
        public string Source { get; }

        public XPEarnedEvent(int amountEarned, int totalXP, string source)
        {
            AmountEarned = amountEarned;
            TotalXP = totalXP;
            Source = source;
        }
    }

    public readonly struct PlayerLevelUpEvent : IGameEvent
    {
        public int NewLevel { get; }
        public int PreviousLevel { get; }

        public PlayerLevelUpEvent(int newLevel, int previousLevel)
        {
            NewLevel = newLevel;
            PreviousLevel = previousLevel;
        }
    }

    public readonly struct PlayerMilestoneReachedEvent : IGameEvent
    {
        public string MilestoneKey { get; }
        public float Value { get; }

        public PlayerMilestoneReachedEvent(string milestoneKey, float value)
        {
            MilestoneKey = milestoneKey;
            Value = value;
        }
    }
}
