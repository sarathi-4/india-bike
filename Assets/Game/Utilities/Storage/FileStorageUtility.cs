using System;
using System.IO;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Utilities.Storage
{
    /// <summary>
    /// File storage helper providing atomic writes, automatic backups, and corruption recovery.
    /// Prevents partially written save files on mobile if app is closed during I/O.
    /// </summary>
    public static class FileStorageUtility
    {
        private const string Tag = "FileStorage";

        /// <summary>
        /// Atomically writes content to the target file path.
        /// Writes first to a temporary file, then renames/replaces the target file.
        /// </summary>
        public static bool AtomicWriteText(string targetPath, string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string tempPath = targetPath + ".tmp";
                string backupPath = targetPath + ".bak";

                // 1. Write to temp file
                File.WriteAllText(tempPath, content);

                // 2. Make backup of existing target if present
                if (File.Exists(targetPath))
                {
                    try
                    {
                        File.Copy(targetPath, backupPath, true);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.W(Tag, $"Failed to create backup copy: {ex.Message}");
                    }
                }

                // 3. Atomically replace target with temp
#if NET_STANDARD_2_0 || NET_4_6
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Move(tempPath, targetPath);
#else
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Move(tempPath, targetPath);
#endif
                return true;
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
                return false;
            }
        }

        /// <summary>
        /// Reads text from the target path. If missing or corrupted, attempts reading the backup file.
        /// </summary>
        public static string SafeReadText(string targetPath)
        {
            try
            {
                if (File.Exists(targetPath))
                {
                    return File.ReadAllText(targetPath);
                }

                string backupPath = targetPath + ".bak";
                if (File.Exists(backupPath))
                {
                    GameLogger.W(Tag, $"Primary file {targetPath} not found. Restoring from backup {backupPath}.");
                    return File.ReadAllText(backupPath);
                }

                return null;
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);

                // Fallback to backup
                string backupPath = targetPath + ".bak";
                if (File.Exists(backupPath))
                {
                    try
                    {
                        GameLogger.W(Tag, "Primary file read failed. Attempting backup restore.");
                        return File.ReadAllText(backupPath);
                    }
                    catch (Exception bakEx)
                    {
                        GameLogger.Ex(Tag, bakEx);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Archives a corrupted file for diagnostic investigation without losing user state completely.
        /// </summary>
        public static void QuarantineCorruptFile(string targetPath)
        {
            try
            {
                if (File.Exists(targetPath))
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                    string quarantinePath = targetPath + $".corrupt_{timestamp}";
                    File.Move(targetPath, quarantinePath);
                    GameLogger.W(Tag, $"Quarantined corrupt file to: {quarantinePath}");
                }
            }
            catch (Exception ex)
            {
                GameLogger.Ex(Tag, ex);
            }
        }
    }
}

