using System;
using System.Collections.Generic;
using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.NavigationSystem
{
    /// <summary>
    /// Generates turn-by-turn navigation routes across registered road splines.
    /// Calculates waypoints, distances, and driving instructions.
    /// </summary>
    public class RouteGenerator
    {
        public static RouteData BuildRoute(Vector3 start, Vector3 destination, string destName, IReadOnlyList<RoadSpline> availableRoads)
        {
            RouteData route = new RouteData
            {
                routeId = Guid.NewGuid().ToString("N"),
                destinationName = destName,
                startPosition = start,
                destinationPosition = destination
            };

            route.waypoints.Add(new RouteWaypoint(start, "Start Journey"));

            // If roads exist, sample waypoints from closest road segments
            if (availableRoads != null && availableRoads.Count > 0)
            {
                for (int r = 0; r < availableRoads.Count; r++)
                {
                    RoadSpline road = availableRoads[r];
                    if (road == null || road.Nodes.Count < 2) continue;

                    // Sample 5 representative waypoints along each road segment
                    for (int s = 1; s <= 5; s++)
                    {
                        float t = (float)s / 5f;
                        Vector3 pos = road.EvaluatePosition(t);
                        string instruction = (s == 5) ? "Approach Junction" : "Follow Road";
                        route.waypoints.Add(new RouteWaypoint(pos, instruction));
                    }
                }
            }

            // Final destination waypoint
            route.waypoints.Add(new RouteWaypoint(destination, $"Arrive at {destName}"));
            route.CalculateTotalDistance();

            return route;
        }
    }
}
