using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Core.Audio
{
    /// <summary>
    /// Object pool for AudioSource components to avoid runtime allocations during gameplay.
    /// Mobile games frequently spawn tire screeches, horns, and crashes that require fast, allocation-free sources.
    /// </summary>
    public class AudioSourcePool
    {
        private readonly Transform _parent;
        private readonly List<AudioSource> _pool = new List<AudioSource>();
        private readonly int _maxSize;

        public AudioSourcePool(Transform parent, int initialSize = 16, int maxSize = 32)
        {
            _parent = parent;
            _maxSize = maxSize;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewSource();
            }
        }

        private AudioSource CreateNewSource()
        {
            GameObject go = new GameObject($"PooledAudioSource_{_pool.Count}");
            go.transform.SetParent(_parent);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _pool.Add(source);
            return source;
        }

        public AudioSource GetAvailableSource()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (!_pool[i].isPlaying)
                {
                    return _pool[i];
                }
            }

            if (_pool.Count < _maxSize)
            {
                return CreateNewSource();
            }

            // If all sources are in use, reuse the oldest source
            return _pool[0];
        }

        public void StopAll()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i] != null && _pool[i].isPlaying)
                {
                    _pool[i].Stop();
                }
            }
        }
    }
}
