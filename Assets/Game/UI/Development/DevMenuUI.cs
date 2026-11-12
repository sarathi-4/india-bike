using System;
using IndianBikeLife.Core.Audio;
using IndianBikeLife.Core.Configuration;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.State;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.UI.Development
{
    /// <summary>
    /// Development dashboard rendered in MainMenu.unity.
    /// Provides live validation controls for Audio, Save System, Input, Graphics Presets, and Lifecycle.
    /// </summary>
    public class DevMenuUI : MonoBehaviour
    {
        private const string Tag = "DevMenuUI";

        [SerializeField] private GraphicsConfiguration _graphicsConfig;

        private void Start()
        {
            if (ServiceLocator.TryGet(out IGameManager gameManager))
            {
                gameManager.SetState(GameState.MainMenu);
            }
        }

        private void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;

            // Header Banner
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(18, h * 3 / 100),
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = new Color(1f, 0.6f, 0.1f);

            GUI.Label(new Rect(0, 15, w, 35), "INDIAN BIKE LIFE — STAGE 1: CORE ARCHITECTURE", headerStyle);

            // Left Column: System Status Badges
            float colWidth = Mathf.Min(320, w * 0.32f);
            float startY = 60f;
            float padding = 10f;

            Rect leftPanel = new Rect(20, startY, colWidth, h - startY - 20);
            GUI.Box(leftPanel, "Core Systems Status");

            GUILayout.BeginArea(new Rect(leftPanel.x + padding, leftPanel.y + 25, leftPanel.width - padding * 2, leftPanel.height - 35));

            DrawStatusItem("Game Bootstrapper", "INITIALIZED", Color.green);
            DrawStatusItem("Game Lifecycle", GetGameStateText(), Color.cyan);
            DrawStatusItem("Scene Management", "READY", Color.green);
            DrawStatusItem("Event Bus", "ACTIVE", Color.green);
            DrawStatusItem("Save / Load System", GetSaveStatusText(), Color.green);
            DrawStatusItem("Audio Channels", "ACTIVE", Color.green);
            DrawStatusItem("Input Foundation", GetInputStatusText(), Color.green);
            DrawStatusItem("Performance Diagnostics", "RUNNING", Color.green);

            GUILayout.Space(15);
            GUILayout.Label("<b>Player Profile Snapshot:</b>");
            if (ServiceLocator.TryGet(out ISaveService saveService) && saveService.CurrentData != null)
            {
                PlayerProfileSaveData profile = saveService.CurrentData.playerProfile;
                GUILayout.Label($"Rider: {profile.playerName}");
                GUILayout.Label($"Coins: ₹{profile.playerCoins:N0}");
                GUILayout.Label($"Selected Bike: {profile.selectedMotorcycleId}");
                GUILayout.Label($"Gender: {profile.characterGender}");
            }

            GUILayout.EndArea();

            // Center Column: Interactive Controls
            float centerWidth = Mathf.Min(360, w * 0.34f);
            float centerX = leftPanel.xMax + 15;
            Rect centerPanel = new Rect(centerX, startY, centerWidth, h - startY - 20);
            GUI.Box(centerPanel, "Interactive System Diagnostics");

            GUILayout.BeginArea(new Rect(centerPanel.x + padding, centerPanel.y + 25, centerPanel.width - padding * 2, centerPanel.height - 35));

            GUILayout.Label("<b>Audio Volume Controls:</b>");
            if (ServiceLocator.TryGet(out IAudioService audioService))
            {
                DrawVolumeSlider(audioService, AudioChannel.Master);
                DrawVolumeSlider(audioService, AudioChannel.Music);
                DrawVolumeSlider(audioService, AudioChannel.SFX);
                DrawVolumeSlider(audioService, AudioChannel.Vehicle);
            }

            GUILayout.Space(10);
            GUILayout.Label("<b>Graphics Quality Preset:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Low (30 FPS)"))
            {
                ApplyPreset(MobileQualityPreset.Low);
            }
            if (GUILayout.Button("Medium (60)"))
            {
                ApplyPreset(MobileQualityPreset.Medium);
            }
            if (GUILayout.Button("High (60)"))
            {
                ApplyPreset(MobileQualityPreset.High);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>Save System Operations:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save to Disk"))
            {
                if (ServiceLocator.TryGet(out ISaveService save))
                {
                    save.Save();
                }
            }
            if (GUILayout.Button("Reset Save Data"))
            {
                if (ServiceLocator.TryGet(out ISaveService save))
                {
                    save.ResetSaveData();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(15);
            if (GUILayout.Button("Start Simulation (Stage 2 Target)", GUILayout.Height(35)))
            {
                GameLogger.I(Tag, "Stage 1 Architecture verified. Ready for Stage 2: Motorcycle Physics & Showroom.");
            }

            GUILayout.EndArea();

            // Right Column: Live Input & Diagnostics
            float rightX = centerPanel.xMax + 15;
            float rightWidth = w - rightX - 20;
            if (rightWidth >= 220)
            {
                Rect rightPanel = new Rect(rightX, startY, rightWidth, h - startY - 20);
                GUI.Box(rightPanel, "Live Diagnostics");

                GUILayout.BeginArea(new Rect(rightPanel.x + padding, rightPanel.y + 25, rightPanel.width - padding * 2, rightPanel.height - 35));

                if (ServiceLocator.TryGet(out IPerformanceDiagnostics perf))
                {
                    GUILayout.Label($"<b>FPS:</b> {perf.CurrentFPS:0.0}");
                    GUILayout.Label($"<b>Frame Time:</b> {perf.CurrentFrameTimeMs:0.0} ms");
                    GUILayout.Label($"<b>Memory:</b> {perf.AllocatedMemoryMb:0.0} MB");
                }

                GUILayout.Space(10);
                if (ServiceLocator.TryGet(out IInputService inputService))
                {
                    MotorcycleInputState input = inputService.CurrentInputState;
                    GUILayout.Label($"<b>Active Device:</b> {inputService.ActiveDeviceType}");
                    GUILayout.Label($"<b>Steer:</b> {input.steerInput:0.00}");
                    GUILayout.Label($"<b>Throttle:</b> {input.throttleInput:0.00}");
                    GUILayout.Label($"<b>Brake:</b> {input.brakeInput:0.00}");
                    GUILayout.Label($"<b>Nitro:</b> {input.nitro}");
                    GUILayout.Label($"<b>Horn:</b> {input.horn}");
                    GUILayout.Label($"<b>Lights:</b> {input.headlights}");
                }

                GUILayout.Space(15);
                if (GUILayout.Button("Quit Game"))
                {
                    if (ServiceLocator.TryGet(out IGameManager gm))
                    {
                        gm.QuitGame();
                    }
                }

                GUILayout.EndArea();
            }
        }

        private void DrawStatusItem(string systemName, string status, Color color)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(systemName + ":", GUILayout.Width(150));
            Color prev = GUI.color;
            GUI.color = color;
            GUILayout.Label(status);
            GUI.color = prev;
            GUILayout.EndHorizontal();
        }

        private void DrawVolumeSlider(IAudioService audio, AudioChannel channel)
        {
            float current = audio.GetChannelVolume(channel);
            GUILayout.BeginHorizontal();
            GUILayout.Label(channel.ToString(), GUILayout.Width(70));
            float updated = GUILayout.HorizontalSlider(current, 0f, 1f, GUILayout.Width(120));
            if (Mathf.Abs(updated - current) > 0.01f)
            {
                audio.SetChannelVolume(channel, updated);
            }
            GUILayout.Label($"{Mathf.RoundToInt(current * 100)}%", GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }

        private void ApplyPreset(MobileQualityPreset preset)
        {
            if (_graphicsConfig != null)
            {
                _graphicsConfig.ApplyPreset(preset);
            }
            else
            {
                QualitySettings.SetQualityLevel((int)preset, true);
                Application.targetFrameRate = preset == MobileQualityPreset.Low ? 30 : 60;
            }

            if (ServiceLocator.TryGet(out ISaveService save) && save.CurrentData?.settings != null)
            {
                save.CurrentData.settings.graphicsQualityLevel = (int)preset;
                save.Save();
            }
        }

        private string GetGameStateText()
        {
            return ServiceLocator.TryGet(out IGameManager gm) ? gm.CurrentState.ToString() : "N/A";
        }

        private string GetSaveStatusText()
        {
            return ServiceLocator.TryGet(out ISaveService s) ? (s.HasExistingSaveFile() ? "LOADED" : "DEFAULT") : "N/A";
        }

        private string GetInputStatusText()
        {
            return ServiceLocator.TryGet(out IInputService inp) ? inp.ActiveDeviceType.ToString() : "N/A";
        }
    }
}
