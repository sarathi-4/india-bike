using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.NavigationSystem.UI
{
    /// <summary>
    /// Renders turn-by-turn motorcycle GPS HUD overlay with distance, time, and direction.
    /// Supports lightweight IMGUI rendering for zero missing asset dependencies.
    /// </summary>
    public class NavigationUIController : MonoBehaviour
    {
        [SerializeField] private bool _showNavHUD = true;

        private INavigationManager _navManager;

        private void Start()
        {
            ServiceLocator.TryGet(out _navManager);
        }

        public void SetVisible(bool show)
        {
            _showNavHUD = show;
        }

        private void OnGUI()
        {
            if (!_showNavHUD || _navManager == null || !_navManager.HasActiveRoute) return;

            int w = Screen.width;
            int h = Screen.height;

            // Top-center turn-by-turn GPS card
            float cardWidth = Mathf.Min(340, w * 0.45f);
            float cardHeight = 70f;
            float cardX = (w - cardWidth) * 0.5f;
            float cardY = 15f;

            GUI.Box(new Rect(cardX, cardY, cardWidth, cardHeight), GUIContent.none);

            GUIStyle destStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(12, h * 2 / 100),
                fontStyle = FontStyle.Bold
            };
            destStyle.normal.textColor = new Color(0.2f, 0.9f, 0.3f); // GPS Green

            GUIStyle instructStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(11, h * 18 / 1000)
            };
            instructStyle.normal.textColor = Color.white;

            GUIStyle subStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11
            };
            subStyle.normal.textColor = Color.yellow;

            string destName = _navManager.ActiveRoute != null ? _navManager.ActiveRoute.destinationName : "Destination";
            float distKm = _navManager.DistanceRemainingMeters / 1000f;
            int timeMins = Mathf.CeilToInt(_navManager.EstimatedTimeRemainingSeconds / 60f);

            GUI.Label(new Rect(cardX, cardY + 5, cardWidth, 20), $"DESTINATION: {destName.ToUpper()}", destStyle);
            GUI.Label(new Rect(cardX, cardY + 25, cardWidth, 20), _navManager.NextDirectionText, instructStyle);
            GUI.Label(new Rect(cardX, cardY + 45, cardWidth, 20), $"{distKm:0.0} km remaining  •  ~{timeMins} mins", subStyle);
        }
    }
}

