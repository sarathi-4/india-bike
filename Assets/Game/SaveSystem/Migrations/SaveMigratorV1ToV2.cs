using System.Collections.Generic;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Core.SaveSystem.Migrations
{
    /// <summary>
    /// Migrates SaveData from Schema v1 to Schema v2.
    /// Introduces the ProgressionSaveData container while preserving legacy coins and completed missions.
    /// </summary>
    public class SaveMigratorV1ToV2 : ISaveMigrator
    {
        public int SourceVersion => 1;
        public int TargetVersion => 2;

        public SaveData Migrate(SaveData data)
        {
            if (data == null) return null;

            GameLogger.I("SaveMigratorV1ToV2", "Executing migration from Schema v1 to Schema v2...");

            if (data.progression == null)
            {
                data.progression = ProgressionSaveData.CreateDefault();
            }

            if (data.playerProfile != null)
            {
                // Transfer coins
                data.progression.silverCoins = data.playerProfile.playerCoins;
                data.progression.currentLevel = data.playerProfile.playerLevel > 0 ? data.playerProfile.playerLevel : 1;

                // Transfer completed missions if present in legacy profile
                if (data.playerProfile.completedMissionIds != null)
                {
                    data.progression.completedMissionIds = new List<string>(data.playerProfile.completedMissionIds);
                }
            }

            data.schemaVersion = 2;
            data.checksum = data.ComputeChecksum();

            GameLogger.I("SaveMigratorV1ToV2", $"Migration complete. SilverCoins={data.progression.silverCoins}, Level={data.progression.currentLevel}");
            return data;
        }
    }
}

