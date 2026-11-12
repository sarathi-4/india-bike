using UnityEngine;

namespace IndianBikeLife.NavigationSystem
{
    /// <summary>
    /// Simulates in-game GPS telemetry without accessing real mobile hardware GPS.
    /// Maps 3D Unity engine coordinates to Indian latitude and longitude coordinates.
    /// </summary>
    public class GPSPositionSimulator : MonoBehaviour
    {
        [Header("Geographic Reference Origin (e.g., Chennai / Ooty)")]
        [SerializeField] private double _originLatitude = 13.0827; // Chennai Central
        [SerializeField] private double _originLongitude = 80.2707;
        [SerializeField] private float _worldMetersPerDegreeLat = 111320f;

        [Header("Target Tracking")]
        [SerializeField] private Transform _targetMotorcycle;

        public double CurrentLatitude { get; private set; }
        public double CurrentLongitude { get; private set; }
        public float CurrentAltitudeMeters { get; private set; }
        public float CurrentHeadingDegrees { get; private set; }

        public void SetTarget(Transform target)
        {
            _targetMotorcycle = target;
        }

        private void Update()
        {
            if (_targetMotorcycle == null) return;

            Vector3 pos = _targetMotorcycle.position;

            // Compute delta latitude and longitude
            double deltaLat = pos.z / _worldMetersPerDegreeLat;
            double metersPerDegreeLon = _worldMetersPerDegreeLat * Mathf.Cos((float)(_originLatitude * Mathf.Deg2Rad));
            double deltaLon = pos.x / metersPerDegreeLon;

            CurrentLatitude = _originLatitude + deltaLat;
            CurrentLongitude = _originLongitude + deltaLon;
            CurrentAltitudeMeters = pos.y;
            CurrentHeadingDegrees = _targetMotorcycle.eulerAngles.y;
        }
    }
}

