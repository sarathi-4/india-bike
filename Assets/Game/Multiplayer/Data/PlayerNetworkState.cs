using System;
using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Compact network state snapshot transmitted at 15-20 Hz to replicate
    /// motorcycle transform, physics state, lighting, and passenger presence across clients.
    /// </summary>
    [Serializable]
    public struct PlayerNetworkState
    {
        public uint sequenceNumber;
        public float timestamp;
        public string playerId;

        // Spatial Transform
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;

        // Dynamics & Simulation
        public float forwardSpeedKmph;
        public float steerAngle;
        public float bikeLeanRoll;
        public float engineRPM;
        public int gear;
        public bool isBraking;

        // Visual Signals & Audio
        public bool headlightOn;
        public bool hornHonking;

        // Passenger State
        public bool hasPassenger;
        public float passengerLeanRoll;

        // Gameplay Progress
        public int currentCheckpointIndex;
        public float damagePercent;

        public static PlayerNetworkState CreateDefault(string pid)
        {
            return new PlayerNetworkState
            {
                sequenceNumber = 0,
                timestamp = 0f,
                playerId = pid,
                position = Vector3.zero,
                rotation = Quaternion.identity,
                velocity = Vector3.zero,
                forwardSpeedKmph = 0f,
                steerAngle = 0f,
                bikeLeanRoll = 0f,
                engineRPM = 1000f,
                gear = 1,
                isBraking = false,
                headlightOn = false,
                hornHonking = false,
                hasPassenger = false,
                passengerLeanRoll = 0f,
                currentCheckpointIndex = 0,
                damagePercent = 0f
            };
        }
    }
}

