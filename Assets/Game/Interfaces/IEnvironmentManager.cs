using System.Collections.Generic;
using IndianBikeLife.EnvironmentSystem;
using UnityEngine;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for Indian environment streaming, procedural spawner coordination, and LOD management.
    /// </summary>
    public interface IEnvironmentManager : IGameService
    {
        EnvironmentTheme CurrentTheme { get; }
        void SetTheme(EnvironmentTheme theme);
        void SpawnEnvironmentSector(Vector3 centerPosition, float radius);
        void ClearEnvironment();
    }
}

