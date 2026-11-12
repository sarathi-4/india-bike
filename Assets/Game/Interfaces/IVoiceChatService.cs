using System;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for voice chat communication (Vivox-compatible architecture).
    /// </summary>
    public interface IVoiceChatService : IGameService
    {
        bool IsConnected { get; }
        bool IsMicMuted { get; }
        bool IsPushToTalkActive { get; }
        float OutputVolume { get; set; }

        bool HasMicrophonePermission { get; }
        void RequestMicrophonePermission(Action<bool> onResult);

        bool JoinChannel(string channelId);
        void LeaveChannel();
        void SetMicMute(bool muted);
        void SetPushToTalk(bool active);

        bool IsPlayerSpeaking(string playerId);

        event Action<string, bool> OnPlayerSpeakingStateChanged; // playerId, isSpeaking
        event Action<bool> OnVoiceChannelStateChanged;          // isJoined
    }
}
