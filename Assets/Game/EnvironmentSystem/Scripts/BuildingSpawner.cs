using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.EnvironmentSystem
{
    public enum BuildingStyle
    {
        VillageHut = 0,
        CommercialShop = 1,
        IndianDhabaTeaStall = 2,
        Temple = 3,
        PetrolPump = 4,
        BusShelter = 5,
        ResidentialApartment = 6
    }

    /// <summary>
    /// Spawns authentic Indian buildings, commercial stalls, and infrastructure along road corridors.
    /// </summary>
    public class BuildingSpawner : MonoBehaviour
    {
        [Header("Building Prefabs")]
        [SerializeField] private GameObject _dhabaPrefab;
        [SerializeField] private GameObject _shopPrefab;
        [SerializeField] private GameObject _templePrefab;
        [SerializeField] private GameObject _petrolPumpPrefab;
        [SerializeField] private GameObject _busShelterPrefab;
        [SerializeField] private GameObject _housePrefab;

        private readonly List<GameObject> _spawnedBuildings = new List<GameObject>();

        public void SpawnBuilding(BuildingStyle style, Vector3 position, Quaternion rotation)
        {
            GameObject prefab = GetPrefabForStyle(style);
            GameObject instance;

            if (prefab != null)
            {
                instance = Instantiate(prefab, position, rotation, transform);
            }
            else
            {
                // Fallback procedural building structure
                instance = CreateProceduralBuilding(style, position, rotation);
            }

            _spawnedBuildings.Add(instance);
        }

        public void ClearBuildings()
        {
            for (int i = 0; i < _spawnedBuildings.Count; i++)
            {
                if (_spawnedBuildings[i] != null) Destroy(_spawnedBuildings[i]);
            }
            _spawnedBuildings.Clear();
        }

        private GameObject GetPrefabForStyle(BuildingStyle style)
        {
            switch (style)
            {
                case BuildingStyle.IndianDhabaTeaStall: return _dhabaPrefab;
                case BuildingStyle.CommercialShop: return _shopPrefab;
                case BuildingStyle.Temple: return _templePrefab;
                case BuildingStyle.PetrolPump: return _petrolPumpPrefab;
                case BuildingStyle.BusShelter: return _busShelterPrefab;
                case BuildingStyle.VillageHut:
                case BuildingStyle.ResidentialApartment:
                default:
                    return _housePrefab;
            }
        }

        private GameObject CreateProceduralBuilding(BuildingStyle style, Vector3 pos, Quaternion rot)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Building_{style}";
            go.transform.position = pos + Vector3.up * 2.5f;
            go.transform.rotation = rot;
            go.transform.localScale = (style == BuildingStyle.IndianDhabaTeaStall || style == BuildingStyle.BusShelter)
                ? new Vector3(8f, 3.5f, 5f)
                : new Vector3(10f, 6f, 10f);
            go.transform.SetParent(transform);
            return go;
        }
    }
}
