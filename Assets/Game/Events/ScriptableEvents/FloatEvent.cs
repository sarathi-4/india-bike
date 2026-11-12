using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndianBikeLife.Core.Events.ScriptableEvents
{
    [CreateAssetMenu(fileName = "NewFloatEvent", menuName = "IndianBikeLife/Events/Float Event")]
    public class FloatEvent : ScriptableObject
    {
        private readonly List<FloatEventListener> _listeners = new List<FloatEventListener>();

        public void Raise(float value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i] != null)
                {
                    _listeners[i].OnEventRaised(value);
                }
            }
        }

        public void RegisterListener(FloatEventListener listener)
        {
            if (listener != null && !_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void UnregisterListener(FloatEventListener listener)
        {
            if (listener != null && _listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }
    }

    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }

    public class FloatEventListener : MonoBehaviour
    {
        [SerializeField] private FloatEvent _event;
        [SerializeField] private UnityFloatEvent _response;

        private void OnEnable()
        {
            if (_event != null) _event.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (_event != null) _event.UnregisterListener(this);
        }

        public void OnEventRaised(float value)
        {
            _response?.Invoke(value);
        }
    }
}
