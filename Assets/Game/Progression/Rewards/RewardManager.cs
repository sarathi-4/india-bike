using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.Progression.Economy;
using UnityEngine;

namespace IndianBikeLife.Progression.Rewards
{
    /// <summary>
    /// Distributes rewards, validates claims, and prevents duplicate payout exploits.
    /// Interacts directly with IEconomyManager and IProgressionManager.
    /// </summary>
    public class RewardManager : MonoBehaviour, IGameService, ISaveable
    {
        private const string Tag = "RewardManager";

        private readonly HashSet<string> _claimedRewardIds = new HashSet<string>();
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "RewardManager";
        public IReadOnlyCollection<string> ClaimedRewardIds => _claimedRewardIds;

        public event Action<RewardData> OnRewardGranted;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            ServiceLocator.Register<RewardManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, "RewardManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<RewardManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public bool IsRewardClaimed(string rewardId)
        {
            if (string.IsNullOrEmpty(rewardId)) return false;
            return _claimedRewardIds.Contains(rewardId);
        }

        public bool ClaimReward(RewardData reward, bool enforceSingleClaim = true)
        {
            if (reward == null) return false;

            string id = reward.RewardId;
            if (enforceSingleClaim && !string.IsNullOrEmpty(id) && _claimedRewardIds.Contains(id))
            {
                GameLogger.W(Tag, $"Reward '{id}' has already been claimed! Duplicate claim rejected.");
                return false;
            }

            ServiceLocator.TryGet<IEconomyManager>(out var economy);
            ServiceLocator.TryGet<IProgressionManager>(out var progression);

            // 1. Grant Silver Coins
            if (reward.SilverCoins > 0 && economy != null)
            {
                economy.AddCurrency(CurrencyType.SilverCoins, reward.SilverCoins, $"Reward: {id}", $"reward_silver_{id}_{DateTime.UtcNow.Ticks}");
            }

            // 2. Grant Gold Coins
            if (reward.GoldCoins > 0 && economy != null)
            {
                economy.AddCurrency(CurrencyType.GoldCoins, reward.GoldCoins, $"Reward: {id}", $"reward_gold_{id}_{DateTime.UtcNow.Ticks}");
            }

            // 3. Grant XP
            if (reward.XP > 0 && progression != null)
            {
                progression.AddXP(reward.XP, $"Reward: {id}");
            }

            // 4. Mark as claimed
            if (enforceSingleClaim && !string.IsNullOrEmpty(id))
            {
                _claimedRewardIds.Add(id);
            }

            GameLogger.I(Tag, $"Successfully claimed reward '{id}': +{reward.SilverCoins} Silver, +{reward.GoldCoins} Gold, +{reward.XP} XP");
            OnRewardGranted?.Invoke(reward);
            return true;
        }

        public void PopulateSaveData(SaveData data)
        {
            if (data == null) return;
            if (data.progression == null) data.progression = new ProgressionSaveData();

            data.progression.claimedRewardIds = new List<string>(_claimedRewardIds);
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data == null) return;

            _claimedRewardIds.Clear();
            if (data.progression != null && data.progression.claimedRewardIds != null)
            {
                for (int i = 0; i < data.progression.claimedRewardIds.Count; i++)
                {
                    _claimedRewardIds.Add(data.progression.claimedRewardIds[i]);
                }
            }
        }
    }
}

