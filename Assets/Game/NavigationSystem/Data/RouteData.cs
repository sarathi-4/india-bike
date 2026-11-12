using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.NavigationSystem
{
    [Serializable]
    public class RouteWaypoint
    {
        public Vector3 position;
        public string instructionText; // e.g. "Turn Left towards NH44", "Keep Straight at Roundabout"
        public float speedLimitKmph = 60f;

        public RouteWaypoint(Vector3 pos, string instruction = "Continue Straight")
        {
            position = pos;
            instructionText = instruction;
        }
    }

    /// <summary>
    /// Calculated travel route connecting start point to destination via road networks.
    /// </summary>
    [Serializable]
    public class RouteData
    {
        public string routeId;
        public string destinationName;
        public Vector3 startPosition;
        public Vector3 destinationPosition;

        public List<RouteWaypoint> waypoints = new List<RouteWaypoint>();
        public float totalDistanceMeters;
        public float estimatedTravelTimeSeconds;

        public int currentWaypointIndex = 0;
        public bool isCompleted = false;

        public float CalculateTotalDistance()
        {
            totalDistanceMeters = 0f;
            if (waypoints.Count < 2) return 0f;

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                totalDistanceMeters += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            }

            // Estimate time assuming an average urban/highway speed of 50 km/h (13.88 m/s)
            float avgSpeedMps = 13.88f;
            estimatedTravelTimeSeconds = totalDistanceMeters / avgSpeedMps;

            return totalDistanceMeters;
        }
    }
}
