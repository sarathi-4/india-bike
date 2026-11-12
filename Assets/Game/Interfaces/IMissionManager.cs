using System;
using System.Collections.Generic;
using IndianBikeLife.MissionSystem;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Status lifecycle states for a game mission.
    /// </summary>
    public enum MissionStatus
    {
        NotStarted,
        Active,
        Paused,
        Completed,
        Failed,
        Cancelled
    }

    /// <summary>
    /// Contract for the mission management subsystem.
    /// Handles mission lifecycle, objective tracking, difficulty scaling, and progress persistence.
    /// </summary>
    public interface IMissionManager : IGameService
    {
        MissionStatus CurrentStatus { get; }
        MissionData ActiveMission { get; }
        MissionDifficulty CurrentDifficulty { get; }
        float MissionElapsedTime { get; }
        float MissionRemainingTime { get; }

        bool StartMission(MissionData mission, MissionDifficulty difficulty = MissionDifficulty.Normal);
        void PauseMission();
        void ResumeMission();
        void CompleteActiveMission();
        void FailActiveMission(string failureReason);
        void RestartActiveMission();
        void CancelActiveMission();

        bool IsMissionCompleted(string missionId);
        int GetCompletedMissionCount();
        IReadOnlyList<string> CompletedMissionIds { get; }
    }
}

