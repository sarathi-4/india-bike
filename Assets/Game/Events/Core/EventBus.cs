using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Core.Events.Core
{
    /// <summary>
    /// High-performance, strongly-typed publish/subscribe event bus.
    /// Eliminates tight coupling between systems while ensuring memory safety and preventing duplicate handlers.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();
        private static readonly object _lock = new object();

        /// <summary>
        /// Subscribes an event listener to events of type T.
        /// Prevents duplicate registrations of the same listener.
        /// </summary>
        /// <typeparam name="T">Event type implementing IGameEvent.</typeparam>
        /// <param name="listener">Action callback invoked on publish.</param>
        public static void Subscribe<T>(Action<T> listener) where T : IGameEvent
        {
            if (listener == null) return;

            lock (_lock)
            {
                Type type = typeof(T);
                if (!_subscribers.TryGetValue(type, out List<Delegate> list))
                {
                    list = new List<Delegate>();
                    _subscribers[type] = list;
                }

                if (!list.Contains(listener))
                {
                    list.Add(listener);
                }
            }
        }

        /// <summary>
        /// Safely unsubscribes a previously registered listener.
        /// </summary>
        /// <typeparam name="T">Event type implementing IGameEvent.</typeparam>
        /// <param name="listener">The action callback to remove.</param>
        public static void Unsubscribe<T>(Action<T> listener) where T : IGameEvent
        {
            if (listener == null) return;

            lock (_lock)
            {
                Type type = typeof(T);
                if (_subscribers.TryGetValue(type, out List<Delegate> list))
                {
                    list.Remove(listener);
                    if (list.Count == 0)
                    {
                        _subscribers.Remove(type);
                    }
                }
            }
        }

        /// <summary>
        /// Publishes an event to all subscribed listeners.
        /// Safe exception handling ensures that an error in one listener does not block subsequent listeners.
        /// </summary>
        /// <typeparam name="T">Event type implementing IGameEvent.</typeparam>
        /// <param name="eventData">The event payload instance.</param>
        public static void Publish<T>(T eventData) where T : IGameEvent
        {
            List<Delegate> listenersCopy = null;

            lock (_lock)
            {
                Type type = typeof(T);
                if (_subscribers.TryGetValue(type, out List<Delegate> list))
                {
                    // Copy list to allow listeners to safely unsubscribe during invocation
                    listenersCopy = new List<Delegate>(list);
                }
            }

            if (listenersCopy == null || listenersCopy.Count == 0) return;

            for (int i = 0; i < listenersCopy.Count; i++)
            {
                if (listenersCopy[i] is Action<T> action)
                {
                    try
                    {
                        action.Invoke(eventData);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.Ex("EventBus", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current number of active subscribers for event type T.
        /// </summary>
        public static int SubscriberCount<T>() where T : IGameEvent
        {
            lock (_lock)
            {
                Type type = typeof(T);
                return _subscribers.TryGetValue(type, out List<Delegate> list) ? list.Count : 0;
            }
        }

        /// <summary>
        /// Clears all event subscriptions. Used during teardown or unit testing.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _subscribers.Clear();
            }
        }
    }
}

