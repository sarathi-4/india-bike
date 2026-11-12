using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Progression.Economy
{
    /// <summary>
    /// Master economy coordinator implementing IEconomyManager, IGameService, and ISaveable.
    /// Manages player wallet, transaction ledger, and currency event dispatching.
    /// </summary>
    public class CurrencyManager : MonoBehaviour, IEconomyManager, ISaveable
    {
        private const string Tag = "CurrencyManager";

        [SerializeField] private int _defaultInitialSilver = 1000;
        [SerializeField] private int _defaultInitialGold = 10;

        private PlayerWallet _wallet;
        private TransactionManager _transactionManager;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public int SilverCoins => _wallet?.SilverCoins ?? 0;
        public int GoldCoins => _wallet?.GoldCoins ?? 0;
        public string SaveKey => "EconomyManager";

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _wallet = new PlayerWallet(_defaultInitialSilver, _defaultInitialGold);
            _transactionManager = new TransactionManager();

            ServiceLocator.Register<IEconomyManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, $"CurrencyManager initialized. Silver: {SilverCoins}, Gold: {GoldCoins}");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<IEconomyManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public int GetBalance(CurrencyType currency)
        {
            return _wallet?.GetBalance(currency) ?? 0;
        }

        public bool CanAfford(CurrencyType currency, int amount)
        {
            return _wallet != null && _wallet.CanAfford(currency, amount);
        }

        public bool AddCurrency(CurrencyType currency, int amount, string source, string transactionId = null)
        {
            if (_wallet == null || amount <= 0) return false;

            // Idempotency check: prevent duplicate reward claims
            if (!string.IsNullOrEmpty(transactionId) && _transactionManager.IsTransactionProcessed(transactionId))
            {
                GameLogger.W(Tag, $"Duplicate transaction rejected: {transactionId} (Source: {source})");
                return false;
            }

            if (_wallet.Credit(currency, amount, out int newBalance))
            {
                var record = new TransactionRecord(transactionId, currency, amount, newBalance, source);
                _transactionManager.RecordTransaction(record);

                EventBus.Publish(new CurrencyEarnedEvent(currency, amount, newBalance, source));
                GameLogger.I(Tag, $"Credited {amount} {currency} ({source}). New Balance: {newBalance}");
                return true;
            }

            return false;
        }

        public bool SpendCurrency(CurrencyType currency, int amount, string reason, string transactionId = null)
        {
            if (_wallet == null || amount <= 0) return false;

            if (!_wallet.CanAfford(currency, amount))
            {
                int currentBal = _wallet.GetBalance(currency);
                EventBus.Publish(new TransactionFailedEvent(currency, amount, currentBal, "Insufficient balance"));
                GameLogger.W(Tag, $"Failed to debit {amount} {currency}. Current: {currentBal} (Reason: {reason})");
                return false;
            }

            if (_wallet.Debit(currency, amount, out int newBalance))
            {
                var record = new TransactionRecord(transactionId, currency, -amount, newBalance, reason);
                _transactionManager.RecordTransaction(record);

                EventBus.Publish(new CurrencySpentEvent(currency, amount, newBalance, reason));
                GameLogger.I(Tag, $"Debited {amount} {currency} ({reason}). New Balance: {newBalance}");
                return true;
            }

            return false;
        }

        public IReadOnlyList<TransactionRecord> GetTransactionHistory()
        {
            return _transactionManager?.Ledger ?? Array.Empty<TransactionRecord>();
        }

        public void PopulateSaveData(SaveData data)
        {
            if (data == null) return;

            // Keep backwards-compatible playerProfile.playerCoins in sync with SilverCoins
            if (data.playerProfile != null)
            {
                data.playerProfile.playerCoins = SilverCoins;
            }

            // Populate dedicated progression container
            if (data.progression == null)
            {
                data.progression = new ProgressionSaveData();
            }

            data.progression.silverCoins = SilverCoins;
            data.progression.goldCoins = GoldCoins;

            if (_transactionManager != null && _transactionManager.Ledger != null)
            {
                data.progression.transactionHistory = new List<TransactionRecord>(_transactionManager.Ledger);
            }
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data == null) return;

            if (data.progression != null)
            {
                _wallet.SetBalances(data.progression.silverCoins, data.progression.goldCoins);
                if (data.progression.transactionHistory != null)
                {
                    _transactionManager.LoadRecords(data.progression.transactionHistory);
                }
            }
            else if (data.playerProfile != null)
            {
                // Fallback for Schema v1 files
                _wallet.SetBalances(data.playerProfile.playerCoins, _defaultInitialGold);
            }
        }
    }
}

