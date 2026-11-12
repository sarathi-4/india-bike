using System;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    /// <summary>
    /// Handles mission timing: countdown for timed missions or stopwatch for time trials.
    /// Emits timeout signals when time limit is exceeded.
    /// </summary>
    public class MissionTimer
    {
        private float _timeLimitSeconds;
        private float _elapsedSeconds;
        private bool _isRunning;
        private bool _isCountdown;

        public event Action OnTimerExpired;
        public event Action<float> OnTimerTick;

        public float ElapsedSeconds => _elapsedSeconds;
        public float RemainingSeconds => _isCountdown ? Mathf.Max(0f, _timeLimitSeconds - _elapsedSeconds) : 0f;
        public bool IsRunning => _isRunning;
        public bool IsCountdown => _isCountdown;

        public void Start(float timeLimit = 0f)
        {
            _timeLimitSeconds = Mathf.Max(0f, timeLimit);
            _isCountdown = _timeLimitSeconds > 0f;
            _elapsedSeconds = 0f;
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Pause()
        {
            _isRunning = false;
        }

        public void Resume()
        {
            _isRunning = true;
        }

        public void Reset()
        {
            _elapsedSeconds = 0f;
            _isRunning = false;
        }

        public void Update(float deltaTime)
        {
            if (!_isRunning) return;

            _elapsedSeconds += deltaTime;
            OnTimerTick?.Invoke(_isCountdown ? RemainingSeconds : _elapsedSeconds);

            if (_isCountdown && _elapsedSeconds >= _timeLimitSeconds)
            {
                _isRunning = false;
                OnTimerExpired?.Invoke();
            }
        }
    }
}
