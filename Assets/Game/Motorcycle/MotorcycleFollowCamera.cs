using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Smooth third-person dynamic follow camera for the motorcycle simulator.
    /// Features speed-based dynamic FOV/distance pull, cornering rotation damping,
    /// rear-view mirror / look-back (C key), and multi-view switching (V key).
    /// </summary>
    public class MotorcycleFollowCamera : MonoBehaviour
    {
        public enum CameraViewMode
        {
            ChaseThirdPerson,
            CloseAction,
            FirstPersonHandlebar
        }

        [Header("Target & View Configuration")]
        [SerializeField] private Transform _target;
        [SerializeField] private CameraViewMode _viewMode = CameraViewMode.ChaseThirdPerson;

        [Header("Third-Person Follow Offsets")]
        [SerializeField] private float _followDistance = 4.8f;
        [SerializeField] private float _followHeight = 1.9f;
        [SerializeField] private float _positionSmoothing = 10f;
        [SerializeField] private float _rotationSmoothing = 8f;

        [Header("360 Degree Orbit Camera")]
        [SerializeField] private float _orbitYaw = 0f;
        [SerializeField] private float _orbitPitch = 15f;
        [SerializeField] private float _orbitSensitivity = 3.5f;
        [SerializeField] private bool _autoTurntableInGarage = true;
        private Vector3 _lastPointerPos;
        private bool _isDraggingOrbit = false;

        public void SetOrbitAngles(float yaw, float pitch)
        {
            _orbitYaw = yaw;
            _orbitPitch = pitch;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            if (_target != null)
            {
                _controller = _target.GetComponent<PlayerMotorcycleController>();
            }
        }

        public void ToggleViewMode()
        {
            _viewMode = (CameraViewMode)(((int)_viewMode + 1) % 3);
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (_camera != null) _baseFov = _camera.fieldOfView;
        }

        private void Start()
        {
            if (_target == null)
            {
                var bike = GameObject.FindWithTag("Motorcycle");
                if (bike != null) SetTarget(bike.transform);
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            // View toggle (V key)
            if (Input.GetKeyDown(KeyCode.V))
            {
                ToggleViewMode();
            }

            // Look-back toggle (C key)
            _isLookingBack = Input.GetKey(KeyCode.C);

            // Handle 360 Degree Orbit Dragging (Mouse & Touch)
            HandleOrbitInput();

            switch (_viewMode)
            {
                case CameraViewMode.ChaseThirdPerson:
                    UpdateChaseCamera(distance: _followDistance, height: _followHeight);
                    break;
                case CameraViewMode.CloseAction:
                    UpdateChaseCamera(distance: 3.2f, height: 1.4f);
                    break;
                case CameraViewMode.FirstPersonHandlebar:
                    UpdateFirstPersonCamera();
                    break;
            }

            // Speed-responsive dynamic FOV (Sense of high speed)
            if (_controller != null && _camera != null)
            {
                float speedRatio = Mathf.Clamp01(_controller.CurrentSpeedKmh / 150f);
                float targetFov = _controller.IsNitroActive ? _baseFov + 16f : Mathf.Lerp(_baseFov, _baseFov + 10f, speedRatio);
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFov, Time.deltaTime * 5f);
            }
        }

        private void HandleOrbitInput()
        {
            bool isMenuOpen = IndianBikeLife.Core.UI.MasterGameFlowUI.Instance != null && IndianBikeLife.Core.UI.MasterGameFlowUI.Instance.IsMenuOpen;

            // In Garage / Main Menu: Continuous smooth 360-degree turntable rotation around the motorcycle
            if (isMenuOpen && _autoTurntableInGarage && !_isDraggingOrbit)
            {
                _orbitYaw += 24f * Time.deltaTime;
                if (_orbitYaw > 360f) _orbitYaw -= 360f;
            }

            // Mouse / Touch Drag Orbit
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                // Only start drag if pointer is not clicking on buttons (center / free area)
                _lastPointerPos = Input.mousePosition;
                _isDraggingOrbit = true;
            }
            else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (_isDraggingOrbit)
                {
                    Vector3 delta = Input.mousePosition - _lastPointerPos;
                    if (delta.sqrMagnitude > 1f)
                    {
                        _orbitYaw += delta.x * 0.25f * _orbitSensitivity;
                        _orbitPitch = Mathf.Clamp(_orbitPitch - delta.y * 0.15f * _orbitSensitivity, -10f, 60f);
                    }
                    _lastPointerPos = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                _isDraggingOrbit = false;
            }

            // When riding forward without dragging, gently ease camera back behind bike
            if (!isMenuOpen && !_isDraggingOrbit && _controller != null && _controller.CurrentSpeedKmh > 15f)
            {
                _orbitYaw = Mathf.LerpAngle(_orbitYaw, 0f, Time.deltaTime * 3.5f);
                _orbitPitch = Mathf.Lerp(_orbitPitch, 15f, Time.deltaTime * 3f);
            }
        }

        private void UpdateChaseCamera(float distance, float height)
        {
            float speedFactor = _controller != null ? Mathf.Clamp01(_controller.CurrentSpeedKmh / 140f) * 0.8f : 0f;
            float actualDist = distance + speedFactor;

            // Base direction combined with 360-degree orbit yaw and pitch
            Quaternion orbitRot = Quaternion.Euler(_orbitPitch, _orbitYaw, 0f);
            Vector3 offset = orbitRot * new Vector3(0f, height, -actualDist);

            if (_isLookingBack)
            {
                offset = new Vector3(0f, height, actualDist);
            }

            Vector3 targetPosition = _target.position + (_target.rotation * offset);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _positionSmoothing);

            Vector3 lookTarget = _target.position + Vector3.up * (height * 0.75f);
            if (_isLookingBack) lookTarget = _target.position - _target.forward * 30f + Vector3.up * height;

            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSmoothing);
        }

        private void UpdateFirstPersonCamera()
        {
            Vector3 handlebarPos = _target.position + _target.forward * 0.45f + Vector3.up * 1.35f;
            transform.position = Vector3.Lerp(transform.position, handlebarPos, Time.deltaTime * 25f);

            Vector3 lookDir = _isLookingBack ? -_target.forward : _target.forward;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Time.deltaTime * 20f);
        }
    }
}

