using System;
using System.Collections.Generic;
using IndianBikeLife.Social;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for player social profiles, friend relationships, and riding clubs.
    /// </summary>
    public interface ISocialService : IGameService
    {
        SocialProfileData LocalProfile { get; }

        // Friends
        IReadOnlyList<FriendEntry> GetFriendsList();
        IReadOnlyList<FriendRequest> GetPendingRequests();
        bool SendFriendRequest(string targetPlayerId);
        bool AcceptFriendRequest(string requestId);
        bool RejectFriendRequest(string requestId);
        bool RemoveFriend(string targetPlayerId);
        bool BlockPlayer(string targetPlayerId);
        bool UnblockPlayer(string targetPlayerId);
        bool IsPlayerBlocked(string targetPlayerId);

        // Clubs
        ClubData MyClub { get; }
        bool CreateClub(string clubName, string description, string tag);
        bool JoinClub(string clubId);
        bool LeaveClub();

        // Profile Updates
        void UpdateProfile(string displayName, string avatarId, string bio);

        event Action OnFriendsListUpdated;
        event Action<ClubData> OnClubUpdated;
    }
}

