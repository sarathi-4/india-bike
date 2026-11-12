using IndianBikeLife.Core.State;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for game lifecycle coordination and overall application state.
    /// </summary>
    public interface IGameManager : IGameService
    {
        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        GameState CurrentState { get; }

        /// <summary>
        /// Gets whether the game is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Requests a transition to a new game state.
        /// </summary>
        /// <param name="newState">The destination game state.</param>
        void SetState(GameState newState);

        /// <summary>
        /// Pauses or unpauses the game simulation.
        /// </summary>
        /// <param name="pause">True to pause, false to resume.</param>
        void SetPaused(bool pause);

        /// <summary>
        /// Toggles the current pause state.
        /// </summary>
        void TogglePause();

        /// <summary>
        /// Initiates a clean exit of the game application.
        /// </summary>
        void QuitGame();
    }
}
