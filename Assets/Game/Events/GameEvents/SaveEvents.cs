using IndianBikeLife.Core.Events.Core;

namespace IndianBikeLife.Core.Events.GameEvents
{
    /// <summary>
    /// Dispatched before saving begins.
    /// </summary>
    public readonly struct SaveStartedEvent : IGameEvent
    {
    }

    /// <summary>
    /// Dispatched after save operations write successfully to disk.
    /// </summary>
    public readonly struct SaveCompletedEvent : IGameEvent
    {
        public bool Success { get; }
        public string SaveFilePath { get; }

        public SaveCompletedEvent(bool success, string saveFilePath)
        {
            Success = success;
            SaveFilePath = saveFilePath;
        }
    }

    /// <summary>
    /// Dispatched when saving or loading encounters an error.
    /// </summary>
    public readonly struct SaveErrorEvent : IGameEvent
    {
        public string Operation { get; }
        public string ErrorMessage { get; }

        public SaveErrorEvent(string operation, string errorMessage)
        {
            Operation = operation;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Dispatched when save data is cleared / reset to defaults.
    /// </summary>
    public readonly struct SaveResetEvent : IGameEvent
    {
    }
}
