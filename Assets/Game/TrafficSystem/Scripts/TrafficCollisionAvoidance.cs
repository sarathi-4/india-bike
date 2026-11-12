using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    /// <summary>
    /// Raycast-based obstacle, vehicle, and player motorcycle collision avoidance for Traffic AI.
    /// Uses throttled sensor ticks to eliminate CPU overhead on mobile hardware.
    /// </summary>
    public class TrafficCollisionAvoidance : MonoBehaviour
    {
        [Header("Sensor Distances")]
        [SerializeField] private float _forwardLookDistance = 14f;
        [SerializeField] private float _raycastOriginHeight = 0.6f;
        [SerializeField] private LayerMask _obstacleLayers = ~0;

        private float _distanceToObstacle = float.MaxValue;
        private bool _isObstacleDetected;
        private Collider _detectedCollider;

        public bool IsObstacleDetected => _isObstacleDetected;
        public float DistanceToObstacle => _distanceToObstacle;
        public Collider DetectedCollider => _detectedCollider;

        public void CheckSensors(float currentSpeedKmph)
        {
            // Dynamic lookahead: faster speeds require longer braking distance
            float speedMps = currentSpeedKmph / 3.6f;
            float dynamicDistance = Mathf.Max(_forwardLookDistance, speedMps * 1.8f);

            Vector3 origin = transform.position + Vector3.up * _raycastOriginHeight + transform.forward * 1.0f;

            // Center forward ray
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, dynamicDistance, _obstacleLayers))
            {
                // Ignore self
                if (hit.collider.transform.root != transform.root)
                {
                    _isObstacleDetected = true;
                    _distanceToObstacle = hit.distance;
                    _detectedCollider = hit.collider;
                    return;
                }
            }

            // Left bumper sensor (checking for overtaking bikes)
            Vector3 leftOrigin = origin - transform.right * 0.7f;
            if (Physics.Raycast(leftOrigin, transform.forward + transform.right * 0.15f, out hit, dynamicDistance * 0.7f, _obstacleLayers))
            {
                if (hit.collider.transform.root != transform.root)
                {
                    _isObstacleDetected = true;
                    _distanceToObstacle = hit.distance;
                    _detectedCollider = hit.collider;
                    return;
                }
            }

            _isObstacleDetected = false;
            _distanceToObstacle = float.MaxValue;
            _detectedCollider = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _isObstacleDetected ? Color.red : Color.green;
            Vector3 origin = transform.position + Vector3.up * _raycastOriginHeight;
            Gizmos.DrawRay(origin, transform.forward * _forwardLookDistance);
        }
    }
}
