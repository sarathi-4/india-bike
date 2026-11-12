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

        private PlayerMotorcycleController _controller;
        private Camera _camera;
        private float _baseFov = 60f;
        private bool _isLookingBack = false;

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

        private void UpdateChaseCamera(float distance, float height)
        {
            float speedFactor = _controller != null ? Mathf.Clamp01(_controller.CurrentSpeedKmh / 140f) * 0.8f : 0f;
            float actualDist = distance + speedFactor;

            Vector3 backDir = _isLookingBack ? _target.forward : -_target.forward;
            Vector3 targetPosition = _target.position + backDir * actualDist + Vector3.up * height;

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
