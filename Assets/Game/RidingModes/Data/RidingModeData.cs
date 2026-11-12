using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.EnvironmentSystem;
using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.RidingModes
{
    /// <summary>
    /// ScriptableObject defining complete environmental, traffic, and vehicle parameters
    /// for a specific Indian Bike Life riding mode.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRidingMode", menuName = "IndianBikeLife/Riding Modes/Riding Mode Data")]
    public class RidingModeData : ScriptableObject
    {
        [Header("Mode Identity")]
        [SerializeField] private RidingModeType _modeType = RidingModeType.HighwayRide;
        [SerializeField] private string _modeTitle = "Highway Cruise";
        [TextArea(2, 4)]
        [SerializeField] private string _description = "High-speed cruising across national highways.";

        [Header("Environmental Parameters")]
        [SerializeField] private EnvironmentTheme _theme = EnvironmentTheme.HighwayCorridor;
        [SerializeField] private RoadSurfaceType _surfaceType = RoadSurfaceType.SmoothAsphalt;
        [SerializeField] private TrafficDensity _trafficDensity = TrafficDensity.Medium;
        [Range(0, 23)]
        [SerializeField] private int _defaultHourOfDay = 14; // 2 PM default
        [SerializeField] private bool _enableRainByDefault = false;

        [Header("Passenger & Vehicle Setup")]
        [SerializeField] private bool _requiresPassenger = false;
        [SerializeField] private string _recommendedMotorcycle = "Classic 350";
        [SerializeField] private bool _isImplementedInStage2 = true;

        public RidingModeType ModeType => _modeType;
        public string ModeTitle => _modeTitle;
        public string Description => _description;
        public EnvironmentTheme Theme => _theme;
        public RoadSurfaceType SurfaceType => _surfaceType;
        public TrafficDensity TrafficDensity => _trafficDensity;
        public int DefaultHourOfDay => _defaultHourOfDay;
        public bool EnableRainByDefault => _enableRainByDefault;
        public bool RequiresPassenger => _requiresPassenger;
        public string RecommendedMotorcycle => _recommendedMotorcycle;
        public bool IsImplementedInStage2 => _isImplementedInStage2;
    }
}

