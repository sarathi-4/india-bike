using IndianBikeLife.Communication;
using IndianBikeLife.Core.Events.Core;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct ChatMessageEvent : IGameEvent
    {
        public ChatMessage Message { get; }
        public ChatMessageEvent(ChatMessage msg) => Message = msg;
    }

    public readonly struct QuickChatEvent : IGameEvent
    {
        public QuickChatMessage Message { get; }
        public QuickChatEvent(QuickChatMessage msg) => Message = msg;
    }

    public readonly struct PlayerMutedEvent : IGameEvent
    {
        public string PlayerId { get; }
        public bool IsMuted { get; }
        public PlayerMutedEvent(string pid, bool muted)
        {
            PlayerId = pid;
            IsMuted = muted;
        }
    }

    public readonly struct PlayerReportedEvent : IGameEvent
    {
        public string ReportedPlayerId { get; }
        public string Reason { get; }
        public PlayerReportedEvent(string pid, string reason)
        {
            ReportedPlayerId = pid;
            Reason = reason;
        }
    }
}
