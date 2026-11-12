using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Input;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Managers
{
    /// <summary>
    /// Core input manager implementing IInputService.
    /// Bridges mobile touch/virtual UI, keyboard fallback for editor development, and gamepad controls.
    /// </summary>
    public class InputManager : MonoBehaviour, IInputService
    {
        private const string Tag = "InputManager";

        private MotorcycleInputState _currentState;
        private InputDeviceType _activeDeviceType = InputDeviceType.TouchScreen;
        private bool _isInitialized;

        // Virtual input cache set by touch UI buttons
        private float _virtualSteer;
        private float _virtualThrottle;
        private float _virtualBrake;
        private bool _virtualNitro;
        private bool _virtualHorn;
        private bool _virtualHeadlights;
        private bool _virtualHandbrake;

        public bool IsInitialized => _isInitialized;
        public MotorcycleInputState CurrentInputState => _currentState;
        public InputDeviceType ActiveDeviceType => _activeDeviceType;

        public void Initialize()
        {
            if (_isInitialized) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            _activeDeviceType = InputDeviceType.TouchScreen;
#else
            _activeDeviceType = InputDeviceType.Keyboard;
#endif

            _currentState.Reset();
            GameLogger.I(Tag, $"InputManager initialized. Default device: {_activeDeviceType}");
            _isInitialized = true;
        }

        public void Shutdown()
        {
            _currentState.Reset();
            _isInitialized = false;
        }

        private void Update()
        {
            if (!_isInitialized) return;

            PollInputs();
        }

        private void PollInputs()
        {
            _currentState.Reset();

            // 1. Gather Desktop/Editor Keyboard Inputs
            float keyboardSteer = 0f;
            float keyboardThrottle = 0f;
            float keyboardBrake = 0f;
            bool keyboardNitro = false;
            bool keyboardHorn = false;
            bool keyboardLights = false;
            bool keyboardHandbrake = false;
            bool pausePressed = false;
            bool cameraPressed = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
                keyboardSteer -= 1.0f;
            if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
                keyboardSteer += 1.0f;

            if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
                keyboardThrottle = 1.0f;

            if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
                keyboardBrake = 1.0f;

            keyboardHandbrake = UnityEngine.Input.GetKey(KeyCode.Space);
            keyboardNitro = UnityEngine.Input.GetKey(KeyCode.LeftShift);
            keyboardHorn = UnityEngine.Input.GetKey(KeyCode.H);
            keyboardLights = UnityEngine.Input.GetKeyDown(KeyCode.L);
            cameraPressed = UnityEngine.Input.GetKeyDown(KeyCode.C);
            pausePressed = UnityEngine.Input.GetKeyDown(KeyCode.Escape);

            if (Mathf.Abs(keyboardSteer) > 0.01f || keyboardThrottle > 0.01f || keyboardBrake > 0.01f)
            {
                if (_activeDeviceType != InputDeviceType.Keyboard)
                {
                    SetDeviceType(InputDeviceType.Keyboard);
                }
            }
#endif

            // 2. Blend Virtual Mobile Inputs with Physical Inputs
            _currentState.steerInput = Mathf.Clamp(keyboardSteer + _virtualSteer, -1.0f, 1.0f);
            _currentState.throttleInput = Mathf.Clamp01(keyboardThrottle + _virtualThrottle);
            _currentState.brakeInput = Mathf.Clamp01(keyboardBrake + _virtualBrake);
            _currentState.handbrake = keyboardHandbrake || _virtualHandbrake;
            _currentState.nitro = keyboardNitro || _virtualNitro;
            _currentState.horn = keyboardHorn || _virtualHorn;
            _currentState.headlights = keyboardLights || _virtualHeadlights;
            _currentState.changeCameraPressed = cameraPressed;
            _currentState.pausePressed = pausePressed;

            // Reset edge triggers for headlights in virtual mode after one frame
            if (_virtualHeadlights) _virtualHeadlights = false;
        }

        public void SetVirtualSteer(float steer)
        {
            _virtualSteer = Mathf.Clamp(steer, -1.0f, 1.0f);
            if (_activeDeviceType != InputDeviceType.TouchScreen && Mathf.Abs(steer) > 0.01f)
            {
                SetDeviceType(InputDeviceType.TouchScreen);
            }
        }

        public void SetVirtualThrottle(float throttle)
        {
            _virtualThrottle = Mathf.Clamp01(throttle);
            if (_activeDeviceType != InputDeviceType.TouchScreen && throttle > 0.01f)
            {
                SetDeviceType(InputDeviceType.TouchScreen);
            }
        }

        public void SetVirtualBrake(float brake)
        {
            _virtualBrake = Mathf.Clamp01(brake);
            if (_activeDeviceType != InputDeviceType.TouchScreen && brake > 0.01f)
            {
                SetDeviceType(InputDeviceType.TouchScreen);
            }
        }

        public void SetVirtualAction(InputActionType action, bool isPressed)
        {
            switch (action)
            {
                case InputActionType.Nitro:
                    _virtualNitro = isPressed;
                    break;
                case InputActionType.Horn:
                    _virtualHorn = isPressed;
                    break;
                case InputActionType.Headlights:
                    _virtualHeadlights = isPressed;
                    break;
                case InputActionType.Handbrake:
                    _virtualHandbrake = isPressed;
                    break;
                case InputActionType.Pause:
                    _currentState.pausePressed = isPressed;
                    break;
                case InputActionType.ChangeCamera:
                    _currentState.changeCameraPressed = isPressed;
                    break;
            }
        }

        private void SetDeviceType(InputDeviceType newType)
        {
            if (_activeDeviceType == newType) return;
            InputDeviceType prev = _activeDeviceType;
            _activeDeviceType = newType;
            EventBus.Publish(new InputDeviceChangedEvent(prev, newType));
        }
    }
}

