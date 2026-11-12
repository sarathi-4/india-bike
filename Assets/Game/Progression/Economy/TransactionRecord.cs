using System;

namespace IndianBikeLife.Progression.Economy
{
    /// <summary>
    /// Immutable record representing a single financial operation in the game economy ledger.
    /// </summary>
    [Serializable]
    public class TransactionRecord
    {
        public string transactionId;
        public string timestampUtc;
        public CurrencyType currency;
        public int amount; // Positive = credit (earned), Negative = debit (spent)
        public int balanceAfter;
        public string description;

        public TransactionRecord() { }

        public TransactionRecord(string id, CurrencyType currencyType, int transAmount, int newBal, string desc)
        {
            transactionId = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString("N") : id;
            timestampUtc = DateTime.UtcNow.ToString("o");
            currency = currencyType;
            amount = transAmount;
            balanceAfter = newBal;
            description = desc;
        }
    }
}
