using IndianBikeLife.Core.Input;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for reading decoupled user inputs across mobile touch, tilt, keyboard, and gamepads.
    /// </summary>
    public interface IInputService : IGameService
    {
        /// <summary>
        /// Gets the current frame's consolidated motorcycle input state.
        /// </summary>
        MotorcycleInputState CurrentInputState { get; }

        /// <summary>
        /// Gets the currently detected or active input device type.
        /// </summary>
        InputDeviceType ActiveDeviceType { get; }

        /// <summary>
        /// Injects steer input programmatically (used by mobile on-screen virtual controls).
        /// </summary>
        /// <param name="steer">Normalized steering value (-1.0 to 1.0).</param>
        void SetVirtualSteer(float steer);

        /// <summary>
        /// Injects throttle input programmatically (used by mobile acceleration pedals).
        /// </summary>
        /// <param name="throttle">Normalized throttle value (0.0 to 1.0).</param>
        void SetVirtualThrottle(float throttle);

        /// <summary>
        /// Injects brake input programmatically (used by mobile brake pedals).
        /// </summary>
        /// <param name="brake">Normalized brake value (0.0 to 1.0).</param>
        void SetVirtualBrake(float brake);

        /// <summary>
        /// Injects binary action state (nitro, horn, lights, handbrake).
        /// </summary>
        void SetVirtualAction(InputActionType action, bool isPressed);
    }
}
