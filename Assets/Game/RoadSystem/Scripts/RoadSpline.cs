using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    [Serializable]
    public class RoadNode
    {
        public Vector3 position;
        public float width = 8.0f; // Standard 2-lane road width
        public float rollAngle = 0f; // Banking on hill curves / hairpin bends
        public float speedLimitKmph = 60f;

        public RoadNode(Vector3 pos, float roadWidth = 8.0f, float roll = 0f, float speedLimit = 60f)
        {
            position = pos;
            width = roadWidth;
            rollAngle = roll;
            speedLimitKmph = speedLimit;
        }
    }

    /// <summary>
    /// Spline curve representing a segment of an Indian road (highway, city, or hill ghat).
    /// Provides smooth interpolation, banking, tangents, and lane coordinates.
    /// </summary>
    public class RoadSpline : MonoBehaviour
    {
        [Header("Spline Nodes")]
        [SerializeField] private List<RoadNode> _nodes = new List<RoadNode>();
        [SerializeField] private bool _isLoop = false;
        [SerializeField] private RoadSurfaceType _surfaceType = RoadSurfaceType.WornTarmac;

        [Header("Lane Configuration")]
        [SerializeField] private int _laneCount = 2;
        [SerializeField] private float _laneWidth = 3.5f;

        public IReadOnlyList<RoadNode> Nodes => _nodes;
        public bool IsLoop => _isLoop;
        public RoadSurfaceType SurfaceType => _surfaceType;
        public int LaneCount => _laneCount;
        public float LaneWidth => _laneWidth;

        public void AddNode(Vector3 position, float width = 8.0f, float roll = 0f)
        {
            _nodes.Add(new RoadNode(position, width, roll));
        }

        public Vector3 EvaluatePosition(float t)
        {
            if (_nodes.Count == 0) return transform.position;
            if (_nodes.Count == 1) return _nodes[0].position;

            t = Mathf.Clamp01(t);
            int count = _isLoop ? _nodes.Count : _nodes.Count - 1;
            float totalSegments = count;
            float scaledT = t * totalSegments;
            int i = Mathf.FloorToInt(scaledT);
            if (i >= count) i = count - 1;
            float segmentT = scaledT - i;

            int p0 = ClampIndex(i - 1);
            int p1 = ClampIndex(i);
            int p2 = ClampIndex(i + 1);
            int p3 = ClampIndex(i + 2);

            return CatmullRom(
                _nodes[p0].position,
                _nodes[p1].position,
                _nodes[p2].position,
                _nodes[p3].position,
                segmentT
            );
        }

        public Vector3 EvaluateTangent(float t)
        {
            float delta = 0.005f;
            Vector3 p1 = EvaluatePosition(Mathf.Clamp01(t - delta));
            Vector3 p2 = EvaluatePosition(Mathf.Clamp01(t + delta));
            return (p2 - p1).normalized;
        }

        public float GetTotalLength(int sampleCount = 50)
        {
            float length = 0f;
            Vector3 prev = EvaluatePosition(0f);
            for (int i = 1; i <= sampleCount; i++)
            {
                Vector3 current = EvaluatePosition((float)i / sampleCount);
                length += Vector3.Distance(prev, current);
                prev = current;
            }
            return length;
        }

        private int ClampIndex(int index)
        {
            if (_isLoop)
            {
                return (index % _nodes.Count + _nodes.Count) % _nodes.Count;
            }
            return Mathf.Clamp(index, 0, _nodes.Count - 1);
        }

        private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * (
                (2.0f * p1) +
                (-p0 + p2) * t +
                (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
                (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
            );
        }

        private void OnDrawGizmos()
        {
            if (_nodes == null || _nodes.Count < 2) return;

            Gizmos.color = Color.yellow;
            Vector3 prev = EvaluatePosition(0f);
            for (int i = 1; i <= 60; i++)
            {
                float t = (float)i / 60f;
                Vector3 current = EvaluatePosition(t);
                Gizmos.DrawLine(prev, current);
                prev = current;
            }

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _nodes.Count; i++)
            {
                Gizmos.DrawWireSphere(_nodes[i].position, 0.4f);
            }
        }
    }
}

