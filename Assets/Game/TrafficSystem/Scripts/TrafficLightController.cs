using System.Collections;
using UnityEngine;

namespace IndianBikeLife.TrafficSystem
{
    public enum SignalState
    {
        Red = 0,
        Yellow = 1,
        Green = 2
    }

    /// <summary>
    /// Controls 3-phase traffic signals at intersections with configurable timing.
    /// </summary>
    public class TrafficLightController : MonoBehaviour
    {
        [Header("Signal Timings (Seconds)")]
        [SerializeField] private float _greenDuration = 18f;
        [SerializeField] private float _yellowDuration = 3f;
        [SerializeField] private float _redDuration = 18f;

        [Header("Visual Indicators (Optional)")]
        [SerializeField] private Renderer _redLight;
        [SerializeField] private Renderer _yellowLight;
        [SerializeField] private Renderer _greenLight;

        [Header("Stop Trigger Volume")]
        [SerializeField] private Collider _stopTrigger;

        private SignalState _currentState = SignalState.Red;

        public SignalState CurrentState => _currentState;

        private void Start()
        {
            StartCoroutine(SignalCycleRoutine());
        }

        private IEnumerator SignalCycleRoutine()
        {
            while (true)
            {
                SetState(SignalState.Green);
                yield return new WaitForSeconds(_greenDuration);

                SetState(SignalState.Yellow);
                yield return new WaitForSeconds(_yellowDuration);

                SetState(SignalState.Red);
                yield return new WaitForSeconds(_redDuration);
            }
        }

        private void SetState(SignalState state)
        {
            _currentState = state;

            if (_stopTrigger != null)
            {
                _stopTrigger.enabled = (_currentState == SignalState.Red);
            }

            if (_redLight != null) _redLight.material.color = (_currentState == SignalState.Red) ? Color.red : Color.gray;
            if (_yellowLight != null) _yellowLight.material.color = (_currentState == SignalState.Yellow) ? Color.yellow : Color.gray;
            if (_greenLight != null) _greenLight.material.color = (_currentState == SignalState.Green) ? Color.green : Color.gray;
        }
    }
}
