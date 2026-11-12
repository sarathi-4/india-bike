using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.MissionSystem;
using IndianBikeLife.PassengerSystem;
using IndianBikeLife.Progression.Economy;
using IndianBikeLife.Progression.Rewards;
using IndianBikeLife.RidingModes;
using UnityEngine;

namespace IndianBikeLife.Core.UI.Development
{
    /// <summary>
    /// Interactive HUD overlay for the Regional Ride Test environment.
    /// Provides live controls for Passengers, Weather, Time, Missions, Economy,
    /// 4-Player Multiplayer, Social Friends, Quick Chat, and Cloud Saves.
    /// </summary>
    public class RegionalRideTestHUD : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private PassengerCameraController _cameraController;
        [SerializeField] private PassengerSeatPosition _motorcycleSeat;
        [SerializeField] private MissionCatalogue _missionCatalogue;

        private IPassengerManager _passengerManager;
        private IWeatherManager _weatherManager;
        private IDayNightManager _dayNightManager;
        private ITrafficManager _trafficManager;
        private INavigationManager _navManager;
        private IRidingModeManager _ridingModeManager;
        private IMissionManager _missionManager;
        private IEconomyManager _economyManager;
        private IProgressionManager _progressionManager;
        private IAchievementManager _achievementManager;
        private LoginRewardManager _loginRewardManager;

        // Stage 4 Subsystems
        private IMultiplayerService _multiplayerService;
        private ISocialService _socialService;
        private IChatService _chatService;
        private IVoiceChatService _voiceChatService;
        private IBackendService _backendService;

        private bool _showHUD = false;

        private void Start()
        {
            ServiceLocator.TryGet(out _passengerManager);
            ServiceLocator.TryGet(out _weatherManager);
            ServiceLocator.TryGet(out _dayNightManager);
            ServiceLocator.TryGet(out _trafficManager);
            ServiceLocator.TryGet(out _navManager);
            ServiceLocator.TryGet(out _ridingModeManager);
            ServiceLocator.TryGet(out _missionManager);
            ServiceLocator.TryGet(out _economyManager);
            ServiceLocator.TryGet(out _progressionManager);
            ServiceLocator.TryGet(out _achievementManager);
            ServiceLocator.TryGet(out _loginRewardManager);

            // Stage 4 Services
            ServiceLocator.TryGet(out _multiplayerService);
            ServiceLocator.TryGet(out _socialService);
            ServiceLocator.TryGet(out _chatService);
            ServiceLocator.TryGet(out _voiceChatService);
            ServiceLocator.TryGet(out _backendService);

            // Auto-mount passenger on start if configured
            if (_passengerManager != null && _motorcycleSeat != null)
            {
                _passengerManager.MountPassenger(_motorcycleSeat);
            }
        }

        private void OnGUI()
        {
            if (IndianBikeLife.Core.UI.MasterGameFlowUI.Instance != null)
            {
                return; // Suppress debug HUD completely - MasterGameFlowUI renders the unified 9-screen AAA experience
            }

            if (GUI.Button(new Rect(15, 88, 110, 26), _showHUD ? "✖ Close Tools" : "🛠️ Debug Tools"))
            {
                _showHUD = !_showHUD;
            }

            if (!_showHUD) return;

            int w = Screen.width;
            int h = Screen.height;

            // Top Banner
            GUIStyle bannerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(16, h * 24 / 1000),
                fontStyle = FontStyle.Bold
            };
            bannerStyle.normal.textColor = new Color(1f, 0.65f, 0.15f);

            GUI.Label(new Rect(0, 8, w, 26), "INDIAN BIKE LIFE — STAGE 4: MULTIPLAYER, SOCIAL & BACKEND", bannerStyle);

            // Left Quick-Control Panel
            float panelWidth = Mathf.Min(310, w * 0.28f);
            float startY = 40f;
            float panelHeight = h - startY - 15;

            Rect controlRect = new Rect(15, startY, panelWidth, panelHeight);
            GUI.Box(controlRect, "Stage 2, 3 & 4 Controls");

            GUILayout.BeginArea(new Rect(controlRect.x + 8, controlRect.y + 22, controlRect.width - 16, controlRect.height - 28));

            // 1. Stage 4 Multiplayer Controls
            GUILayout.Label("<b>1. Multiplayer (4-Player Rooms):</b>");
            var room = _multiplayerService?.CurrentRoom;
            if (room != null)
            {
                GUILayout.Label($"Room: <b>{room.roomCode}</b> ({room.players.Count}/4 Riders)");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Ready")) _multiplayerService.SetPlayerReady(true);
                if (_multiplayerService.IsHost && GUILayout.Button("Start Ride")) _multiplayerService.StartGroupRide();
                if (GUILayout.Button("Leave")) _multiplayerService.LeaveRoom();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Host 4P Room"))
                {
                    _multiplayerService?.CreateRoom("NH44 Tour", "", 4);
                }
                if (GUILayout.Button("Join Code"))
                {
                    _multiplayerService?.JoinRoom("RJ44BK");
                }
                GUILayout.EndHorizontal();
            }

            // 2. Chat & Communication
            GUILayout.Space(4);
            GUILayout.Label("<b>2. Communication & Voice:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Quick Chat: Horn OK!"))
            {
                _chatService?.SendQuickChat(0);
            }
            bool micMuted = _voiceChatService?.IsMicMuted ?? true;
            if (GUILayout.Button(micMuted ? "Unmute Mic" : "Mute Mic"))
            {
                _voiceChatService?.SetMicMute(!micMuted);
            }
            GUILayout.EndHorizontal();

            // 3. Backend & Cloud
            GUILayout.Space(4);
            GUILayout.Label("<b>3. Backend & Cloud Save:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Guest Login"))
            {
                _backendService?.LoginAsGuest(null);
            }
            if (GUILayout.Button("Cloud Sync"))
            {
                _backendService?.SyncOfflineQueue(null);
            }
            GUILayout.EndHorizontal();

            // 4. Missions
            GUILayout.Space(4);
            GUILayout.Label("<b>4. Mission Controls:</b>");
            if (_missionManager != null && _missionManager.CurrentStatus == MissionStatus.Active)
            {
                GUILayout.Label($"Active: <b>{_missionManager.ActiveMission?.Title}</b>");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Complete")) _missionManager.CompleteActiveMission();
                if (GUILayout.Button("Fail")) _missionManager.FailActiveMission("Player failed mission.");
                if (GUILayout.Button("Cancel")) _missionManager.CancelActiveMission();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Highway Sprint")) StartSampleMission(MissionCategory.Highway);
                if (GUILayout.Button("Village Run")) StartSampleMission(MissionCategory.VillageRiding);
                GUILayout.EndHorizontal();
            }

            // 5. Economy & XP
            GUILayout.Space(4);
            GUILayout.Label("<b>5. Wallet & Progression:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+500 Silver")) _economyManager?.AddCurrency(CurrencyType.SilverCoins, 500, "Debug Credit");
            if (GUILayout.Button("+350 XP")) _progressionManager?.AddXP(350, "Debug Ride XP");
            GUILayout.EndHorizontal();

            // 6. Passenger & Views
            GUILayout.Space(4);
            GUILayout.Label("<b>6. Passenger & Camera:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Mount Pillion"))
            {
                if (_passengerManager != null && _motorcycleSeat != null) _passengerManager.MountPassenger(_motorcycleSeat);
            }
            if (GUILayout.Button("Dismount")) _passengerManager?.DismountPassenger();
            if (GUILayout.Button("Cam POV")) _cameraController?.CycleCameraView();
            GUILayout.EndHorizontal();

            // 7. Stage 5: Android Quality & Optimization
            GUILayout.Space(4);
            GUILayout.Label("<b>7. Quality & Optimization:</b>");
            if (ServiceLocator.TryGet<IQualityService>(out var qualityService))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(qualityService.CurrentPreset == IndianBikeLife.Core.Configuration.MobileQualityPreset.Low ? "[Low 30]" : "Low"))
                    qualityService.SetQualityPreset(IndianBikeLife.Core.Configuration.MobileQualityPreset.Low);
                if (GUILayout.Button(qualityService.CurrentPreset == IndianBikeLife.Core.Configuration.MobileQualityPreset.Medium ? "[Med 60]" : "Med"))
                    qualityService.SetQualityPreset(IndianBikeLife.Core.Configuration.MobileQualityPreset.Medium);
                if (GUILayout.Button(qualityService.CurrentPreset == IndianBikeLife.Core.Configuration.MobileQualityPreset.High ? "[High 60]" : "High"))
                    qualityService.SetQualityPreset(IndianBikeLife.Core.Configuration.MobileQualityPreset.High);
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Trigger Memory Sweep (GC)"))
            {
                if (ServiceLocator.TryGet<IndianBikeLife.Core.Managers.MobilePerformanceOptimizer>(out var opt))
                    opt.TriggerSceneTransitionCleanup();
            }

            GUILayout.EndArea();

            // Top Right Progression & Economy Telemetry
            float topW = 280f;
            float topH = 135f;
            Rect topRect = new Rect(w - topW - 15, 40, topW, topH);
            GUI.Box(topRect, "Player Progression & Wallet");
            GUILayout.BeginArea(new Rect(topRect.x + 8, topRect.y + 20, topRect.width - 16, topRect.height - 25));

            int silver = _economyManager?.SilverCoins ?? 0;
            int gold = _economyManager?.GoldCoins ?? 0;
            int level = _progressionManager?.CurrentLevel ?? 1;
            int xp = _progressionManager?.CurrentXP ?? 0;
            int completedMissions = _missionManager?.GetCompletedMissionCount() ?? 0;
            int achCount = _achievementManager?.UnlockedAchievementCount ?? 0;

            GUILayout.Label($"🪙 Silver: <b>{silver:N0}</b>  |  🟡 Gold: <b>{gold:N0}</b>");
            GUILayout.Label($"⭐ Level: <b>{level}</b> (Total XP: <b>{xp:N0}</b>)");
            GUILayout.Label($"🏆 Achievements: <b>{achCount} / {_achievementManager?.TotalAchievementCount ?? 8}</b>");
            GUILayout.Label($"🎯 Missions Completed: <b>{completedMissions}</b>");

            GUILayout.EndArea();

            // Bottom Right Status Info
            float statusW = 280f;
            float statusH = 130f;
            Rect statusRect = new Rect(w - statusW - 15, h - statusH - 15, statusW, statusH);
            GUI.Box(statusRect, "Multiplayer & Network Telemetry");
            GUILayout.BeginArea(new Rect(statusRect.x + 8, statusRect.y + 20, statusRect.width - 16, statusRect.height - 25));

            string netState = _multiplayerService != null ? _multiplayerService.ConnectionState.ToString() : "Disconnected";
            string roomStr = room != null ? $"{room.roomCode} ({room.players.Count}/4)" : "None";
            string authStr = _backendService != null ? _backendService.CurrentAuthState.ToString() : "Guest";
            string friendsCount = _socialService != null ? _socialService.GetFriendsList().Count.ToString() : "0";

            GUILayout.Label($"Net State: <b>{netState}</b>");
            GUILayout.Label($"Active Room: <b>{roomStr}</b>");
            GUILayout.Label($"Backend Auth: <b>{authStr}</b>");
            GUILayout.Label($"Friends Online: <b>{friendsCount}</b>");
            GUILayout.Label($"Traffic Density: {_trafficManager?.ActiveDensity}");

            GUILayout.EndArea();
        }

        private void StartSampleMission(MissionCategory category)
        {
            if (_missionManager == null) return;

            MissionData sample = null;
            if (_missionCatalogue != null)
            {
                var list = _missionCatalogue.GetMissionsByCategory(category);
                if (list.Count > 0) sample = list[0];
            }

            if (sample == null)
            {
                sample = ScriptableObject.CreateInstance<MissionData>();
                sample.name = $"Sample_{category}_Mission";
            }

            _missionManager.StartMission(sample, MissionDifficulty.Normal);
        }
    }
}
