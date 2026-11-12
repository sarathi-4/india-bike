using UnityEngine;
using UnityEngine.Events;

namespace IndianBikeLife.Core.Events.ScriptableEvents
{
    /// <summary>
    /// MonoBehaviour component listening to a ScriptableObject GameEvent.
    /// Handles registration in OnEnable/OnDisable to completely prevent memory leaks.
    /// </summary>
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("The ScriptableObject event channel to listen to.")]
        [SerializeField] private GameEvent _event;

        [Tooltip("The UnityEvent invoked when the GameEvent is raised.")]
        [SerializeField] private UnityEvent _response;

        public GameEvent Event
        {
            get => _event;
            set => _event = value;
        }

        private void OnEnable()
        {
            if (_event != null)
            {
                _event.RegisterListener(this);
            }
        }

        private void OnDisable()
        {
            if (_event != null)
            {
                _event.UnregisterListener(this);
            }
        }

        public void OnEventRaised()
        {
            _response?.Invoke();
        }
    }
}

