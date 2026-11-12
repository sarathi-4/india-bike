using System;
using System.Collections.Generic;
using System.IO;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.SaveSystem.Migrations;
using IndianBikeLife.Core.SaveSystem.Serializers;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.Core.Utilities.Storage;
using UnityEngine;

namespace IndianBikeLife.Core.Managers
{
    /// <summary>
    /// Core persistent manager implementing ISaveService.
    /// Handles disk serialization, atomic backups, corruption recovery, and system state synchronization.
    /// </summary>
    public class SaveManager : MonoBehaviour, ISaveService
    {
        private const string Tag = "SaveManager";
        private const string SaveFileName = "indian_bike_save.json";

        private readonly List<ISaveable> _saveables = new List<ISaveable>();
        private readonly SaveDataMigrationManager _migrationManager = new SaveDataMigrationManager();
        private ISerializer _serializer;

        private SaveData _currentData;
        private string _saveFilePath;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public SaveData CurrentData => _currentData;
        public string SaveFilePath => _saveFilePath;

        public void Initialize()
        {
            if (_isInitialized) return;

            _saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
            _serializer = new JsonSaveSerializer(prettyPrint: true);

            GameLogger.I(Tag, $"Initializing Save System. Path: {_saveFilePath}");

            // Load existing data or generate fresh defaults
            Load();

            _isInitialized = true;
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            // Auto-persist on shutdown
            Save();
            _saveables.Clear();
            _isInitialized = false;
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable == null) return;
            if (!_saveables.Contains(saveable))
            {
                _saveables.Add(saveable);
                // Immediately restore state from existing memory snapshot
                if (_currentData != null)
                {
                    try
                    {
                        saveable.RestoreFromSaveData(_currentData);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.Ex(Tag, ex);
                    }
                }
            }
        }

        public void UnregisterSaveable(ISaveable saveable)
        {
            if (saveable != null && _saveables.Contains(saveable))
            {
                _saveables.Remove(saveable);
            }
        }

        public bool Save()
        {
            if (_currentData == null)
            {
                _currentData = SaveData.CreateDefault();
            }

            EventBus.Publish(new SaveStartedEvent());

            try
            {
                // Gather state from all registered saveable entities
                for (int i = 0; i < _saveables.Count; i++)
                {
                    if (_saveables[i] != null)
                    {
                        try
                        {
                            _saveables[i].PopulateSaveData(_currentData);
                        }
                        catch (Exception ex)
                        {
                            GameLogger.Ex(Tag, ex);
                        }
                    }
                }

                _currentData.saveTimestampUtc = DateTime.UtcNow.ToString("o");

                string serializedJson = _serializer.Serialize(_currentData);
                bool success = FileStorageUtility.AtomicWriteText(_saveFilePath, serializedJson);

                if (success)
                {
                    GameLogger.I(Tag, "Save operation completed successfully.");
                    EventBus.Publish(new SaveCompletedEvent(true, _saveFilePath));
                    return true;
                }
                else
                {
                    GameLogger.E(Tag, "Failed to write save file to disk.");
                    EventBus.Publish(new SaveErrorEvent("Save", "Disk write failure"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
                EventBus.Publish(new SaveErrorEvent("Save", ex.Message));
                return false;
            }
        }

        public bool Load()
        {
            try
            {
                if (!File.Exists(_saveFilePath))
                {
                    GameLogger.I(Tag, "No existing save file found. Initializing fresh defaults.");
                    _currentData = SaveData.CreateDefault();
                    NotifySaveablesRestored();
                    return false;
                }

                string rawJson = FileStorageUtility.SafeReadText(_saveFilePath);
                if (string.IsNullOrEmpty(rawJson))
                {
                    GameLogger.W(Tag, "Save file was empty or unreadable. Falling back to defaults.");
                    _currentData = SaveData.CreateDefault();
                    NotifySaveablesRestored();
                    return false;
                }

                SaveData loadedData = _serializer.Deserialize(rawJson);
                if (loadedData == null)
                {
                    GameLogger.E(Tag, "Deserialization returned null. Quarantining corrupt file.");
                    FileStorageUtility.QuarantineCorruptFile(_saveFilePath);
                    _currentData = SaveData.CreateDefault();
                    NotifySaveablesRestored();
                    return false;
                }

                // Verify checksum integrity
                string expectedChecksum = loadedData.ComputeChecksum();
                if (!string.IsNullOrEmpty(loadedData.checksum) && loadedData.checksum != expectedChecksum)
                {
                    GameLogger.W(Tag, "Save file checksum mismatch! Data may have been tampered with or corrupted.");
                }

                // Migrate schema if loaded from an older build
                if (loadedData.schemaVersion < SaveData.CurrentSchemaVersion)
                {
                    loadedData = _migrationManager.MigrateIfNeeded(loadedData);
                }

                _currentData = loadedData;
                GameLogger.I(Tag, $"Loaded save data successfully. Schema v{_currentData.schemaVersion}, timestamp: {_currentData.saveTimestampUtc}");

                NotifySaveablesRestored();
                return true;
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
                FileStorageUtility.QuarantineCorruptFile(_saveFilePath);
                _currentData = SaveData.CreateDefault();
                NotifySaveablesRestored();
                return false;
            }
        }

        public void ResetSaveData()
        {
            GameLogger.I(Tag, "Resetting save data to default values.");
            _currentData = SaveData.CreateDefault();

            try
            {
                if (File.Exists(_saveFilePath))
                {
                    File.Delete(_saveFilePath);
                }
                string bakPath = _saveFilePath + ".bak";
                if (File.Exists(bakPath))
                {
                    File.Delete(bakPath);
                }
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
            }

            Save();
            NotifySaveablesRestored();
            EventBus.Publish(new SaveResetEvent());
        }

        public bool HasExistingSaveFile()
        {
            return File.Exists(_saveFilePath);
        }

        private void NotifySaveablesRestored()
        {
            for (int i = 0; i < _saveables.Count; i++)
            {
                if (_saveables[i] != null)
                {
                    try
                    {
                        _saveables[i].RestoreFromSaveData(_currentData);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.Ex(Tag, ex);
                    }
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // Auto-save when mobile app loses focus or is backgrounded
            if (pauseStatus && _isInitialized)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            if (_isInitialized)
            {
                Save();
            }
        }
    }
}
