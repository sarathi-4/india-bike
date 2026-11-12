using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    /// <summary>
    /// Governs road spline traversal, left-lane positioning (Indian traffic law),
    /// and smooth steering interpolation along curves.
    /// </summary>
    public class TrafficLaneFollower : MonoBehaviour
    {
        [Header("Road Tracking")]
        [SerializeField] private RoadSpline _currentRoad;
        [SerializeField] private bool _travelsInLeftLane = true;
        [SerializeField] private float _laneOffset = -1.8f; // Left side of road center

        private float _splineProgress = 0f;
        private float _speedMultiplier = 1.0f;

        public RoadSpline CurrentRoad => _currentRoad;
        public float SplineProgress => _splineProgress;

        public void SetRoad(RoadSpline road, float startT = 0f, bool leftLane = true)
        {
            _currentRoad = road;
            _splineProgress = Mathf.Clamp01(startT);
            _travelsInLeftLane = leftLane;
            _laneOffset = leftLane ? -1.8f : 1.8f;

            if (_currentRoad != null)
            {
                UpdateTransform(0f);
            }
        }

        public void ProgressAlongRoad(float speedKmph, float deltaTime)
        {
            if (_currentRoad == null) return;

            float roadLength = _currentRoad.GetTotalLength();
            if (roadLength <= 0.01f) return;

            float speedMps = (speedKmph / 3.6f) * _speedMultiplier;
            float deltaT = (speedMps * deltaTime) / roadLength;

            if (_travelsInLeftLane)
            {
                _splineProgress += deltaT;
                if (_splineProgress >= 1.0f)
                {
                    _splineProgress = _currentRoad.IsLoop ? 0f : 1.0f;
                }
            }
            else
            {
                // Oncoming traffic moves backwards along spline
                _splineProgress -= deltaT;
                if (_splineProgress <= 0f)
                {
                    _splineProgress = _currentRoad.IsLoop ? 1.0f : 0f;
                }
            }

            UpdateTransform(deltaTime);
        }

        private void UpdateTransform(float deltaTime)
        {
            Vector3 center = _currentRoad.EvaluatePosition(_splineProgress);
            Vector3 tangent = _currentRoad.EvaluateTangent(_splineProgress);
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

            Vector3 targetPosition = center + right * _laneOffset;
            Vector3 forwardDirection = _travelsInLeftLane ? tangent : -tangent;

            transform.position = targetPosition;
            if (forwardDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
            }
        }
    }
}
