using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.NavigationSystem
{
    /// <summary>
    /// Master GPS navigation service implementing INavigationManager and IGameService.
    /// Bridges road network queries, route generation, turn-by-turn guidance, and checkpoint validation.
    /// </summary>
    [RequireComponent(typeof(RouteManager))]
    [RequireComponent(typeof(CheckpointManager))]
    public class NavigationManager : MonoBehaviour, INavigationManager
    {
        private const string Tag = "NavigationManager";

        [SerializeField] private Transform _playerMotorcycle;

        private RouteManager _routeManager;
        private CheckpointManager _checkpointManager;
        private IRoadManager _roadManager;

        private float _distanceRemainingMeters;
        private float _estimatedTimeRemainingSeconds;
        private string _nextDirectionText = "Ready for navigation";
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public bool HasActiveRoute => _routeManager != null && _routeManager.HasActiveRoute;
        public RouteData ActiveRoute => _routeManager?.ActiveRoute;
        public float DistanceRemainingMeters => _distanceRemainingMeters;
        public float EstimatedTimeRemainingSeconds => _estimatedTimeRemainingSeconds;
        public string NextDirectionText => _nextDirectionText;

        public void Initialize()
        {
            if (_isInitialized) return;

            _routeManager = GetComponent<RouteManager>();
            _checkpointManager = GetComponent<CheckpointManager>();
            ServiceLocator.TryGet(out _roadManager);

            if (_checkpointManager != null)
            {
                _checkpointManager.OnCheckpointPassed += AdvanceCheckpoint;
            }

            if (_routeManager != null)
            {
                _routeManager.OnOffRouteDetected += RecalculateRoute;
                _routeManager.OnDestinationReached += HandleDestinationReached;
            }

            _isInitialized = true;
            GameLogger.I(Tag, "NavigationManager initialized.");
        }

        public void Shutdown()
        {
            CancelRoute();
            _isInitialized = false;
        }

        public void SetPlayerTarget(Transform target)
        {
            _playerMotorcycle = target;
        }

        public void GenerateRoute(Vector3 startPoint, Vector3 destinationPoint, string destinationName)
        {
            if (_routeManager == null) return;

            IReadOnlyList<RoadSpline> roads = _roadManager != null ? _roadManager.ActiveRoads : null;
            RouteData route = RouteGenerator.BuildRoute(startPoint, destinationPoint, destinationName, roads);

            _routeManager.SetRoute(route);

            // Populate checkpoints along route waypoints
            List<Vector3> checkpointPositions = new List<Vector3>();
            for (int i = 1; i < route.waypoints.Count; i++)
            {
                checkpointPositions.Add(route.waypoints[i].position);
            }
            _checkpointManager.SetCheckpoints(checkpointPositions);

            _nextDirectionText = route.waypoints.Count > 1 ? route.waypoints[1].instructionText : "Proceed to destination";
            GameLogger.I(Tag, $"Generated route to '{destinationName}' ({route.totalDistanceMeters:0} meters).");
        }

        public void CancelRoute()
        {
            if (_routeManager != null) _routeManager.ClearRoute();
            _distanceRemainingMeters = 0f;
            _estimatedTimeRemainingSeconds = 0f;
            _nextDirectionText = "Route cancelled";
        }

        public void RecalculateRoute()
        {
            if (!HasActiveRoute || _playerMotorcycle == null) return;
            GameLogger.W(Tag, "Player is off-route. Recalculating route from current position...");
            GenerateRoute(_playerMotorcycle.position, ActiveRoute.destinationPosition, ActiveRoute.destinationName);
        }

        public void AdvanceCheckpoint(int checkpointIndex)
        {
            GameLogger.I(Tag, $"Checkpoint {checkpointIndex + 1} cleared!");
        }

        private void HandleDestinationReached()
        {
            GameLogger.I(Tag, $"Destination reached: {ActiveRoute?.destinationName}!");
        }

        private void Update()
        {
            if (!_isInitialized || !HasActiveRoute) return;

            Vector3 playerPos = _playerMotorcycle != null ? _playerMotorcycle.position : transform.position;

            _routeManager.UpdatePlayerProgress(playerPos, out _distanceRemainingMeters, out _nextDirectionText);
            _estimatedTimeRemainingSeconds = _distanceRemainingMeters / 13.88f; // ~50 km/h average speed

            if (_checkpointManager != null)
            {
                _checkpointManager.CheckPlayerPosition(playerPos);
            }
        }
    }
}

