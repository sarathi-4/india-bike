using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.State;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Managers
{
    /// <summary>
    /// Central lifecycle coordinator implementing IGameManager.
    /// Maintains game state, pause state, and high-level application lifecycle.
    /// Does not contain monolithic gameplay logic; coordinates via interfaces and events.
    /// </summary>
    public class GameManager : MonoBehaviour, IGameManager
    {
        private const string Tag = "GameManager";

        private GameState _currentState = GameState.None;
        private bool _isPaused;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public GameState CurrentState => _currentState;
        public bool IsPaused => _isPaused;

        public void Initialize()
        {
            if (_isInitialized) return;

            GameLogger.I(Tag, "GameManager initialized.");
            _isInitialized = true;
        }

        public void Shutdown()
        {
            SetState(GameState.ShuttingDown);
            _isInitialized = false;
        }

        public void SetState(GameState newState)
        {
            if (_currentState == newState) return;

            GameState previousState = _currentState;
            _currentState = newState;

            GameLogger.I(Tag, $"State Transition: {previousState} -> {newState}");
            EventBus.Publish(new GameStateChangedEvent(previousState, newState));
        }

        public void SetPaused(bool pause)
        {
            if (_isPaused == pause) return;

            _isPaused = pause;
            Time.timeScale = _isPaused ? 0f : 1f;

            GameLogger.I(Tag, $"Game pause set to: {_isPaused}");
            EventBus.Publish(new GamePauseToggledEvent(_isPaused));
        }

        public void TogglePause()
        {
            SetPaused(!_isPaused);
        }

        public void QuitGame()
        {
            GameLogger.I(Tag, "Application quit requested.");

            // Ensure save data is flushed before exiting
            if (ServiceLocator.TryGet(out ISaveService saveService))
            {
                saveService.Save();
            }

            SetState(GameState.ShuttingDown);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && _currentState == GameState.Gameplay && !_isPaused)
            {
                // Auto-pause if focus is lost (incoming phone call, Android notification)
                SetPaused(true);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _currentState == GameState.Gameplay && !_isPaused)
            {
                SetPaused(true);
            }
        }
    }
}

