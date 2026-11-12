using IndianBikeLife.Core.Audio;
using IndianBikeLife.Core.Events.Core;

namespace IndianBikeLife.Core.Events.GameEvents
{
    /// <summary>
    /// Dispatched when an audio channel volume changes.
    /// </summary>
    public readonly struct AudioVolumeChangedEvent : IGameEvent
    {
        public AudioChannel Channel { get; }
        public float Volume { get; }

        public AudioVolumeChangedEvent(AudioChannel channel, float volume)
        {
            Channel = channel;
            Volume = volume;
        }
    }

    /// <summary>
    /// Dispatched when an audio channel mute status is toggled.
    /// </summary>
    public readonly struct AudioMuteToggledEvent : IGameEvent
    {
        public AudioChannel Channel { get; }
        public bool IsMuted { get; }

        public AudioMuteToggledEvent(AudioChannel channel, bool isMuted)
        {
            Channel = channel;
            IsMuted = isMuted;
        }
    }
}

