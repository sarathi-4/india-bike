using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndianBikeLife.Core.Events.ScriptableEvents
{
    [CreateAssetMenu(fileName = "NewStringEvent", menuName = "IndianBikeLife/Events/String Event")]
    public class StringEvent : ScriptableObject
    {
        private readonly List<StringEventListener> _listeners = new List<StringEventListener>();

        public void Raise(string value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i] != null)
                {
                    _listeners[i].OnEventRaised(value);
                }
            }
        }

        public void RegisterListener(StringEventListener listener)
        {
            if (listener != null && !_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void UnregisterListener(StringEventListener listener)
        {
            if (listener != null && _listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }
    }

    [Serializable]
    public class UnityStringEvent : UnityEvent<string> { }

    public class StringEventListener : MonoBehaviour
    {
        [SerializeField] private StringEvent _event;
        [SerializeField] private UnityStringEvent _response;

        private void OnEnable()
        {
            if (_event != null) _event.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (_event != null) _event.UnregisterListener(this);
        }

        public void OnEventRaised(string value)
        {
            _response?.Invoke(value);
        }
    }
}
