using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.MissionSystem;
using IndianBikeLife.Progression.Rewards;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct MissionStartedEvent : IGameEvent
    {
        public MissionData Mission { get; }
        public MissionDifficulty Difficulty { get; }

        public MissionStartedEvent(MissionData mission, MissionDifficulty difficulty)
        {
            Mission = mission;
            Difficulty = difficulty;
        }
    }

    public readonly struct MissionObjectiveUpdatedEvent : IGameEvent
    {
        public MissionData Mission { get; }
        public int ObjectiveIndex { get; }
        public MissionObjective Objective { get; }

        public MissionObjectiveUpdatedEvent(MissionData mission, int objectiveIndex, MissionObjective objective)
        {
            Mission = mission;
            ObjectiveIndex = objectiveIndex;
            Objective = objective;
        }
    }

    public readonly struct MissionCheckpointReachedEvent : IGameEvent
    {
        public MissionData Mission { get; }
        public int CheckpointIndex { get; }
        public int TotalCheckpoints { get; }

        public MissionCheckpointReachedEvent(MissionData mission, int checkpointIndex, int totalCheckpoints)
        {
            Mission = mission;
            CheckpointIndex = checkpointIndex;
            TotalCheckpoints = totalCheckpoints;
        }
    }

    public readonly struct MissionCompletedEvent : IGameEvent
    {
        public MissionData Mission { get; }
        public float TimeTakenSeconds { get; }
        public RewardData RewardsEarned { get; }

        public MissionCompletedEvent(MissionData mission, float timeTakenSeconds, RewardData rewardsEarned)
        {
            Mission = mission;
            TimeTakenSeconds = timeTakenSeconds;
            RewardsEarned = rewardsEarned;
        }
    }

    public readonly struct MissionFailedEvent : IGameEvent
    {
        public MissionData Mission { get; }
        public string FailureReason { get; }

        public MissionFailedEvent(MissionData mission, string failureReason)
        {
            Mission = mission;
            FailureReason = failureReason;
        }
    }

    public readonly struct MissionCancelledEvent : IGameEvent
    {
        public MissionData Mission { get; }

        public MissionCancelledEvent(MissionData mission)
        {
            Mission = mission;
        }
    }

    public readonly struct MissionRestartedEvent : IGameEvent
    {
        public MissionData Mission { get; }

        public MissionRestartedEvent(MissionData mission)
        {
            Mission = mission;
        }
    }
}
