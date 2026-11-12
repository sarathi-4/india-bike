using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.RidingModes
{
    /// <summary>
    /// Master riding mode manager implementing IRidingModeManager and ISaveable.
    /// Distributes riding mode environment, traffic, passenger, and time settings across systems.
    /// </summary>
    [RequireComponent(typeof(RidingModeSelector))]
    public class RidingModeManager : MonoBehaviour, IRidingModeManager, ISaveable
    {
        private const string Tag = "RidingModeManager";

        [SerializeField] private RidingModeConfiguration _catalogue;

        private RidingModeSelector _selector;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "RidingModeManager";

        public RidingModeType ActiveModeType => _selector != null ? _selector.CurrentSelection : RidingModeType.HighwayRide;
        public RidingModeData ActiveModeData => _selector != null ? _selector.GetSelectedModeData() : null;
        public IReadOnlyList<RidingModeData> AllModes => _catalogue != null ? _catalogue.Modes : null;

        public void Initialize()
        {
            if (_isInitialized) return;

            _selector = GetComponent<RidingModeSelector>();
            _isInitialized = true;
            GameLogger.I(Tag, "RidingModeManager initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void SelectMode(RidingModeType modeType)
        {
            if (_selector != null)
            {
                _selector.SelectMode(modeType);
                ApplyActiveModeSettings();
            }
        }

        public void ApplyActiveModeSettings()
        {
            RidingModeData data = ActiveModeData;
            if (data == null) return;

            GameLogger.I(Tag, $"Applying configuration for mode: {data.ModeTitle}");

            // 1. Configure Traffic Density
            if (ServiceLocator.TryGet(out ITrafficManager traffic))
            {
                traffic.SetTrafficDensity(data.TrafficDensity);
            }

            // 2. Configure Environment Theme
            if (ServiceLocator.TryGet(out IEnvironmentManager env))
            {
                env.SetTheme(data.Theme);
            }

            // 3. Configure Passenger
            if (ServiceLocator.TryGet(out IPassengerManager passenger))
            {
                passenger.SetPassengerActive(data.RequiresPassenger);
            }
        }

        #region ISaveable Implementation

        public void PopulateSaveData(SaveData data)
        {
            // Extensible save hook: store last selected riding mode
        }

        public void RestoreFromSaveData(SaveData data)
        {
            // Restore last selected riding mode
        }

        #endregion
    }
}

