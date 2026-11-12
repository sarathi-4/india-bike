using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.TimeAndSeasons
{
    /// <summary>
    /// Coordinates nighttime activation of streetlights, building windows, and motorcycle headlights.
    /// </summary>
    public class NightLightingManager : MonoBehaviour
    {
        [SerializeField] private List<Light> _streetLights = new List<Light>();
        [SerializeField] private List<GameObject> _illuminatedNightProps = new List<GameObject>();

        private bool _isNightActive;

        public void RegisterStreetLight(Light light)
        {
            if (light != null && !_streetLights.Contains(light))
            {
                _streetLights.Add(light);
                light.enabled = _isNightActive;
            }
        }

        public void SetNightLighting(bool isNight)
        {
            if (_isNightActive == isNight) return;
            _isNightActive = isNight;

            for (int i = 0; i < _streetLights.Count; i++)
            {
                if (_streetLights[i] != null)
                {
                    _streetLights[i].enabled = isNight;
                }
            }

            for (int i = 0; i < _illuminatedNightProps.Count; i++)
            {
                if (_illuminatedNightProps[i] != null)
                {
                    _illuminatedNightProps[i].SetActive(isNight);
                }
            }
        }
    }
}

