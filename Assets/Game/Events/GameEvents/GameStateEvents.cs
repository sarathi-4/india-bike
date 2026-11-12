using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.State;

namespace IndianBikeLife.Core.Events.GameEvents
{
    /// <summary>
    /// Dispatched when the game lifecycle state transitions.
    /// </summary>
    public readonly struct GameStateChangedEvent : IGameEvent
    {
        public GameState PreviousState { get; }
        public GameState NewState { get; }

        public GameStateChangedEvent(GameState previousState, GameState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }

    /// <summary>
    /// Dispatched when the game is paused or resumed.
    /// </summary>
    public readonly struct GamePauseToggledEvent : IGameEvent
    {
        public bool IsPaused { get; }

        public GamePauseToggledEvent(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }
}

