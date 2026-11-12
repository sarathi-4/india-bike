using System;
using System.Collections.Generic;
using IndianBikeLife.Progression.Economy;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for the dual-currency game economy.
    /// Enforces non-negative balances, transaction auditing, and safe currency transfers.
    /// </summary>
    public interface IEconomyManager : IGameService
    {
        int SilverCoins { get; }
        int GoldCoins { get; }

        int GetBalance(CurrencyType currency);
        bool CanAfford(CurrencyType currency, int amount);
        bool AddCurrency(CurrencyType currency, int amount, string source, string transactionId = null);
        bool SpendCurrency(CurrencyType currency, int amount, string reason, string transactionId = null);
        IReadOnlyList<TransactionRecord> GetTransactionHistory();
    }
}

