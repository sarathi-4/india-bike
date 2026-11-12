using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Social
{
    /// <summary>
    /// Master social subsystem implementing ISocialService, IGameService, and ISaveable.
    /// Coordinates player profiles, friend lists, requests, clubs, and group rides.
    /// </summary>
    public class FriendsManager : MonoBehaviour, ISocialService, ISaveable
    {
        private const string Tag = "FriendsManager";

        private readonly List<FriendEntry> _friends = new List<FriendEntry>();
        private FriendRequestController _requestController;
        private SocialProfileManager _profileManager;
        private ClubManager _clubManager;
        private GroupRideManager _groupRideManager;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "FriendsManager";
        public SocialProfileData LocalProfile => _profileManager?.LocalProfile;
        public ClubData MyClub => _clubManager?.MyClub;

        public event Action OnFriendsListUpdated;
        public event Action<ClubData> OnClubUpdated;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _requestController = new FriendRequestController();
            _profileManager = new SocialProfileManager();
            _clubManager = new ClubManager();
            _groupRideManager = new GroupRideManager();

            _profileManager.InitializeLocalProfile("player_local", "Indian Rider");

            ServiceLocator.Register<ISocialService>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, "FriendsManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<ISocialService>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public IReadOnlyList<FriendEntry> GetFriendsList() => _friends;
        public IReadOnlyList<FriendRequest> GetPendingRequests() => _requestController.IncomingRequests;

        public bool SendFriendRequest(string targetPlayerId)
        {
            if (!_requestController.CanSendRequest(targetPlayerId, _friends))
            {
                GameLogger.W(Tag, $"Cannot send friend request to {targetPlayerId}.");
                return false;
            }

            EventBus.Publish(new FriendRequestSentEvent(targetPlayerId));
            GameLogger.I(Tag, $"Friend request sent to {targetPlayerId}");
            return true;
        }

        public void ReceiveIncomingRequest(string requestId, string fromPlayerId, string fromPlayerName)
        {
            _requestController.AddIncomingRequest(new FriendRequest(requestId, fromPlayerId, fromPlayerName));
            OnFriendsListUpdated?.Invoke();
        }

        public bool AcceptFriendRequest(string requestId)
        {
            var req = _requestController.ResolveRequest(requestId, accept: true);
            if (req != null)
            {
                var friend = new FriendEntry(req.fromPlayerId, req.fromPlayerName, 1);
                _friends.Add(friend);

                EventBus.Publish(new FriendAddedEvent(friend));
                OnFriendsListUpdated?.Invoke();
                GameLogger.I(Tag, $"Accepted friend request from {req.fromPlayerName}!");
                return true;
            }
            return false;
        }

        public bool RejectFriendRequest(string requestId)
        {
            var req = _requestController.ResolveRequest(requestId, accept: false);
            if (req != null)
            {
                OnFriendsListUpdated?.Invoke();
                return true;
            }
            return false;
        }

        public bool RemoveFriend(string targetPlayerId)
        {
            var friend = _friends.Find(f => f.playerId == targetPlayerId);
            if (friend != null)
            {
                _friends.Remove(friend);
                EventBus.Publish(new FriendRemovedEvent(targetPlayerId));
                OnFriendsListUpdated?.Invoke();
                GameLogger.I(Tag, $"Removed friend {friend.username}.");
                return true;
            }
            return false;
        }

        public bool BlockPlayer(string targetPlayerId)
        {
            RemoveFriend(targetPlayerId);
            bool blocked = _requestController.BlockPlayer(targetPlayerId);
            if (blocked)
            {
                EventBus.Publish(new PlayerBlockedEvent(targetPlayerId));
                GameLogger.I(Tag, $"Blocked player {targetPlayerId}.");
            }
            return blocked;
        }

        public bool UnblockPlayer(string targetPlayerId)
        {
            return _requestController.UnblockPlayer(targetPlayerId);
        }

        public bool IsPlayerBlocked(string targetPlayerId)
        {
            return _requestController.IsBlocked(targetPlayerId);
        }

        public bool CreateClub(string clubName, string description, string tag)
        {
            var club = _clubManager.CreateClub(clubName, description, tag, LocalProfile.playerId, LocalProfile.username);
            if (club != null)
            {
                EventBus.Publish(new ClubJoinedEvent(club));
                OnClubUpdated?.Invoke(club);
                return true;
            }
            return false;
        }

        public bool JoinClub(string clubId)
        {
            if (_clubManager.JoinClub(clubId, LocalProfile.playerId, LocalProfile.username))
            {
                EventBus.Publish(new ClubJoinedEvent(_clubManager.MyClub));
                OnClubUpdated?.Invoke(_clubManager.MyClub);
                return true;
            }
            return false;
        }

        public bool LeaveClub()
        {
            if (MyClub != null)
            {
                string id = MyClub.clubId;
                if (_clubManager.LeaveClub(LocalProfile.playerId))
                {
                    EventBus.Publish(new ClubLeftEvent(id));
                    OnClubUpdated?.Invoke(null);
                    return true;
                }
            }
            return false;
        }

        public void UpdateProfile(string displayName, string avatarId, string bio)
        {
            _profileManager.UpdateProfile(displayName, avatarId, bio, LocalProfile.privacy);
        }

        public void PopulateSaveData(SaveData data)
        {
            // Social relationships persisted into ProgressionSaveData
        }

        public void RestoreFromSaveData(SaveData data)
        {
            // Restored from ProgressionSaveData
        }
    }
}

