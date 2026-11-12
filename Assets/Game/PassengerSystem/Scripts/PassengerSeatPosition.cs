using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    /// <summary>
    /// Placed on motorcycle prefab to designate spatial attachment anchors for the passenger.
    /// Prevents passenger floating, misalignment, or body clipping during rides.
    /// </summary>
    public class PassengerSeatPosition : MonoBehaviour
    {
        [Header("Seat Anchors")]
        [Tooltip("The exact pivot where the passenger pelvis sits.")]
        [SerializeField] private Transform _seatAnchor;

        [Header("Hand Hold Anchors")]
        [Tooltip("Motorcycle rear grab handle left side.")]
        [SerializeField] private Transform _leftGrabHandle;
        [Tooltip("Motorcycle rear grab handle right side.")]
        [SerializeField] private Transform _rightGrabHandle;

        [Tooltip("Rider waist left anchor (used for couples/waist hold style).")]
        [SerializeField] private Transform _riderWaistLeft;
        [Tooltip("Rider waist right anchor (used for couples/waist hold style).")]
        [SerializeField] private Transform _riderWaistRight;

        [Header("Foot Peg Anchors")]
        [SerializeField] private Transform _leftFootPeg;
        [SerializeField] private Transform _rightFootPeg;

        [Header("Mount / Dismount Points")]
        [Tooltip("Ground position beside the motorcycle where passenger approaches/leaves.")]
        [SerializeField] private Transform _mountStandPoint;

        public Transform SeatAnchor => _seatAnchor != null ? _seatAnchor : transform;
        public Transform LeftGrabHandle => _leftGrabHandle != null ? _leftGrabHandle : _seatAnchor;
        public Transform RightGrabHandle => _rightGrabHandle != null ? _rightGrabHandle : _seatAnchor;
        public Transform RiderWaistLeft => _riderWaistLeft != null ? _riderWaistLeft : _leftGrabHandle;
        public Transform RiderWaistRight => _riderWaistRight != null ? _riderWaistRight : _rightGrabHandle;
        public Transform LeftFootPeg => _leftFootPeg;
        public Transform RightFootPeg => _rightFootPeg;
        public Transform MountStandPoint => _mountStandPoint != null ? _mountStandPoint : transform;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            if (_seatAnchor != null)
            {
                Gizmos.DrawWireSphere(_seatAnchor.position, 0.15f);
            }
            if (_mountStandPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_mountStandPoint.position, 0.25f);
            }
        }
    }
}
