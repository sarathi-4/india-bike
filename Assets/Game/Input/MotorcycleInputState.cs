using System;

namespace IndianBikeLife.Core.Input
{
    /// <summary>
    /// Decoupled data contract representing raw motorcycle control inputs for each frame.
    /// Physics simulation systems consume this state without knowing whether the source
    /// is an on-screen touch button, tilt accelerometer, or Bluetooth controller.
    /// </summary>
    [Serializable]
    public struct MotorcycleInputState
    {
        public float steerInput;      // -1.0 (Full Left) to 1.0 (Full Right)
        public float throttleInput;   // 0.0 (Idle) to 1.0 (Full Throttle)
        public float brakeInput;      // 0.0 (Released) to 1.0 (Full Brake)
        public bool handbrake;
        public bool nitro;
        public bool horn;
        public bool headlights;
        public bool lookBack;
        public bool pausePressed;
        public bool changeCameraPressed;

        public void Reset()
        {
            steerInput = 0f;
            throttleInput = 0f;
            brakeInput = 0f;
            handbrake = false;
            nitro = false;
            horn = false;
            headlights = false;
            lookBack = false;
            pausePressed = false;
            changeCameraPressed = false;
        }
    }
}

