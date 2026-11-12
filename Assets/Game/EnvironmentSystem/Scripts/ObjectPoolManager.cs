using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.EnvironmentSystem
{
    /// <summary>
    /// Generic object pool manager for mobile performance.
    /// Eliminates runtime Instantiate() and Destroy() memory churn during world traversal.
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        private readonly Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null) return null;
            string key = prefab.name;

            if (!_pools.TryGetValue(key, out Queue<GameObject> queue))
            {
                queue = new Queue<GameObject>();
                _pools.Add(key, queue);
            }

            GameObject instance;
            if (queue.Count > 0)
            {
                instance = queue.Dequeue();
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                if (parent != null) instance.transform.SetParent(parent);
                instance.SetActive(true);
            }
            else
            {
                instance = Instantiate(prefab, position, rotation, parent);
                instance.name = key;
            }

            return instance;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null) return;
            string key = instance.name;

            instance.SetActive(false);
            if (!_pools.TryGetValue(key, out Queue<GameObject> queue))
            {
                queue = new Queue<GameObject>();
                _pools.Add(key, queue);
            }
            queue.Enqueue(instance);
        }

        public void ClearPools()
        {
            foreach (var kvp in _pools)
            {
                while (kvp.Value.Count > 0)
                {
                    GameObject go = kvp.Value.Dequeue();
                    if (go != null) Destroy(go);
                }
            }
            _pools.Clear();
        }
    }
}

