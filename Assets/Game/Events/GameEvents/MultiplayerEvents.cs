using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Multiplayer;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct RoomCreatedEvent : IGameEvent
    {
        public RoomData Room { get; }
        public RoomCreatedEvent(RoomData room) => Room = room;
    }

    public readonly struct RoomJoinedEvent : IGameEvent
    {
        public RoomData Room { get; }
        public RoomJoinedEvent(RoomData room) => Room = room;
    }

    public readonly struct RoomLeftEvent : IGameEvent
    {
        public string RoomCode { get; }
        public RoomLeftEvent(string roomCode) => RoomCode = roomCode;
    }

    public readonly struct PlayerJoinedRoomEvent : IGameEvent
    {
        public string PlayerId { get; }
        public string PlayerName { get; }
        public PlayerJoinedRoomEvent(string pid, string name)
        {
            PlayerId = pid;
            PlayerName = name;
        }
    }

    public readonly struct PlayerLeftRoomEvent : IGameEvent
    {
        public string PlayerId { get; }
        public PlayerLeftRoomEvent(string pid) => PlayerId = pid;
    }

    public readonly struct MultiplayerRideStartedEvent : IGameEvent
    {
        public RoomData Room { get; }
        public MultiplayerRideStartedEvent(RoomData room) => Room = room;
    }
}
