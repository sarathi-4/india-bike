using IndianBikeLife.Core.SaveSystem.Data;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for the persistent save/load foundation.
    /// Handles local disk persistence, corruption recovery, and cloud save abstractions.
    /// </summary>
    public interface ISaveService : IGameService
    {
        /// <summary>
        /// Gets the current active in-memory save data snapshot.
        /// </summary>
        SaveData CurrentData { get; }

        /// <summary>
        /// Registers a saveable component into the save pipeline.
        /// </summary>
        /// <param name="saveable">Component implementing ISaveable.</param>
        void RegisterSaveable(ISaveable saveable);

        /// <summary>
        /// Unregisters a saveable component.
        /// </summary>
        /// <param name="saveable">Component to unregister.</param>
        void UnregisterSaveable(ISaveable saveable);

        /// <summary>
        /// Persists all current state to non-volatile local storage.
        /// </summary>
        /// <returns>True if save succeeded, false otherwise.</returns>
        bool Save();

        /// <summary>
        /// Loads existing saved state from storage. Falls back to default on error.
        /// </summary>
        /// <returns>True if load succeeded from disk, false if fresh defaults were generated.</returns>
        bool Load();

        /// <summary>
        /// Clears saved state on disk and resets memory state to fresh defaults.
        /// </summary>
        void ResetSaveData();

        /// <summary>
        /// Gets whether an existing valid save file is present on disk.
        /// </summary>
        bool HasExistingSaveFile();
    }

    /// <summary>
    /// Implemented by any gameplay system, manager, or entity that serializes its state.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// Unique key identifying this saveable element.
        /// </summary>
        string SaveKey { get; }

        /// <summary>
        /// Serializes current state into the target save data object.
        /// </summary>
        void PopulateSaveData(SaveData data);

        /// <summary>
        /// Restores state from the target save data object.
        /// </summary>
        void RestoreFromSaveData(SaveData data);
    }
}
