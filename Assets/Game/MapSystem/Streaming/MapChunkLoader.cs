using System;
using UnityEngine;

namespace IndianBikeLife.MapSystem.Streaming
{
    /// <summary>
    /// Represents a discrete geographical map sector or scene chunk that streams in and out.
    /// </summary>
    public class MapChunkLoader : MonoBehaviour
    {
        [Header("Chunk Identity")]
        [SerializeField] private string _chunkId = "Chunk_0_0";
        [SerializeField] private Vector3 _chunkCenter;
        [SerializeField] private float _loadDistance = 250f;
        [SerializeField] private float _unloadDistance = 350f;

        [Header("Content Hierarchy")]
        [SerializeField] private GameObject _chunkRoot;
        [SerializeField] private string _additiveSceneName;

        private bool _isLoaded = true;

        public string ChunkId => _chunkId;
        public Vector3 ChunkCenter => _chunkCenter;
        public bool IsLoaded => _isLoaded;

        public void SetLoaded(bool loaded)
        {
            if (_isLoaded == loaded) return;
            _isLoaded = loaded;

            if (_chunkRoot != null)
            {
                _chunkRoot.SetActive(loaded);
            }
        }

        public void CheckStreamingDistance(Vector3 playerPosition)
        {
            float distSqr = (playerPosition - _chunkCenter).sqrMagnitude;
            float loadDistSqr = _loadDistance * _loadDistance;
            float unloadDistSqr = _unloadDistance * _unloadDistance;

            if (!_isLoaded && distSqr <= loadDistSqr)
            {
                SetLoaded(true);
            }
            else if (_isLoaded && distSqr > unloadDistSqr)
            {
                SetLoaded(false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _isLoaded ? Color.green : Color.red;
            Gizmos.DrawWireCube(_chunkCenter, new Vector3(_loadDistance * 2f, 50f, _loadDistance * 2f));
        }
    }
}
