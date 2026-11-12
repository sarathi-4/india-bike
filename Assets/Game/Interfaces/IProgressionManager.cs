using System;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for player XP accumulation, level progression, and level-gated feature unlocks.
    /// </summary>
    public interface IProgressionManager : IGameService
    {
        int CurrentXP { get; }
        int CurrentLevel { get; }
        int XPForCurrentLevelStart { get; }
        int XPForNextLevel { get; }
        float LevelProgressNormalized { get; }

        void AddXP(int amount, string source);
        bool IsLevelRequirementMet(int requiredLevel);
        int GetRequiredXPForLevel(int level);
    }
}

