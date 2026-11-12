using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    /// <summary>
    /// Master road system coordinator implementing IRoadManager.
    /// Tracks road networks, intersections, and surface friction queries for motorcycle tires.
    /// </summary>
    public class RoadManager : MonoBehaviour, IRoadManager
    {
        private const string Tag = "RoadManager";

        private readonly List<RoadSpline> _activeRoads = new List<RoadSpline>();
        private readonly List<RoadIntersection> _intersections = new List<RoadIntersection>();
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public IReadOnlyList<RoadSpline> ActiveRoads => _activeRoads;
        public IReadOnlyList<RoadIntersection> Intersections => _intersections;

        public void Initialize()
        {
            if (_isInitialized) return;

            // Automatically find roads in the active scene if present
            RoadSpline[] foundRoads = FindObjectsOfType<RoadSpline>();
            for (int i = 0; i < foundRoads.Length; i++)
            {
                RegisterRoad(foundRoads[i]);
            }

            RoadIntersection[] foundJunctions = FindObjectsOfType<RoadIntersection>();
            for (int i = 0; i < foundJunctions.Length; i++)
            {
                if (!_intersections.Contains(foundJunctions[i]))
                {
                    _intersections.Add(foundJunctions[i]);
                }
            }

            _isInitialized = true;
            GameLogger.I(Tag, $"RoadManager initialized with {_activeRoads.Count} road segments and {_intersections.Count} junctions.");
        }

        public void Shutdown()
        {
            _activeRoads.Clear();
            _intersections.Clear();
            _isInitialized = false;
        }

        public void RegisterRoad(RoadSpline road)
        {
            if (road != null && !_activeRoads.Contains(road))
            {
                _activeRoads.Add(road);
            }
        }

        public void UnregisterRoad(RoadSpline road)
        {
            if (road != null)
            {
                _activeRoads.Remove(road);
            }
        }

        public RoadPhysicsSurface GetSurfaceAtPosition(Vector3 worldPosition)
        {
            // Raycast downward to detect specific RoadPhysicsSurface
            if (Physics.Raycast(worldPosition + Vector3.up * 1.0f, Vector3.down, out RaycastHit hit, 3.0f))
            {
                return hit.collider.GetComponent<RoadPhysicsSurface>();
            }
            return null;
        }

        public Vector3 GetNearestRoadPoint(Vector3 worldPosition, out Vector3 forwardDirection)
        {
            float closestDistSqr = float.MaxValue;
            Vector3 closestPoint = worldPosition;
            forwardDirection = Vector3.forward;

            for (int r = 0; r < _activeRoads.Count; r++)
            {
                RoadSpline spline = _activeRoads[r];
                if (spline == null || spline.Nodes.Count == 0) continue;

                // Sample 20 points along the spline
                for (int i = 0; i <= 20; i++)
                {
                    float t = (float)i / 20f;
                    Vector3 samplePos = spline.EvaluatePosition(t);
                    float dSqr = (samplePos - worldPosition).sqrMagnitude;
                    if (dSqr < closestDistSqr)
                    {
                        closestDistSqr = dSqr;
                        closestPoint = samplePos;
                        forwardDirection = spline.EvaluateTangent(t);
                    }
                }
            }

            return closestPoint;
        }
    }
}
