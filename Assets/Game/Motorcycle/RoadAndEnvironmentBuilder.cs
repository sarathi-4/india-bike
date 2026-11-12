using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Builds a sprawling, realistic 2-kilometer Indian Highway corridor (NH-44 / Expressway style)
    /// complete with asphalt road surface, white and yellow lane markings, distance milestone markers,
    /// highway streetlights, roadside trees, and traditional Indian Highway Dhabas.
    /// </summary>
    public class RoadAndEnvironmentBuilder : MonoBehaviour
    {
        [Header("Highway Dimensions")]
        [SerializeField] private float _roadLength = 2000f;
        [SerializeField] private float _roadWidth = 14f;
        [SerializeField] private float _segmentLength = 25f;

        private Material _asphaltMat;
        private Material _markingWhiteMat;
        private Material _markingYellowMat;
        private Material _grassMat;
        private Material _dhabaWallMat;
        private Material _dhabaRoofMat;
        private Material _woodMat;
        private Material _leafMat;

        private void Awake()
        {
            BuildHighwayWorld();
        }

        public void BuildHighwayWorld()
        {
            CreateMaterials();

            GameObject worldRoot = new GameObject("[NH44_Highway_Corridor]");
            worldRoot.transform.SetParent(transform, false);

            // 1. Endless Ground / Grass Landscape
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Terrain_Landscape";
            ground.transform.SetParent(worldRoot.transform, false);
            ground.transform.localPosition = new Vector3(0f, -0.05f, _roadLength * 0.5f);
            ground.transform.localScale = new Vector3(20f, 1f, _roadLength * 0.1f);
            ground.GetComponent<Renderer>().sharedMaterial = _grassMat;

            // 2. Asphalt Highway Road with solid collider
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "NH44_MainAsphalt_Surface";
            road.transform.SetParent(worldRoot.transform, false);
            road.transform.localPosition = new Vector3(0f, 0f, _roadLength * 0.5f);
            road.transform.localScale = new Vector3(_roadWidth, 0.2f, _roadLength);
            road.GetComponent<Renderer>().sharedMaterial = _asphaltMat;

            // Ensure road has BoxCollider for motorcycle tires
            var col = road.GetComponent<BoxCollider>();
            if (col == null) col = road.AddComponent<BoxCollider>();

            // 3. Yellow Shoulder Lines (Left & Right edges)
            float halfWidth = _roadWidth * 0.5f - 0.4f;
            CreateRoadLine("Shoulder_Yellow_L", worldRoot.transform, new Vector3(-halfWidth, 0.12f, _roadLength * 0.5f),
                new Vector3(0.25f, 0.02f, _roadLength), _markingYellowMat);

            CreateRoadLine("Shoulder_Yellow_R", worldRoot.transform, new Vector3(halfWidth, 0.12f, _roadLength * 0.5f),
                new Vector3(0.25f, 0.02f, _roadLength), _markingYellowMat);

            // 4. White Center Dashed Markings
            float dashLen = 6f;
            float gapLen = 6f;
            float step = dashLen + gapLen;
            for (float z = 5f; z < _roadLength; z += step)
            {
                CreateRoadLine($"Dash_White_{z:0}", worldRoot.transform, new Vector3(0f, 0.12f, z + dashLen * 0.5f),
                    new Vector3(0.25f, 0.02f, dashLen), _markingWhiteMat);
            }

            // 5. Roadside Highway Streetlights & Milestone Pillars
            for (float z = 30f; z < _roadLength; z += 60f)
            {
                CreateStreetlight(worldRoot.transform, new Vector3(-halfWidth - 1.2f, 0f, z), isLeft: true);
                CreateStreetlight(worldRoot.transform, new Vector3(halfWidth + 1.2f, 0f, z + 30f), isLeft: false);

                // Indian Highway Distance Milestone every 120 meters
                if (((int)z % 120) == 0)
                {
                    CreateMilestone(worldRoot.transform, new Vector3(-halfWidth - 2.5f, 0f, z), (int)(_roadLength - z));
                }
            }

            // 6. Roadside Indian Dhabas (Eateries & Fuel Stops)
            CreateDhaba(worldRoot.transform, new Vector3(-halfWidth - 16f, 0f, 180f), "SHER-E-PUNJAB DHABA");
            CreateDhaba(worldRoot.transform, new Vector3(halfWidth + 16f, 0f, 420f), "CHAI SHAI EXPRESS POINT");
            CreateDhaba(worldRoot.transform, new Vector3(-halfWidth - 16f, 0f, 750f), "HIGHWAY KING RESTAURANT");
            CreateDhaba(worldRoot.transform, new Vector3(halfWidth + 16f, 0f, 1150f), "ROYAL ENFIELD SERVICE & PUNCTURE");
            CreateDhaba(worldRoot.transform, new Vector3(-halfWidth - 16f, 0f, 1600f), "WELCOME TO DECCAN TOLL PLAZA");

            // 7. Roadside Trees & Lush Palms
            for (float z = 20f; z < _roadLength; z += 35f)
            {
                CreateTree(worldRoot.transform, new Vector3(-halfWidth - 4.5f - Random.Range(0f, 6f), 0f, z));
                CreateTree(worldRoot.transform, new Vector3(halfWidth + 4.5f + Random.Range(0f, 6f), 0f, z + 17f));
            }
        }

        private void CreateRoadLine(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = name;
            line.transform.SetParent(parent, false);
            line.transform.localPosition = pos;
            line.transform.localScale = scale;
            Destroy(line.GetComponent<Collider>());
            line.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private void CreateStreetlight(Transform parent, Vector3 pos, bool isLeft)
        {
            GameObject lightPost = new GameObject("Streetlight");
            lightPost.transform.SetParent(parent, false);
            lightPost.transform.localPosition = pos;

            // Pole
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.SetParent(lightPost.transform, false);
            pole.transform.localPosition = new Vector3(0f, 3.5f, 0f);
            pole.transform.localScale = new Vector3(0.18f, 3.5f, 0.18f);
            Destroy(pole.GetComponent<Collider>());
            pole.GetComponent<Renderer>().sharedMaterial = _woodMat;

            // Overhanging Arm
            float armDir = isLeft ? 1f : -1f;
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.transform.SetParent(lightPost.transform, false);
            arm.transform.localPosition = new Vector3(armDir * 0.9f, 6.8f, 0f);
            arm.transform.localScale = new Vector3(1.8f, 0.14f, 0.14f);
            Destroy(arm.GetComponent<Collider>());
            arm.GetComponent<Renderer>().sharedMaterial = _woodMat;

            // Lamp Housing
            GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lamp.transform.SetParent(lightPost.transform, false);
            lamp.transform.localPosition = new Vector3(armDir * 1.6f, 6.6f, 0f);
            lamp.transform.localScale = new Vector3(0.45f, 0.25f, 0.45f);
            Destroy(lamp.GetComponent<Collider>());
            lamp.GetComponent<Renderer>().sharedMaterial = _markingYellowMat;
        }

        private void CreateMilestone(Transform parent, Vector3 pos, int distanceKm)
        {
            GameObject stone = new GameObject($"Milestone_NH44_{distanceKm}km");
            stone.transform.SetParent(parent, false);
            stone.transform.localPosition = pos;

            // White Base
            GameObject baseBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseBlock.transform.SetParent(stone.transform, false);
            baseBlock.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            baseBlock.transform.localScale = new Vector3(0.55f, 0.9f, 0.35f);
            Destroy(baseBlock.GetComponent<Collider>());
            baseBlock.GetComponent<Renderer>().sharedMaterial = _markingWhiteMat;

            // Yellow Curved Cap (Indian NH standard)
            GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cap.transform.SetParent(stone.transform, false);
            cap.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            cap.transform.localScale = new Vector3(0.55f, 0.18f, 0.35f);
            Destroy(cap.GetComponent<Collider>());
            cap.GetComponent<Renderer>().sharedMaterial = _markingYellowMat;
        }

        private void CreateDhaba(Transform parent, Vector3 pos, string title)
        {
            GameObject dhaba = new GameObject("Indian_Highway_Dhaba");
            dhaba.transform.SetParent(parent, false);
            dhaba.transform.localPosition = pos;

            // Main Building
            GameObject mainHall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mainHall.transform.SetParent(dhaba.transform, false);
            mainHall.transform.localPosition = new Vector3(0f, 2.5f, 0f);
            mainHall.transform.localScale = new Vector3(14f, 5f, 18f);
            mainHall.GetComponent<Renderer>().sharedMaterial = _dhabaWallMat;

            // Slanted Tin Roof
            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.transform.SetParent(dhaba.transform, false);
            roof.transform.localPosition = new Vector3(0f, 5.3f, 0f);
            roof.transform.localScale = new Vector3(16f, 0.4f, 20f);
            roof.GetComponent<Renderer>().sharedMaterial = _dhabaRoofMat;

            // Outdoor Dining Canopy / Verandah
            GameObject verandah = GameObject.CreatePrimitive(PrimitiveType.Cube);
            verandah.transform.SetParent(dhaba.transform, false);
            verandah.transform.localPosition = new Vector3(pos.x < 0 ? 8f : -8f, 1.8f, 0f);
            verandah.transform.localScale = new Vector3(6f, 0.2f, 14f);
            verandah.GetComponent<Renderer>().sharedMaterial = _dhabaRoofMat;

            // Signboard Billboard
            GameObject billboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            billboard.name = $"Billboard_{title}";
            billboard.transform.SetParent(dhaba.transform, false);
            billboard.transform.localPosition = new Vector3(pos.x < 0 ? 7f : -7f, 4.8f, 0f);
            billboard.transform.localScale = new Vector3(0.3f, 1.6f, 10f);
            billboard.GetComponent<Renderer>().sharedMaterial = _markingYellowMat;
        }

        private void CreateTree(Transform parent, Vector3 pos)
        {
            GameObject tree = new GameObject("Roadside_Tree");
            tree.transform.SetParent(parent, false);
            tree.transform.localPosition = pos;

            // Trunk
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform, false);
            trunk.transform.localPosition = new Vector3(0f, 2.5f, 0f);
            trunk.transform.localScale = new Vector3(0.5f, 2.5f, 0.5f);
            Destroy(trunk.GetComponent<Collider>());
            trunk.GetComponent<Renderer>().sharedMaterial = _woodMat;

            // Foliage (stacked leafy spheres)
            GameObject foliage1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage1.transform.SetParent(tree.transform, false);
            foliage1.transform.localPosition = new Vector3(0f, 5.2f, 0f);
            foliage1.transform.localScale = new Vector3(3.5f, 2.8f, 3.5f);
            Destroy(foliage1.GetComponent<Collider>());
            foliage1.GetComponent<Renderer>().sharedMaterial = _leafMat;

            GameObject foliage2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage2.transform.SetParent(tree.transform, false);
            foliage2.transform.localPosition = new Vector3(0.4f, 6.6f, -0.2f);
            foliage2.transform.localScale = new Vector3(2.6f, 2.2f, 2.6f);
            Destroy(foliage2.GetComponent<Collider>());
            foliage2.GetComponent<Renderer>().sharedMaterial = _leafMat;
        }

        private void CreateMaterials()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            _asphaltMat = new Material(shader) { name = "Mat_NH44_Asphalt", color = new Color(0.18f, 0.18f, 0.20f) };
            if (_asphaltMat.HasProperty("_Smoothness")) _asphaltMat.SetFloat("_Smoothness", 0.15f);

            _markingWhiteMat = new Material(shader) { name = "Mat_RoadMarking_White", color = new Color(0.95f, 0.95f, 0.95f) };
            _markingYellowMat = new Material(shader) { name = "Mat_RoadMarking_Yellow", color = new Color(0.98f, 0.78f, 0.12f) };

            _grassMat = new Material(shader) { name = "Mat_Roadside_Grass", color = new Color(0.28f, 0.42f, 0.22f) };
            _dhabaWallMat = new Material(shader) { name = "Mat_Dhaba_Wall", color = new Color(0.92f, 0.82f, 0.65f) };
            _dhabaRoofMat = new Material(shader) { name = "Mat_Dhaba_Roof", color = new Color(0.68f, 0.22f, 0.18f) };
            _woodMat = new Material(shader) { name = "Mat_Wood_Trunk", color = new Color(0.35f, 0.22f, 0.14f) };
            _leafMat = new Material(shader) { name = "Mat_Tree_Leaf", color = new Color(0.18f, 0.48f, 0.18f) };
        }
    }
}
