using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Core.Events.ScriptableEvents
{
    /// <summary>
    /// ScriptableObject-based event channel. Allows scene objects, prefabs, and UI elements
    /// to signal each other through the Unity Inspector without direct code dependencies.
    /// </summary>
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "IndianBikeLife/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

        public void Raise()
        {
            // Reverse iteration allows listeners to safely unregister during Raise()
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i] != null)
                {
                    _listeners[i].OnEventRaised();
                }
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (listener != null && !_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (listener != null && _listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }
    }
}

