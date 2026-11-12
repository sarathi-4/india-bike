using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Replicates remote passenger presence, seating position, and synchronized lean roll.
    /// Ensures all riders in a multiplayer room see consistent two-person riding.
    /// </summary>
    public class NetworkPassengerController : MonoBehaviour
    {
        [SerializeField] private GameObject _passengerModel;
        [SerializeField] private Transform _pillionSeatAnchor;

        private NetworkPlayerController _playerController;
        private float _currentPassengerRoll;

        private void Awake()
        {
            _playerController = GetComponent<NetworkPlayerController>();
        }

        private void Update()
        {
            if (_playerController == null) return;

            var state = _playerController.LatestState;

            // 1. Replicate Passenger Active State
            if (_passengerModel != null)
            {
                if (_passengerModel.activeSelf != state.hasPassenger)
                {
                    _passengerModel.SetActive(state.hasPassenger);
                }
            }

            // 2. Replicate Passenger Lean Roll
            if (state.hasPassenger && _passengerModel != null)
            {
                _currentPassengerRoll = Mathf.Lerp(_currentPassengerRoll, state.passengerLeanRoll, Time.deltaTime * 10f);
                _passengerModel.transform.localRotation = Quaternion.Euler(0f, 0f, -_currentPassengerRoll);
            }
        }

        public bool HasPassenger => _passengerModel != null && _passengerModel.activeSelf;
        public float PassengerLeanRoll => _currentPassengerRoll;

        public void ApplyPassengerState(bool hasPassenger, float passengerLeanRoll)
        {
            _currentPassengerRoll = passengerLeanRoll;
            if (_passengerModel != null)
            {
                _passengerModel.SetActive(hasPassenger);
                _passengerModel.transform.localRotation = Quaternion.Euler(0f, 0f, -passengerLeanRoll);
            }
        }
    }
}

