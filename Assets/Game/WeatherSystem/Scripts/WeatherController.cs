using System.Collections;
using IndianBikeLife.Core.Interfaces;
using UnityEngine;

namespace IndianBikeLife.WeatherSystem
{
    /// <summary>
    /// Coordinates smooth interpolation between weather states, particles, and fog.
    /// </summary>
    public class WeatherController : MonoBehaviour
    {
        [SerializeField] private WeatherConfiguration _config;
        [SerializeField] private RainController _rain;
        [SerializeField] private FogController _fog;
        [SerializeField] private WindController _wind;
        [SerializeField] private WetRoadController _wetRoad;

        private WeatherConditionSettings _currentSettings;
        private WeatherConditionSettings _targetSettings;
        private float _transitionProgress = 1.0f;
        private float _transitionDuration = 2.0f;

        public float CurrentRoadGrip => _currentSettings != null ? _currentSettings.roadGripModifier : 1.0f;

        private void Awake()
        {
            if (_rain == null) _rain = GetComponentInChildren<RainController>();
            if (_fog == null) _fog = GetComponentInChildren<FogController>();
            if (_wind == null) _wind = GetComponentInChildren<WindController>();
            if (_wetRoad == null) _wetRoad = GetComponentInChildren<WetRoadController>();

            if (_config != null)
            {
                _currentSettings = _config.GetSettings(WeatherType.Sunny);
                _targetSettings = _currentSettings;
            }
        }

        public void TransitionToWeather(WeatherType type, float duration = 2.0f)
        {
            if (_config == null) return;
            _targetSettings = _config.GetSettings(type);
            _transitionDuration = duration;
            _transitionProgress = 0f;

            bool isWet = (type == WeatherType.LightRain || type == WeatherType.HeavyRain || type == WeatherType.Thunderstorm);
            if (_wetRoad != null)
            {
                _wetRoad.ApplyWetness(isWet, _targetSettings.roadGripModifier);
            }
        }

        private void Update()
        {
            if (_targetSettings == null) return;

            float dt = Time.deltaTime;

            if (_transitionProgress < 1.0f)
            {
                _transitionProgress += dt / _transitionDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
            }

            // Interpolate rain
            if (_rain != null)
            {
                float targetRain = _targetSettings.rainEmissionRate;
                _rain.SetRainIntensity(targetRain * _transitionProgress);
            }

            // Interpolate fog
            if (_fog != null)
            {
                _fog.SetFog(_targetSettings.fogDensity, _targetSettings.fogColor, dt);
            }

            // Interpolate wind
            if (_wind != null)
            {
                _wind.SetBaseSpeed(_targetSettings.windSpeedMps);
            }
        }
    }
}
