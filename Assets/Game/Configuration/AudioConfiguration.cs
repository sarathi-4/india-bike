using System;
using IndianBikeLife.Core.Audio;
using UnityEngine;

namespace IndianBikeLife.Core.Configuration
{
    /// <summary>
    /// ScriptableObject managing audio bus defaults, decibel conversion curves, and source pooling.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioConfiguration", menuName = "IndianBikeLife/Configuration/Audio Configuration")]
    public class AudioConfiguration : ScriptableObject
    {
        [Header("Default Normalized Volumes (0.0 to 1.0)")]
        [Range(0f, 1f)] [SerializeField] private float _defaultMasterVolume = 0.85f;
        [Range(0f, 1f)] [SerializeField] private float _defaultMusicVolume = 0.75f;
        [Range(0f, 1f)] [SerializeField] private float _defaultSfxVolume = 0.85f;
        [Range(0f, 1f)] [SerializeField] private float _defaultVehicleVolume = 0.90f;
        [Range(0f, 1f)] [SerializeField] private float _defaultAmbientVolume = 0.70f;
        [Range(0f, 1f)] [SerializeField] private float _defaultUiVolume = 0.80f;

        [Header("Audio Pool Settings")]
        [Tooltip("Pre-allocated audio source instances in the SFX pool to prevent GC allocations on mobile.")]
        [SerializeField] private int _initialSfxPoolSize = 16;
        [SerializeField] private int _maxSfxPoolSize = 32;

        [Header("Decibel Attenuation")]
        [Tooltip("Minimum decibels corresponding to 0 volume slider.")]
        [SerializeField] private float _minDecibels = -80f;

        public float DefaultMasterVolume => _defaultMasterVolume;
        public float DefaultMusicVolume => _defaultMusicVolume;
        public float DefaultSfxVolume => _defaultSfxVolume;
        public float DefaultVehicleVolume => _defaultVehicleVolume;
        public float DefaultAmbientVolume => _defaultAmbientVolume;
        public float DefaultUiVolume => _defaultUiVolume;

        public int InitialSfxPoolSize => _initialSfxPoolSize;
        public int MaxSfxPoolSize => _maxSfxPoolSize;
        public float MinDecibels => _minDecibels;

        /// <summary>
        /// Converts a linear normalized volume (0.0 to 1.0) to logarithmic decibels (-80dB to 0dB).
        /// </summary>
        public float LinearToDecibels(float linearVolume)
        {
            linearVolume = Mathf.Clamp01(linearVolume);
            if (linearVolume <= 0.0001f) return _minDecibels;
            return Mathf.Log10(linearVolume) * 20f;
        }

        /// <summary>
        /// Gets the default volume for a designated channel.
        /// </summary>
        public float GetDefaultVolumeForChannel(AudioChannel channel)
        {
            switch (channel)
            {
                case AudioChannel.Master: return _defaultMasterVolume;
                case AudioChannel.Music: return _defaultMusicVolume;
                case AudioChannel.SFX: return _defaultSfxVolume;
                case AudioChannel.Vehicle: return _defaultVehicleVolume;
                case AudioChannel.Ambient: return _defaultAmbientVolume;
                case AudioChannel.UI: return _defaultUiVolume;
                default: return 0.8f;
            }
        }
    }
}
