using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Input;

namespace IndianBikeLife.Core.Events.GameEvents
{
    /// <summary>
    /// Dispatched when the active input control scheme switches (e.g., touch to gamepad).
    /// </summary>
    public readonly struct InputDeviceChangedEvent : IGameEvent
    {
        public InputDeviceType PreviousDevice { get; }
        public InputDeviceType NewDevice { get; }

        public InputDeviceChangedEvent(InputDeviceType previousDevice, InputDeviceType newDevice)
        {
            PreviousDevice = previousDevice;
            NewDevice = newDevice;
        }
    }
}
