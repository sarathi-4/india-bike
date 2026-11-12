using System;
using System.Collections.Generic;
using IndianBikeLife.Multiplayer;

namespace IndianBikeLife.Core.Interfaces
{
    public enum NetworkConnectionState
    {
        Disconnected,
        Connecting,
        ConnectedInLobby,
        InRoom,
        InGameRide,
        Reconnecting
    }

    /// <summary>
    /// Contract for online multiplayer networking, private rooms, and real-time riding state replication.
    /// </summary>
    public interface IMultiplayerService : IGameService
    {
        NetworkConnectionState ConnectionState { get; }
        RoomData CurrentRoom { get; }
        bool IsHost { get; }
        string LocalPlayerId { get; }
        int LocalSlotIndex { get; }

        bool CreateRoom(string roomName, string password = "", int maxPlayers = 4, string modeId = "MultiplayerFreeRide");
        bool JoinRoom(string roomCode, string password = "");
        void LeaveRoom();
        void SetPlayerReady(bool isReady);
        bool StartGroupRide();
        void BroadcastPlayerState(PlayerNetworkState state);

        event Action<NetworkConnectionState> OnConnectionStateChanged;
        event Action<RoomData> OnRoomUpdated;
        event Action<PlayerNetworkState> OnRemotePlayerStateReceived;
        event Action<string, string> OnPlayerJoinedRoom; // id, name
        event Action<string> OnPlayerLeftRoom;           // id
        event Action OnRideStarted;
    }
}

