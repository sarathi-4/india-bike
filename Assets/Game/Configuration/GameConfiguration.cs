using IndianBikeLife.Core.SceneManagement;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Configuration
{
    /// <summary>
    /// Central ScriptableObject holding top-level game configuration parameters.
    /// Eliminates hardcoded values across managers and lifecycle scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "IndianBikeLife/Configuration/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Application Identity")]
        [SerializeField] private string _gameTitle = "Indian Bike Life";
        [SerializeField] private string _versionString = "0.1.0-alpha";

        [Header("Frame Rate & Display")]
        [Tooltip("Target frame rate on mobile. Typically 30 for low-end battery saver or 60 for smooth simulation.")]
        [Range(30, 120)]
        [SerializeField] private int _targetFrameRate = 60;

        [Tooltip("VSync count. On Android, this must always be 0 (Disabled) so Application.targetFrameRate takes effect.")]
        [SerializeField] private int _vSyncCount = 0;

        [Tooltip("Keep the screen awake during gameplay (prevents dimming while riding).")]
        [SerializeField] private bool _preventSleep = true;

        [Header("Diagnostics & Logging")]
        [SerializeField] private LogLevel _minimumLogLevel = LogLevel.Info;
        [SerializeField] private bool _enableDevDiagnosticsHud = true;

        [Header("Scene Routing")]
        [SerializeField] private SceneIndex _bootSceneIndex = SceneIndex.Boot;
        [SerializeField] private SceneIndex _loadingSceneIndex = SceneIndex.Loading;
        [SerializeField] private SceneIndex _mainMenuSceneIndex = SceneIndex.MainMenu;
        [SerializeField] private bool _autoAdvanceFromBoot = true;

        // Public properties
        public string GameTitle => _gameTitle;
        public string VersionString => _versionString;
        public int TargetFrameRate => _targetFrameRate;
        public int VSyncCount => _vSyncCount;
        public bool PreventSleep => _preventSleep;
        public LogLevel MinimumLogLevel => _minimumLogLevel;
        public bool EnableDevDiagnosticsHud => _enableDevDiagnosticsHud;
        public SceneIndex BootSceneIndex => _bootSceneIndex;
        public SceneIndex LoadingSceneIndex => _loadingSceneIndex;
        public SceneIndex MainMenuSceneIndex => _mainMenuSceneIndex;
        public bool AutoAdvanceFromBoot => _autoAdvanceFromBoot;

        /// <summary>
        /// Applies runtime display and platform parameters.
        /// </summary>
        public void ApplyRuntimeSettings()
        {
            QualitySettings.vSyncCount = _vSyncCount;
            Application.targetFrameRate = _targetFrameRate;

            if (_preventSleep)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
        }
    }
}

