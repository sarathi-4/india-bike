using System;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    public enum MissionDifficulty
    {
        Beginner,
        Easy,
        Normal,
        Hard,
        Expert
    }

    /// <summary>
    /// Scaling multipliers applied based on selected mission difficulty.
    /// Modulates challenge parameters without altering fundamental motorcycle physics.
    /// </summary>
    [Serializable]
    public class MissionDifficultySettings
    {
        public MissionDifficulty difficulty = MissionDifficulty.Normal;
        public float timeLimitMultiplier = 1.0f;       // E.g. Beginner = 1.3x time, Expert = 0.8x time
        public float trafficDensityMultiplier = 1.0f; // E.g. Beginner = 0.5x, Expert = 1.5x
        public float maxAllowedDamage = 100f;          // Health / crash threshold before failure
        public float rewardMultiplier = 1.0f;          // E.g. Expert = 2.0x silver/XP
        public float checkpointRadius = 8.0f;          // E.g. Beginner = 12m, Expert = 6m

        public static MissionDifficultySettings GetDefaultSettings(MissionDifficulty diff)
        {
            return diff switch
            {
                MissionDifficulty.Beginner => new MissionDifficultySettings
                {
                    difficulty = diff,
                    timeLimitMultiplier = 1.35f,
                    trafficDensityMultiplier = 0.4f,
                    maxAllowedDamage = 100f,
                    rewardMultiplier = 0.8f,
                    checkpointRadius = 12.0f
                },
                MissionDifficulty.Easy => new MissionDifficultySettings
                {
                    difficulty = diff,
                    timeLimitMultiplier = 1.15f,
                    trafficDensityMultiplier = 0.7f,
                    maxAllowedDamage = 90f,
                    rewardMultiplier = 0.9f,
                    checkpointRadius = 10.0f
                },
                MissionDifficulty.Normal => new MissionDifficultySettings
                {
                    difficulty = diff,
                    timeLimitMultiplier = 1.0f,
                    trafficDensityMultiplier = 1.0f,
                    maxAllowedDamage = 75f,
                    rewardMultiplier = 1.0f,
                    checkpointRadius = 8.0f
                },
                MissionDifficulty.Hard => new MissionDifficultySettings
                {
                    difficulty = diff,
                    timeLimitMultiplier = 0.85f,
                    trafficDensityMultiplier = 1.3f,
                    maxAllowedDamage = 50f,
                    rewardMultiplier = 1.4f,
                    checkpointRadius = 7.0f
                },
                MissionDifficulty.Expert => new MissionDifficultySettings
                {
                    difficulty = diff,
                    timeLimitMultiplier = 0.75f,
                    trafficDensityMultiplier = 1.6f,
                    maxAllowedDamage = 30f,
                    rewardMultiplier = 1.8f,
                    checkpointRadius = 6.0f
                },
                _ => new MissionDifficultySettings()
            };
        }
    }
}
