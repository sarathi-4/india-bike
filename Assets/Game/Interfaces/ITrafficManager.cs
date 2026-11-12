using System.Collections.Generic;
using IndianBikeLife.TrafficSystem;
using UnityEngine;

namespace IndianBikeLife.Core.Interfaces
{
    public enum TrafficDensity
    {
        Low = 0,     // Sparse traffic, battery saver mode
        Medium = 1,  // Balanced Indian road traffic
        High = 2     // Dense city rush hour traffic
    }

    /// <summary>
    /// Contract for Indian road traffic management, density regulation, and vehicle pooling.
    /// </summary>
    public interface ITrafficManager : IGameService
    {
        TrafficDensity ActiveDensity { get; }
        int ActiveVehicleCount { get; }

        void SetTrafficDensity(TrafficDensity density);
        void SpawnTrafficVehicle(IndianVehicleType vehicleType, Vector3 position, Quaternion rotation);
        void ClearAllTraffic();
    }
}

