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
    /// Provides live toggles for Passenger mounting, Camera views, Weather conditions,
    /// Day/Night cycle, Traffic density, Missions, Currency, XP, and Daily Rewards.
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

        private bool _showHUD = true;

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

            // Auto-mount passenger on start if seat position is configured
            if (_passengerManager != null && _motorcycleSeat != null)
            {
                _passengerManager.MountPassenger(_motorcycleSeat);
            }
        }

        private void OnGUI()
        {
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

            GUI.Label(new Rect(0, 8, w, 26), "INDIAN BIKE LIFE — STAGE 3: MISSIONS, PROGRESSION & REWARDS", bannerStyle);

            // Left Quick-Control Panel
            float panelWidth = Mathf.Min(300, w * 0.28f);
            float startY = 40f;
            float panelHeight = h - startY - 15;

            Rect controlRect = new Rect(15, startY, panelWidth, panelHeight);
            GUI.Box(controlRect, "Stage 2 & 3 System Controls");

            GUILayout.BeginArea(new Rect(controlRect.x + 8, controlRect.y + 22, controlRect.width - 16, controlRect.height - 28));

            // 1. Stage 3 Missions
            GUILayout.Label("<b>1. Mission Controls:</b>");
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
                if (GUILayout.Button("Start Highway"))
                {
                    StartSampleMission(MissionCategory.Highway);
                }
                if (GUILayout.Button("Start Village"))
                {
                    StartSampleMission(MissionCategory.VillageRiding);
                }
                GUILayout.EndHorizontal();
            }

            // 2. Economy & XP
            GUILayout.Space(6);
            GUILayout.Label("<b>2. Economy & Progression:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+500 Silver"))
            {
                _economyManager?.AddCurrency(CurrencyType.SilverCoins, 500, "HUD Debug Credit");
            }
            if (GUILayout.Button("-200 Silver"))
            {
                _economyManager?.SpendCurrency(CurrencyType.SilverCoins, 200, "HUD Debug Debit");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+350 XP"))
            {
                _progressionManager?.AddXP(350, "HUD Debug Ride XP");
            }
            if (GUILayout.Button("Claim Daily"))
            {
                _loginRewardManager?.ClaimDailyReward();
            }
            GUILayout.EndHorizontal();

            // 3. Passenger System
            GUILayout.Space(6);
            GUILayout.Label("<b>3. Passenger System:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Mount"))
            {
                if (_passengerManager != null && _motorcycleSeat != null)
                {
                    _passengerManager.MountPassenger(_motorcycleSeat);
                }
            }
            if (GUILayout.Button("Dismount"))
            {
                if (_passengerManager != null) _passengerManager.DismountPassenger();
            }
            GUILayout.EndHorizontal();

            // 4. Camera Perspectives
            GUILayout.Space(6);
            GUILayout.Label("<b>4. Camera Angle:</b>");
            string camText = _cameraController != null ? _cameraController.CurrentView.ToString() : "Chase";
            if (GUILayout.Button($"View: {camText}"))
            {
                if (_cameraController != null) _cameraController.CycleCameraView();
            }

            // 5. Weather Conditions
            GUILayout.Space(6);
            GUILayout.Label("<b>5. Dynamic Weather:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sunny")) SetWeather(WeatherType.Sunny);
            if (GUILayout.Button("Rain")) SetWeather(WeatherType.HeavyRain);
            if (GUILayout.Button("Fog")) SetWeather(WeatherType.Fog);
            GUILayout.EndHorizontal();

            // 6. Day / Night Cycle
            GUILayout.Space(6);
            GUILayout.Label("<b>6. Time of Day:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Morning")) SetTime(8f);
            if (GUILayout.Button("Noon")) SetTime(13f);
            if (GUILayout.Button("Sunset")) SetTime(17.5f);
            if (GUILayout.Button("Night")) SetTime(22f);
            GUILayout.EndHorizontal();

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
            float statusW = 240f;
            float statusH = 90f;
            Rect statusRect = new Rect(w - statusW - 15, h - statusH - 15, statusW, statusH);
            GUI.Box(statusRect, "Environment Telemetry");
            GUILayout.BeginArea(new Rect(statusRect.x + 8, statusRect.y + 20, statusRect.width - 16, statusRect.height - 25));

            string timeStr = _dayNightManager != null ? $"{_dayNightManager.CurrentPeriod} ({_dayNightManager.CurrentHour:0.0}h)" : "14:00";
            string weatherStr = _weatherManager != null ? _weatherManager.CurrentWeather.ToString() : "Sunny";
            string gripStr = _weatherManager != null ? $"{_weatherManager.RoadGripFactor * 100:0}%" : "100%";

            GUILayout.Label($"Time: {timeStr}");
            GUILayout.Label($"Weather: {weatherStr} (Grip: {gripStr})");
            GUILayout.Label($"Traffic: {_trafficManager?.ActiveDensity}");

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

        private void SetWeather(WeatherType type)
        {
            if (_weatherManager != null) _weatherManager.SetWeather(type);
        }

        private void SetTime(float hour)
        {
            if (_dayNightManager != null) _dayNightManager.SetHour(hour);
        }
    }
}
