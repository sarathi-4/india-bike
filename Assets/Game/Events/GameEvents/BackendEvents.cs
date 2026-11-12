using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Interfaces;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct AuthSuccessEvent : IGameEvent
    {
        public AuthState State { get; }
        public string UserId { get; }
        public AuthSuccessEvent(AuthState state, string uid)
        {
            State = state;
            UserId = uid;
        }
    }

    public readonly struct AuthFailedEvent : IGameEvent
    {
        public string ErrorMessage { get; }
        public AuthFailedEvent(string error) => ErrorMessage = error;
    }

    public readonly struct CloudSaveCompletedEvent : IGameEvent
    {
        public bool Success { get; }
        public string Message { get; }
        public CloudSaveCompletedEvent(bool ok, string msg)
        {
            Success = ok;
            Message = msg;
        }
    }
}

