using System.Collections.Generic;
using IndianBikeLife.NavigationSystem;
using UnityEngine;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for motorcycle GPS navigation, waypoint route generation, and arrival tracking.
    /// </summary>
    public interface INavigationManager : IGameService
    {
        bool HasActiveRoute { get; }
        RouteData ActiveRoute { get; }
        float DistanceRemainingMeters { get; }
        float EstimatedTimeRemainingSeconds { get; }
        string NextDirectionText { get; }

        void GenerateRoute(Vector3 startPoint, Vector3 destinationPoint, string destinationName);
        void CancelRoute();
        void RecalculateRoute();
        void AdvanceCheckpoint(int checkpointIndex);
    }
}

