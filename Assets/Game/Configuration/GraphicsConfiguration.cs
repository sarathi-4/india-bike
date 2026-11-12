using System;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Configuration
{
    /// <summary>
    /// Quality preset levels for Android mobile scalability.
    /// </summary>
    public enum MobileQualityPreset
    {
        Low = 0,     // Battery saver / Low-tier devices (30 FPS)
        Medium = 1,  // Balanced / Mid-tier devices (60 FPS)
        High = 2     // Fidelity / Flagship devices (60 FPS)
    }

    /// <summary>
    /// Configuration asset defining Android graphics quality presets and runtime application logic.
    /// </summary>
    [CreateAssetMenu(fileName = "GraphicsConfiguration", menuName = "IndianBikeLife/Configuration/Graphics Configuration")]
    public class GraphicsConfiguration : ScriptableObject
    {
        [Header("Default Mobile Preset")]
        [SerializeField] private MobileQualityPreset _defaultPreset = MobileQualityPreset.Medium;

        [Header("Low Preset (Low-End / Battery Saver)")]
        [SerializeField] private int _lowTargetFps = 30;
        [SerializeField] private float _lowRenderScale = 0.8f;
        [SerializeField] private float _lowShadowDistance = 25f;

        [Header("Medium Preset (Mid-Tier Devices)")]
        [SerializeField] private int _medTargetFps = 60;
        [SerializeField] private float _medRenderScale = 1.0f;
        [SerializeField] private float _medShadowDistance = 50f;

        [Header("High Preset (High-End Devices)")]
        [SerializeField] private int _highTargetFps = 60;
        [SerializeField] private float _highRenderScale = 1.0f;
        [SerializeField] private float _highShadowDistance = 80f;

        public MobileQualityPreset DefaultPreset => _defaultPreset;

        /// <summary>
        /// Applies the chosen mobile quality preset to Unity's QualitySettings and target framerate.
        /// </summary>
        public void ApplyPreset(MobileQualityPreset preset)
        {
            int qualityIndex = (int)preset;
            if (qualityIndex < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(qualityIndex, true);
            }

            int targetFps = preset == MobileQualityPreset.Low ? _lowTargetFps :
                            preset == MobileQualityPreset.Medium ? _medTargetFps : _highTargetFps;

            QualitySettings.vSyncCount = 0; // Essential for Android
            Application.targetFrameRate = targetFps;
            QualitySettings.shadowDistance = preset == MobileQualityPreset.Low ? _lowShadowDistance :
                                             preset == MobileQualityPreset.Medium ? _medShadowDistance : _highShadowDistance;

            GameLogger.I("GraphicsConfig", $"Applied quality preset: {preset} (Target FPS: {targetFps})");
        }

        public float GetRenderScaleForPreset(MobileQualityPreset preset)
        {
            switch (preset)
            {
                case MobileQualityPreset.Low: return _lowRenderScale;
                case MobileQualityPreset.Medium: return _medRenderScale;
                case MobileQualityPreset.High: return _highRenderScale;
                default: return 1.0f;
            }
        }

        public float GetShadowDistanceForPreset(MobileQualityPreset preset)
        {
            switch (preset)
            {
                case MobileQualityPreset.Low: return _lowShadowDistance;
                case MobileQualityPreset.Medium: return _medShadowDistance;
                case MobileQualityPreset.High: return _highShadowDistance;
                default: return 50f;
            }
        }

        public int GetTargetFpsForPreset(MobileQualityPreset preset)
        {
            switch (preset)
            {
                case MobileQualityPreset.Low: return _lowTargetFps;
                case MobileQualityPreset.Medium: return _medTargetFps;
                case MobileQualityPreset.High: return _highTargetFps;
                default: return 60;
            }
        }
    }
}

