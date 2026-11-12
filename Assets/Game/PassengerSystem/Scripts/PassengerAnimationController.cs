using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    public enum PassengerAnimState
    {
        Standing = 0,
        Approaching = 1,
        Mounting = 2,
        SeatedIdle = 3,
        SeatedBraking = 4,
        SeatedAccelerating = 5,
        SeatedLeaning = 6,
        Dismounting = 7
    }

    /// <summary>
    /// Controls passenger animation states, procedural leaning during turns,
    /// and acceleration/braking inertia reactions.
    /// </summary>
    public class PassengerAnimationController : MonoBehaviour
    {
        [Header("Animator (Optional)")]
        [SerializeField] private Animator _animator;

        [Header("Procedural Spine & Lean Dynamics")]
        [SerializeField] private Transform _spineBone;
        [SerializeField] private Transform _headBone;
        [SerializeField] private float _leanSmoothing = 5.0f;
        [SerializeField] private float _maxSpineLeanAngle = 20.0f;
        [SerializeField] private float _maxPitchInertiaAngle = 15.0f;

        private PassengerAnimState _currentState = PassengerAnimState.SeatedIdle;
        private float _currentLeanAngle;
        private float _currentPitchAngle;
        private float _headLookTimer;
        private float _targetHeadYaw;

        public PassengerAnimState CurrentState => _currentState;

        public void SetState(PassengerAnimState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;

            if (_animator != null)
            {
                _animator.SetInteger("AnimState", (int)newState);
            }
        }

        public void UpdateRidingDynamics(float bikeRollAngle, float bikeAcceleration, float deltaTime)
        {
            if (_currentState != PassengerAnimState.SeatedIdle &&
                _currentState != PassengerAnimState.SeatedLeaning &&
                _currentState != PassengerAnimState.SeatedBraking &&
                _currentState != PassengerAnimState.SeatedAccelerating)
            {
                return;
            }

            // 1. Roll (Cornering Lean)
            float targetLean = Mathf.Clamp(bikeRollAngle * 0.8f, -_maxSpineLeanAngle, _maxSpineLeanAngle);
            _currentLeanAngle = Mathf.Lerp(_currentLeanAngle, targetLean, deltaTime * _leanSmoothing);

            // 2. Pitch (Acceleration / Braking Inertia)
            // Positive acceleration tilts back slightly; negative (braking) tilts forward towards rider
            float targetPitch = Mathf.Clamp(-bikeAcceleration * 2.0f, -_maxPitchInertiaAngle, _maxPitchInertiaAngle);
            _currentPitchAngle = Mathf.Lerp(_currentPitchAngle, targetPitch, deltaTime * _leanSmoothing);

            // Update state flag
            if (bikeAcceleration < -2.0f)
            {
                _currentState = PassengerAnimState.SeatedBraking;
            }
            else if (bikeAcceleration > 2.0f)
            {
                _currentState = PassengerAnimState.SeatedAccelerating;
            }
            else if (Mathf.Abs(bikeRollAngle) > 5.0f)
            {
                _currentState = PassengerAnimState.SeatedLeaning;
            }
            else
            {
                _currentState = PassengerAnimState.SeatedIdle;
            }

            // 3. Head Look Around (Natural idle behavior)
            _headLookTimer -= deltaTime;
            if (_headLookTimer <= 0f)
            {
                _headLookTimer = Random.Range(3.0f, 7.0f);
                _targetHeadYaw = Random.Range(-25.0f, 25.0f);
            }

            // Apply procedural bone rotations if bones are bound
            if (_spineBone != null)
            {
                _spineBone.localRotation = Quaternion.Euler(_currentPitchAngle, 0f, -_currentLeanAngle);
            }

            if (_headBone != null)
            {
                Quaternion targetHeadRot = Quaternion.Euler(0f, _targetHeadYaw, 0f);
                _headBone.localRotation = Quaternion.Slerp(_headBone.localRotation, targetHeadRot, deltaTime * 2.0f);
            }
        }
    }
}
