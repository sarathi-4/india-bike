using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    /// <summary>
    /// Spawns vehicles ahead of the player along road splines and manages mobile pooling/culling.
    /// </summary>
    public class TrafficSpawner : MonoBehaviour
    {
        [SerializeField] private TrafficConfiguration _config;
        [SerializeField] private GameObject _autoRickshawPrefab;
        [SerializeField] private GameObject _carPrefab;
        [SerializeField] private GameObject _busPrefab;
        [SerializeField] private GameObject _truckPrefab;
        [SerializeField] private GameObject _twoWheelerPrefab;

        private readonly List<TrafficVehicleAI> _activeVehicles = new List<TrafficVehicleAI>();
        private readonly Queue<GameObject> _pool = new Queue<GameObject>();

        public IReadOnlyList<TrafficVehicleAI> ActiveVehicles => _activeVehicles;

        public void SpawnVehicle(IndianVehicleType type, RoadSpline road, float progressT, bool leftLane)
        {
            GameObject instance = GetFromPool(type);
            TrafficVehicleAI ai = instance.GetComponent<TrafficVehicleAI>();
            if (ai == null) ai = instance.AddComponent<TrafficVehicleAI>();

            TrafficLaneFollower follower = instance.GetComponent<TrafficLaneFollower>();
            if (follower == null) follower = instance.AddComponent<TrafficLaneFollower>();

            follower.SetRoad(road, progressT, leftLane);

            float speedLimit = _config != null ? _config.GetConfigForType(type).maxSpeedKmph : 45f;
            ai.Configure(type, speedLimit);

            _activeVehicles.Add(ai);
        }

        public void DespawnVehicle(TrafficVehicleAI vehicle)
        {
            if (vehicle == null) return;
            _activeVehicles.Remove(vehicle);
            vehicle.gameObject.SetActive(false);
            _pool.Enqueue(vehicle.gameObject);
        }

        public void ClearAll()
        {
            for (int i = 0; i < _activeVehicles.Count; i++)
            {
                if (_activeVehicles[i] != null)
                {
                    _activeVehicles[i].gameObject.SetActive(false);
                    _pool.Enqueue(_activeVehicles[i].gameObject);
                }
            }
            _activeVehicles.Clear();
        }

        private GameObject GetFromPool(IndianVehicleType type)
        {
            if (_pool.Count > 0)
            {
                GameObject pooled = _pool.Dequeue();
                pooled.SetActive(true);
                return pooled;
            }

            GameObject prefab = GetPrefabForType(type);
            if (prefab != null)
            {
                return Instantiate(prefab, transform);
            }

            // Procedural vehicle stand-in
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Traffic_{type}";
            go.transform.SetParent(transform);

            // Shape according to vehicle type
            if (type == IndianVehicleType.CityBus || type == IndianVehicleType.HeavyTruck)
            {
                go.transform.localScale = new Vector3(2.4f, 2.8f, 8.5f);
            }
            else if (type == IndianVehicleType.AutoRickshaw)
            {
                go.transform.localScale = new Vector3(1.4f, 1.8f, 2.6f);
            }
            else if (type == IndianVehicleType.ScooterTwoWheeler || type == IndianVehicleType.Motorcycle)
            {
                go.transform.localScale = new Vector3(0.7f, 1.2f, 1.9f);
            }
            else
            {
                go.transform.localScale = new Vector3(1.8f, 1.5f, 4.2f);
            }

            return go;
        }

        private GameObject GetPrefabForType(IndianVehicleType type)
        {
            switch (type)
            {
                case IndianVehicleType.AutoRickshaw: return _autoRickshawPrefab;
                case IndianVehicleType.CityBus: return _busPrefab;
                case IndianVehicleType.HeavyTruck: return _truckPrefab;
                case IndianVehicleType.ScooterTwoWheeler:
                case IndianVehicleType.Motorcycle:
                    return _twoWheelerPrefab;
                case IndianVehicleType.HatchbackCar:
                case IndianVehicleType.SedanCar:
                default:
                    return _carPrefab;
            }
        }
    }
}

