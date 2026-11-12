using System;
using IndianBikeLife.Core.SaveSystem.Data;

namespace IndianBikeLife.Core.SaveSystem.Cloud
{
    /// <summary>
    /// Abstraction for future cloud synchronization providers (e.g., Google Play Saved Games, Firebase).
    /// </summary>
    public interface ICloudSaveProvider
    {
        bool IsCloudAvailable { get; }
        void UploadSave(SaveData data, Action<bool, string> onComplete);
        void DownloadSave(Action<bool, SaveData, string> onComplete);
    }

    /// <summary>
    /// Fallback provider when offline or before cloud accounts are linked.
    /// </summary>
    public class LocalFallbackCloudProvider : ICloudSaveProvider
    {
        public bool IsCloudAvailable => false;

        public void UploadSave(SaveData data, Action<bool, string> onComplete)
        {
            onComplete?.Invoke(false, "Cloud save provider is not configured in Stage 1.");
        }

        public void DownloadSave(Action<bool, SaveData, string> onComplete)
        {
            onComplete?.Invoke(false, null, "Cloud save provider is not configured in Stage 1.");
        }
    }
}
