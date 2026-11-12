using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    public enum PassengerCameraView
    {
        Driver = 0,
        PassengerFocused = 1,
        ThirdPerson = 2,
        Cinematic = 3
    }

    /// <summary>
    /// Specialized multi-angle camera controller accommodating driver, passenger,
    /// third-person chase, and cinematic perspectives.
    /// </summary>
    public class PassengerCameraController : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField] private Transform _motorcycleTarget;
        [SerializeField] private Transform _passengerHeadTarget;

        [Header("Camera View Settings")]
        [SerializeField] private PassengerCameraView _currentView = PassengerCameraView.ThirdPerson;
        [SerializeField] private float _followDistance = 3.5f;
        [SerializeField] private float _followHeight = 1.4f;
        [SerializeField] private float _positionSmoothing = 8.0f;
        [SerializeField] private float _rotationSmoothing = 5.0f;

        private Vector3 _currentVelocity;

        public PassengerCameraView CurrentView => _currentView;

        public void SetTarget(Transform motorcycle, Transform passengerHead)
        {
            _motorcycleTarget = motorcycle;
            _passengerHeadTarget = passengerHead;
        }

        public void SetCameraView(PassengerCameraView view)
        {
            _currentView = view;
        }

        public void CycleCameraView()
        {
            int next = ((int)_currentView + 1) % 4;
            _currentView = (PassengerCameraView)next;
        }

        private void LateUpdate()
        {
            if (_motorcycleTarget == null) return;

            Vector3 targetPosition = Vector3.zero;
            Quaternion targetRotation = Quaternion.identity;

            switch (_currentView)
            {
                case PassengerCameraView.Driver:
                    // First person over the handlebars
                    targetPosition = _motorcycleTarget.position + _motorcycleTarget.forward * 0.4f + _motorcycleTarget.up * 1.1f;
                    targetRotation = _motorcycleTarget.rotation;
                    break;

                case PassengerCameraView.PassengerFocused:
                    // Over-the-shoulder view focusing on passenger and driver interaction
                    if (_passengerHeadTarget != null)
                    {
                        targetPosition = _passengerHeadTarget.position - _motorcycleTarget.forward * 1.2f + _motorcycleTarget.right * 0.45f + Vector3.up * 0.2f;
                        targetRotation = Quaternion.LookRotation((_motorcycleTarget.position + _motorcycleTarget.forward * 2f) - targetPosition);
                    }
                    else
                    {
                        goto case PassengerCameraView.ThirdPerson;
                    }
                    break;

                case PassengerCameraView.ThirdPerson:
                    // Standard motorcycle follow camera
                    Vector3 offset = -_motorcycleTarget.forward * _followDistance + Vector3.up * _followHeight;
                    targetPosition = _motorcycleTarget.position + offset;
                    targetRotation = Quaternion.LookRotation((_motorcycleTarget.position + Vector3.up * 1.0f) - targetPosition);
                    break;

                case PassengerCameraView.Cinematic:
                    // Low angle dynamic side view
                    Vector3 sideOffset = -_motorcycleTarget.forward * 2.5f + _motorcycleTarget.right * 1.8f + Vector3.up * 0.7f;
                    targetPosition = _motorcycleTarget.position + sideOffset;
                    targetRotation = Quaternion.LookRotation((_motorcycleTarget.position + Vector3.up * 0.8f) - targetPosition);
                    break;
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, 1.0f / _positionSmoothing);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSmoothing);
        }
    }
}

