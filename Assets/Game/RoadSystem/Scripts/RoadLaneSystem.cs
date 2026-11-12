using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    /// <summary>
    /// Lane waypoint manager for Indian roads (strictly Left-Hand Traffic).
    /// Provides dedicated travel lines for player guidance and AI traffic lane-following.
    /// </summary>
    public class RoadLaneSystem : MonoBehaviour
    {
        [Header("Lane Settings (Left-Hand Traffic)")]
        [Tooltip("Standard Indian driving side: Left.")]
        [SerializeField] private bool _leftHandTraffic = true;
        [SerializeField] private float _leftLaneOffset = -1.8f;  // Normal cruising lane
        [SerializeField] private float _rightLaneOffset = 1.8f;  // Oncoming lane

        [SerializeField] private RoadSpline _parentSpline;

        private void Awake()
        {
            if (_parentSpline == null) _parentSpline = GetComponent<RoadSpline>();
        }

        public Vector3 GetLanePoint(float t, bool isLeftLane)
        {
            if (_parentSpline == null) return transform.position;

            Vector3 center = _parentSpline.EvaluatePosition(t);
            Vector3 tangent = _parentSpline.EvaluateTangent(t);
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

            float offset = isLeftLane ? _leftLaneOffset : _rightLaneOffset;
            return center + right * offset;
        }

        public Vector3 GetForwardTangent(float t, bool isLeftLane)
        {
            if (_parentSpline == null) return transform.forward;
            Vector3 forward = _parentSpline.EvaluateTangent(t);
            // Oncoming traffic travels in reverse direction along the spline
            return isLeftLane ? forward : -forward;
        }
    }
}
