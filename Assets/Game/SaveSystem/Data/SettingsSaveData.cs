using System;
using IndianBikeLife.Core.Audio;
using IndianBikeLife.Core.Configuration;

namespace IndianBikeLife.Core.SaveSystem.Data
{
    /// <summary>
    /// User preferences and hardware settings saved across sessions.
    /// </summary>
    [Serializable]
    public class SettingsSaveData
    {
        // Audio Settings
        public float masterVolume = 0.85f;
        public float musicVolume = 0.75f;
        public float sfxVolume = 0.85f;
        public float vehicleVolume = 0.90f;
        public float ambientVolume = 0.70f;
        public float uiVolume = 0.80f;

        public bool masterMuted = false;
        public bool musicMuted = false;
        public bool sfxMuted = false;

        // Graphics Settings
        public int graphicsQualityLevel = (int)MobileQualityPreset.Medium;
        public int targetFrameRate = 60;

        // Control Settings
        public int controlType = 0; // 0 = TouchButtons, 1 = Tilt, 2 = Gamepad
        public float steeringSensitivity = 1.0f;
        public bool invertSteering = false;
        public bool hapticFeedbackEnabled = true;

        public static SettingsSaveData CreateDefault()
        {
            return new SettingsSaveData();
        }
    }
}
