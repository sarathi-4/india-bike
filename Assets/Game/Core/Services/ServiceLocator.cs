using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using UnityEngine;

namespace IndianBikeLife.Core.Services
{
    /// <summary>
    /// Decoupled, thread-safe Service Locator for registering and accessing core game services.
    /// Eliminates tight coupling to static singletons while preserving global lifetime across scenes.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IGameService> _services = new Dictionary<Type, IGameService>();
        private static readonly object _lock = new object();

        /// <summary>
        /// Registers a service implementation for type T.
        /// </summary>
        /// <typeparam name="T">The service interface or base type.</typeparam>
        /// <param name="service">The service implementation instance.</param>
        /// <param name="overwrite">Whether to replace an existing registration if present.</param>
        public static void Register<T>(T service, bool overwrite = false) where T : class, IGameService
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Cannot register a null service.");
            }

            lock (_lock)
            {
                Type type = typeof(T);
                if (_services.ContainsKey(type))
                {
                    if (overwrite)
                    {
                        Debug.LogWarning($"[ServiceLocator] Overwriting existing registration for {type.Name}.");
                        _services[type] = service;
                    }
                    else
                    {
                        Debug.LogWarning($"[ServiceLocator] Service of type {type.Name} is already registered. Skipping.");
                    }
                }
                else
                {
                    _services.Add(type, service);
                }
            }
        }

        /// <summary>
        /// Retrieves the registered service instance for type T.
        /// Throws InvalidOperationException if the service is not found.
        /// </summary>
        /// <typeparam name="T">The requested service interface.</typeparam>
        /// <returns>Registered service instance.</returns>
        public static T Get<T>() where T : class, IGameService
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (_services.TryGetValue(type, out IGameService service))
                {
                    return service as T;
                }

                throw new InvalidOperationException($"[ServiceLocator] Service {type.Name} is not registered. Ensure the bootstrap initialization has executed.");
            }
        }

        /// <summary>
        /// Attempts to retrieve the registered service instance for type T.
        /// </summary>
        /// <typeparam name="T">The requested service interface.</typeparam>
        /// <param name="service">The found service or null.</param>
        /// <returns>True if registered, false otherwise.</returns>
        public static bool TryGet<T>(out T service) where T : class, IGameService
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (_services.TryGetValue(type, out IGameService registeredService))
                {
                    service = registeredService as T;
                    return true;
                }

                service = null;
                return false;
            }
        }

        /// <summary>
        /// Unregisters a service type.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        public static void Unregister<T>() where T : class, IGameService
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (_services.ContainsKey(type))
                {
                    _services.Remove(type);
                }
            }
        }

        /// <summary>
        /// Resets all registrations and cleans up. Used during application shutdown or test teardown.
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                foreach (var kvp in _services)
                {
                    try
                    {
                        if (kvp.Value != null && kvp.Value.IsInitialized)
                        {
                            kvp.Value.Shutdown();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[ServiceLocator] Error shutting down service {kvp.Key.Name}: {ex.Message}");
                    }
                }

                _services.Clear();
            }
        }
    }
}
