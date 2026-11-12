using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.MapSystem.Streaming
{
    /// <summary>
    /// Coordinates distance-based map chunk streaming to ensure minimal mobile RAM consumption.
    /// Unloads distant environmental sectors while keeping active road corridors loaded.
    /// </summary>
    public class MapStreamingManager : MonoBehaviour
    {
        private const string Tag = "MapStreaming";

        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _updateInterval = 1.0f;
        [SerializeField] private List<MapChunkLoader> _registeredChunks = new List<MapChunkLoader>();

        private float _timer;

        public void SetTarget(Transform target)
        {
            _targetTransform = target;
        }

        public void RegisterChunk(MapChunkLoader chunk)
        {
            if (chunk != null && !_registeredChunks.Contains(chunk))
            {
                _registeredChunks.Add(chunk);
            }
        }

        public void UnregisterChunk(MapChunkLoader chunk)
        {
            if (chunk != null)
            {
                _registeredChunks.Remove(chunk);
            }
        }

        private void Update()
        {
            if (_targetTransform == null) return;

            _timer += Time.deltaTime;
            if (_timer >= _updateInterval)
            {
                _timer = 0f;
                UpdateChunks();
            }
        }

        private void UpdateChunks()
        {
            Vector3 targetPos = _targetTransform.position;
            for (int i = 0; i < _registeredChunks.Count; i++)
            {
                if (_registeredChunks[i] != null)
                {
                    _registeredChunks[i].CheckStreamingDistance(targetPos);
                }
            }
        }
    }
}

