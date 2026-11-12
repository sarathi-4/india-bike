using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.Multiplayer.UI
{
    /// <summary>
    /// UI panel for hosting and joining 4-player private multiplayer rooms,
    /// toggling ready state, and starting group rides.
    /// </summary>
    public class MultiplayerLobbyUI : MonoBehaviour
    {
        private IMultiplayerService _multiplayerService;
        private string _inputRoomCode = "";
        private string _inputRoomName = "NH44 Group Ride";
        private string _inputPassword = "";
        private bool _isOpen = false;

        private void Start()
        {
            ServiceLocator.TryGet(out _multiplayerService);
        }

        public void ToggleOpen()
        {
            _isOpen = !_isOpen;
        }

        public void SetOpen(bool open)
        {
            _isOpen = open;
        }

        private void OnGUI()
        {
            if (!_isOpen || _multiplayerService == null) return;

            int w = Screen.width;
            int h = Screen.height;

            float panelWidth = Mathf.Min(480f, w * 0.85f);
            float panelHeight = Mathf.Min(420f, h * 0.85f);
            Rect dialogRect = new Rect((w - panelWidth) * 0.5f, (h - panelHeight) * 0.5f, panelWidth, panelHeight);

            GUI.Box(dialogRect, "<b>MULTIPLAYER GROUP RIDE LOBBY (4-PLAYER)</b>");

            GUILayout.BeginArea(new Rect(dialogRect.x + 15, dialogRect.y + 30, dialogRect.width - 30, dialogRect.height - 40));

            var room = _multiplayerService.CurrentRoom;

            if (room == null)
            {
                // Not in room: Show Create / Join controls
                GUILayout.Label("<b>Create Private Room:</b>");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", GUILayout.Width(50));
                _inputRoomName = GUILayout.TextField(_inputRoomName, GUILayout.Width(180));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Pass:", GUILayout.Width(50));
                _inputPassword = GUILayout.PasswordField(_inputPassword, '*', 12, GUILayout.Width(180));
                if (GUILayout.Button("Create Room", GUILayout.Height(26)))
                {
                    _multiplayerService.CreateRoom(_inputRoomName, _inputPassword, 4);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(12);
                GUILayout.Label("<b>Join Existing Room:</b>");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Code:", GUILayout.Width(50));
                _inputRoomCode = GUILayout.TextField(_inputRoomCode.ToUpperInvariant(), 6, GUILayout.Width(100));
                if (GUILayout.Button("Join by Code", GUILayout.Height(26)))
                {
                    _multiplayerService.JoinRoom(_inputRoomCode, _inputPassword);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                // In room: Show Room details & player roster
                GUILayout.Label($"Room: <b>{room.roomName}</b> | Code: <color=yellow><b>{room.roomCode}</b></color>");
                GUILayout.Label($"Players: <b>{room.players.Count} / {room.maxPlayers}</b> (Max 4)");
                GUILayout.Space(6);

                GUILayout.Label("<b>Rider Roster:</b>");
                for (int i = 0; i < room.players.Count; i++)
                {
                    var p = room.players[i];
                    string hostTag = p.isHost ? " [HOST]" : "";
                    string readyTag = p.isReady ? "<color=green>[READY]</color>" : "<color=orange>[NOT READY]</color>";
                    GUILayout.Label($"• Slot {p.slotIndex + 1}: <b>{p.playerName}</b>{hostTag} - {readyTag} ({p.pingMs}ms)");
                }

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                bool myReady = room.FindPlayer(_multiplayerService.LocalPlayerId)?.isReady ?? false;
                if (GUILayout.Button(myReady ? "Cancel Ready" : "Ready Up", GUILayout.Height(32)))
                {
                    _multiplayerService.SetPlayerReady(!myReady);
                }

                if (_multiplayerService.IsHost)
                {
                    if (GUILayout.Button("Start Group Ride", GUILayout.Height(32)))
                    {
                        if (_multiplayerService.StartGroupRide())
                        {
                            _isOpen = false;
                        }
                    }
                }

                if (GUILayout.Button("Leave Room", GUILayout.Height(32)))
                {
                    _multiplayerService.LeaveRoom();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close Lobby Window", GUILayout.Height(26)))
            {
                _isOpen = false;
            }

            GUILayout.EndArea();
        }
    }
}

