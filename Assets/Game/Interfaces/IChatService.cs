using System;
using System.Collections.Generic;
using IndianBikeLife.Communication;

namespace IndianBikeLife.Core.Interfaces
{
    public enum ChatChannel
    {
        Room,
        Club,
        DirectMessage
    }

    /// <summary>
    /// Contract for text chat, quick chat phrases, and text moderation.
    /// </summary>
    public interface IChatService : IGameService
    {
        void SendRoomMessage(string messageText);
        void SendQuickChat(int quickChatPhraseIndex);
        void SendDirectMessage(string targetPlayerId, string messageText);

        bool MutePlayer(string playerId);
        bool UnmutePlayer(string playerId);
        bool IsPlayerMuted(string playerId);
        void ReportPlayer(string playerId, string reason, string details);

        IReadOnlyList<ChatMessage> GetMessageHistory(ChatChannel channel);

        event Action<ChatMessage> OnMessageReceived;
        event Action<QuickChatMessage> OnQuickChatReceived;
    }
}

