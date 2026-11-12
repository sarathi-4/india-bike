using System;
using System.Collections;
using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    /// <summary>
    /// Coordinates the mounting and dismounting sequence of the passenger character.
    /// Handles physical alignment, smooth position interpolation, and seat parenting.
    /// </summary>
    public class PassengerMountController : MonoBehaviour
    {
        [SerializeField] private float _approachSpeed = 1.8f;
        [SerializeField] private float _mountDuration = 1.2f;

        private PassengerSeatPosition _currentSeat;
        private PassengerAnimationController _animController;
        private bool _isTransitioning;

        public bool IsMounted { get; private set; }
        public bool IsTransitioning => _isTransitioning;

        private void Awake()
        {
            _animController = GetComponent<PassengerAnimationController>();
        }

        public void Mount(PassengerSeatPosition seat, Action onComplete = null)
        {
            if (_isTransitioning || seat == null) return;
            _currentSeat = seat;
            StartCoroutine(MountRoutine(onComplete));
        }

        public void Dismount(Action onComplete = null)
        {
            if (_isTransitioning || !IsMounted || _currentSeat == null) return;
            StartCoroutine(DismountRoutine(onComplete));
        }

        private IEnumerator MountRoutine(Action onComplete)
        {
            _isTransitioning = true;
            if (_animController != null) _animController.SetState(PassengerAnimState.Approaching);

            // 1. Approach the side mount stand point
            Vector3 startPos = transform.position;
            Vector3 approachTarget = _currentSeat.MountStandPoint.position;
            float approachDist = Vector3.Distance(startPos, approachTarget);

            if (approachDist > 0.1f)
            {
                float t = 0f;
                float duration = Mathf.Clamp(approachDist / _approachSpeed, 0.5f, 2.0f);
                while (t < 1.0f)
                {
                    t += Time.deltaTime / duration;
                    transform.position = Vector3.Lerp(startPos, approachTarget, t);
                    transform.rotation = Quaternion.Slerp(transform.rotation, _currentSeat.MountStandPoint.rotation, t);
                    yield return null;
                }
            }

            // 2. Play Mounting animation & interpolate onto seat
            if (_animController != null) _animController.SetState(PassengerAnimState.Mounting);

            float mountTimer = 0f;
            Vector3 mountStart = transform.position;
            Quaternion mountStartRot = transform.rotation;

            while (mountTimer < _mountDuration)
            {
                mountTimer += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, mountTimer / _mountDuration);

                transform.position = Vector3.Lerp(mountStart, _currentSeat.SeatAnchor.position, progress);
                transform.rotation = Quaternion.Slerp(mountStartRot, _currentSeat.SeatAnchor.rotation, progress);
                yield return null;
            }

            // 3. Parent to seat anchor to lock position during high-speed riding
            transform.SetParent(_currentSeat.SeatAnchor);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            IsMounted = true;
            _isTransitioning = false;
            if (_animController != null) _animController.SetState(PassengerAnimState.SeatedIdle);

            onComplete?.Invoke();
        }

        private IEnumerator DismountRoutine(Action onComplete)
        {
            _isTransitioning = true;
            if (_animController != null) _animController.SetState(PassengerAnimState.Dismounting);

            transform.SetParent(null); // Unparent from motorcycle

            Vector3 dismountStart = transform.position;
            Quaternion dismountStartRot = transform.rotation;
            Vector3 dismountTarget = _currentSeat.MountStandPoint.position;
            Quaternion dismountTargetRot = _currentSeat.MountStandPoint.rotation;

            float timer = 0f;
            while (timer < _mountDuration)
            {
                timer += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, timer / _mountDuration);

                transform.position = Vector3.Lerp(dismountStart, dismountTarget, progress);
                transform.rotation = Quaternion.Slerp(dismountStartRot, dismountTargetRot, progress);
                yield return null;
            }

            transform.position = dismountTarget;
            transform.rotation = dismountTargetRot;

            IsMounted = false;
            _isTransitioning = false;
            if (_animController != null) _animController.SetState(PassengerAnimState.Standing);

            onComplete?.Invoke();
        }
    }
}
