using System;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for centralized, category-tagged logging with configurable stripping.
    /// </summary>
    public interface ILoggerService : IGameService
    {
        /// <summary>
        /// Gets or sets current minimum log level threshold.
        /// </summary>
        LogLevel MinimumLogLevel { get; set; }

        /// <summary>
        /// Logs verbose diagnostic information.
        /// </summary>
        void LogVerbose(string tag, string message);

        /// <summary>
        /// Logs general informational messages.
        /// </summary>
        void LogInfo(string tag, string message);

        /// <summary>
        /// Logs warning messages.
        /// </summary>
        void LogWarning(string tag, string message);

        /// <summary>
        /// Logs error messages.
        /// </summary>
        void LogError(string tag, string message);

        /// <summary>
        /// Logs exception details safely with context.
        /// </summary>
        void LogException(string tag, Exception exception);
    }
}
