using System;
using IndianBikeLife.Core.Interfaces;
using UnityEngine;

namespace IndianBikeLife.TimeAndSeasons
{
    /// <summary>
    /// Tracks clock progression, time speed multipliers, and periods of the day.
    /// </summary>
    public class TimeOfDayController : MonoBehaviour
    {
        [Header("Clock Settings")]
        [Range(0f, 24f)]
        [SerializeField] private float _currentHour = 14.0f; // 2:00 PM default
        [SerializeField] private float _timeSpeedMultiplier = 60.0f; // 1 real second = 1 game minute

        public event Action<float> OnHourChanged;

        public float CurrentHour => _currentHour;
        public float TimeSpeedMultiplier => _timeSpeedMultiplier;

        public bool IsNight => (_currentHour < 5.5f || _currentHour > 18.5f);

        public TimeOfDayPeriod CurrentPeriod
        {
            get
            {
                if (_currentHour >= 5f && _currentHour < 7f) return TimeOfDayPeriod.Sunrise;
                if (_currentHour >= 7f && _currentHour < 11f) return TimeOfDayPeriod.Morning;
                if (_currentHour >= 11f && _currentHour < 14f) return TimeOfDayPeriod.Noon;
                if (_currentHour >= 14f && _currentHour < 17f) return TimeOfDayPeriod.Afternoon;
                if (_currentHour >= 17f && _currentHour < 19f) return TimeOfDayPeriod.Sunset;
                if (_currentHour >= 19f && _currentHour < 21f) return TimeOfDayPeriod.Evening;
                if (_currentHour >= 21f && _currentHour < 24f) return TimeOfDayPeriod.Night;
                return TimeOfDayPeriod.Midnight;
            }
        }

        public void SetHour(float hour)
        {
            _currentHour = Mathf.Repeat(hour, 24f);
            OnHourChanged?.Invoke(_currentHour);
        }

        public void SetTimeSpeed(float multiplier)
        {
            _timeSpeedMultiplier = Mathf.Max(0f, multiplier);
        }

        private void Update()
        {
            if (_timeSpeedMultiplier <= 0.001f) return;

            // 1 game day = 86400 seconds
            float deltaHours = (Time.deltaTime * _timeSpeedMultiplier) / 3600f;
            _currentHour = Mathf.Repeat(_currentHour + deltaHours, 24f);
            OnHourChanged?.Invoke(_currentHour);
        }
    }
}

