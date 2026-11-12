using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Communication
{
    /// <summary>
    /// Master text and quick chat service implementing IChatService and IGameService.
    /// Provides client-side profanity filtering, spam rate-limiting, and mute suppression.
    /// </summary>
    public class ChatManager : MonoBehaviour, IChatService
    {
        private const string Tag = "ChatManager";
        private const float MinMessageInterval = 0.5f; // Max 2 messages/sec

        private readonly Dictionary<ChatChannel, List<ChatMessage>> _messageHistory 
            = new Dictionary<ChatChannel, List<ChatMessage>>();

        private PlayerMuteManager _muteManager;
        private PlayerReportManager _reportManager;

        private float _lastMessageSentTime;
        private string _localPlayerId = "local_rider";
        private string _localPlayerName = "Indian Rider";
        private bool _isInitialized;

        // Baseline regex for common abusive words / profanity
        private static readonly Regex _profanityFilter = new Regex(
            @"\b(badword|abuse|scam|cheat|hack|idiot|stupid)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public bool IsInitialized => _isInitialized;

        public event Action<ChatMessage> OnMessageReceived;
        public event Action<QuickChatMessage> OnQuickChatReceived;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _muteManager = new PlayerMuteManager();
            _reportManager = new PlayerReportManager();

            foreach (ChatChannel channel in Enum.GetValues(typeof(ChatChannel)))
            {
                _messageHistory[channel] = new List<ChatMessage>();
            }

            ServiceLocator.Register<IChatService>(this);
            _isInitialized = true;
            GameLogger.I(Tag, "ChatManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;
            ServiceLocator.Unregister<IChatService>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public void SetLocalIdentity(string pid, string name)
        {
            _localPlayerId = pid;
            _localPlayerName = name;
        }

        public void SendRoomMessage(string messageText)
        {
            SendMessageInternal(ChatChannel.Room, messageText);
        }

        public void SendDirectMessage(string targetPlayerId, string messageText)
        {
            SendMessageInternal(ChatChannel.DirectMessage, messageText);
        }

        public void SendQuickChat(int quickChatPhraseIndex)
        {
            string phrase = QuickChatController.GetPhrase(quickChatPhraseIndex);
            var qMsg = new QuickChatMessage(quickChatPhraseIndex, _localPlayerId, _localPlayerName, phrase);

            EventBus.Publish(new QuickChatEvent(qMsg));
            OnQuickChatReceived?.Invoke(qMsg);
            GameLogger.I(Tag, $"[QuickChat] {_localPlayerName}: {phrase}");
        }

        private void SendMessageInternal(ChatChannel channel, string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText)) return;

            // Spam rate-limiting
            if (Time.time - _lastMessageSentTime < MinMessageInterval)
            {
                GameLogger.W(Tag, "Message throttled (rate limit exceeded).");
                return;
            }
            _lastMessageSentTime = Time.time;

            // Moderation profanity sanitization
            string sanitized = SanitizeMessage(rawText);

            string msgId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var msg = new ChatMessage(msgId, _localPlayerId, _localPlayerName, channel, sanitized);

            ReceiveIncomingMessage(msg);
        }

        public void ReceiveIncomingMessage(ChatMessage msg)
        {
            if (msg == null) return;

            // Mute check: suppress messages from muted players
            if (_muteManager.IsMuted(msg.senderId))
            {
                GameLogger.I(Tag, $"Dropped message from muted sender {msg.senderId}");
                return;
            }

            if (!_messageHistory.ContainsKey(msg.channel))
            {
                _messageHistory[msg.channel] = new List<ChatMessage>();
            }

            var list = _messageHistory[msg.channel];
            list.Add(msg);
            if (list.Count > 100) list.RemoveAt(0);

            EventBus.Publish(new ChatMessageEvent(msg));
            OnMessageReceived?.Invoke(msg);
        }

        public bool MutePlayer(string playerId)
        {
            bool ok = _muteManager.Mute(playerId);
            if (ok) EventBus.Publish(new PlayerMutedEvent(playerId, true));
            return ok;
        }

        public bool UnmutePlayer(string playerId)
        {
            bool ok = _muteManager.Unmute(playerId);
            if (ok) EventBus.Publish(new PlayerMutedEvent(playerId, false));
            return ok;
        }

        public bool IsPlayerMuted(string playerId) => _muteManager.IsMuted(playerId);

        public void ReportPlayer(string playerId, string reason, string details)
        {
            _reportManager.SubmitReport(_localPlayerId, playerId, reason, details);
            EventBus.Publish(new PlayerReportedEvent(playerId, reason));
        }

        public IReadOnlyList<ChatMessage> GetMessageHistory(ChatChannel channel)
        {
            if (_messageHistory.TryGetValue(channel, out var list)) return list;
            return Array.Empty<ChatMessage>();
        }

        public static string SanitizeMessage(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return _profanityFilter.Replace(text, "***");
        }
    }
}
