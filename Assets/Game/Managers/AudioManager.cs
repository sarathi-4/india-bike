using System;
using System.Collections;
using System.Collections.Generic;
using IndianBikeLife.Core.Audio;
using IndianBikeLife.Core.Configuration;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Managers
{
    /// <summary>
    /// Core audio management foundation implementing IAudioService and ISaveable.
    /// Supports music crossfades, spatial SFX pooling, motorcycle engine hooks, and persistent channel mixing.
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioService, ISaveable
    {
        private const string Tag = "AudioManager";

        [SerializeField] private AudioConfiguration _audioConfig;

        private AudioSource _musicSourceA;
        private AudioSource _musicSourceB;
        private AudioSource _vehicleSource;
        private AudioSource _ambientSource;
        private AudioSourcePool _sfxPool;

        private readonly Dictionary<AudioChannel, float> _channelVolumes = new Dictionary<AudioChannel, float>();
        private readonly Dictionary<AudioChannel, bool> _channelMutes = new Dictionary<AudioChannel, bool>();

        private bool _activeMusicSourceIsA = true;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "AudioManager";

        public void Initialize()
        {
            if (_isInitialized) return;

            // Initialize default channel volumes
            foreach (AudioChannel channel in Enum.GetValues(typeof(AudioChannel)))
            {
                float defaultVol = _audioConfig != null ? _audioConfig.GetDefaultVolumeForChannel(channel) : 0.8f;
                _channelVolumes[channel] = defaultVol;
                _channelMutes[channel] = false;
            }

            // Create dedicated music sources for crossfading
            _musicSourceA = CreateAudioSource("MusicSource_A", loop: true);
            _musicSourceB = CreateAudioSource("MusicSource_B", loop: true);
            _vehicleSource = CreateAudioSource("VehicleSource", loop: true);
            _ambientSource = CreateAudioSource("AmbientSource", loop: true);

            // Create SFX pool
            GameObject poolParent = new GameObject("SFX_Pool");
            poolParent.transform.SetParent(transform);
            int initialPool = _audioConfig != null ? _audioConfig.InitialSfxPoolSize : 16;
            int maxPool = _audioConfig != null ? _audioConfig.MaxSfxPoolSize : 32;
            _sfxPool = new AudioSourcePool(poolParent.transform, initialPool, maxPool);

            GameLogger.I(Tag, "AudioManager initialized with channels and source pools.");
            _isInitialized = true;
        }

        public void Shutdown()
        {
            StopAllCoroutines();
            if (_sfxPool != null) _sfxPool.StopAll();
            if (_musicSourceA != null) _musicSourceA.Stop();
            if (_musicSourceB != null) _musicSourceB.Stop();
            if (_vehicleSource != null) _vehicleSource.Stop();
            if (_ambientSource != null) _ambientSource.Stop();
            _isInitialized = false;
        }

        private AudioSource CreateAudioSource(string sourceName, bool loop = false)
        {
            GameObject go = new GameObject(sourceName);
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            return source;
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1.0f, bool loop = true)
        {
            if (clip == null) return;

            AudioSource incoming = _activeMusicSourceIsA ? _musicSourceB : _musicSourceA;
            AudioSource outgoing = _activeMusicSourceIsA ? _musicSourceA : _musicSourceB;
            _activeMusicSourceIsA = !_activeMusicSourceIsA;

            incoming.clip = clip;
            incoming.loop = loop;
            incoming.Play();

            StartCoroutine(CrossfadeRoutine(incoming, outgoing, fadeDuration));
        }

        public void StopMusic(float fadeDuration = 1.0f)
        {
            AudioSource current = _activeMusicSourceIsA ? _musicSourceA : _musicSourceB;
            if (current != null && current.isPlaying)
            {
                StartCoroutine(FadeOutRoutine(current, fadeDuration));
            }
        }

        private IEnumerator CrossfadeRoutine(AudioSource incoming, AudioSource outgoing, float duration)
        {
            float timer = 0f;
            float targetVolume = GetEffectiveVolume(AudioChannel.Music);

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(timer / duration);

                if (incoming != null) incoming.volume = Mathf.Lerp(0f, targetVolume, t);
                if (outgoing != null) outgoing.volume = Mathf.Lerp(targetVolume, 0f, t);

                yield return null;
            }

            if (incoming != null) incoming.volume = targetVolume;
            if (outgoing != null)
            {
                outgoing.volume = 0f;
                outgoing.Stop();
            }
        }

        private IEnumerator FadeOutRoutine(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
                yield return null;
            }

            source.volume = 0f;
            source.Stop();
        }

        public void PlaySFX(AudioClip clip, Vector3? position = null, float volumeScale = 1.0f, float pitch = 1.0f)
        {
            if (clip == null || _sfxPool == null) return;

            AudioSource source = _sfxPool.GetAvailableSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = GetEffectiveVolume(AudioChannel.SFX) * Mathf.Clamp01(volumeScale);
            source.pitch = pitch;

            if (position.HasValue)
            {
                source.transform.position = position.Value;
                source.spatialBlend = 1.0f; // 3D
                source.minDistance = 2f;
                source.maxDistance = 60f;
            }
            else
            {
                source.spatialBlend = 0.0f; // 2D
            }

            source.Play();
        }

        public void PlaySoundCue(SoundCue cue, Vector3? position = null)
        {
            if (cue == null) return;

            AudioClip clip = cue.GetRandomClip();
            if (clip == null) return;

            AudioSource source = _sfxPool.GetAvailableSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = GetEffectiveVolume(cue.Channel) * cue.GetRandomizedVolume();
            source.pitch = cue.GetRandomizedPitch();
            source.loop = cue.Loop;

            if (position.HasValue)
            {
                source.transform.position = position.Value;
                source.spatialBlend = cue.SpatialBlend;
                source.minDistance = cue.MinDistance;
                source.maxDistance = cue.MaxDistance;
            }
            else
            {
                source.spatialBlend = 0f;
            }

            source.Play();
        }

        public void SetChannelVolume(AudioChannel channel, float volume)
        {
            volume = Mathf.Clamp01(volume);
            _channelVolumes[channel] = volume;

            UpdateLiveSourceVolumes();
            EventBus.Publish(new AudioVolumeChangedEvent(channel, volume));
        }

        public float GetChannelVolume(AudioChannel channel)
        {
            return _channelVolumes.TryGetValue(channel, out float volume) ? volume : 0.8f;
        }

        public void SetChannelMute(AudioChannel channel, bool mute)
        {
            _channelMutes[channel] = mute;
            UpdateLiveSourceVolumes();
            EventBus.Publish(new AudioMuteToggledEvent(channel, mute));
        }

        public bool IsChannelMuted(AudioChannel channel)
        {
            return _channelMutes.TryGetValue(channel, out bool isMuted) && isMuted;
        }

        private float GetEffectiveVolume(AudioChannel channel)
        {
            if (IsChannelMuted(AudioChannel.Master) || IsChannelMuted(channel))
            {
                return 0f;
            }

            float master = GetChannelVolume(AudioChannel.Master);
            float channelVol = GetChannelVolume(channel);
            return master * channelVol;
        }

        private void UpdateLiveSourceVolumes()
        {
            float musicVol = GetEffectiveVolume(AudioChannel.Music);
            AudioSource activeMusic = _activeMusicSourceIsA ? _musicSourceA : _musicSourceB;
            if (activeMusic != null && activeMusic.isPlaying)
            {
                activeMusic.volume = musicVol;
            }

            if (_vehicleSource != null)
            {
                _vehicleSource.volume = GetEffectiveVolume(AudioChannel.Vehicle);
            }

            if (_ambientSource != null)
            {
                _ambientSource.volume = GetEffectiveVolume(AudioChannel.Ambient);
            }
        }

        #region ISaveable Implementation

        public void PopulateSaveData(SaveData data)
        {
            if (data?.settings == null) return;

            data.settings.masterVolume = GetChannelVolume(AudioChannel.Master);
            data.settings.musicVolume = GetChannelVolume(AudioChannel.Music);
            data.settings.sfxVolume = GetChannelVolume(AudioChannel.SFX);
            data.settings.vehicleVolume = GetChannelVolume(AudioChannel.Vehicle);
            data.settings.ambientVolume = GetChannelVolume(AudioChannel.Ambient);
            data.settings.uiVolume = GetChannelVolume(AudioChannel.UI);

            data.settings.masterMuted = IsChannelMuted(AudioChannel.Master);
            data.settings.musicMuted = IsChannelMuted(AudioChannel.Music);
            data.settings.sfxMuted = IsChannelMuted(AudioChannel.SFX);
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data?.settings == null) return;

            SetChannelVolume(AudioChannel.Master, data.settings.masterVolume);
            SetChannelVolume(AudioChannel.Music, data.settings.musicVolume);
            SetChannelVolume(AudioChannel.SFX, data.settings.sfxVolume);
            SetChannelVolume(AudioChannel.Vehicle, data.settings.vehicleVolume);
            SetChannelVolume(AudioChannel.Ambient, data.settings.ambientVolume);
            SetChannelVolume(AudioChannel.UI, data.settings.uiVolume);

            SetChannelMute(AudioChannel.Master, data.settings.masterMuted);
            SetChannelMute(AudioChannel.Music, data.settings.musicMuted);
            SetChannelMute(AudioChannel.SFX, data.settings.sfxMuted);
        }

        #endregion
    }
}

