using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    public enum IndianVehicleType
    {
        AutoRickshaw = 0,     // 3-Wheeler Bajaj / Piaggio style
        CityBus = 1,          // Red/Green state transport bus
        HeavyTruck = 2,       // Tata / Ashok Leyland decorated lorry
        HatchbackCar = 3,     // Swift / Alto city car
        SedanCar = 4,         // Dzire / Honda taxi
        ScooterTwoWheeler = 5,// Activa / Chetak
        Motorcycle = 6,       // Splendor / Pulsar commuter
        Tractor = 7,          // Rural agricultural tractor
        DeliveryVan = 8       // Omni / Eeco van
    }

    [Serializable]
    public class VehicleTypeConfig
    {
        public IndianVehicleType vehicleType;
        public float maxSpeedKmph = 50f;
        public float acceleration = 3.5f;
        public float brakeDeceleration = 7.0f;
        public float safeFollowDistanceMeters = 7.0f;
        public float lengthMeters = 4.0f;
        public float widthMeters = 1.8f;
    }

    /// <summary>
    /// ScriptableObject defining Indian traffic density caps, vehicle specifications, and spawn ratios.
    /// </summary>
    [CreateAssetMenu(fileName = "TrafficConfiguration", menuName = "IndianBikeLife/Traffic/Traffic Configuration")]
    public class TrafficConfiguration : ScriptableObject
    {
        [Header("Density Caps (Mobile Optimized)")]
        [SerializeField] private int _lowDensityMaxVehicles = 8;
        [SerializeField] private int _mediumDensityMaxVehicles = 16;
        [SerializeField] private int _highDensityMaxVehicles = 28;

        [Header("Spawn Distances")]
        [SerializeField] private float _minSpawnDistanceAhead = 40f;
        [SerializeField] private float _maxSpawnDistanceAhead = 120f;
        [SerializeField] private float _despawnDistanceBehind = 60f;

        [Header("Vehicle Presets")]
        [SerializeField] private List<VehicleTypeConfig> _vehicleConfigs = new List<VehicleTypeConfig>();

        public int GetMaxVehicles(TrafficDensity density)
        {
            switch (density)
            {
                case TrafficDensity.Low: return _lowDensityMaxVehicles;
                case TrafficDensity.High: return _highDensityMaxVehicles;
                case TrafficDensity.Medium:
                default:
                    return _mediumDensityMaxVehicles;
            }
        }

        public float MinSpawnDistanceAhead => _minSpawnDistanceAhead;
        public float MaxSpawnDistanceAhead => _maxSpawnDistanceAhead;
        public float DespawnDistanceBehind => _despawnDistanceBehind;
        public IReadOnlyList<VehicleTypeConfig> VehicleConfigs => _vehicleConfigs;

        public VehicleTypeConfig GetConfigForType(IndianVehicleType type)
        {
            VehicleTypeConfig found = _vehicleConfigs.Find(v => v.vehicleType == type);
            if (found != null) return found;

            // Default fallback
            return new VehicleTypeConfig
            {
                vehicleType = type,
                maxSpeedKmph = 50f,
                acceleration = 3.5f,
                brakeDeceleration = 7.0f,
                safeFollowDistanceMeters = 7.0f
            };
        }
    }
}

