using System;
using IndianBikeLife.Core.Configuration;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Managers
{
    /// <summary>
    /// Master quality settings service managing Low, Medium, and High mobile presets.
    /// Dynamically applies shadow distance, render scale, LOD bias, and target framerate (30/60 FPS).
    /// </summary>
    public class QualitySettingsManager : MonoBehaviour, IQualityService
    {
        private const string Tag = "QualitySettingsManager";
        private const string PrefKey = "IBL_SavedQualityPreset";

        [SerializeField] private MobileQualityPreset _activePreset = MobileQualityPreset.Medium;

        private bool _isInitialized;
        private int _currentTargetFps = 60;
        private float _currentShadowDist = 50f;
        private float _currentRenderScale = 1.0f;
        private float _currentLodBias = 1.0f;

        public bool IsInitialized => _isInitialized;
        public MobileQualityPreset CurrentPreset => _activePreset;
        public int CurrentTargetFPS => _currentTargetFps;
        public float CurrentShadowDistance => _currentShadowDist;
        public float CurrentRenderScale => _currentRenderScale;
        public float CurrentLODBias => _currentLodBias;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            // Load saved user preference or default based on device RAM
            int defaultPreset = (int)MobileQualityPreset.Medium;
            if (SystemInfo.systemMemorySize > 0 && SystemInfo.systemMemorySize <= 3500)
            {
                // Low-tier device with <= 3.5 GB RAM defaults to Low
                defaultPreset = (int)MobileQualityPreset.Low;
            }
            else if (SystemInfo.systemMemorySize > 7000)
            {
                // Flagship device with > 7 GB RAM defaults to High
                defaultPreset = (int)MobileQualityPreset.High;
            }

            int saved = PlayerPrefs.GetInt(PrefKey, defaultPreset);
            _activePreset = (MobileQualityPreset)Mathf.Clamp(saved, 0, 2);

            ApplyQualityPresetInternal(_activePreset);

            ServiceLocator.Register<IQualityService>(this);
            _isInitialized = true;
            GameLogger.I(Tag, $"QualitySettingsManager initialized with preset: {_activePreset} (Target FPS: {_currentTargetFps}).");
        }

        public void Shutdown()
        {
            ServiceLocator.Unregister<IQualityService>();
            _isInitialized = false;
        }

        public void SetQualityPreset(MobileQualityPreset preset)
        {
            if (_activePreset == preset && _isInitialized) return;

            var prev = _activePreset;
            _activePreset = preset;
            PlayerPrefs.SetInt(PrefKey, (int)preset);
            PlayerPrefs.Save();

            ApplyQualityPresetInternal(preset);

            EventBus.Publish(new QualityPresetChangedEvent(prev, preset, _currentTargetFps));
            GameLogger.I(Tag, $"Switched quality preset from {prev} to {preset}.");
        }

        private void ApplyQualityPresetInternal(MobileQualityPreset preset)
        {
            // 1. Unity QualitySettings level
            int qualityLevel = (int)preset;
            if (qualityLevel < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(qualityLevel, applyExpensiveChanges: true);
            }

            // 2. Mobile Framerate & VSync
            QualitySettings.vSyncCount = 0; // Essential for Android battery & thermal management

            switch (preset)
            {
                case MobileQualityPreset.Low:
                    _currentTargetFps = 30; // 30 FPS battery saver for budget devices
                    _currentShadowDist = 25f;
                    _currentRenderScale = 0.75f;
                    _currentLodBias = 0.7f;
                    QualitySettings.shadowCascades = 1;
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    break;

                case MobileQualityPreset.Medium:
                    _currentTargetFps = 60; // 60 FPS balanced for mid-range
                    _currentShadowDist = 50f;
                    _currentRenderScale = 1.0f;
                    _currentLodBias = 1.0f;
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;

                case MobileQualityPreset.High:
                    _currentTargetFps = 60; // 60 FPS fidelity for flagship
                    _currentShadowDist = 80f;
                    _currentRenderScale = 1.0f;
                    _currentLodBias = 1.5f;
                    QualitySettings.shadowCascades = 4;
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;
            }

            Application.targetFrameRate = _currentTargetFps;
            QualitySettings.shadowDistance = _currentShadowDist;
            QualitySettings.lodBias = _currentLodBias;
        }
    }
}
