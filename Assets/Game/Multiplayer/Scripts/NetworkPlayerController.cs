using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Smooths and interpolates remote player transforms, applies dead-reckoning extrapolation,
    /// and validates physical velocity to reject teleportation and speed-hack anomalies.
    /// </summary>
    public class NetworkPlayerController : MonoBehaviour
    {
        private const string Tag = "NetworkPlayerController";
        private const float MaxAllowedSpeedMps = 48.0f; // ~172 km/h (covers max bike top speed + lag jitter)
        private const float MaxAllowedTeleportDist = 30.0f;

        [SerializeField] private float _interpolationSpeed = 12.0f;

        private string _playerId;
        private PlayerNetworkState _latestState;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _targetVelocity;
        private float _lastPacketTime;
        private bool _isInitialized;

        public string PlayerId => _playerId;
        public PlayerNetworkState LatestState => _latestState;

        public void Initialize(string playerId, Vector3 spawnPos, Quaternion spawnRot)
        {
            _playerId = playerId;
            transform.position = spawnPos;
            transform.rotation = spawnRot;
            _targetPosition = spawnPos;
            _targetRotation = spawnRot;
            _targetVelocity = Vector3.zero;
            _lastPacketTime = Time.time;
            _isInitialized = true;
        }

        public bool ReceiveNetworkState(PlayerNetworkState newState)
        {
            if (!_isInitialized) return false;

            float dt = Mathf.Max(0.001f, Time.time - _lastPacketTime);
            _lastPacketTime = Time.time;

            // Anti-Speedhack & Anti-Teleportation Check
            float distanceJump = Vector3.Distance(_targetPosition, newState.position);
            float impliedSpeed = distanceJump / dt;

            if (distanceJump > MaxAllowedTeleportDist && impliedSpeed > MaxAllowedSpeedMps)
            {
                GameLogger.W(Tag, $"Suspicious position jump detected for player {_playerId}: {distanceJump:0.0}m in {dt:0.00}s ({impliedSpeed * 3.6f:0} km/h). Rejected.");
                return false;
            }

            _latestState = newState;
            _targetPosition = newState.position;
            _targetRotation = newState.rotation;
            _targetVelocity = newState.velocity;
            return true;
        }

        private void Update()
        {
            if (!_isInitialized) return;

            float dt = Time.deltaTime;

            // Smooth interpolation towards target position & rotation
            transform.position = Vector3.Lerp(transform.position, _targetPosition + _targetVelocity * (dt * 0.1f), dt * _interpolationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, dt * _interpolationSpeed);
        }
    }
}
