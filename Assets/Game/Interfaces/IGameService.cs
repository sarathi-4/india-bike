namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Base interface for core services managed within the service architecture.
    /// Ensures consistent lifecycle management (initialization and shutdown).
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Gets whether the service has completed its initialization.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the service with any required dependencies.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gracefully shuts down the service, releasing resources and unsubscribing from events.
        /// </summary>
        void Shutdown();
    }
}

