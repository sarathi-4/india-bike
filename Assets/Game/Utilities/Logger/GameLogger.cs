using System;
using IndianBikeLife.Core.Interfaces;
using UnityEngine;

namespace IndianBikeLife.Core.Utilities.Logger
{
    /// <summary>
    /// Centralized logging utility adhering to ILoggerService.
    /// Provides consistent category tags, level filtering, and safe exception logging.
    /// In release non-development builds, lower severity logs are safely suppressed.
    /// </summary>
    public class GameLogger : ILoggerService
    {
        private static GameLogger _instance;
        private LogLevel _minimumLogLevel = LogLevel.Info;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public LogLevel MinimumLogLevel
        {
            get => _minimumLogLevel;
            set => _minimumLogLevel = value;
        }

        public static GameLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameLogger();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public void Initialize()
        {
            if (_isInitialized) return;

#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
            // Default to Warning or higher in release builds for mobile performance
            _minimumLogLevel = LogLevel.Warning;
#else
            _minimumLogLevel = LogLevel.Verbose;
#endif

            _isInitialized = true;
            _instance = this;
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void LogVerbose(string tag, string message)
        {
            if (_minimumLogLevel > LogLevel.Verbose) return;
            Debug.Log(FormatLog("VERBOSE", tag, message));
        }

        public void LogInfo(string tag, string message)
        {
            if (_minimumLogLevel > LogLevel.Info) return;
            Debug.Log(FormatLog("INFO", tag, message));
        }

        public void LogWarning(string tag, string message)
        {
            if (_minimumLogLevel > LogLevel.Warning) return;
            Debug.LogWarning(FormatLog("WARN", tag, message));
        }

        public void LogError(string tag, string message)
        {
            if (_minimumLogLevel > LogLevel.Error) return;
            Debug.LogError(FormatLog("ERROR", tag, message));
        }

        public void LogException(string tag, Exception exception)
        {
            if (_minimumLogLevel > LogLevel.Error) return;
            Debug.LogError($"[IndianBikeLife] [{tag}] [EXCEPTION] {exception?.Message}\n{exception?.StackTrace}");
        }

        private static string FormatLog(string level, string tag, string message)
        {
            return $"[IndianBikeLife] [{tag}] [{level}] {message}";
        }

        #region Static Convenience Helpers

        public static void V(string tag, string message) => Instance.LogVerbose(tag, message);
        public static void I(string tag, string message) => Instance.LogInfo(tag, message);
        public static void W(string tag, string message) => Instance.LogWarning(tag, message);
        public static void E(string tag, string message) => Instance.LogError(tag, message);
        public static void Ex(string tag, Exception exception) => Instance.LogException(tag, exception);

        #endregion
    }
}
