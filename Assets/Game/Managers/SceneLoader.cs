using System;
using System.Collections;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SceneManagement;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace IndianBikeLife.Core.Managers
{
    /// <summary>
    /// Core scene loading manager implementing ISceneLoader.
    /// Handles asynchronous scene loading, progress smoothing, loading screens, and additive streaming.
    /// </summary>
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        private const string Tag = "SceneLoader";

        [SerializeField] private float _defaultMinLoadDuration = 1.0f;
        [SerializeField] private float _progressSmoothingSpeed = 3.0f;

        private SceneIndex _currentScene = SceneIndex.Boot;
        private bool _isLoading;
        private float _loadingProgress;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public SceneIndex CurrentScene => _currentScene;
        public bool IsLoading => _isLoading;
        public float LoadingProgress => _loadingProgress;

        public void Initialize()
        {
            if (_isInitialized) return;

            int activeBuildIndex = UnitySceneManager.GetActiveScene().buildIndex;
            _currentScene = Enum.IsDefined(typeof(SceneIndex), activeBuildIndex)
                ? (SceneIndex)activeBuildIndex
                : SceneIndex.Boot;

            GameLogger.I(Tag, $"SceneLoader initialized. Active scene: {_currentScene} (Index: {activeBuildIndex})");
            _isInitialized = true;
        }

        public void Shutdown()
        {
            StopAllCoroutines();
            _isLoading = false;
            _isInitialized = false;
        }

        public void LoadScene(SceneIndex targetScene, Action onComplete = null)
        {
            if (_isLoading)
            {
                GameLogger.W(Tag, $"Cannot load {targetScene} because another transition is in progress.");
                return;
            }

            StartCoroutine(LoadSceneRoutine(targetScene, (int)targetScene, targetScene.ToString(), onComplete));
        }

        public void LoadScene(int sceneBuildIndex, Action onComplete = null)
        {
            if (_isLoading)
            {
                GameLogger.W(Tag, $"Cannot load scene {sceneBuildIndex} because another transition is in progress.");
                return;
            }

            SceneIndex targetIndex = Enum.IsDefined(typeof(SceneIndex), sceneBuildIndex)
                ? (SceneIndex)sceneBuildIndex
                : SceneIndex.Boot;

            StartCoroutine(LoadSceneRoutine(targetIndex, sceneBuildIndex, $"Index_{sceneBuildIndex}", onComplete));
        }

        public void LoadScene(string sceneName, Action onComplete = null)
        {
            if (_isLoading)
            {
                GameLogger.W(Tag, $"Cannot load {sceneName} because another transition is in progress.");
                return;
            }

            SceneIndex targetIndex = SceneIndex.Boot;
            if (Enum.TryParse(sceneName, out SceneIndex parsed))
            {
                targetIndex = parsed;
            }

            StartCoroutine(LoadSceneRoutine(targetIndex, -1, sceneName, onComplete));
        }

        private IEnumerator LoadSceneRoutine(SceneIndex targetIndex, int buildIndex, string sceneName, Action onComplete)
        {
            _isLoading = true;
            _loadingProgress = 0f;

            GameLogger.I(Tag, $"Starting scene load: {sceneName}");
            EventBus.Publish(new SceneLoadStartedEvent(targetIndex, sceneName));

            float startTime = Time.unscaledTime;
            AsyncOperation operation;

            if (buildIndex >= 0)
            {
                operation = UnitySceneManager.LoadSceneAsync(buildIndex);
            }
            else
            {
                operation = UnitySceneManager.LoadSceneAsync(sceneName);
            }

            if (operation == null)
            {
                GameLogger.E(Tag, $"Failed to initiate async load for scene: {sceneName}");
                _isLoading = false;
                yield break;
            }

            // Prevent scene from activating until we finish smooth progress
            operation.allowSceneActivation = false;

            float smoothedProgress = 0f;

            while (!operation.isDone)
            {
                // Unity reports progress from 0.0 to 0.9 before activation
                float targetNormalized = Mathf.Clamp01(operation.progress / 0.9f);

                smoothedProgress = Mathf.MoveTowards(
                    smoothedProgress,
                    targetNormalized,
                    Time.unscaledDeltaTime * _progressSmoothingSpeed
                );

                _loadingProgress = smoothedProgress;
                EventBus.Publish(new SceneLoadProgressEvent(targetIndex, _loadingProgress));

                float elapsed = Time.unscaledTime - startTime;

                // When Unity is ready (progress >= 0.9) and minimum display duration has passed
                if (operation.progress >= 0.9f && smoothedProgress >= 0.99f && elapsed >= _defaultMinLoadDuration)
                {
                    _loadingProgress = 1.0f;
                    EventBus.Publish(new SceneLoadProgressEvent(targetIndex, 1.0f));
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            _currentScene = targetIndex;
            _isLoading = false;

            GameLogger.I(Tag, $"Scene load completed: {sceneName}");
            EventBus.Publish(new SceneLoadCompletedEvent(targetIndex, sceneName));

            onComplete?.Invoke();
        }

        #region Additive Scene Streaming Support

        public void LoadSceneAdditive(string sceneName, Action onComplete = null)
        {
            StartCoroutine(LoadAdditiveRoutine(sceneName, onComplete));
        }

        public void UnloadSceneAdditive(string sceneName, Action onComplete = null)
        {
            StartCoroutine(UnloadAdditiveRoutine(sceneName, onComplete));
        }

        private IEnumerator LoadAdditiveRoutine(string sceneName, Action onComplete)
        {
            GameLogger.I(Tag, $"Streaming in additive scene: {sceneName}");
            AsyncOperation operation = UnitySceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            if (operation == null)
            {
                GameLogger.E(Tag, $"Failed to additively load scene {sceneName}");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            GameLogger.I(Tag, $"Additive scene loaded: {sceneName}");
            onComplete?.Invoke();
        }

        private IEnumerator UnloadAdditiveRoutine(string sceneName, Action onComplete)
        {
            GameLogger.I(Tag, $"Unloading additive scene: {sceneName}");
            AsyncOperation operation = UnitySceneManager.UnloadSceneAsync(sceneName);

            if (operation == null)
            {
                GameLogger.W(Tag, $"Scene {sceneName} was not loaded additively.");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            GameLogger.I(Tag, $"Additive scene unloaded: {sceneName}");
            onComplete?.Invoke();
        }

        #endregion
    }
}

