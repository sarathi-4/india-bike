using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.PassengerSystem;
using IndianBikeLife.RidingModes;
using UnityEngine;

namespace IndianBikeLife.Core.UI.Development
{
    /// <summary>
    /// Interactive HUD overlay for the Stage 2 Regional Ride Test environment.
    /// Provides live toggles for Passenger mounting, Camera views, Weather conditions,
    /// Day/Night cycle, Traffic density, and Navigation routes.
    /// </summary>
    public class RegionalRideTestHUD : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private PassengerCameraController _cameraController;
        [SerializeField] private PassengerSeatPosition _motorcycleSeat;

        private IPassengerManager _passengerManager;
        private IWeatherManager _weatherManager;
        private IDayNightManager _dayNightManager;
        private ITrafficManager _trafficManager;
        private INavigationManager _navManager;
        private IRidingModeManager _ridingModeManager;

        private bool _showHUD = true;

        private void Start()
        {
            ServiceLocator.TryGet(out _passengerManager);
            ServiceLocator.TryGet(out _weatherManager);
            ServiceLocator.TryGet(out _dayNightManager);
            ServiceLocator.TryGet(out _trafficManager);
            ServiceLocator.TryGet(out _navManager);
            ServiceLocator.TryGet(out _ridingModeManager);

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
                fontSize = Mathf.Max(16, h * 25 / 1000),
                fontStyle = FontStyle.Bold
            };
            bannerStyle.normal.textColor = new Color(1f, 0.65f, 0.15f);

            GUI.Label(new Rect(0, 10, w, 28), "INDIAN BIKE LIFE — STAGE 2: REGIONAL RIDE ENVIRONMENT", bannerStyle);

            // Left Quick-Control Panel
            float panelWidth = Mathf.Min(280, w * 0.28f);
            float startY = 45f;
            float panelHeight = h - startY - 20;

            Rect controlRect = new Rect(15, startY, panelWidth, panelHeight);
            GUI.Box(controlRect, "Stage 2 System Controls");

            GUILayout.BeginArea(new Rect(controlRect.x + 10, controlRect.y + 25, controlRect.width - 20, controlRect.height - 35));

            // 1. Passenger System
            GUILayout.Label("<b>1. Passenger System:</b>");
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
                if (_passengerManager != null)
                {
                    _passengerManager.DismountPassenger();
                }
            }
            GUILayout.EndHorizontal();

            // 2. Camera Perspectives
            GUILayout.Space(6);
            GUILayout.Label("<b>2. Camera Angle:</b>");
            string camText = _cameraController != null ? _cameraController.CurrentView.ToString() : "Chase";
            if (GUILayout.Button($"View: {camText}"))
            {
                if (_cameraController != null) _cameraController.CycleCameraView();
            }

            // 3. Weather Conditions
            GUILayout.Space(6);
            GUILayout.Label("<b>3. Weather Condition:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sunny")) SetWeather(WeatherType.Sunny);
            if (GUILayout.Button("Rain")) SetWeather(WeatherType.HeavyRain);
            if (GUILayout.Button("Fog")) SetWeather(WeatherType.Fog);
            GUILayout.EndHorizontal();

            // 4. Time of Day
            GUILayout.Space(6);
            GUILayout.Label("<b>4. Time of Day:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Morning")) SetTime(8f);
            if (GUILayout.Button("Noon")) SetTime(13f);
            if (GUILayout.Button("Sunset")) SetTime(17.5f);
            if (GUILayout.Button("Night")) SetTime(22f);
            GUILayout.EndHorizontal();

            // 5. Traffic Density
            GUILayout.Space(6);
            GUILayout.Label("<b>5. Indian Traffic Density:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Low")) SetTraffic(TrafficDensity.Low);
            if (GUILayout.Button("Med")) SetTraffic(TrafficDensity.Medium);
            if (GUILayout.Button("High")) SetTraffic(TrafficDensity.High);
            GUILayout.EndHorizontal();

            // 6. Navigation
            GUILayout.Space(6);
            GUILayout.Label("<b>6. Navigation Route:</b>");
            if (GUILayout.Button("Route: Ooty Hill Ghat"))
            {
                if (_navManager != null)
                {
                    _navManager.GenerateRoute(Vector3.zero, new Vector3(80f, 25f, 240f), "Ooty Hill Ghat");
                }
            }

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
            GUILayout.Label($"Traffic Density: {_trafficManager?.ActiveDensity}");

            GUILayout.EndArea();
        }

        private void SetWeather(WeatherType type)
        {
            if (_weatherManager != null) _weatherManager.SetWeather(type);
        }

        private void SetTime(float hour)
        {
            if (_dayNightManager != null) _dayNightManager.SetHour(hour);
        }

        private void SetTraffic(TrafficDensity density)
        {
            if (_trafficManager != null) _trafficManager.SetTrafficDensity(density);
        }
    }
}
