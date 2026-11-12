using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.WeatherSystem
{
    /// <summary>
    /// Master weather manager implementing IWeatherManager and IGameService.
    /// Controls environmental conditions and weather effects across Indian regions.
    /// </summary>
    [RequireComponent(typeof(WeatherController))]
    public class WeatherManager : MonoBehaviour, IWeatherManager
    {
        private const string Tag = "WeatherManager";

        [SerializeField] private WeatherType _currentWeather = WeatherType.Sunny;
        [Range(0f, 1f)] [SerializeField] private float _weatherIntensity = 1.0f;

        private WeatherController _controller;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public WeatherType CurrentWeather => _currentWeather;
        public float WeatherIntensity => _weatherIntensity;
        public float RoadGripFactor => _controller != null ? _controller.CurrentRoadGrip : 1.0f;

        public void Initialize()
        {
            if (_isInitialized) return;

            _controller = GetComponent<WeatherController>();
            _isInitialized = true;
            GameLogger.I(Tag, $"WeatherManager initialized with weather: {_currentWeather}");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void SetWeather(WeatherType type, float transitionDuration = 2.0f)
        {
            _currentWeather = type;
            if (_controller != null)
            {
                _controller.TransitionToWeather(type, transitionDuration);
            }
            GameLogger.I(Tag, $"Weather set to: {_currentWeather} (Transition: {transitionDuration}s)");
        }

        public void SetIntensity(float intensity)
        {
            _weatherIntensity = Mathf.Clamp01(intensity);
        }
    }
}

