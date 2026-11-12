using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Manages room creation, joining, slot allocation (up to 4 players),
    /// password authentication, host delegation, and ride start authorization.
    /// </summary>
    public class RoomManager
    {
        private const string Tag = "RoomManager";

        private readonly Dictionary<string, RoomData> _activeRooms = new Dictionary<string, RoomData>();
        private RoomData _currentRoom;

        public RoomData CurrentRoom => _currentRoom;
        public bool IsInRoom => _currentRoom != null;

        public RoomData CreateRoom(string roomName, string hostId, string hostName, string password = "", int maxPlayers = 4, string modeId = "MultiplayerFreeRide")
        {
            string code;
            int attempts = 0;
            do
            {
                code = RoomCodeManager.GenerateRoomCode();
                attempts++;
            } while (_activeRooms.ContainsKey(code) && attempts < 100);

            var room = new RoomData(code, roomName, hostId, Mathf.Clamp(maxPlayers, 2, RoomData.MaxRoomPlayers), modeId);
            if (!string.IsNullOrEmpty(password))
            {
                room.passwordHash = HashPassword(password);
                room.isPrivate = true;
            }

            var hostSlot = new RoomPlayerSlot(hostId, hostName, 0, isHost: true);
            room.players.Add(hostSlot);

            _activeRooms[code] = room;
            _currentRoom = room;

            GameLogger.I(Tag, $"Created room '{room.roomName}' [Code: {code}] with host {hostName} (Max: {room.maxPlayers})");
            return room;
        }

        public bool JoinRoom(string roomCode, string playerId, string playerName, string password, out string failureReason)
        {
            failureReason = string.Empty;
            string code = RoomCodeManager.SanitizeCode(roomCode);

            if (!_activeRooms.TryGetValue(code, out var room))
            {
                failureReason = "Room code not found.";
                return false;
            }

            if (room.IsFull)
            {
                failureReason = "Room is full (maximum 4 players allowed).";
                return false;
            }

            if (room.isRideActive)
            {
                failureReason = "Ride is already in progress.";
                return false;
            }

            if (room.isPrivate)
            {
                if (string.IsNullOrEmpty(password) || HashPassword(password) != room.passwordHash)
                {
                    failureReason = "Incorrect room password.";
                    return false;
                }
            }

            if (room.FindPlayer(playerId) != null)
            {
                // Already in room (reconnect scenario)
                _currentRoom = room;
                return true;
            }

            int availableSlot = room.GetNextAvailableSlot();
            var newSlot = new RoomPlayerSlot(playerId, playerName, availableSlot, isHost: false);
            room.players.Add(newSlot);
            _currentRoom = room;

            GameLogger.I(Tag, $"Player {playerName} joined room {code} (Slot {availableSlot}, {room.players.Count}/{room.maxPlayers})");
            return true;
        }

        public void LeaveRoom(string playerId)
        {
            if (_currentRoom == null) return;

            var slot = _currentRoom.FindPlayer(playerId);
            if (slot != null)
            {
                _currentRoom.players.Remove(slot);
                GameLogger.I(Tag, $"Player {slot.playerName} left room {_currentRoom.roomCode}.");

                // If room is empty, dismantle it
                if (_currentRoom.players.Count == 0)
                {
                    _activeRooms.Remove(_currentRoom.roomCode);
                    _currentRoom = null;
                    return;
                }

                // If host left, migrate host authority to the next player
                if (slot.isHost)
                {
                    var newHost = _currentRoom.players[0];
                    newHost.isHost = true;
                    _currentRoom.hostPlayerId = newHost.playerId;
                    GameLogger.I(Tag, $"Host migrated to {newHost.playerName}.");
                }
            }

            _currentRoom = null;
        }

        public bool SetReady(string playerId, bool isReady)
        {
            if (_currentRoom == null) return false;

            var slot = _currentRoom.FindPlayer(playerId);
            if (slot != null)
            {
                slot.isReady = isReady;
                return true;
            }
            return false;
        }

        public bool StartRide(string requestingPlayerId, out string failureReason)
        {
            failureReason = string.Empty;
            if (_currentRoom == null)
            {
                failureReason = "No active room.";
                return false;
            }

            if (_currentRoom.hostPlayerId != requestingPlayerId)
            {
                failureReason = "Only the room host can start the ride.";
                return false;
            }

            if (!_currentRoom.AreAllPlayersReady())
            {
                failureReason = "All players must be Ready before starting.";
                return false;
            }

            _currentRoom.isRideActive = true;
            GameLogger.I(Tag, $"Starting multiplayer ride in room {_currentRoom.roomCode} with {_currentRoom.players.Count} riders!");
            return true;
        }

        private static string HashPassword(string raw)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes("IBL_SALT_" + raw));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}

