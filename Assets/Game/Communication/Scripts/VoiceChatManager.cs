using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Communication
{
    /// <summary>
    /// Voice chat management service implementing IVoiceChatService and IGameService.
    /// Provides Vivox-compatible channel abstractions, push-to-talk, speaking indicators,
    /// and enforces Android RECORD_AUDIO runtime permission checks.
    /// </summary>
    public class VoiceChatManager : MonoBehaviour, IVoiceChatService
    {
        private const string Tag = "VoiceChatManager";

        private readonly Dictionary<string, bool> _speakingStates = new Dictionary<string, bool>();

        private string _activeChannelId;
        private bool _isConnected;
        private bool _isMicMuted = true; // Muted by default on mobile for user privacy
        private bool _isPushToTalkActive;
        private float _outputVolume = 1.0f;
        private bool _hasMicrophonePermission;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public bool IsConnected => _isConnected;
        public bool IsMicMuted => _isMicMuted;
        public bool IsPushToTalkActive => _isPushToTalkActive;
        public float OutputVolume
        {
            get => _outputVolume;
            set => _outputVolume = Mathf.Clamp01(value);
        }

        public bool HasMicrophonePermission => _hasMicrophonePermission;

        public event Action<string, bool> OnPlayerSpeakingStateChanged;
        public event Action<bool> OnVoiceChannelStateChanged;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            CheckPermission();

            ServiceLocator.Register<IVoiceChatService>(this);
            _isInitialized = true;
            GameLogger.I(Tag, "VoiceChatManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            LeaveChannel();
            ServiceLocator.Unregister<IVoiceChatService>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private void CheckPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _hasMicrophonePermission = UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone);
#else
            _hasMicrophonePermission = true;
#endif
        }

        public void RequestMicrophonePermission(Action<bool> onResult)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
            {
                _hasMicrophonePermission = true;
                onResult?.Invoke(true);
            }
            else
            {
                var callbacks = new UnityEngine.Android.PermissionCallbacks();
                callbacks.PermissionGranted += (perm) =>
                {
                    _hasMicrophonePermission = true;
                    GameLogger.I(Tag, "Microphone permission granted by user.");
                    onResult?.Invoke(true);
                };
                callbacks.PermissionDenied += (perm) =>
                {
                    _hasMicrophonePermission = false;
                    GameLogger.W(Tag, "Microphone permission denied by user.");
                    onResult?.Invoke(false);
                };
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone, callbacks);
            }
#else
            _hasMicrophonePermission = true;
            onResult?.Invoke(true);
#endif
        }

        public bool JoinChannel(string channelId)
        {
            if (string.IsNullOrEmpty(channelId)) return false;

            if (!_hasMicrophonePermission)
            {
                GameLogger.W(Tag, "Cannot join voice channel: RECORD_AUDIO permission not granted.");
                return false;
            }

            _activeChannelId = channelId;
            _isConnected = true;
            OnVoiceChannelStateChanged?.Invoke(true);

            GameLogger.I(Tag, $"Joined Vivox-compatible voice channel: {channelId}");
            return true;
        }

        public void LeaveChannel()
        {
            if (_isConnected)
            {
                _isConnected = false;
                _activeChannelId = null;
                _speakingStates.Clear();
                OnVoiceChannelStateChanged?.Invoke(false);
                GameLogger.I(Tag, "Left voice channel.");
            }
        }

        public void SetMicMute(bool muted)
        {
            _isMicMuted = muted;
            GameLogger.I(Tag, $"Microphone {(muted ? "muted" : "unmuted")}.");
        }

        public void SetPushToTalk(bool active)
        {
            _isPushToTalkActive = active;
        }

        public bool IsPlayerSpeaking(string playerId)
        {
            return _speakingStates.TryGetValue(playerId, out bool spk) && spk;
        }

        public void SetPlayerSpeakingState(string playerId, bool isSpeaking)
        {
            _speakingStates[playerId] = isSpeaking;
            OnPlayerSpeakingStateChanged?.Invoke(playerId, isSpeaking);
        }
    }
}
