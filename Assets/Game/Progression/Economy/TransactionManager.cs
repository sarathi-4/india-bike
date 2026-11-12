using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Progression.Economy
{
    /// <summary>
    /// Maintains an audited transaction ledger and protects against duplicate reward execution.
    /// </summary>
    public class TransactionManager
    {
        private const int MaxLedgerEntries = 100;
        private readonly List<TransactionRecord> _ledger = new List<TransactionRecord>();
        private readonly HashSet<string> _processedTransactionIds = new HashSet<string>();

        public IReadOnlyList<TransactionRecord> Ledger => _ledger;

        public bool IsTransactionProcessed(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId)) return false;
            return _processedTransactionIds.Contains(transactionId);
        }

        public void RecordTransaction(TransactionRecord record)
        {
            if (record == null) return;

            if (!string.IsNullOrEmpty(record.transactionId))
            {
                _processedTransactionIds.Add(record.transactionId);
            }

            _ledger.Insert(0, record);
            if (_ledger.Count > MaxLedgerEntries)
            {
                _ledger.RemoveAt(_ledger.Count - 1);
            }
        }

        public void LoadRecords(List<TransactionRecord> records)
        {
            _ledger.Clear();
            _processedTransactionIds.Clear();

            if (records != null)
            {
                for (int i = 0; i < records.Count; i++)
                {
                    var rec = records[i];
                    if (rec != null)
                    {
                        _ledger.Add(rec);
                        if (!string.IsNullOrEmpty(rec.transactionId))
                        {
                            _processedTransactionIds.Add(rec.transactionId);
                        }
                    }
                }
            }
        }
    }
}

