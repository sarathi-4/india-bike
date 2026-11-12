using System;
using IndianBikeLife.Core.Interfaces;

namespace IndianBikeLife.Communication
{
    [Serializable]
    public class ChatMessage
    {
        public string messageId;
        public string senderId;
        public string senderName;
        public ChatChannel channel;
        public string messageText;
        public string timestampUtc;

        public ChatMessage() { }

        public ChatMessage(string id, string sid, string sname, ChatChannel chan, string text)
        {
            messageId = id;
            senderId = sid;
            senderName = sname;
            channel = chan;
            messageText = text;
            timestampUtc = DateTime.UtcNow.ToString("o");
        }
    }

    [Serializable]
    public class QuickChatMessage
    {
        public int phraseIndex;
        public string senderId;
        public string senderName;
        public string phraseText;
        public string timestampUtc;

        public QuickChatMessage() { }

        public QuickChatMessage(int idx, string sid, string sname, string text)
        {
            phraseIndex = idx;
            senderId = sid;
            senderName = sname;
            phraseText = text;
            timestampUtc = DateTime.UtcNow.ToString("o");
        }
    }
}
