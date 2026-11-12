using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.WeatherSystem
{
    /// <summary>
    /// Synchronizes road surface materials and physics friction when rain occurs.
    /// Modifies road tire grip factor safely without making the motorcycle uncontrollably slippery.
    /// </summary>
    public class WetRoadController : MonoBehaviour
    {
        private RoadPhysicsSurface[] _cachedRoadSurfaces;

        private void Start()
        {
            _cachedRoadSurfaces = FindObjectsByType<RoadPhysicsSurface>(FindObjectsSortMode.None);
        }

        public void ApplyWetness(bool isRaining, float roadGripFactor)
        {
            if (_cachedRoadSurfaces == null || _cachedRoadSurfaces.Length == 0)
            {
                _cachedRoadSurfaces = FindObjectsByType<RoadPhysicsSurface>(FindObjectsSortMode.None);
            }

            for (int i = 0; i < _cachedRoadSurfaces.Length; i++)
            {
                if (_cachedRoadSurfaces[i] != null)
                {
                    _cachedRoadSurfaces[i].SetWet(isRaining);
                }
            }
        }
    }
}

