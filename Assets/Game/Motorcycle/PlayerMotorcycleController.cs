using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Master physics and simulation controller for the player's Indian motorcycle.
    /// Delivers realistic acceleration, 5-speed transmission, dynamic cornering lean,
    /// wheel spinning, Nitro boost, headlight toggling, and Indian dual-tone horn.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotorcycleController : MonoBehaviour
    {
        [Header("Engine & Speed Parameters")]
        [SerializeField] private float _topSpeedKmh = 130f;
        [SerializeField] private float _nitroTopSpeedKmh = 165f;
        [SerializeField] private float _accelerationRate = 18f;
        [SerializeField] private float _brakePower = 28f;
        [SerializeField] private float _reverseSpeedKmh = 15f;
        [SerializeField] private float _dragFactor = 0.985f;

        [Header("Handling & Cornering Dynamics")]
        [SerializeField] private float _steerSensitivity = 45f;
        [SerializeField] private float _maxLeanAngle = 32f;
        [SerializeField] private float _leanSmoothing = 8f;
        [SerializeField] private float _gravity = 25f;

        [Header("Nitro Boost System")]
        [SerializeField] private float _maxNitro = 100f;
        [SerializeField] private float _nitroDrainRate = 25f;
        [SerializeField] private float _nitroRechargeRate = 12f;

        [Header("Visual Transform Anchors")]
        [SerializeField] private Transform _visualChassis;
        [SerializeField] private Transform _frontFork;
        [SerializeField] private Transform _frontWheel;
        [SerializeField] private Transform _rearWheel;
        [SerializeField] private Light _headlight;
        [SerializeField] private Transform _passengerSeatAnchor;

        // Runtime Physics State
        private CharacterController _characterController;
        private float _currentSpeedKmh;
        private float _currentLeanAngle;
        private float _currentSteerAngle;
        private float _currentNitro;
        private bool _isNitroActive;
        private float _verticalVelocity;
        private int _currentGear = 1;
        private float _currentRpm = 1000f;
        private float _wheelRadius = 0.35f;

        // Input Overrides (Mobile / Keyboard)
        private float _externalThrottle;
        private float _externalSteer;
        private float _externalBrake;
        private bool _externalNitro;
        private bool _externalHorn;

        // Services
        private IInputService _inputService;

        // Public Telemetry Properties
        public float CurrentSpeedKmh => _currentSpeedKmh;
        public float CurrentRpm => _currentRpm;
        public int CurrentGear => _currentGear;
        public float CurrentNitroPercent => _currentNitro / _maxNitro;
        public bool IsNitroActive => _isNitroActive;
        public float CurrentLeanAngle => _currentLeanAngle;
        public Transform PassengerSeatAnchor => _passengerSeatAnchor;

        public event Action OnHornHonked;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _characterController.stepOffset = 0.35f;
            _characterController.slopeLimit = 45f;
            _currentNitro = _maxNitro;
        }

        private void Start()
        {
            ServiceLocator.TryGet(out _inputService);
        }

        private void Update()
        {
            ReadInputs(out float throttle, out float steer, out float brake, out bool nitro, out bool horn, out bool lights);

            UpdateNitro(nitro);
            UpdateEnginePhysics(throttle, brake);
            UpdateSteeringAndLean(steer);
            UpdateMotion();
            UpdateVisuals();
            UpdateGearsAndRpm();

            // Auto-decay external mobile inputs so buttons don't lock or spin infinitely
            _externalThrottle = 0f;
            _externalSteer = Mathf.MoveTowards(_externalSteer, 0f, Time.deltaTime * 8f);
            _externalBrake = 0f;

            // R key respawns motorcycle back upright on road
            if (Input.GetKeyDown(KeyCode.R) || transform.position.y < -5f)
            {
                RespawnOnRoad();
            }

            if (horn)
            {
                OnHornHonked?.Invoke();
            }

            if (lights && _headlight != null)
            {
                _headlight.enabled = !_headlight.enabled;
            }
        }

        private void ReadInputs(out float throttle, out float steer, out float brake, out bool nitro, out bool horn, out bool lights)
        {
            throttle = 0f;
            steer = 0f;
            brake = 0f;
            nitro = false;
            horn = false;
            lights = false;

            // 1. Read from Input Service contract if available
            if (_inputService != null)
            {
                var state = _inputService.CurrentInputState;
                throttle = state.throttleInput;
                steer = state.steerInput;
                brake = state.brakeInput;
                nitro = state.nitro;
                horn = state.horn;
                lights = state.headlights;
            }

            // 2. Direct fallback to keyboard / desktop axes
            float vert = Input.GetAxisRaw("Vertical");
            float horiz = Input.GetAxisRaw("Horizontal");

            if (vert > 0.05f) throttle = Mathf.Max(throttle, vert);
            else if (vert < -0.05f) brake = Mathf.Max(brake, -vert);

            if (Mathf.Abs(horiz) > 0.05f) steer = horiz;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) nitro = true;
            if (Input.GetKey(KeyCode.Space)) brake = 1f;
            if (Input.GetKeyDown(KeyCode.H)) horn = true;
            if (Input.GetKeyDown(KeyCode.L)) lights = true;

            // 3. UI Mobile button overrides
            if (_externalThrottle > 0f) throttle = Mathf.Max(throttle, _externalThrottle);
            if (Mathf.Abs(_externalSteer) > 0f) steer = _externalSteer;
            if (_externalBrake > 0f) brake = Mathf.Max(brake, _externalBrake);
            if (_externalNitro) nitro = true;
            if (_externalHorn) { horn = true; _externalHorn = false; }
        }

        private void UpdateNitro(bool nitroRequested)
        {
            if (nitroRequested && _currentNitro > 5f && _currentSpeedKmh > 10f)
            {
                _isNitroActive = true;
                _currentNitro = Mathf.Max(0f, _currentNitro - _nitroDrainRate * Time.deltaTime);
            }
            else
            {
                _isNitroActive = false;
                _currentNitro = Mathf.Min(_maxNitro, _currentNitro + _nitroRechargeRate * Time.deltaTime);
            }
        }

        private void UpdateEnginePhysics(float throttle, float brake)
        {
            float maxSpeed = _isNitroActive ? _nitroTopSpeedKmh : _topSpeedKmh;

            if (throttle > 0.05f)
            {
                float accelFactor = _isNitroActive ? 1.6f : 1.0f;
                // Higher acceleration at lower speeds
                float speedRatio = Mathf.Clamp01(_currentSpeedKmh / maxSpeed);
                float torqueCurve = Mathf.Lerp(1.2f, 0.45f, speedRatio);

                _currentSpeedKmh += throttle * _accelerationRate * accelFactor * torqueCurve * Time.deltaTime * 3.6f;
                if (_currentSpeedKmh > maxSpeed)
                {
                    _currentSpeedKmh = Mathf.MoveTowards(_currentSpeedKmh, maxSpeed, 10f * Time.deltaTime);
                }
            }
            else if (brake > 0.05f)
            {
                if (_currentSpeedKmh > 0.5f)
                {
                    _currentSpeedKmh = Mathf.Max(0f, _currentSpeedKmh - brake * _brakePower * Time.deltaTime * 3.6f);
                }
                else
                {
                    // Reversing
                    _currentSpeedKmh = Mathf.MoveTowards(_currentSpeedKmh, -_reverseSpeedKmh, _accelerationRate * 0.4f * Time.deltaTime * 3.6f);
                }
            }
            else
            {
                // Coasting Drag
                _currentSpeedKmh *= Mathf.Pow(_dragFactor, Time.deltaTime * 60f);
                if (Mathf.Abs(_currentSpeedKmh) < 0.2f) _currentSpeedKmh = 0f;
            }
        }

        private void UpdateSteeringAndLean(float steerInput)
        {
            // Speed-adaptive steering sensitivity: high speed requires smooth gentle turns
            float speedAbs = Mathf.Abs(_currentSpeedKmh);
            float steerFactor = Mathf.Lerp(1.0f, 0.35f, Mathf.Clamp01(speedAbs / 90f));
            float targetSteerAngle = steerInput * _steerSensitivity * steerFactor;

            _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, targetSteerAngle, Time.deltaTime * 14f);

            // Turn motorcycle yaw smoothly only when moving with realistic turning radius
            if (speedAbs > 0.4f)
            {
                float turnDir = Mathf.Sign(_currentSpeedKmh);
                float speedWeight = Mathf.Clamp(speedAbs / 45f, 0.25f, 1.1f);
                float maxTurnPerSec = 75f; // Clamped maximum turn rate prevents wild 360 spinning
                float rawTurn = _currentSteerAngle * turnDir * speedWeight * Time.deltaTime * 28f;
                float clampedTurn = Mathf.Clamp(rawTurn, -maxTurnPerSec * Time.deltaTime, maxTurnPerSec * Time.deltaTime);
                transform.Rotate(0f, clampedTurn, 0f);
            }

            // Calculate dynamic motorcycle cornering lean
            float targetLean = -steerInput * _maxLeanAngle * Mathf.Clamp01(speedAbs / 35f);
            _currentLeanAngle = Mathf.Lerp(_currentLeanAngle, targetLean, Time.deltaTime * _leanSmoothing);
        }

        public void RespawnOnRoad()
        {
            _currentSpeedKmh = 0f;
            _verticalVelocity = 0f;
            _currentSteerAngle = 0f;
            _currentLeanAngle = 0f;
            _externalSteer = 0f;
            _externalThrottle = 0f;
            _externalBrake = 0f;

            // Re-align upright facing along forward road direction
            Vector3 currentEuler = transform.eulerAngles;
            transform.eulerAngles = new Vector3(0f, currentEuler.y, 0f);

            // Snap onto road surface
            Vector3 p = transform.position;
            transform.position = new Vector3(Mathf.Clamp(p.x, -5f, 5f), 0.65f, p.z);
        }

        private void UpdateMotion()
        {
            // Forward movement vector
            Vector3 forwardMove = transform.forward * (_currentSpeedKmh / 3.6f);

            // Gravity & Ground adherence
            if (_characterController.isGrounded)
            {
                _verticalVelocity = -2f; // Slight downward push to keep snapped to road
            }
            else
            {
                _verticalVelocity -= _gravity * Time.deltaTime;
            }

            Vector3 totalMove = forwardMove + Vector3.up * _verticalVelocity;
            _characterController.Move(totalMove * Time.deltaTime);
        }

        private void UpdateVisuals()
        {
            // 1. Roll the motorcycle chassis visually into corners
            if (_visualChassis != null)
            {
                _visualChassis.localRotation = Quaternion.Euler(0f, 0f, _currentLeanAngle);
            }

            // 2. Steer front fork
            if (_frontFork != null)
            {
                _frontFork.localRotation = Quaternion.Euler(0f, _currentSteerAngle, 0f);
            }

            // 3. Spin wheels based on actual distance traveled
            float distanceTraveled = (_currentSpeedKmh / 3.6f) * Time.deltaTime;
            float rotationDegrees = (distanceTraveled / (2f * Mathf.PI * _wheelRadius)) * 360f;

            if (_frontWheel != null)
            {
                _frontWheel.Rotate(rotationDegrees, 0f, 0f, Space.Self);
            }
            if (_rearWheel != null)
            {
                _rearWheel.Rotate(rotationDegrees, 0f, 0f, Space.Self);
            }
        }

        private void UpdateGearsAndRpm()
        {
            float speed = Mathf.Max(0f, _currentSpeedKmh);

            if (speed < 20f) _currentGear = 1;
            else if (speed < 45f) _currentGear = 2;
            else if (speed < 75f) _currentGear = 3;
            else if (speed < 105f) _currentGear = 4;
            else _currentGear = 5;

            float[] gearMinSpeeds = { 0f, 0f, 20f, 45f, 75f, 105f };
            float[] gearMaxSpeeds = { 0f, 25f, 50f, 80f, 110f, 170f };

            float gearSpan = gearMaxSpeeds[_currentGear] - gearMinSpeeds[_currentGear];
            float gearFraction = Mathf.Clamp01((speed - gearMinSpeeds[_currentGear]) / gearSpan);

            _currentRpm = Mathf.Lerp(1200f, 6200f, gearFraction);
            if (_isNitroActive) _currentRpm = Mathf.Min(6800f, _currentRpm + 400f);
        }

        // Public methods for Mobile UI buttons
        public void SetThrottleInput(float val) => _externalThrottle = val;
        public void SetSteerInput(float val) => _externalSteer = val;
        public void SetBrakeInput(float val) => _externalBrake = val;
        public void SetNitroInput(bool val) => _externalNitro = val;
        public void TriggerHorn() => _externalHorn = true;

        private string _currentBikeModelName = "Royal Enfield Classic 350";
        public string CurrentBikeModelName => _currentBikeModelName;

        public void ApplyBikeTuning(string modelName, float topSpeed, float acceleration, float brakePower)
        {
            _currentBikeModelName = modelName;
            _topSpeedKmh = topSpeed;
            _accelerationRate = acceleration;
            _brakePower = brakePower;
        }

        public void BindVisualAnchors(Transform chassis, Transform fork, Transform fWheel, Transform rWheel, Light headlight, Transform seat)
        {
            _visualChassis = chassis;
            _frontFork = fork;
            _frontWheel = fWheel;
            _rearWheel = rWheel;
            _headlight = headlight;
            _passengerSeatAnchor = seat;
        }
    }
}

