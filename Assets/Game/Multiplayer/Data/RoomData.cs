using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    [Serializable]
    public class RoomPlayerSlot
    {
        public string playerId;
        public string playerName;
        public int slotIndex; // 0 to 3
        public bool isHost;
        public bool isReady;
        public string selectedBikeId;
        public bool hasPassenger;
        public int pingMs;

        public RoomPlayerSlot() { }

        public RoomPlayerSlot(string pid, string name, int slot, bool host)
        {
            playerId = pid;
            playerName = name;
            slotIndex = slot;
            isHost = host;
            isReady = host; // Host is ready by default
            selectedBikeId = "bike_classic_350";
            hasPassenger = false;
            pingMs = 30;
        }
    }

    /// <summary>
    /// Represents a private multiplayer room session limited to 4 players.
    /// </summary>
    [Serializable]
    public class RoomData
    {
        public const int MaxRoomPlayers = 4;

        public string roomCode;
        public string roomName;
        public string hostPlayerId;
        public string passwordHash = "";
        public bool isPrivate = false;
        public int maxPlayers = MaxRoomPlayers;
        public string gameModeId = "MultiplayerFreeRide";
        public bool isRideActive = false;

        public List<RoomPlayerSlot> players = new List<RoomPlayerSlot>();

        public RoomData() { }

        public RoomData(string code, string name, string hostId, int maxP = MaxRoomPlayers, string mode = "MultiplayerFreeRide")
        {
            roomCode = code;
            roomName = name;
            hostPlayerId = hostId;
            maxPlayers = Mathf.Clamp(maxP, 2, MaxRoomPlayers);
            gameModeId = mode;
            isRideActive = false;
        }

        public bool IsFull => players.Count >= maxPlayers;

        public bool AreAllPlayersReady()
        {
            if (players.Count == 0) return false;
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].isReady) return false;
            }
            return true;
        }

        public RoomPlayerSlot FindPlayer(string playerId)
        {
            return players.Find(p => p.playerId == playerId);
        }

        public int GetNextAvailableSlot()
        {
            for (int slot = 0; slot < maxPlayers; slot++)
            {
                if (!players.Exists(p => p.slotIndex == slot))
                    return slot;
            }
            return -1;
        }
    }
}

