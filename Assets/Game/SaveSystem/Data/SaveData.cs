using System;
using UnityEngine;

namespace IndianBikeLife.Core.SaveSystem.Data
{
    /// <summary>
    /// Root data container for Indian Bike Life persistence.
    /// Includes schema versioning and integrity verification.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public const int CurrentSchemaVersion = 2;

        public int schemaVersion = CurrentSchemaVersion;
        public string saveTimestampUtc;
        public SettingsSaveData settings;
        public PlayerProfileSaveData playerProfile;
        public ProgressionSaveData progression;
        public string checksum;

        public static SaveData CreateDefault()
        {
            return new SaveData
            {
                schemaVersion = CurrentSchemaVersion,
                saveTimestampUtc = DateTime.UtcNow.ToString("o"),
                settings = SettingsSaveData.CreateDefault(),
                playerProfile = PlayerProfileSaveData.CreateDefault(),
                progression = ProgressionSaveData.CreateDefault(),
                checksum = string.Empty
            };
        }

        /// <summary>
        /// Computes a lightweight checksum hash across core fields to detect corrupted files.
        /// </summary>
        public string ComputeChecksum()
        {
            int hash = 17;
            hash = hash * 31 + schemaVersion.GetHashCode();
            if (settings != null)
            {
                hash = hash * 31 + settings.masterVolume.GetHashCode();
                hash = hash * 31 + settings.graphicsQualityLevel.GetHashCode();
            }
            if (playerProfile != null)
            {
                hash = hash * 31 + (playerProfile.playerId?.GetHashCode() ?? 0);
                hash = hash * 31 + playerProfile.playerCoins.GetHashCode();
            }
            if (progression != null)
            {
                hash = hash * 31 + progression.silverCoins.GetHashCode();
                hash = hash * 31 + progression.goldCoins.GetHashCode();
                hash = hash * 31 + progression.currentLevel.GetHashCode();
            }
            return hash.ToString("X8");
        }
    }
}

