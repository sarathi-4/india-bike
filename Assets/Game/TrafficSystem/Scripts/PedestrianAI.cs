using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    public enum PedestrianState
    {
        Walking = 0,
        WaitingToCross = 1,
        CrossingRoad = 2,
        StandingIdle = 3
    }

    /// <summary>
    /// Autonomous pedestrian AI simulating authentic Indian road crossings and sidewalk walking.
    /// </summary>
    public class PedestrianAI : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed = 1.3f;
        [SerializeField] private PedestrianState _state = PedestrianState.Walking;
        [SerializeField] private Vector3 _targetDestination;

        private float _waitTimer;

        public PedestrianState State => _state;

        public void SetTarget(Vector3 destination, PedestrianState initialState = PedestrianState.Walking)
        {
            _targetDestination = destination;
            _state = initialState;
        }

        private void Update()
        {
            switch (_state)
            {
                case PedestrianState.Walking:
                case PedestrianState.CrossingRoad:
                    Vector3 diff = _targetDestination - transform.position;
                    diff.y = 0f;
                    if (diff.sqrMagnitude > 0.2f)
                    {
                        transform.position += diff.normalized * (_walkSpeed * Time.deltaTime);
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diff), Time.deltaTime * 6f);
                    }
                    else
                    {
                        _state = PedestrianState.StandingIdle;
                        _waitTimer = Random.Range(3f, 8f);
                    }
                    break;

                case PedestrianState.WaitingToCross:
                case PedestrianState.StandingIdle:
                    _waitTimer -= Time.deltaTime;
                    if (_waitTimer <= 0f)
                    {
                        _state = PedestrianState.CrossingRoad;
                    }
                    break;
            }
        }
    }
}
