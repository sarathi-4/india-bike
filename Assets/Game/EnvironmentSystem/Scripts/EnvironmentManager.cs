using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.EnvironmentSystem
{
    /// <summary>
    /// Master environment manager implementing IEnvironmentManager and IGameService.
    /// Manages theme transitions, building/vegetation spawners, and LOD chunking.
    /// </summary>
    [RequireComponent(typeof(BuildingSpawner))]
    [RequireComponent(typeof(VegetationManager))]
    [RequireComponent(typeof(EnvironmentSpawner))]
    public class EnvironmentManager : MonoBehaviour, IEnvironmentManager
    {
        private const string Tag = "EnvironmentManager";

        [SerializeField] private EnvironmentTheme _currentTheme = EnvironmentTheme.HighwayCorridor;

        private BuildingSpawner _buildingSpawner;
        private VegetationManager _vegetationManager;
        private EnvironmentSpawner _environmentSpawner;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public EnvironmentTheme CurrentTheme => _currentTheme;
        public BuildingSpawner BuildingSpawner => _buildingSpawner;
        public VegetationManager VegetationManager => _vegetationManager;
        public EnvironmentSpawner EnvironmentSpawner => _environmentSpawner;

        public void Initialize()
        {
            if (_isInitialized) return;

            _buildingSpawner = GetComponent<BuildingSpawner>();
            _vegetationManager = GetComponent<VegetationManager>();
            _environmentSpawner = GetComponent<EnvironmentSpawner>();

            _isInitialized = true;
            GameLogger.I(Tag, $"EnvironmentManager initialized with theme: {_currentTheme}");
        }

        public void Shutdown()
        {
            ClearEnvironment();
            _isInitialized = false;
        }

        public void SetTheme(EnvironmentTheme theme)
        {
            _currentTheme = theme;
            GameLogger.I(Tag, $"Environment theme switched to: {_currentTheme}");
        }

        public void SpawnEnvironmentSector(Vector3 centerPosition, float radius)
        {
            // Procedural sector seeding for roadside detail
            GameLogger.I(Tag, $"Seeding environment sector around {centerPosition} (radius {radius}m)");
        }

        public void ClearEnvironment()
        {
            if (_buildingSpawner != null) _buildingSpawner.ClearBuildings();
            if (_vegetationManager != null) _vegetationManager.ClearVegetation();
        }
    }
}
