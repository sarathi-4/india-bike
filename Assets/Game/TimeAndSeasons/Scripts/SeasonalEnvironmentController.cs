using IndianBikeLife.Core.Interfaces;
using UnityEngine;

namespace IndianBikeLife.TimeAndSeasons
{
    /// <summary>
    /// Adjusts ambient ground lighting and vegetation hues according to the active Indian season.
    /// </summary>
    public class SeasonalEnvironmentController : MonoBehaviour
    {
        [Header("Seasonal Ambient Colors")]
        [SerializeField] private Color _summerAmbient = new Color(0.35f, 0.32f, 0.28f);
        [SerializeField] private Color _monsoonAmbient = new Color(0.25f, 0.30f, 0.25f); // Lush green tint
        [SerializeField] private Color _winterAmbient = new Color(0.28f, 0.30f, 0.35f);  // Crisp cool tint

        public void ApplySeasonalAesthetics(IndianSeason season)
        {
            switch (season)
            {
                case IndianSeason.Monsoon:
                    RenderSettings.ambientGroundColor = _monsoonAmbient;
                    break;
                case IndianSeason.Winter:
                    RenderSettings.ambientGroundColor = _winterAmbient;
                    break;
                case IndianSeason.Summer:
                default:
                    RenderSettings.ambientGroundColor = _summerAmbient;
                    break;
            }
        }
    }
}
