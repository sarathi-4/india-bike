using UnityEngine;

namespace IndianBikeLife.TimeAndSeasons
{
    /// <summary>
    /// Controls directional sunlight and moonlight rotation, intensity, and color temperature.
    /// </summary>
    public class SunController : MonoBehaviour
    {
        [Header("Directional Celestial Light")]
        [SerializeField] private Light _celestialLight;

        [Header("Sun Colors & Intensity")]
        [SerializeField] private Color _dayColor = new Color(1.0f, 0.96f, 0.90f);
        [SerializeField] private Color _sunriseSunsetColor = new Color(1.0f, 0.55f, 0.25f);
        [SerializeField] private Color _nightColor = new Color(0.25f, 0.35f, 0.60f);

        [SerializeField] private float _dayIntensity = 1.2f;
        [SerializeField] private float _nightIntensity = 0.25f;

        public void UpdateCelestialLighting(float hour)
        {
            if (_celestialLight == null) return;

            // Map hour (0 to 24) to sun pitch angle (-90 to 270)
            // 06:00 is sunrise (pitch 0), 12:00 is noon (pitch 90), 18:00 is sunset (pitch 180)
            float sunAngle = ((hour - 6f) / 24f) * 360f;
            _celestialLight.transform.rotation = Quaternion.Euler(sunAngle, 50f, 0f);

            bool isDay = (hour >= 6f && hour <= 18f);

            if (isDay)
            {
                // Day / Dawn / Dusk
                if (hour < 8f || hour > 16.5f)
                {
                    _celestialLight.color = _sunriseSunsetColor;
                    _celestialLight.intensity = Mathf.Lerp(0.5f, _dayIntensity, 0.5f);
                }
                else
                {
                    _celestialLight.color = _dayColor;
                    _celestialLight.intensity = _dayIntensity;
                }
                RenderSettings.ambientIntensity = 1.0f;
            }
            else
            {
                // Night (Moonlight)
                _celestialLight.color = _nightColor;
                _celestialLight.intensity = _nightIntensity;
                RenderSettings.ambientIntensity = 0.35f;
            }
        }
    }
}

