namespace IndianBikeLife.Core.Input
{
    /// <summary>
    /// Supported input device hardware types for Indian Bike Life.
    /// </summary>
    public enum InputDeviceType
    {
        TouchScreen = 0,
        Keyboard = 1,
        Gamepad = 2,
        TiltSensor = 3
    }

    /// <summary>
    /// Discrete binary motorcycle control actions.
    /// </summary>
    public enum InputActionType
    {
        Nitro = 0,
        Horn = 1,
        Headlights = 2,
        Handbrake = 3,
        Pause = 4,
        ChangeCamera = 5,
        LookBack = 6
    }
}

