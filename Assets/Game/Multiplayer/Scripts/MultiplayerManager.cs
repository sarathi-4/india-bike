using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Master multiplayer service implementing IMultiplayerService and IGameService.
    /// Coordinates room sessions (up to 4 players), local network state broadcasting at 15-20 Hz,
    /// and manages remote player surrogate instances.
    /// </summary>
    public class MultiplayerManager : MonoBehaviour, IMultiplayerService
    {
        private const string Tag = "MultiplayerManager";
        private const float BroadcastInterval = 0.066f; // ~15 Hz broadcast rate for mobile bandwidth

        [SerializeField] private GameObject _remoteRiderPrefab;

        private RoomManager _roomManager;
        private NetworkConnectionHandler _connectionHandler;
        private readonly Dictionary<string, NetworkPlayerController> _remotePlayers = new Dictionary<string, NetworkPlayerController>();

        private string _localPlayerId = "local_rider_1";
        private string _localPlayerName = "Indian Rider";
        private float _broadcastTimer;
        private uint _localSequenceNumber;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public NetworkConnectionState ConnectionState => _connectionHandler?.State ?? NetworkConnectionState.Disconnected;
        public RoomData CurrentRoom => _roomManager?.CurrentRoom;
        public bool IsHost => CurrentRoom != null && CurrentRoom.hostPlayerId == _localPlayerId;
        public string LocalPlayerId => _localPlayerId;
        public int LocalSlotIndex => CurrentRoom?.FindPlayer(_localPlayerId)?.slotIndex ?? 0;

        public event Action<NetworkConnectionState> OnConnectionStateChanged;
        public event Action<RoomData> OnRoomUpdated;
        public event Action<PlayerNetworkState> OnRemotePlayerStateReceived;
        public event Action<string, string> OnPlayerJoinedRoom;
        public event Action<string> OnPlayerLeftRoom;
        public event Action OnRideStarted;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _roomManager = new RoomManager();
            _connectionHandler = new NetworkConnectionHandler();
            _connectionHandler.OnStateChanged += HandleConnectionStateChanged;
            _connectionHandler.Connect();

            ServiceLocator.Register<IMultiplayerService>(this);
            _isInitialized = true;
            GameLogger.I(Tag, "MultiplayerManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            LeaveRoom();
            _connectionHandler?.Disconnect();

            ServiceLocator.Unregister<IMultiplayerService>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public void SetLocalIdentity(string playerId, string playerName)
        {
            if (!string.IsNullOrEmpty(playerId)) _localPlayerId = playerId;
            if (!string.IsNullOrEmpty(playerName)) _localPlayerName = playerName;
        }

        public bool CreateRoom(string roomName, string password = "", int maxPlayers = 4, string modeId = "MultiplayerFreeRide")
        {
            var room = _roomManager.CreateRoom(roomName, _localPlayerId, _localPlayerName, password, maxPlayers, modeId);
            if (room != null)
            {
                _connectionHandler.SetState(NetworkConnectionState.InRoom);
                EventBus.Publish(new RoomCreatedEvent(room));
                OnRoomUpdated?.Invoke(room);
                return true;
            }
            return false;
        }

        public bool JoinRoom(string roomCode, string password = "")
        {
            if (_roomManager.JoinRoom(roomCode, _localPlayerId, _localPlayerName, password, out string failReason))
            {
                _connectionHandler.SetState(NetworkConnectionState.InRoom);
                var room = _roomManager.CurrentRoom;
                EventBus.Publish(new RoomJoinedEvent(room));
                OnRoomUpdated?.Invoke(room);
                return true;
            }

            GameLogger.W(Tag, $"Failed to join room {roomCode}: {failReason}");
            return false;
        }

        public void LeaveRoom()
        {
            if (CurrentRoom != null)
            {
                string code = CurrentRoom.roomCode;
                _roomManager.LeaveRoom(_localPlayerId);
                _connectionHandler.SetState(NetworkConnectionState.ConnectedInLobby);
                EventBus.Publish(new RoomLeftEvent(code));
                ClearRemotePlayers();
                OnRoomUpdated?.Invoke(null);
            }
        }

        public void SetPlayerReady(bool isReady)
        {
            if (_roomManager.SetReady(_localPlayerId, isReady))
            {
                OnRoomUpdated?.Invoke(CurrentRoom);
            }
        }

        public bool StartGroupRide()
        {
            if (_roomManager.StartRide(_localPlayerId, out string failReason))
            {
                _connectionHandler.SetState(NetworkConnectionState.InGameRide);
                EventBus.Publish(new MultiplayerRideStartedEvent(CurrentRoom));
                OnRideStarted?.Invoke();
                return true;
            }

            GameLogger.W(Tag, $"Cannot start group ride: {failReason}");
            return false;
        }

        public void BroadcastPlayerState(PlayerNetworkState state)
        {
            if (ConnectionState != NetworkConnectionState.InGameRide) return;

            state.sequenceNumber = ++_localSequenceNumber;
            state.timestamp = Time.time;
            state.playerId = _localPlayerId;

            // In local/peer simulation, packet is delivered to remote surrogates
            _connectionHandler.NotifyPacketReceived();
        }

        public void ReceiveRemoteState(PlayerNetworkState state)
        {
            if (state.playerId == _localPlayerId) return;

            if (!_remotePlayers.TryGetValue(state.playerId, out var controller))
            {
                controller = SpawnRemoteRider(state.playerId, state.position, state.rotation);
                _remotePlayers[state.playerId] = controller;
            }

            if (controller != null)
            {
                controller.ReceiveNetworkState(state);
            }

            _connectionHandler.NotifyPacketReceived();
            OnRemotePlayerStateReceived?.Invoke(state);
        }

        private NetworkPlayerController SpawnRemoteRider(string playerId, Vector3 pos, Quaternion rot)
        {
            GameObject go;
            if (_remoteRiderPrefab != null)
            {
                go = Instantiate(_remoteRiderPrefab, pos, rot);
            }
            else
            {
                // Fallback remote placeholder
                go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.name = $"RemoteRider_{playerId}";
                var rb = go.GetComponent<Collider>();
                if (rb != null) rb.isTrigger = true;
            }

            var controller = go.GetComponent<NetworkPlayerController>();
            if (controller == null) controller = go.AddComponent<NetworkPlayerController>();

            controller.Initialize(playerId, pos, rot);
            GameLogger.I(Tag, $"Spawned remote rider instance for player {playerId} at {pos}");
            return controller;
        }

        private void ClearRemotePlayers()
        {
            foreach (var kvp in _remotePlayers)
            {
                if (kvp.Value != null) Destroy(kvp.Value.gameObject);
            }
            _remotePlayers.Clear();
        }

        private void HandleConnectionStateChanged(NetworkConnectionState state)
        {
            OnConnectionStateChanged?.Invoke(state);
        }

        private void Update()
        {
            if (!_isInitialized) return;

            float dt = Time.deltaTime;
            _connectionHandler?.Update(dt);

            // Tick broadcast rate
            _broadcastTimer += dt;
            if (_broadcastTimer >= BroadcastInterval)
            {
                _broadcastTimer = 0f;
                // Periodic state broadcast handled by gameplay driver
            }
        }
    }
}

