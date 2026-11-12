using System.Collections.Generic;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Core.SaveSystem.Migrations
{
    public interface ISaveMigrator
    {
        int SourceVersion { get; }
        int TargetVersion { get; }
        SaveData Migrate(SaveData data);
    }

    /// <summary>
    /// Coordinates sequential migrations when older save versions are loaded.
    /// Ensures backwards compatibility across game updates.
    /// </summary>
    public class SaveDataMigrationManager
    {
        private readonly List<ISaveMigrator> _migrators = new List<ISaveMigrator>();

        public void RegisterMigrator(ISaveMigrator migrator)
        {
            if (migrator != null && !_migrators.Contains(migrator))
            {
                _migrators.Add(migrator);
            }
        }

        public SaveData MigrateIfNeeded(SaveData data)
        {
            if (data == null) return null;

            int currentVersion = data.schemaVersion;
            while (currentVersion < SaveData.CurrentSchemaVersion)
            {
                ISaveMigrator nextMigrator = _migrators.Find(m => m.SourceVersion == currentVersion);
                if (nextMigrator == null)
                {
                    GameLogger.W("SaveMigration", $"No migration path found from version {currentVersion} to {SaveData.CurrentSchemaVersion}. Retaining current data.");
                    break;
                }

                GameLogger.I("SaveMigration", $"Migrating save data from schema v{nextMigrator.SourceVersion} to v{nextMigrator.TargetVersion}...");
                data = nextMigrator.Migrate(data);
                currentVersion = nextMigrator.TargetVersion;
                data.schemaVersion = currentVersion;
            }

            return data;
        }
    }
}
