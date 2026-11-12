using UnityEngine;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Replicates visual and auditory motorcycle state for remote players:
    /// lean roll, steering angle, headlight beams, brake lights, and horn sounds.
    /// </summary>
    public class NetworkBikeController : MonoBehaviour
    {
        [SerializeField] private Transform _bikeBodyTransform;
        [SerializeField] private Transform _frontForkTransform;
        [SerializeField] private Light _headlight;
        [SerializeField] private AudioSource _hornAudioSource;

        private NetworkPlayerController _playerController;
        private float _currentLeanRoll;

        private void Awake()
        {
            _playerController = GetComponent<NetworkPlayerController>();
        }

        private void Update()
        {
            if (_playerController == null) return;

            var state = _playerController.LatestState;

            // 1. Replicate motorcycle lean roll
            _currentLeanRoll = Mathf.Lerp(_currentLeanRoll, state.bikeLeanRoll, Time.deltaTime * 10f);
            if (_bikeBodyTransform != null)
            {
                _bikeBodyTransform.localRotation = Quaternion.Euler(0f, 0f, -_currentLeanRoll);
            }

            // 2. Front fork steering angle
            if (_frontForkTransform != null)
            {
                _frontForkTransform.localRotation = Quaternion.Euler(0f, state.steerAngle, 0f);
            }

            // 3. Headlight replication
            if (_headlight != null)
            {
                _headlight.enabled = state.headlightOn;
            }

            // 4. Horn replication
            if (state.hornHonking && _hornAudioSource != null && !_hornAudioSource.isPlaying)
            {
                _hornAudioSource.Play();
            }
        }
    }
}
