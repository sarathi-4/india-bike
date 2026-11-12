using UnityEngine;

namespace IndianBikeLife.Progression.XP
{
    /// <summary>
    /// Pure logic evaluator for player level calculations, XP normalization, and level-up detection.
    /// </summary>
    public class LevelProgression
    {
        private readonly LevelProgressionConfig _config;

        public LevelProgression(LevelProgressionConfig config)
        {
            _config = config;
        }

        public int CalculateLevelFromXP(int totalXP, out int xpIntoCurrentLevel, out int xpNeededForNext)
        {
            if (_config == null)
            {
                // Fallback default formula: 1000 XP per level
                int fallbackLvl = Mathf.Max(1, 1 + totalXP / 1000);
                xpIntoCurrentLevel = totalXP % 1000;
                xpNeededForNext = 1000;
                return fallbackLvl;
            }

            int level = 1;
            int remainingXP = totalXP;

            while (level < _config.MaxLevel)
            {
                int req = _config.GetXPRequiredForLevelUp(level);
                if (remainingXP >= req)
                {
                    remainingXP -= req;
                    level++;
                }
                else
                {
                    break;
                }
            }

            xpIntoCurrentLevel = remainingXP;
            xpNeededForNext = (level < _config.MaxLevel) ? _config.GetXPRequiredForLevelUp(level) : int.MaxValue;
            return level;
        }

        public float CalculateNormalizedProgress(int xpIntoCurrentLevel, int xpNeededForNext)
        {
            if (xpNeededForNext <= 0 || xpNeededForNext == int.MaxValue) return 1.0f;
            return Mathf.Clamp01((float)xpIntoCurrentLevel / xpNeededForNext);
        }
    }
}
