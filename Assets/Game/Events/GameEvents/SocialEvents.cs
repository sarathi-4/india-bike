using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Social;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct FriendRequestSentEvent : IGameEvent
    {
        public string TargetPlayerId { get; }
        public FriendRequestSentEvent(string target) => TargetPlayerId = target;
    }

    public readonly struct FriendAddedEvent : IGameEvent
    {
        public FriendEntry Friend { get; }
        public FriendAddedEvent(FriendEntry friend) => Friend = friend;
    }

    public readonly struct FriendRemovedEvent : IGameEvent
    {
        public string FriendPlayerId { get; }
        public FriendRemovedEvent(string pid) => FriendPlayerId = pid;
    }

    public readonly struct PlayerBlockedEvent : IGameEvent
    {
        public string BlockedPlayerId { get; }
        public PlayerBlockedEvent(string pid) => BlockedPlayerId = pid;
    }

    public readonly struct ClubJoinedEvent : IGameEvent
    {
        public ClubData Club { get; }
        public ClubJoinedEvent(ClubData club) => Club = club;
    }

    public readonly struct ClubLeftEvent : IGameEvent
    {
        public string ClubId { get; }
        public ClubLeftEvent(string cid) => ClubId = cid;
    }
}

