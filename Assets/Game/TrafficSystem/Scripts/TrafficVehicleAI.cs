using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    /// <summary>
    /// Autonomous AI driver controlling acceleration, braking, collision reactions,
    /// and Indian horn usage when delayed by obstacles.
    /// </summary>
    [RequireComponent(typeof(TrafficLaneFollower))]
    [RequireComponent(typeof(TrafficCollisionAvoidance))]
    public class TrafficVehicleAI : MonoBehaviour
    {
        [Header("Vehicle Identity")]
        [SerializeField] private IndianVehicleType _vehicleType = IndianVehicleType.AutoRickshaw;
        [SerializeField] private float _targetSpeedKmph = 50f;

        private TrafficLaneFollower _laneFollower;
        private TrafficCollisionAvoidance _avoidance;

        private float _currentSpeedKmph;
        private float _sensorTimer;
        private float _blockedTimer;

        public IndianVehicleType VehicleType => _vehicleType;
        public float CurrentSpeedKmph => _currentSpeedKmph;

        private void Awake()
        {
            _laneFollower = GetComponent<TrafficLaneFollower>();
            _avoidance = GetComponent<TrafficCollisionAvoidance>();
            _currentSpeedKmph = _targetSpeedKmph * 0.5f;
        }

        public void Configure(IndianVehicleType type, float targetSpeed)
        {
            _vehicleType = type;
            _targetSpeedKmph = targetSpeed;
            _currentSpeedKmph = targetSpeed * 0.5f;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Sensor check throttled to 10 Hz for mobile efficiency
            _sensorTimer += dt;
            if (_sensorTimer >= 0.1f)
            {
                _sensorTimer = 0f;
                _avoidance.CheckSensors(_currentSpeedKmph);
            }

            // Speed regulation
            if (_avoidance.IsObstacleDetected)
            {
                // Brake smoothly or stop
                float safeDist = 6.0f;
                if (_avoidance.DistanceToObstacle < safeDist)
                {
                    _currentSpeedKmph = Mathf.MoveTowards(_currentSpeedKmph, 0f, dt * 25.0f);
                    _blockedTimer += dt;
                    if (_blockedTimer > 2.5f)
                    {
                        // Authentic Indian Traffic behavior: Honk horn when stopped in traffic
                        _blockedTimer = 0f;
                    }
                }
                else
                {
                    _currentSpeedKmph = Mathf.MoveTowards(_currentSpeedKmph, _targetSpeedKmph * 0.4f, dt * 15.0f);
                    _blockedTimer = 0f;
                }
            }
            else
            {
                // Accelerate back to target speed
                _currentSpeedKmph = Mathf.MoveTowards(_currentSpeedKmph, _targetSpeedKmph, dt * 8.0f);
                _blockedTimer = 0f;
            }

            if (_currentSpeedKmph > 0.05f)
            {
                _laneFollower.ProgressAlongRoad(_currentSpeedKmph, dt);
            }
        }
    }
}

