using UnityEngine;

namespace IndianBikeLife.WeatherSystem
{
    /// <summary>
    /// Governs Unity environmental fog settings, density, and colors for mountain mist and heavy fog.
    /// </summary>
    public class FogController : MonoBehaviour
    {
        [SerializeField] private bool _enableFog = true;

        public void SetFog(float targetDensity, Color targetColor, float deltaTime, float smoothSpeed = 2.0f)
        {
            RenderSettings.fog = _enableFog && targetDensity > 0.001f;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetDensity, deltaTime * smoothSpeed);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetColor, deltaTime * smoothSpeed);
        }
    }
}
