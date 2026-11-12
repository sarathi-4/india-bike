namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for diagnostic runtime performance monitoring (FPS, frame time, memory).
    /// </summary>
    public interface IPerformanceDiagnostics : IGameService
    {
        /// <summary>
        /// Current smoothed frames per second.
        /// </summary>
        float CurrentFPS { get; }

        /// <summary>
        /// Current frame time in milliseconds.
        /// </summary>
        float CurrentFrameTimeMs { get; }

        /// <summary>
        /// Total allocated managed memory in megabytes.
        /// </summary>
        float AllocatedMemoryMb { get; }

        /// <summary>
        /// Shows or hides the diagnostic on-screen overlay.
        /// </summary>
        /// <param name="show">True to show overlay.</param>
        void SetOverlayVisible(bool show);

        /// <summary>
        /// Toggles overlay visibility.
        /// </summary>
        void ToggleOverlay();
    }
}
