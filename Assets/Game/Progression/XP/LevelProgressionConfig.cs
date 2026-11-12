using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Progression.XP
{
    public enum LevelUnlockType
    {
        MotorcycleCategory,
        RidingMode,
        MissionCategory,
        CosmeticGear,
        SpecialFeature
    }

    [Serializable]
    public class LevelUnlockData
    {
        public int level;
        public LevelUnlockType unlockType;
        public string unlockId;
        public string title;
        public string description;

        public LevelUnlockData(int lvl, LevelUnlockType type, string id, string tit, string desc)
        {
            level = lvl;
            unlockType = type;
            unlockId = id;
            title = tit;
            description = desc;
        }
    }

    /// <summary>
    /// Configures the progression XP curve and level milestone unlocks.
    /// Formula: XP(L) = BaseXP * L^Exponent + FlatPerLevel * L
    /// </summary>
    [CreateAssetMenu(fileName = "LevelProgressionConfig", menuName = "Indian Bike Life/Progression/Level Progression Config")]
    public class LevelProgressionConfig : ScriptableObject
    {
        [Header("XP Formula Parameters")]
        [SerializeField] private int _baseXP = 500;
        [SerializeField] private float _exponent = 1.25f;
        [SerializeField] private int _flatPerLevel = 250;
        [SerializeField] private int _maxLevel = 50;

        [Header("Level Unlocks")]
        [SerializeField] private List<LevelUnlockData> _unlocks = new List<LevelUnlockData>();

        public int BaseXP => _baseXP;
        public float Exponent => _exponent;
        public int FlatPerLevel => _flatPerLevel;
        public int MaxLevel => _maxLevel;
        public IReadOnlyList<LevelUnlockData> Unlocks => _unlocks;

        /// <summary>
        /// Calculates the total cumulative XP required to reach the start of a given level.
        /// </summary>
        public int GetCumulativeXPForLevel(int level)
        {
            if (level <= 1) return 0;

            int total = 0;
            for (int l = 1; l < level; l++)
            {
                total += GetXPRequiredForLevelUp(l);
            }
            return total;
        }

        /// <summary>
        /// Calculates the XP delta required to advance from 'level' to 'level + 1'.
        /// </summary>
        public int GetXPRequiredForLevelUp(int level)
        {
            if (level >= _maxLevel) return int.MaxValue;
            return Mathf.RoundToInt(_baseXP * Mathf.Pow(level, _exponent) + _flatPerLevel * level);
        }

        public IReadOnlyList<LevelUnlockData> GetUnlocksForLevel(int level)
        {
            var results = new List<LevelUnlockData>();
            for (int i = 0; i < _unlocks.Count; i++)
            {
                if (_unlocks[i].level == level)
                {
                    results.Add(_unlocks[i]);
                }
            }
            return results;
        }
    }
}

