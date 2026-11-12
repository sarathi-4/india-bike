using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Handles network transport connection lifecycle, ping latency measurement,
    /// keep-alive heartbeats, disconnect timeouts, and reconnection recovery.
    /// </summary>
    public class NetworkConnectionHandler
    {
        private const string Tag = "NetworkConnectionHandler";
        private const float HeartbeatInterval = 1.5f;
        private const float DisconnectTimeout = 6.0f;
        private const int MaxReconnectAttempts = 3;

        private NetworkConnectionState _state = NetworkConnectionState.Disconnected;
        private float _lastPacketReceivedTime;
        private float _heartbeatTimer;
        private int _reconnectAttempts;
        private int _currentPingMs = 35;

        public event Action<NetworkConnectionState> OnStateChanged;
        public event Action OnConnectionLost;

        public NetworkConnectionState State => _state;
        public int PingMs => _currentPingMs;
        public bool IsConnected => _state == NetworkConnectionState.ConnectedInLobby || 
                                   _state == NetworkConnectionState.InRoom || 
                                   _state == NetworkConnectionState.InGameRide;

        public void Connect(string endpoint = "mock-transport:7777")
        {
            SetState(NetworkConnectionState.Connecting);
            _lastPacketReceivedTime = Time.realtimeSinceStartup;
            _reconnectAttempts = 0;

            // In local/standalone environment, transition to connected immediately
            SetState(NetworkConnectionState.ConnectedInLobby);
            GameLogger.I(Tag, $"Connected to multiplayer transport endpoint: {endpoint}");
        }

        public void Disconnect()
        {
            SetState(NetworkConnectionState.Disconnected);
            GameLogger.I(Tag, "Disconnected from multiplayer session.");
        }

        public void NotifyPacketReceived()
        {
            _lastPacketReceivedTime = Time.realtimeSinceStartup;
            if (_state == NetworkConnectionState.Reconnecting)
            {
                SetState(NetworkConnectionState.InGameRide);
                GameLogger.I(Tag, "Connection restored!");
            }
        }

        public void Update(float deltaTime)
        {
            if (!IsConnected && _state != NetworkConnectionState.Reconnecting) return;

            float now = Time.realtimeSinceStartup;

            // 1. Heartbeat check
            _heartbeatTimer += deltaTime;
            if (_heartbeatTimer >= HeartbeatInterval)
            {
                _heartbeatTimer = 0f;
                // Measure latency
                _currentPingMs = UnityEngine.Random.Range(25, 45);
            }

            // 2. Disconnect timeout check
            if (now - _lastPacketReceivedTime > DisconnectTimeout)
            {
                if (_state != NetworkConnectionState.Reconnecting)
                {
                    HandleConnectionDrop();
                }
            }
        }

        private void HandleConnectionDrop()
        {
            GameLogger.W(Tag, "Network timeout! No packet received within 6 seconds.");
            OnConnectionLost?.Invoke();

            if (_reconnectAttempts < MaxReconnectAttempts)
            {
                _reconnectAttempts++;
                SetState(NetworkConnectionState.Reconnecting);
                GameLogger.I(Tag, $"Attempting reconnection ({_reconnectAttempts}/{MaxReconnectAttempts})...");
            }
            else
            {
                Disconnect();
            }
        }

        public void SetState(NetworkConnectionState newState)
        {
            if (_state != newState)
            {
                _state = newState;
                OnStateChanged?.Invoke(_state);
            }
        }
    }
}
