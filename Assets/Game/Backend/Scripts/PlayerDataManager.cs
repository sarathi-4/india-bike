using System;
using IndianBikeLife.Backend.Security;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Backend
{
    /// <summary>
    /// Bridges player save data with cloud backend records and enforces server-validated transaction rules.
    /// </summary>
    public class PlayerDataManager
    {
        private const string Tag = "PlayerDataManager";

        public bool ValidateTransaction(string transactionId, int amount, string currency, string hmacSignature)
        {
            if (string.IsNullOrEmpty(transactionId) || amount <= 0) return false;

            string expectedData = $"{transactionId}:{amount}:{currency}";
            string expectedHmac = TokenSecurityUtility.ComputeHmac(expectedData);

            if (!string.Equals(expectedHmac, hmacSignature, StringComparison.OrdinalIgnoreCase))
            {
                GameLogger.W(Tag, $"Transaction signature mismatch! Rejecting unauthorized {amount} {currency} credit for tx {transactionId}.");
                return false;
            }

            return true;
        }

        public void SynchronizeWithSaveData(SaveData localData, Action<bool> onSynced)
        {
            if (localData == null)
            {
                onSynced?.Invoke(false);
                return;
            }

            // Sync wallet and progression
            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.Save();
            }

            onSynced?.Invoke(true);
        }
    }
}

