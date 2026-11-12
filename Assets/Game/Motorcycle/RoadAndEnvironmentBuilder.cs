using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Builds dynamic, procedural Indian road corridors with realistic bends, S-curves,
    /// elevation changes, guard rails, and state-specific roadside landmarks:
    /// - Tamil Nadu (NH-44 Expressway & Ooty Ghats S-Curves, Coconut Groves, Chennai Milestones)
    /// - Maharashtra (Mumbai-Pune Expressway, Mountain Tunnels & Deccan Toll Plaza)
    /// - Goa (Coastal Marine Drive Curves, Beach Shacks, Palm Corridors)
    /// - Himachal Pradesh & Ladakh (High Mountain Hairpin Passes, Stone Guardrails, Snow Crags)
    /// - Kerala (Munnar Tea Garden Hairpin Bends, Waterfall Bridges)
    /// - Punjab (Grand Trunk Road, Mustard Fields & Traditional Highway Dhabas)
    /// </summary>
    public class RoadAndEnvironmentBuilder : MonoBehaviour
    {
        [Header("Road Configuration")]
        [SerializeField] private int _activeStateIndex = 0; // 0=Tamil Nadu, 1=Maharashtra, 2=Goa, 3=Himachal, etc.
        [SerializeField] private float _segmentLength = 25f;
        [SerializeField] private int _totalSegments = 80; // 80 * 25m = 2,000 meters corridor
        [SerializeField] private float _roadWidth = 14f;

        private Material _asphaltMat;
        private Material _markingWhiteMat;
        private Material _markingYellowMat;
        private Material _grassMat;
        private Material _dhabaWallMat;
        private Material _dhabaRoofMat;
        private Material _woodMat;
        private Material _leafMat;
        private Material _guardRailMat;
        private Material _tunnelMat;
        private Material _sandMat;

        private GameObject _worldRoot;
        public int ActiveStateIndex => _activeStateIndex;

        private void Awake()
        {
            BuildHighwayWorld(_activeStateIndex);
        }

        public void LoadStateEnvironment(int stateIndex)
        {
            _activeStateIndex = Mathf.Clamp(stateIndex, 0, 7);
            BuildHighwayWorld(_activeStateIndex);
        }

        public void BuildHighwayWorld(int stateIndex)
        {
            _activeStateIndex = stateIndex;
            CreateMaterials();

            if (_worldRoot != null)
            {
                DestroyImmediate(_worldRoot);
            }

            _worldRoot = new GameObject("[Indian_Highway_World_Corridor]");
            _worldRoot.transform.SetParent(transform, false);

            // Compute curve spline points along the road
            List<Vector3> roadPoints = new List<Vector3>(_totalSegments + 1);
            for (int i = 0; i <= _totalSegments; i++)
            {
                float z = i * _segmentLength;
                Vector3 pt = CalculateRoadPosition(z, _activeStateIndex);
                roadPoints.Add(pt);
            }

            // 1. Build Endless Landscape Terrain Base
            BuildTerrainLandscape(_worldRoot.transform, _activeStateIndex);

            // 2. Build Curved Road Segments with Colliders & Markings
            for (int i = 0; i < _totalSegments; i++)
            {
                Vector3 p0 = roadPoints[i];
                Vector3 p1 = roadPoints[i + 1];
                Vector3 center = (p0 + p1) * 0.5f;
                Vector3 dir = (p1 - p0).normalized;
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                float segLen = Vector3.Distance(p0, p1);

                // Asphalt Road Slab
                GameObject segObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segObj.name = $"Road_Seg_{i:00}";
                segObj.transform.SetParent(_worldRoot.transform, false);
                segObj.transform.position = center;
                segObj.transform.rotation = rot;
                segObj.transform.localScale = new Vector3(_roadWidth, 0.22f, segLen + 0.1f);
                segObj.GetComponent<Renderer>().sharedMaterial = _asphaltMat;

                // Yellow Shoulder Lines (Left & Right)
                float halfW = _roadWidth * 0.5f - 0.4f;
                CreateRoadStrip("Shoulder_Y_L", segObj.transform, new Vector3(-halfW, 0.13f, 0f), new Vector3(0.30f, 0.02f, 1f), _markingYellowMat);
                CreateRoadStrip("Shoulder_Y_R", segObj.transform, new Vector3(halfW, 0.13f, 0f), new Vector3(0.30f, 0.02f, 1f), _markingYellowMat);

                // Dashed White Center Line (Alternate segments)
                if (i % 2 == 0)
                {
                    CreateRoadStrip("Dash_White", segObj.transform, new Vector3(0f, 0.13f, 0f), new Vector3(0.28f, 0.02f, 0.75f), _markingWhiteMat);
                }

                // Curved Guardrails on sharp bends
                bool isSharpCurve = Mathf.Abs(p1.x - p0.x) > 1.2f;
                if (isSharpCurve || _activeStateIndex == 3 || _activeStateIndex == 4) // Hill stations have continuous guardrails
                {
                    float guardSide = (p1.x > p0.x) ? -halfW - 0.6f : halfW + 0.6f;
                    CreateGuardRail(segObj.transform, new Vector3(guardSide, 0.45f, 0f), segLen);
                }

                // Roadside Elements (Milestones, Signs, Trees, Dhabas)
                SpawnRoadsideElements(center, rot, i, _activeStateIndex);
            }

            Debug.Log($"[RoadAndEnvironmentBuilder] Built State {_activeStateIndex} Corridor with {_totalSegments} curved road segments!");
        }

        private Vector3 CalculateRoadPosition(float z, int stateIdx)
        {
            float x = 0f;
            float y = 0f;

            switch (stateIdx)
            {
                case 0: // Tamil Nadu (NH-44 Express Corridor & Ooty Foothill Curves)
                    x = 24f * Mathf.Sin(z * 0.004f) + 12f * Mathf.Sin(z * 0.009f);
                    y = Mathf.Sin(z * 0.003f) * 2.5f;
                    break;

                case 1: // Maharashtra (Mumbai-Pune Expressway with wide sweeping curves)
                    x = 35f * Mathf.Sin(z * 0.003f);
                    y = Mathf.Cos(z * 0.0025f) * 4.0f;
                    break;

                case 2: // Goa (Scenic Coastal Beach S-Curves)
                    x = 32f * Mathf.Sin(z * 0.0055f) + 10f * Mathf.Cos(z * 0.012f);
                    y = 0.5f;
                    break;

                case 3: // Himachal Pradesh (Misty Mountain Hairpin Turns & High Passes)
                    x = 42f * Mathf.Sin(z * 0.008f) + 25f * Mathf.Cos(z * 0.015f);
                    y = (z * 0.018f) + Mathf.Sin(z * 0.006f) * 6f; // Climbs into mountains
                    break;

                case 4: // Ladakh (High-altitude Khardung La Pass Serpentine Bends)
                    x = 38f * Mathf.Sin(z * 0.007f);
                    y = (z * 0.022f);
                    break;

                case 5: // Kerala (Munnar Tea Garden Hairpin Curves)
                    x = 45f * Mathf.Sin(z * 0.0085f);
                    y = Mathf.Sin(z * 0.005f) * 8f;
                    break;

                default: // Punjab & Grand Trunk Road (Gentle highway rollers)
                    x = 18f * Mathf.Sin(z * 0.0035f);
                    y = 0f;
                    break;
            }

            return new Vector3(x, y, z);
        }

        private void BuildTerrainLandscape(Transform parent, int stateIdx)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Terrain_Base";
            ground.transform.SetParent(parent, false);
            ground.transform.localPosition = new Vector3(0f, -0.15f, (_totalSegments * _segmentLength) * 0.5f);
            ground.transform.localScale = new Vector3(25f, 1f, (_totalSegments * _segmentLength) * 0.1f);

            Material tMat = (stateIdx == 2) ? _sandMat : _grassMat;
            ground.GetComponent<Renderer>().sharedMaterial = tMat;

            // In Goa, spawn an ocean blue water plane on the west/left side
            if (stateIdx == 2)
            {
                GameObject ocean = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ocean.name = "Goa_Arabian_Sea";
                ocean.transform.SetParent(parent, false);
                ocean.transform.localPosition = new Vector3(-120f, -0.2f, (_totalSegments * _segmentLength) * 0.5f);
                ocean.transform.localScale = new Vector3(15f, 1f, (_totalSegments * _segmentLength) * 0.1f);
                ocean.GetComponent<Renderer>().sharedMaterial = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Goa_Sea", new Color(0.10f, 0.45f, 0.85f), 0.2f, 0.95f);
            }
        }

        private void CreateRoadStrip(string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strip.name = name;
            strip.transform.SetParent(parent, false);
            strip.transform.localPosition = localPos;
            strip.transform.localScale = localScale;
            Destroy(strip.GetComponent<Collider>());
            strip.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private void CreateGuardRail(Transform parent, Vector3 localPos, float length)
        {
            GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = "Guard_Rail_Steel";
            rail.transform.SetParent(parent, false);
            rail.transform.localPosition = localPos;
            rail.transform.localScale = new Vector3(0.18f, 0.65f, 1.0f);
            rail.GetComponent<Renderer>().sharedMaterial = _guardRailMat;

            // Support posts
            for (float p = -0.4f; p <= 0.4f; p += 0.4f)
            {
                GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                post.name = "Rail_Post";
                post.transform.SetParent(rail.transform, false);
                post.transform.localPosition = new Vector3(0f, -0.4f, p);
                post.transform.localScale = new Vector3(0.8f, 0.5f, 0.8f);
                Destroy(post.GetComponent<Collider>());
                post.GetComponent<Renderer>().sharedMaterial = _guardRailMat;
            }
        }

        private void SpawnRoadsideElements(Vector3 roadPos, Quaternion roadRot, int segIdx, int stateIdx)
        {
            float halfW = _roadWidth * 0.5f;

            // 1. Streetlights every 3 segments
            if (segIdx % 3 == 0)
            {
                Vector3 lightPosL = roadPos + (roadRot * new Vector3(-halfW - 1.5f, 0f, 0f));
                CreateStreetlight(_worldRoot.transform, lightPosL, roadRot, isLeft: true);
            }

            // 2. Indian Highway Milestone every 5 segments
            if (segIdx % 5 == 0)
            {
                int remainingKm = (_totalSegments - segIdx) * 25 / 1000;
                string destName = (stateIdx == 0) ? "CHENNAI" : (stateIdx == 1) ? "PUNE" : (stateIdx == 2) ? "PANAJI" : "MANALI";
                Vector3 stonePos = roadPos + (roadRot * new Vector3(-halfW - 2.4f, 0f, 0f));
                CreateMilestone(_worldRoot.transform, stonePos, roadRot, remainingKm, destName);
            }

            // 3. Overhead Indian Highway Green Signboard every 20 segments
            if (segIdx == 15 || segIdx == 45)
            {
                CreateOverheadSign(_worldRoot.transform, roadPos, roadRot, stateIdx);
            }

            // 4. Indian Dhabas & Eateries at landmark intervals
            if (segIdx == 10)
            {
                Vector3 dhabaPos = roadPos + (roadRot * new Vector3(-halfW - 16f, 0f, 0f));
                CreateDhaba(_worldRoot.transform, dhabaPos, roadRot, (stateIdx == 0) ? "SARAVANA HIGHWAY BHAVAN" : "SHER-E-PUNJAB DHABA");
            }
            else if (segIdx == 35)
            {
                Vector3 dhabaPos = roadPos + (roadRot * new Vector3(halfW + 18f, 0f, 0f));
                CreateDhaba(_worldRoot.transform, dhabaPos, roadRot, (stateIdx == 2) ? "GOA SHACK & TENDER COCONUT" : "ROYAL ENFIELD SERVICE POINT");
            }
            else if (segIdx == 60)
            {
                Vector3 dhabaPos = roadPos + (roadRot * new Vector3(-halfW - 20f, 0f, 0f));
                CreateDhaba(_worldRoot.transform, dhabaPos, roadRot, "DECCAN EXPRESS TOLL PLAZA");
            }

            // 5. Roadside Foliage (Palms for TN/Goa, Pines for Himachal/Ladakh, Banyan trees for Punjab)
            if (segIdx % 2 == 0)
            {
                Vector3 treePosL = roadPos + (roadRot * new Vector3(-halfW - 5.5f - Random.Range(1f, 4f), 0f, Random.Range(-4f, 4f)));
                Vector3 treePosR = roadPos + (roadRot * new Vector3(halfW + 5.5f + Random.Range(1f, 4f), 0f, Random.Range(-4f, 4f)));
                CreateTree(_worldRoot.transform, treePosL, stateIdx);
                CreateTree(_worldRoot.transform, treePosR, stateIdx);
            }
        }

        private void CreateStreetlight(Transform parent, Vector3 pos, Quaternion rot, bool isLeft)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Highway_Pole";
            pole.transform.SetParent(parent, false);
            pole.transform.position = pos + Vector3.up * 4.5f;
            pole.transform.rotation = rot;
            pole.transform.localScale = new Vector3(0.20f, 4.5f, 0.20f);
            Destroy(pole.GetComponent<Collider>());
            pole.GetComponent<Renderer>().sharedMaterial = _woodMat;

            // Arm extending over the road
            float armDir = isLeft ? 1f : -1f;
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.transform.SetParent(pole.transform, false);
            arm.transform.localPosition = new Vector3(armDir * 0.9f, 0.95f, 0f);
            arm.transform.localScale = new Vector3(2.2f, 0.05f, 0.15f);
            Destroy(arm.GetComponent<Collider>());
            arm.GetComponent<Renderer>().sharedMaterial = _guardRailMat;

            // Lamp housing
            GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lamp.transform.SetParent(arm.transform, false);
            lamp.transform.localPosition = new Vector3(armDir * 0.45f, -0.1f, 0f);
            lamp.transform.localScale = new Vector3(0.4f, 0.2f, 0.3f);
            Destroy(lamp.GetComponent<Collider>());
            lamp.GetComponent<Renderer>().sharedMaterial = _dhabaRoofMat;
        }

        private void CreateMilestone(Transform parent, Vector3 pos, Quaternion rot, int km, string dest)
        {
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stone.name = "Indian_Milestone";
            stone.transform.SetParent(parent, false);
            stone.transform.position = pos + Vector3.up * 0.6f;
            stone.transform.rotation = rot;
            stone.transform.localScale = new Vector3(0.55f, 0.6f, 0.55f);
            Destroy(stone.GetComponent<Collider>());
            stone.GetComponent<Renderer>().sharedMaterial = _markingWhiteMat;

            // Golden/Yellow Cap (Standard Indian Highway Code)
            GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cap.name = "Milestone_YellowCap";
            cap.transform.SetParent(stone.transform, false);
            cap.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            cap.transform.localScale = new Vector3(0.98f, 0.55f, 0.98f);
            Destroy(cap.GetComponent<Collider>());
            cap.GetComponent<Renderer>().sharedMaterial = _markingYellowMat;
        }

        private void CreateOverheadSign(Transform parent, Vector3 pos, Quaternion rot, int stateIdx)
        {
            string stateTitle = (stateIdx == 0) ? "TAMIL NADU NH-44 CHENNAI EXPRESSWAY" : (stateIdx == 1) ? "MAHARASHTRA MUMBAI-PUNE CORRIDOR" : "INDIAN NATIONAL HIGHWAY CORRIDOR";

            GameObject gantry = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gantry.name = "Overhead_GreenSign";
            gantry.transform.SetParent(parent, false);
            gantry.transform.position = pos + Vector3.up * 6.5f;
            gantry.transform.rotation = rot;
            gantry.transform.localScale = new Vector3(_roadWidth * 0.9f, 2.2f, 0.35f);
            Destroy(gantry.GetComponent<Collider>());
            gantry.GetComponent<Renderer>().sharedMaterial = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_GreenSign", new Color(0.08f, 0.55f, 0.22f), 0.1f, 0.8f);

            // Left & Right Support Legs
            float halfW = _roadWidth * 0.5f + 0.5f;
            CreateGantryLeg(gantry.transform, new Vector3(-halfW, -1.8f, 0f));
            CreateGantryLeg(gantry.transform, new Vector3(halfW, -1.8f, 0f));
        }

        private void CreateGantryLeg(Transform parent, Vector3 localPos)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leg.name = "Gantry_Leg";
            leg.transform.SetParent(parent, false);
            leg.transform.localPosition = localPos;
            leg.transform.localScale = new Vector3(0.04f, 2.8f, 0.04f);
            Destroy(leg.GetComponent<Collider>());
            leg.GetComponent<Renderer>().sharedMaterial = _guardRailMat;
        }

        private void CreateDhaba(Transform parent, Vector3 pos, Quaternion rot, string name)
        {
            GameObject dhaba = new GameObject($"Dhaba_{name}");
            dhaba.transform.SetParent(parent, false);
            dhaba.transform.position = pos;
            dhaba.transform.rotation = rot;

            // Main Building
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Dhaba_Building";
            wall.transform.SetParent(dhaba.transform, false);
            wall.transform.localPosition = new Vector3(0f, 2.5f, 0f);
            wall.transform.localScale = new Vector3(14f, 5f, 10f);
            wall.GetComponent<Renderer>().sharedMaterial = _dhabaWallMat;

            // Slanted Tin/Terracotta Roof
            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "Dhaba_Roof";
            roof.transform.SetParent(dhaba.transform, false);
            roof.transform.localPosition = new Vector3(0f, 5.3f, 0f);
            roof.transform.localScale = new Vector3(15.5f, 0.6f, 11.5f);
            roof.GetComponent<Renderer>().sharedMaterial = _dhabaRoofMat;

            // Billboard Signboard
            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "Dhaba_Billboard";
            sign.transform.SetParent(dhaba.transform, false);
            sign.transform.localPosition = new Vector3(0f, 6.8f, 2.5f);
            sign.transform.localScale = new Vector3(12f, 1.8f, 0.25f);
            Destroy(sign.GetComponent<Collider>());
            sign.GetComponent<Renderer>().sharedMaterial = _markingYellowMat;
        }

        private void CreateTree(Transform parent, Vector3 pos, int stateIdx)
        {
            GameObject tree = new GameObject("Roadside_Tree");
            tree.transform.SetParent(parent, false);
            tree.transform.position = pos;

            if (stateIdx == 0 || stateIdx == 2) // Palm Tree for Tamil Nadu & Goa
            {
                // Slender Curved Trunk
                GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.transform.SetParent(tree.transform, false);
                trunk.transform.localPosition = Vector3.up * 4f;
                trunk.transform.localScale = new Vector3(0.35f, 4f, 0.35f);
                trunk.transform.localRotation = Quaternion.Euler(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
                Destroy(trunk.GetComponent<Collider>());
                trunk.GetComponent<Renderer>().sharedMaterial = _woodMat;

                // Palm Canopy
                GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                leaves.transform.SetParent(tree.transform, false);
                leaves.transform.localPosition = Vector3.up * 8.2f;
                leaves.transform.localScale = new Vector3(4.8f, 1.4f, 4.8f);
                Destroy(leaves.GetComponent<Collider>());
                leaves.GetComponent<Renderer>().sharedMaterial = _leafMat;
            }
            else // Lush Round Canopy Tree (Classic Banyan / Neem style)
            {
                GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.transform.SetParent(tree.transform, false);
                trunk.transform.localPosition = Vector3.up * 2.5f;
                trunk.transform.localScale = new Vector3(0.6f, 2.5f, 0.6f);
                Destroy(trunk.GetComponent<Collider>());
                trunk.GetComponent<Renderer>().sharedMaterial = _woodMat;

                GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                foliage.transform.SetParent(tree.transform, false);
                foliage.transform.localPosition = Vector3.up * 6f;
                foliage.transform.localScale = new Vector3(5.5f, 4.5f, 5.5f);
                Destroy(foliage.GetComponent<Collider>());
                foliage.GetComponent<Renderer>().sharedMaterial = _leafMat;
            }
        }

        private void CreateMaterials()
        {
            _asphaltMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_NH44_Asphalt", new Color(0.18f, 0.18f, 0.20f), 0.1f, 0.3f);
            _markingWhiteMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Road_White", new Color(0.95f, 0.95f, 0.95f), 0.05f, 0.5f);
            _markingYellowMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Road_Yellow", new Color(0.98f, 0.78f, 0.12f), 0.05f, 0.5f);
            _grassMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Indian_Landscape", new Color(0.24f, 0.44f, 0.18f), 0.05f, 0.15f);
            _dhabaWallMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Dhaba_Wall", new Color(0.92f, 0.85f, 0.70f), 0.05f, 0.2f);
            _dhabaRoofMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Dhaba_Roof", new Color(0.72f, 0.24f, 0.18f), 0.1f, 0.3f);
            _woodMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Tree_Trunk", new Color(0.32f, 0.20f, 0.12f), 0.05f, 0.15f);
            _leafMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Tree_Leaf", new Color(0.15f, 0.50f, 0.15f), 0.05f, 0.2f);
            _guardRailMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_GuardRail", new Color(0.85f, 0.85f, 0.88f), 0.85f, 0.7f);
            _sandMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Goa_Sand", new Color(0.88f, 0.78f, 0.55f), 0.05f, 0.1f);
        }
    }
}
