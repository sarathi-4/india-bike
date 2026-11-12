using System;
using System.Collections.Generic;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.SaveSystem.Serializers;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Backend
{
    public enum ConflictResolutionPolicy
    {
        ServerWins,
        ClientWinsIfNewer,
        ManualPrompt
    }

    /// <summary>
    /// Synchronizes SaveData between local disk and cloud storage,
    /// detecting conflicts and enforcing deterministic conflict-resolution policies.
    /// </summary>
    public class CloudSaveManager
    {
        private const string Tag = "CloudSaveManager";

        private readonly Dictionary<string, string> _cloudStorageMock = new Dictionary<string, string>();
        private readonly ISerializer _serializer = new JsonSaveSerializer();
        private ConflictResolutionPolicy _policy = ConflictResolutionPolicy.ClientWinsIfNewer;

        public ConflictResolutionPolicy Policy
        {
            get => _policy;
            set => _policy = value;
        }

        public void SaveToCloud(string userId, SaveData localData, Action<bool, string> onComplete)
        {
            if (string.IsNullOrEmpty(userId) || localData == null)
            {
                onComplete?.Invoke(false, "Invalid user or null save data.");
                return;
            }

            try
            {
                // Check if existing cloud save exists for conflict detection
                if (_cloudStorageMock.TryGetValue(userId, out string existingJson))
                {
                    SaveData serverData = _serializer.Deserialize(existingJson);
                    if (serverData != null)
                    {
                        DateTime localTime = DateTime.Parse(localData.saveTimestampUtc);
                        DateTime serverTime = DateTime.Parse(serverData.saveTimestampUtc);

                        if (_policy == ConflictResolutionPolicy.ClientWinsIfNewer && serverTime > localTime)
                        {
                            GameLogger.W(Tag, $"Cloud conflict detected! Server data is newer ({serverTime:O}) than local ({localTime:O}). Cloud save rejected.");
                            onComplete?.Invoke(false, "Cloud data is newer than local data. Perform sync first.");
                            return;
                        }
                    }
                }

                localData.saveTimestampUtc = DateTime.UtcNow.ToString("o");
                localData.checksum = localData.ComputeChecksum();

                string json = _serializer.Serialize(localData);
                _cloudStorageMock[userId] = json;

                GameLogger.I(Tag, $"Successfully persisted cloud save for user {userId} (Schema v{localData.schemaVersion}).");
                onComplete?.Invoke(true, "Cloud save succeeded.");
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
                onComplete?.Invoke(false, ex.Message);
            }
        }

        public void LoadFromCloud(string userId, Action<bool, SaveData, string> onComplete)
        {
            if (string.IsNullOrEmpty(userId))
            {
                onComplete?.Invoke(false, null, "User not authenticated.");
                return;
            }

            if (!_cloudStorageMock.TryGetValue(userId, out string json))
            {
                onComplete?.Invoke(false, null, "No cloud save found for this user.");
                return;
            }

            try
            {
                SaveData loaded = _serializer.Deserialize(json);
                if (loaded != null)
                {
                    GameLogger.I(Tag, $"Loaded cloud save for user {userId} (Timestamp: {loaded.saveTimestampUtc}).");
                    onComplete?.Invoke(true, loaded, "Cloud load succeeded.");
                }
                else
                {
                    onComplete?.Invoke(false, null, "Failed to deserialize cloud save.");
                }
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
                onComplete?.Invoke(false, null, ex.Message);
            }
        }
    }
}
