using System;
using UnityEngine;

namespace IndianBikeLife.NavigationSystem
{
    /// <summary>
    /// Tracks active route progression, calculates distance remaining,
    /// and flags off-route deviations when the player leaves the road.
    /// </summary>
    public class RouteManager : MonoBehaviour
    {
        [SerializeField] private float _offRouteThresholdMeters = 35.0f;
        [SerializeField] private float _waypointReachRadius = 12.0f;

        private RouteData _activeRoute;

        public event Action OnOffRouteDetected;
        public event Action OnDestinationReached;

        public bool HasActiveRoute => _activeRoute != null && !_activeRoute.isCompleted;
        public RouteData ActiveRoute => _activeRoute;

        public void SetRoute(RouteData route)
        {
            _activeRoute = route;
        }

        public void ClearRoute()
        {
            _activeRoute = null;
        }

        public void UpdatePlayerProgress(Vector3 playerPosition, out float distanceRemaining, out string nextInstruction)
        {
            distanceRemaining = 0f;
            nextInstruction = "No active route";

            if (!HasActiveRoute) return;

            int currentIdx = _activeRoute.currentWaypointIndex;
            if (currentIdx >= _activeRoute.waypoints.Count)
            {
                _activeRoute.isCompleted = true;
                OnDestinationReached?.Invoke();
                nextInstruction = $"Arrived at {_activeRoute.destinationName}!";
                return;
            }

            Vector3 targetWp = _activeRoute.waypoints[currentIdx].position;
            float distToCurrent = Vector3.Distance(playerPosition, targetWp);

            // Check if player reached current waypoint
            if (distToCurrent <= _waypointReachRadius)
            {
                _activeRoute.currentWaypointIndex++;
                if (_activeRoute.currentWaypointIndex >= _activeRoute.waypoints.Count)
                {
                    _activeRoute.isCompleted = true;
                    OnDestinationReached?.Invoke();
                    nextInstruction = $"Arrived at {_activeRoute.destinationName}!";
                    return;
                }
            }

            // Check for off-route deviation
            if (distToCurrent > _offRouteThresholdMeters * 3.0f && currentIdx > 0)
            {
                OnOffRouteDetected?.Invoke();
            }

            // Calculate remaining distance along upcoming waypoints
            distanceRemaining = distToCurrent;
            for (int i = _activeRoute.currentWaypointIndex; i < _activeRoute.waypoints.Count - 1; i++)
            {
                distanceRemaining += Vector3.Distance(_activeRoute.waypoints[i].position, _activeRoute.waypoints[i + 1].position);
            }

            nextInstruction = _activeRoute.waypoints[_activeRoute.currentWaypointIndex].instructionText;
        }
    }
}

