using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    /// <summary>
    /// Master traffic coordinator implementing ITrafficManager and IGameService.
    /// Controls traffic density (Low, Medium, High) and regulates vehicle pooling.
    /// </summary>
    [RequireComponent(typeof(TrafficSpawner))]
    public class TrafficManager : MonoBehaviour, ITrafficManager
    {
        private const string Tag = "TrafficManager";

        [SerializeField] private TrafficConfiguration _config;
        [SerializeField] private TrafficDensity _activeDensity = TrafficDensity.Medium;

        private TrafficSpawner _spawner;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public TrafficDensity ActiveDensity => _activeDensity;
        public int ActiveVehicleCount => _spawner != null ? _spawner.ActiveVehicles.Count : 0;

        public void Initialize()
        {
            if (_isInitialized) return;

            _spawner = GetComponent<TrafficSpawner>();
            _isInitialized = true;
            GameLogger.I(Tag, $"TrafficManager initialized with density: {_activeDensity}");
        }

        public void Shutdown()
        {
            ClearAllTraffic();
            _isInitialized = false;
        }

        public void SetTrafficDensity(TrafficDensity density)
        {
            _activeDensity = density;
            GameLogger.I(Tag, $"Traffic density set to: {_activeDensity}");
        }

        public void SpawnTrafficVehicle(IndianVehicleType vehicleType, Vector3 position, Quaternion rotation)
        {
            // Immediate programmatic vehicle injection
        }

        public void ClearAllTraffic()
        {
            if (_spawner != null) _spawner.ClearAll();
        }
    }
}

