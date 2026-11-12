using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.TimeAndSeasons
{
    /// <summary>
    /// Master time-of-day and seasonal manager implementing IDayNightManager and IGameService.
    /// Synchronizes sun/moon angle, night lighting, and seasonal aesthetic conditions.
    /// </summary>
    [RequireComponent(typeof(TimeOfDayController))]
    [RequireComponent(typeof(SunController))]
    [RequireComponent(typeof(NightLightingManager))]
    [RequireComponent(typeof(SeasonManager))]
    [RequireComponent(typeof(SeasonalEnvironmentController))]
    public class DayNightManager : MonoBehaviour, IDayNightManager
    {
        private const string Tag = "DayNightManager";

        private TimeOfDayController _timeController;
        private SunController _sunController;
        private NightLightingManager _nightLighting;
        private SeasonManager _seasonManager;
        private SeasonalEnvironmentController _seasonalEnv;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public float CurrentHour => _timeController != null ? _timeController.CurrentHour : 12f;
        public TimeOfDayPeriod CurrentPeriod => _timeController != null ? _timeController.CurrentPeriod : TimeOfDayPeriod.Noon;
        public IndianSeason CurrentSeason => _seasonManager != null ? _seasonManager.CurrentSeason : IndianSeason.Summer;
        public bool IsNight => _timeController != null && _timeController.IsNight;

        public void Initialize()
        {
            if (_isInitialized) return;

            _timeController = GetComponent<TimeOfDayController>();
            _sunController = GetComponent<SunController>();
            _nightLighting = GetComponent<NightLightingManager>();
            _seasonManager = GetComponent<SeasonManager>();
            _seasonalEnv = GetComponent<SeasonalEnvironmentController>();

            if (_timeController != null)
            {
                _timeController.OnHourChanged += HandleHourChanged;
            }

            if (_seasonManager != null)
            {
                _seasonManager.OnSeasonChanged += HandleSeasonChanged;
            }

            // Initial sync
            HandleHourChanged(CurrentHour);
            HandleSeasonChanged(CurrentSeason);

            _isInitialized = true;
            GameLogger.I(Tag, "DayNightManager initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void SetHour(float hour)
        {
            if (_timeController != null)
            {
                _timeController.SetHour(hour);
            }
        }

        public void SetTimeSpeedMultiplier(float speed)
        {
            if (_timeController != null)
            {
                _timeController.SetTimeSpeed(speed);
            }
        }

        public void SetSeason(IndianSeason season)
        {
            if (_seasonManager != null)
            {
                _seasonManager.SetSeason(season);
            }
        }

        private void HandleHourChanged(float hour)
        {
            if (_sunController != null)
            {
                _sunController.UpdateCelestialLighting(hour);
            }

            if (_nightLighting != null && _timeController != null)
            {
                _nightLighting.SetNightLighting(_timeController.IsNight);
            }
        }

        private void HandleSeasonChanged(IndianSeason season)
        {
            if (_seasonalEnv != null)
            {
                _seasonalEnv.ApplySeasonalAesthetics(season);
            }
        }
    }
}
