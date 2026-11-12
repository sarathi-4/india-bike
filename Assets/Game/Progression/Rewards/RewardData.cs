using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Progression.Rewards
{
    [Serializable]
    public class RewardItem
    {
        public RewardType type;
        public int amount = 1;
        public string itemId = "";
        public string description = "";

        public RewardItem() { }

        public RewardItem(RewardType rewardType, int count, string id = "", string desc = "")
        {
            type = rewardType;
            amount = count;
            itemId = id;
            description = desc;
        }
    }

    /// <summary>
    /// Bundled reward configuration for missions, achievements, and login streaks.
    /// Can be instantiated as an asset or dynamically created at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "RewardData", menuName = "Indian Bike Life/Progression/Reward Data")]
    public class RewardData : ScriptableObject
    {
        [SerializeField] private string _rewardId = "";
        [SerializeField] private int _silverCoins = 0;
        [SerializeField] private int _goldCoins = 0;
        [SerializeField] private int _xp = 0;
        [SerializeField] private List<RewardItem> _bonusItems = new List<RewardItem>();

        public string RewardId => string.IsNullOrEmpty(_rewardId) ? name : _rewardId;
        public int SilverCoins => _silverCoins;
        public int GoldCoins => _goldCoins;
        public int XP => _xp;
        public IReadOnlyList<RewardItem> BonusItems => _bonusItems;

        public static RewardData CreateRuntimeReward(string id, int silver, int gold, int xp)
        {
            var reward = CreateInstance<RewardData>();
            reward._rewardId = id;
            reward._silverCoins = Mathf.Max(0, silver);
            reward._goldCoins = Mathf.Max(0, gold);
            reward._xp = Mathf.Max(0, xp);
            return reward;
        }

        public void SetValues(int silver, int gold, int xp)
        {
            _silverCoins = Mathf.Max(0, silver);
            _goldCoins = Mathf.Max(0, gold);
            _xp = Mathf.Max(0, xp);
        }

        public void AddItem(RewardItem item)
        {
            if (item != null) _bonusItems.Add(item);
        }
    }
}

