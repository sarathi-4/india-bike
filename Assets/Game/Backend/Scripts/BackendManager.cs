using System;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Backend
{
    /// <summary>
    /// Master cloud backend service implementing IBackendService and IGameService.
    /// Manages player authentication, cloud persistence, database collections, and offline sync.
    /// </summary>
    public class BackendManager : MonoBehaviour, IBackendService
    {
        private const string Tag = "BackendManager";

        [SerializeField] private BackendConfiguration _configuration;

        private AuthenticationManager _authManager;
        private CloudSaveManager _cloudSaveManager;
        private OfflineSyncManager _offlineSyncManager;
        private DatabaseManager _databaseManager;
        private PlayerDataManager _playerDataManager;

        private bool _isOnline = true;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public AuthState CurrentAuthState => _authManager?.State ?? AuthState.Unauthenticated;
        public string CurrentUserId => _authManager?.CurrentUserId ?? "";
        public string AuthToken => _authManager?.SessionToken ?? "";
        public bool IsOnline => _isOnline;

        public event Action<AuthState> OnAuthStateChanged;
        public event Action<bool> OnNetworkConnectivityChanged;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            if (_configuration == null)
            {
                _configuration = ScriptableObject.CreateInstance<BackendConfiguration>();
            }

            _authManager = new AuthenticationManager();
            _cloudSaveManager = new CloudSaveManager();
            _offlineSyncManager = new OfflineSyncManager();
            _databaseManager = new DatabaseManager();
            _playerDataManager = new PlayerDataManager();

            _databaseManager.InitializeSchema();

            ServiceLocator.Register<IBackendService>(this);
            _isInitialized = true;
            GameLogger.I(Tag, "BackendManager initialized (REST/Cloud service ready).");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            ServiceLocator.Unregister<IBackendService>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public void LoginAsGuest(Action<bool, string> onComplete)
        {
            _authManager.LoginAsGuest((success, msg) =>
            {
                if (success)
                {
                    EventBus.Publish(new AuthSuccessEvent(_authManager.State, _authManager.CurrentUserId));
                    OnAuthStateChanged?.Invoke(_authManager.State);
                }
                else
                {
                    EventBus.Publish(new AuthFailedEvent(msg));
                }
                onComplete?.Invoke(success, msg);
            });
        }

        public void LoginWithEmail(string email, string password, Action<bool, string> onComplete)
        {
            _authManager.LoginWithEmail(email, password, (success, msg) =>
            {
                if (success)
                {
                    EventBus.Publish(new AuthSuccessEvent(_authManager.State, _authManager.CurrentUserId));
                    OnAuthStateChanged?.Invoke(_authManager.State);
                }
                else
                {
                    EventBus.Publish(new AuthFailedEvent(msg));
                }
                onComplete?.Invoke(success, msg);
            });
        }

        public void RegisterAccount(string email, string password, string username, Action<bool, string> onComplete)
        {
            _authManager.RegisterAccount(email, password, username, (success, msg) =>
            {
                if (success)
                {
                    EventBus.Publish(new AuthSuccessEvent(_authManager.State, _authManager.CurrentUserId));
                    OnAuthStateChanged?.Invoke(_authManager.State);
                }
                else
                {
                    EventBus.Publish(new AuthFailedEvent(msg));
                }
                onComplete?.Invoke(success, msg);
            });
        }

        public void Logout()
        {
            _authManager.Logout();
            OnAuthStateChanged?.Invoke(AuthState.Unauthenticated);
        }

        public void SaveToCloud(SaveData data, Action<bool, string> onComplete)
        {
            if (!_isOnline)
            {
                _offlineSyncManager.EnqueueAction("CloudSave", "pending_save_payload");
                onComplete?.Invoke(true, "Offline: Queued save for synchronization.");
                return;
            }

            _cloudSaveManager.SaveToCloud(CurrentUserId, data, (ok, msg) =>
            {
                EventBus.Publish(new CloudSaveCompletedEvent(ok, msg));
                onComplete?.Invoke(ok, msg);
            });
        }

        public void LoadFromCloud(Action<bool, SaveData, string> onComplete)
        {
            _cloudSaveManager.LoadFromCloud(CurrentUserId, onComplete);
        }

        public void SyncOfflineQueue(Action<int> onItemsSynced)
        {
            _offlineSyncManager.FlushQueue(onItemsSynced);
        }

        public bool ValidateTransactionReceipt(string transactionId, int amount, string currency, string signature)
        {
            return _playerDataManager.ValidateTransaction(transactionId, amount, currency, signature);
        }

        public void RequestAccountDeletion(Action<bool, string> onComplete)
        {
            _authManager.DeleteAccount(onComplete);
        }

        public void SetNetworkOnline(bool online)
        {
            if (_isOnline != online)
            {
                _isOnline = online;
                OnNetworkConnectivityChanged?.Invoke(_isOnline);
                if (_isOnline)
                {
                    SyncOfflineQueue(null);
                }
            }
        }
    }
}
