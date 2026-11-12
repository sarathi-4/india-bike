using IndianBikeLife.Core.Audio;
using UnityEngine;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for decoupled audio management, supporting master, music, sfx, vehicle, and ambient channels.
    /// </summary>
    public interface IAudioService : IGameService
    {
        /// <summary>
        /// Plays a background music track with smooth crossfade.
        /// </summary>
        /// <param name="clip">Audio clip to play.</param>
        /// <param name="fadeDuration">Crossfade duration in seconds.</param>
        /// <param name="loop">Whether to loop the track.</param>
        void PlayMusic(AudioClip clip, float fadeDuration = 1.0f, bool loop = true);

        /// <summary>
        /// Stops the current background music with a fade out.
        /// </summary>
        /// <param name="fadeDuration">Fade out duration in seconds.</param>
        void StopMusic(float fadeDuration = 1.0f);

        /// <summary>
        /// Plays a sound effect at a designated 3D world position or as a 2D one-shot.
        /// </summary>
        /// <param name="clip">Audio clip to play.</param>
        /// <param name="position">World position for 3D spatialization. Null for 2D.</param>
        /// <param name="volumeScale">Relative volume scaling (0.0 to 1.0).</param>
        /// <param name="pitch">Pitch modulation factor.</param>
        void PlaySFX(AudioClip clip, Vector3? position = null, float volumeScale = 1.0f, float pitch = 1.0f);

        /// <summary>
        /// Sets the linear volume (0.0 to 1.0) for a specific audio channel.
        /// </summary>
        /// <param name="channel">Target audio channel.</param>
        /// <param name="volume">Normalized volume (0.0 to 1.0).</param>
        void SetChannelVolume(AudioChannel channel, float volume);

        /// <summary>
        /// Gets the current normalized volume for a channel.
        /// </summary>
        float GetChannelVolume(AudioChannel channel);

        /// <summary>
        /// Sets the mute status for a channel.
        /// </summary>
        void SetChannelMute(AudioChannel channel, bool mute);

        /// <summary>
        /// Gets the mute status for a channel.
        /// </summary>
        bool IsChannelMuted(AudioChannel channel);
    }
}
