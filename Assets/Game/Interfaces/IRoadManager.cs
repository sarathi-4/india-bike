using System.Collections.Generic;
using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for road networks, spline geometries, and surface physics queries.
    /// </summary>
    public interface IRoadManager : IGameService
    {
        IReadOnlyList<RoadSpline> ActiveRoads { get; }
        IReadOnlyList<RoadIntersection> Intersections { get; }

        void RegisterRoad(RoadSpline road);
        void UnregisterRoad(RoadSpline road);
        RoadPhysicsSurface GetSurfaceAtPosition(Vector3 worldPosition);
        Vector3 GetNearestRoadPoint(Vector3 worldPosition, out Vector3 forwardDirection);
    }
}
