using System.Collections;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SceneManagement;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.State;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace IndianBikeLife.Core.UI.Loading
{
    /// <summary>
    /// Coordinates loading screen visuals, displays progress, and auto-loads MainMenu.
    /// Supports both uGUI Canvas elements and lightweight IMGUI fallback.
    /// </summary>
    public class LoadingScreenUI : MonoBehaviour
    {
        private const string Tag = "LoadingUI";

        [Header("Optional UI References")]
        [SerializeField] private Slider _progressBar;
        [SerializeField] private Text _progressText;
        [SerializeField] private Text _tipText;

        [Header("Loading Tips")]
        [SerializeField] private string[] _tips = new string[]
        {
            "Indian Bike Life Tip: Always wear a helmet when riding in Mumbai traffic!",
            "Indian Bike Life Tip: Maintain proper tire pressure for better highway fuel efficiency.",
            "Indian Bike Life Tip: Use high beam only when necessary on unlit rural highways.",
            "Indian Bike Life Tip: Watch out for sudden speed breakers in city alleyways."
        };

        private float _currentProgress;
        private string _activeTip = "Preparing Indian Bike Life Simulator...";

        private void OnEnable()
        {
            EventBus.Subscribe<SceneLoadProgressEvent>(OnProgressUpdated);
            EventBus.Subscribe<SceneLoadCompletedEvent>(OnLoadCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SceneLoadProgressEvent>(OnProgressUpdated);
            EventBus.Unsubscribe<SceneLoadCompletedEvent>(OnLoadCompleted);
        }

        private void Start()
        {
            if (_tips != null && _tips.Length > 0)
            {
                _activeTip = _tips[Random.Range(0, _tips.Length)];
            }

            if (_tipText != null)
            {
                _tipText.text = _activeTip;
            }

            // If we are currently in the Loading scene, advance to MainMenu
            StartCoroutine(AutoAdvanceToMainMenuRoutine());
        }

        private IEnumerator AutoAdvanceToMainMenuRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            if (ServiceLocator.TryGet(out ISceneLoader sceneLoader) &&
                ServiceLocator.TryGet(out IGameManager gameManager))
            {
                if (sceneLoader.CurrentScene == SceneIndex.Loading && !sceneLoader.IsLoading)
                {
                    GameLogger.I(Tag, "Loading scene active. Initiating transition to MainMenu...");
                    gameManager.SetState(GameState.Loading);
                    sceneLoader.LoadScene(SceneIndex.MainMenu, () =>
                    {
                        gameManager.SetState(GameState.MainMenu);
                    });
                }
            }
        }

        private void OnProgressUpdated(SceneLoadProgressEvent evt)
        {
            _currentProgress = evt.Progress;

            if (_progressBar != null)
            {
                _progressBar.value = _currentProgress;
            }

            if (_progressText != null)
            {
                _progressText.text = $"{Mathf.RoundToInt(_currentProgress * 100f)}%";
            }
        }

        private void OnLoadCompleted(SceneLoadCompletedEvent evt)
        {
            _currentProgress = 1.0f;
            if (_progressBar != null) _progressBar.value = 1.0f;
            if (_progressText != null) _progressText.text = "100%";
        }

        private void OnGUI()
        {
            // Lightweight IMGUI fallback if uGUI Canvas is not bound
            if (_progressBar != null) return;

            int w = Screen.width;
            int h = Screen.height;

            GUIStyle titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = Mathf.Max(20, h * 4 / 100);
            titleStyle.normal.textColor = new Color(1f, 0.6f, 0.1f); // Indian Saffron tint

            GUIStyle tipStyle = new GUIStyle();
            tipStyle.alignment = TextAnchor.MiddleCenter;
            tipStyle.fontSize = Mathf.Max(14, h * 2 / 100);
            tipStyle.normal.textColor = Color.white;

            // Background
            GUI.Box(new Rect(0, 0, w, h), GUIContent.none);

            // Title
            GUI.Label(new Rect(0, h * 0.35f, w, 40), "INDIAN BIKE LIFE", titleStyle);

            // Tip
            GUI.Label(new Rect(w * 0.1f, h * 0.45f, w * 0.8f, 30), _activeTip, tipStyle);

            // Progress bar
            float barWidth = w * 0.6f;
            float barHeight = 24f;
            float barX = (w - barWidth) * 0.5f;
            float barY = h * 0.55f;

            GUI.Box(new Rect(barX, barY, barWidth, barHeight), GUIContent.none);
            GUI.Box(new Rect(barX + 2, barY + 2, (barWidth - 4) * _currentProgress, barHeight - 4), GUIContent.none);

            // Percentage Text
            GUIStyle percentStyle = new GUIStyle();
            percentStyle.alignment = TextAnchor.MiddleCenter;
            percentStyle.fontSize = 12;
            percentStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(barX, barY, barWidth, barHeight), $"Loading {Mathf.RoundToInt(_currentProgress * 100f)}%", percentStyle);
        }
    }
}

