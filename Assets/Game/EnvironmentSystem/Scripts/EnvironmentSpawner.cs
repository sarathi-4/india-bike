using System.Collections.Generic;
using IndianBikeLife.RoadSystem;
using UnityEngine;

namespace IndianBikeLife.EnvironmentSystem
{
    /// <summary>
    /// Spawns authentic roadside buildings, dhabas, tea stalls, and trees alongside road splines
    /// according to the active regional Indian environmental theme.
    /// </summary>
    public class EnvironmentSpawner : MonoBehaviour
    {
        [SerializeField] private BuildingSpawner _buildingSpawner;
        [SerializeField] private VegetationManager _vegetationManager;

        public void PopulateCorridor(RoadSpline road, EnvironmentTheme theme)
        {
            if (road == null || road.Nodes.Count < 2) return;

            float roadLength = road.GetTotalLength();
            int spawnSteps = Mathf.FloorToInt(roadLength / 35.0f); // Spawn prop every 35 meters

            for (int i = 1; i <= spawnSteps; i++)
            {
                float t = (float)i / spawnSteps;
                Vector3 center = road.EvaluatePosition(t);
                Vector3 tangent = road.EvaluateTangent(t);
                Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

                float roadHalfWidth = (road.LaneCount * road.LaneWidth) * 0.5f;

                // Left side building / stall placement
                if (i % 2 == 0)
                {
                    Vector3 leftPos = center - right * (roadHalfWidth + 7.0f);
                    Quaternion leftRot = Quaternion.LookRotation(right); // Face the road

                    BuildingStyle style = GetBuildingStyleForTheme(theme, i);
                    if (_buildingSpawner != null)
                    {
                        _buildingSpawner.SpawnBuilding(style, leftPos, leftRot);
                    }
                }

                // Right side tree / vegetation placement
                Vector3 rightPos = center + right * (roadHalfWidth + Random.Range(4.0f, 9.0f));
                IndianVegetationType vegType = GetVegetationForTheme(theme);
                if (_vegetationManager != null)
                {
                    _vegetationManager.SpawnTree(vegType, rightPos, Random.Range(0.85f, 1.25f));
                }
            }
        }

        private BuildingStyle GetBuildingStyleForTheme(EnvironmentTheme theme, int index)
        {
            switch (theme)
            {
                case EnvironmentTheme.HighwayCorridor:
                    return (index % 4 == 0) ? BuildingStyle.PetrolPump : BuildingStyle.IndianDhabaTeaStall;
                case EnvironmentTheme.VillageRural:
                    return (index % 3 == 0) ? BuildingStyle.Temple : BuildingStyle.VillageHut;
                case EnvironmentTheme.TownCommercial:
                case EnvironmentTheme.CityMetropolitan:
                    return (index % 3 == 0) ? BuildingStyle.BusShelter : BuildingStyle.CommercialShop;
                case EnvironmentTheme.HillStationGhat:
                default:
                    return BuildingStyle.IndianDhabaTeaStall;
            }
        }

        private IndianVegetationType GetVegetationForTheme(EnvironmentTheme theme)
        {
            switch (theme)
            {
                case EnvironmentTheme.CoastalBeach:
                    return IndianVegetationType.CoconutPalm;
                case EnvironmentTheme.HillStationGhat:
                    return IndianVegetationType.PineEucalyptus;
                case EnvironmentTheme.VillageRural:
                    return (Random.value > 0.5f) ? IndianVegetationType.BanyanTree : IndianVegetationType.NeemTree;
                case EnvironmentTheme.HighwayCorridor:
                default:
                    return IndianVegetationType.Gulmohar;
            }
        }
    }
}
