using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.EnvironmentSystem
{
    public enum IndianVegetationType
    {
        BanyanTree = 0,
        NeemTree = 1,
        Gulmohar = 2,
        CoconutPalm = 3,       // Coastal & South India
        PineEucalyptus = 4,    // Hill Stations (Ooty, Munnar, Kodaikanal)
        RoadsideShrub = 5
    }

    /// <summary>
    /// Spawns and organizes region-specific Indian trees, palms, and roadside vegetation.
    /// Uses instanced structures to maintain high frame rates on Android.
    /// </summary>
    public class VegetationManager : MonoBehaviour
    {
        [Header("Vegetation Prefabs")]
        [SerializeField] private GameObject _banyanPrefab;
        [SerializeField] private GameObject _neemPrefab;
        [SerializeField] private GameObject _palmPrefab;
        [SerializeField] private GameObject _pinePrefab;
        [SerializeField] private GameObject _shrubPrefab;

        private readonly List<GameObject> _spawnedVegetation = new List<GameObject>();

        public void SpawnTree(IndianVegetationType type, Vector3 position, float scale = 1.0f)
        {
            GameObject prefab = GetPrefab(type);
            GameObject tree;

            if (prefab != null)
            {
                tree = Instantiate(prefab, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), transform);
            }
            else
            {
                // Procedural tree stand-in
                tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tree.name = $"Tree_{type}";
                tree.transform.position = position + Vector3.up * 3f;
                tree.transform.localScale = new Vector3(0.6f * scale, 3f * scale, 0.6f * scale);
                tree.transform.SetParent(transform);

                // Add foliage sphere
                GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                foliage.name = "Foliage";
                foliage.transform.SetParent(tree.transform);
                foliage.transform.localPosition = new Vector3(0f, 1.2f, 0f);
                foliage.transform.localScale = new Vector3(4f, 2.5f, 4f);
            }

            tree.transform.localScale *= scale;
            _spawnedVegetation.Add(tree);
        }

        public void ClearVegetation()
        {
            for (int i = 0; i < _spawnedVegetation.Count; i++)
            {
                if (_spawnedVegetation[i] != null) Destroy(_spawnedVegetation[i]);
            }
            _spawnedVegetation.Clear();
        }

        private GameObject GetPrefab(IndianVegetationType type)
        {
            switch (type)
            {
                case IndianVegetationType.BanyanTree: return _banyanPrefab;
                case IndianVegetationType.CoconutPalm: return _palmPrefab;
                case IndianVegetationType.PineEucalyptus: return _pinePrefab;
                case IndianVegetationType.RoadsideShrub: return _shrubPrefab;
                case IndianVegetationType.NeemTree:
                case IndianVegetationType.Gulmohar:
                default:
                    return _neemPrefab;
            }
        }
    }
}

