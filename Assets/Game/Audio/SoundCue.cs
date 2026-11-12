using UnityEngine;

namespace IndianBikeLife.Core.Audio
{
    /// <summary>
    /// ScriptableObject data container defining audio properties for sound effects,
    /// motorcycle sounds, and UI interactions without hardcoding audio sources.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSoundCue", menuName = "IndianBikeLife/Audio/Sound Cue")]
    public class SoundCue : ScriptableObject
    {
        [Header("Audio Content")]
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private AudioChannel _channel = AudioChannel.SFX;

        [Header("Playback Dynamics")]
        [Range(0f, 1f)] [SerializeField] private float _volume = 1.0f;
        [Range(0.1f, 3f)] [SerializeField] private float _pitch = 1.0f;
        [Range(0f, 0.5f)] [SerializeField] private float _randomPitchVariation = 0.05f;
        [Range(0f, 0.3f)] [SerializeField] private float _randomVolumeVariation = 0.05f;

        [Header("Spatialization")]
        [Range(0f, 1f)] [SerializeField] private float _spatialBlend = 0f; // 0 = 2D, 1 = 3D
        [SerializeField] private float _minDistance = 1f;
        [SerializeField] private float _maxDistance = 50f;
        [SerializeField] private bool _loop = false;

        public AudioChannel Channel => _channel;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public float SpatialBlend => _spatialBlend;
        public float MinDistance => _minDistance;
        public float MaxDistance => _maxDistance;
        public bool Loop => _loop;

        /// <summary>
        /// Selects a clip from the configured clip array.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (_clips == null || _clips.Length == 0) return null;
            if (_clips.Length == 1) return _clips[0];
            return _clips[Random.Range(0, _clips.Length)];
        }

        public float GetRandomizedPitch()
        {
            if (_randomPitchVariation <= 0.001f) return _pitch;
            return _pitch + Random.Range(-_randomPitchVariation, _randomPitchVariation);
        }

        public float GetRandomizedVolume()
        {
            if (_randomVolumeVariation <= 0.001f) return _volume;
            return Mathf.Clamp01(_volume + Random.Range(-_randomVolumeVariation, _randomVolumeVariation));
        }
    }
}

