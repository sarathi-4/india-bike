using System;
using IndianBikeLife.Backend;
using IndianBikeLife.Core.SaveSystem.Data;

namespace IndianBikeLife.Core.Interfaces
{
    public enum AuthState
    {
        Unauthenticated,
        Authenticating,
        AuthenticatedAsGuest,
        AuthenticatedWithAccount
    }

    /// <summary>
    /// Contract for backend cloud services, authentication, cloud persistence, and economy validation.
    /// </summary>
    public interface IBackendService : IGameService
    {
        AuthState CurrentAuthState { get; }
        string CurrentUserId { get; }
        string AuthToken { get; }
        bool IsOnline { get; }

        void LoginAsGuest(Action<bool, string> onComplete);
        void LoginWithEmail(string email, string password, Action<bool, string> onComplete);
        void RegisterAccount(string email, string password, string username, Action<bool, string> onComplete);
        void Logout();

        void SaveToCloud(SaveData data, Action<bool, string> onComplete);
        void LoadFromCloud(Action<bool, SaveData, string> onComplete);
        void SyncOfflineQueue(Action<int> onItemsSynced);

        bool ValidateTransactionReceipt(string transactionId, int amount, string currency, string signature);
        void RequestAccountDeletion(Action<bool, string> onComplete);

        event Action<AuthState> OnAuthStateChanged;
        event Action<bool> OnNetworkConnectivityChanged;
    }
}
