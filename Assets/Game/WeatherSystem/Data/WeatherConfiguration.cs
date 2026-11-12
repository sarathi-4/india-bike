using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using UnityEngine;

namespace IndianBikeLife.WeatherSystem
{
    [Serializable]
    public class WeatherConditionSettings
    {
        public WeatherType weatherType;
        [Range(0f, 1000f)] public float rainEmissionRate = 0f;
        [Range(0f, 0.08f)] public float fogDensity = 0.005f;
        public Color fogColor = new Color(0.7f, 0.75f, 0.8f);
        [Range(0f, 30f)] public float windSpeedMps = 3.0f;
        [Range(0.4f, 1.0f)] public float roadGripModifier = 1.0f;
        public bool hasThunder = false;
        public float cloudAlpha = 0.2f;
    }

    /// <summary>
    /// ScriptableObject defining visual, particle, audio, and physics settings for all weather types.
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherConfiguration", menuName = "IndianBikeLife/Weather/Weather Configuration")]
    public class WeatherConfiguration : ScriptableObject
    {
        [SerializeField] private List<WeatherConditionSettings> _settings = new List<WeatherConditionSettings>();

        public WeatherConditionSettings GetSettings(WeatherType type)
        {
            WeatherConditionSettings found = _settings.Find(s => s.weatherType == type);
            if (found != null) return found;

            // Default fallback
            return new WeatherConditionSettings
            {
                weatherType = type,
                rainEmissionRate = (type == WeatherType.HeavyRain || type == WeatherType.Thunderstorm) ? 500f : (type == WeatherType.LightRain ? 150f : 0f),
                fogDensity = (type == WeatherType.Fog || type == WeatherType.Mist) ? 0.035f : 0.005f,
                roadGripModifier = (type == WeatherType.HeavyRain || type == WeatherType.Thunderstorm) ? 0.65f : (type == WeatherType.LightRain ? 0.80f : 1.0f),
                hasThunder = (type == WeatherType.Thunderstorm)
            };
        }
    }
}
