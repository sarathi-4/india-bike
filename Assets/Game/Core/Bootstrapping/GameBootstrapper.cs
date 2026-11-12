using System;
using System.Collections;
using IndianBikeLife.Core.Configuration;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Managers;
using IndianBikeLife.Core.SceneManagement;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.State;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;
using AudioConfiguration = IndianBikeLife.Core.Configuration.AudioConfiguration;

namespace IndianBikeLife.Core.Bootstrapping
{
    /// <summary>
    /// Master entry point and bootstrap coordinator for Indian Bike Life.
    /// Executes the deterministic startup pipeline, enforces persistent singleton hierarchy,
    /// and safely transitions into the game runtime.
    /// </summary>
    [DefaultExecutionOrder(-1000)] // Ensures bootstrapper executes before all other scripts
    public class GameBootstrapper : MonoBehaviour
    {
        private const string Tag = "Bootstrap";
        private static GameBootstrapper _instance;

        [Header("Configurations")]
        [SerializeField] private GameConfiguration _gameConfig;
        [SerializeField] private AudioConfiguration _audioConfig;
        [SerializeField] private GraphicsConfiguration _graphicsConfig;

        [Header("Manager References (Optional - Auto-Created if Missing)")]
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private SaveManager _saveManager;
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private PerformanceManager _performanceManager;

        private bool _isBootstrapped;

        public static GameBootstrapper Instance => _instance;
        public bool IsBootstrapped => _isBootstrapped;

        private void Awake()
        {
            // Enforce single persistent bootstrap hierarchy
            if (_instance != null && _instance != this)
            {
                GameLogger.I(Tag, "Duplicate Bootstrapper detected. Destroying duplicate instance.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            ExecuteBootstrap();
        }

        /// <summary>
        /// Executes the ordered system startup sequence.
        /// </summary>
        public void ExecuteBootstrap()
        {
            if (_isBootstrapped) return;

            try
            {
                // Step 1: Initialize Logging
                GameLogger.Instance.Initialize();
                if (_gameConfig != null)
                {
                    GameLogger.Instance.MinimumLogLevel = _gameConfig.MinimumLogLevel;
                }
                GameLogger.I(Tag, "==================================================");
                GameLogger.I(Tag, "   INDIAN BIKE LIFE — STARTING ENGINE BOOTSTRAP   ");
                GameLogger.I(Tag, "==================================================");

                // Step 2: Ensure manager components exist on this hierarchy
                EnsureManagersExist();

                // Step 3: Register services in ServiceLocator
                RegisterCoreServices();

                // Step 4: Initialize all services in dependency order
                InitializeServices();

                // Step 5: Apply platform and configuration settings
                ApplyConfigurations();

                // Step 6: Hook saveable components
                RegisterSaveables();

                // Step 7: Transition Game State
                _gameManager.SetState(GameState.Booting);
                _isBootstrapped = true;
                GameLogger.I(Tag, "Bootstrap pipeline completed successfully.");

                // Step 8: Route to the next scene (Loading -> MainMenu)
                if (_gameConfig != null && _gameConfig.AutoAdvanceFromBoot)
                {
                    StartCoroutine(AdvanceFromBootRoutine());
                }
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
                Debug.LogError($"[Bootstrap] Critical startup failure: {ex.Message}");
            }
        }

        private void EnsureManagersExist()
        {
            if (_gameManager == null) _gameManager = GetOrAddComponent<GameManager>();
            if (_sceneLoader == null) _sceneLoader = GetOrAddComponent<SceneLoader>();
            if (_saveManager == null) _saveManager = GetOrAddComponent<SaveManager>();
            if (_audioManager == null) _audioManager = GetOrAddComponent<AudioManager>();
            if (_inputManager == null) _inputManager = GetOrAddComponent<InputManager>();
            if (_performanceManager == null) _performanceManager = GetOrAddComponent<PerformanceManager>();
        }

        private T GetOrAddComponent<T>() where T : Component
        {
            T component = GetComponentInChildren<T>();
            if (component == null)
            {
                GameObject child = new GameObject(typeof(T).Name);
                child.transform.SetParent(transform);
                component = child.AddComponent<T>();
            }
            return component;
        }

        private void RegisterCoreServices()
        {
            ServiceLocator.Reset();

            ServiceLocator.Register<ILoggerService>(GameLogger.Instance);
            ServiceLocator.Register<IGameManager>(_gameManager);
            ServiceLocator.Register<ISceneLoader>(_sceneLoader);
            ServiceLocator.Register<ISaveService>(_saveManager);
            ServiceLocator.Register<IAudioService>(_audioManager);
            ServiceLocator.Register<IInputService>(_inputManager);
            ServiceLocator.Register<IPerformanceDiagnostics>(_performanceManager);

            GameLogger.I(Tag, "Registered 7 core services in ServiceLocator.");
        }

        private void InitializeServices()
        {
            _performanceManager.Initialize();
            _saveManager.Initialize();
            _audioManager.Initialize();
            _inputManager.Initialize();
            _sceneLoader.Initialize();
            _gameManager.Initialize();
        }

        private void ApplyConfigurations()
        {
            if (_gameConfig != null)
            {
                _gameConfig.ApplyRuntimeSettings();
            }

            if (_graphicsConfig != null)
            {
                // Check if user has saved graphics preference
                int savedPresetIndex = _saveManager.CurrentData?.settings?.graphicsQualityLevel ?? (int)_graphicsConfig.DefaultPreset;
                MobileQualityPreset preset = (MobileQualityPreset)savedPresetIndex;
                _graphicsConfig.ApplyPreset(preset);
            }
        }

        private void RegisterSaveables()
        {
            _saveManager.RegisterSaveable(_audioManager);
        }

        private IEnumerator AdvanceFromBootRoutine()
        {
            // Give one frame for all Awake/Start calls in the scene to settle
            yield return null;

            SceneIndex nextScene = _gameConfig.LoadingSceneIndex;
            GameLogger.I(Tag, $"Advancing from Boot to: {nextScene}");

            _gameManager.SetState(GameState.Loading);
            _sceneLoader.LoadScene(nextScene);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                GameLogger.I(Tag, "Bootstrapper destroyed. Cleaning up services.");
                ServiceLocator.Reset();
                _instance = null;
            }
        }
    }
}

