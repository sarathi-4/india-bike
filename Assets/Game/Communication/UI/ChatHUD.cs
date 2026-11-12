using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.Communication.UI
{
    /// <summary>
    /// In-game chat display and Quick Chat phrase trigger interface.
    /// </summary>
    public class ChatHUD : MonoBehaviour
    {
        private IChatService _chatService;
        private bool _showQuickChatGrid = false;
        private bool _showHUD = true;

        private void Start()
        {
            ServiceLocator.TryGet(out _chatService);
        }

        public void SetVisibility(bool visible)
        {
            _showHUD = visible;
        }

        private void OnGUI()
        {
            if (!_showHUD || _chatService == null) return;
            if (IndianBikeLife.Core.UI.MasterGameFlowUI.Instance != null && IndianBikeLife.Core.UI.MasterGameFlowUI.Instance.IsMenuOpen) return;

            int w = Screen.width;
            int h = Screen.height;

            // Bottom-Left Quick Chat Floating Pill
            float pillW = 160f;
            float pillH = 32f;
            if (GUI.Button(new Rect(15, h - pillH - 15, pillW, pillH), "💬 Quick Chat"))
            {
                _showQuickChatGrid = !_showQuickChatGrid;
            }

            if (_showQuickChatGrid)
            {
                float gridW = Mathf.Min(340f, w * 0.4f);
                float gridH = 220f;
                Rect gridRect = new Rect(15, h - pillH - gridH - 25, gridW, gridH);
                GUI.Box(gridRect, "<b>Indian Quick Chat Phrases</b>");

                GUILayout.BeginArea(new Rect(gridRect.x + 8, gridRect.y + 22, gridRect.width - 16, gridRect.height - 28));

                for (int i = 0; i < QuickChatController.PhraseCount; i++)
                {
                    if (GUILayout.Button(QuickChatController.GetPhrase(i)))
                    {
                        _chatService.SendQuickChat(i);
                        _showQuickChatGrid = false;
                    }
                }

                GUILayout.EndArea();
            }

            // Message History Card (top left under main panel)
            var messages = _chatService.GetMessageHistory(ChatChannel.Room);
            if (messages.Count > 0)
            {
                float logW = Mathf.Min(320f, w * 0.35f);
                float logH = Mathf.Min(110f, messages.Count * 22f + 30f);
                Rect logRect = new Rect(15, 340, logW, logH);
                GUI.Box(logRect, "Room Chat");
                GUILayout.BeginArea(new Rect(logRect.x + 6, logRect.y + 20, logRect.width - 12, logRect.height - 24));

                int startIdx = Mathf.Max(0, messages.Count - 4);
                for (int i = startIdx; i < messages.Count; i++)
                {
                    var m = messages[i];
                    GUILayout.Label($"<b>{m.senderName}:</b> {m.messageText}");
                }

                GUILayout.EndArea();
            }
        }
    }
}

