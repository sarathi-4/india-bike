using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Procedurally constructs a high-quality 3D Indian Cruiser motorcycle model
    /// (Classic Royal Enfield cruiser profile: teardrop tank, chrome exhaust, twin wheels,
    /// front fork, round classic headlight, leather dual seat, rider & pillion passenger).
    /// </summary>
    public class MotorcycleVisualBuilder : MonoBehaviour
    {
        [Header("Motorcycle Colors & Styling")]
        [SerializeField] private Color _bodyPaintColor = new Color(0.72f, 0.08f, 0.12f); // Royal Indian Maroon
        [SerializeField] private Color _chromeColor = new Color(0.92f, 0.92f, 0.95f);    // Brilliant Chrome
        [SerializeField] private Color _engineColor = new Color(0.22f, 0.22f, 0.24f);    // Cast Iron / Steel
        [SerializeField] private Color _rubberColor = new Color(0.12f, 0.12f, 0.12f);    // Vulcanized Rubber
        [SerializeField] private Color _seatColor = new Color(0.15f, 0.12f, 0.10f);      // Distressed Brown/Black Leather

        private Material _bodyMat;
        private Material _chromeMat;
        private Material _engineMat;
        private Material _rubberMat;
        private Material _seatMat;
        private Material _lightMat;

        private GameObject _passengerVisual;

        public GameObject PassengerVisual => _passengerVisual;

        private void Awake()
        {
            BuildMotorcycle();
        }

        public void BuildMotorcycle()
        {
            CreateMaterials();

            var controller = GetComponent<PlayerMotorcycleController>();

            // Root visual container (receives cornering lean roll)
            GameObject visualRoot = new GameObject("VisualChassis_Root");
            visualRoot.transform.SetParent(transform, false);
            visualRoot.transform.localPosition = Vector3.zero;
            visualRoot.transform.localRotation = Quaternion.identity;

            // 1. Frame / Chassis (Tubular steel backbone)
            GameObject frame = CreatePrimitive(PrimitiveType.Cube, "Chassis_Frame", visualRoot.transform,
                new Vector3(0f, 0.45f, 0f), new Vector3(0.22f, 0.25f, 1.4f), _engineMat);

            // 2. Engine Block & Transmission Case
            GameObject engine = CreatePrimitive(PrimitiveType.Cube, "Engine_Block", visualRoot.transform,
                new Vector3(0f, 0.38f, -0.05f), new Vector3(0.38f, 0.36f, 0.45f), _chromeMat);
            
            // Engine Cylinders with cooling fins
            CreatePrimitive(PrimitiveType.Cylinder, "Engine_Cylinder", visualRoot.transform,
                new Vector3(0f, 0.60f, 0.05f), new Vector3(0.26f, 0.18f, 0.26f), _engineMat);

            // 3. Exhaust Pipes (Swept chrome exhaust on right side)
            GameObject exhaust = CreatePrimitive(PrimitiveType.Cylinder, "Exhaust_Pipe", visualRoot.transform,
                new Vector3(0.22f, 0.22f, -0.35f), new Vector3(0.08f, 0.55f, 0.08f), _chromeMat);
            exhaust.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // 4. Iconic Teardrop Fuel Tank (Royal Indian Cruiser styling)
            GameObject tank = CreatePrimitive(PrimitiveType.Sphere, "Fuel_Tank", visualRoot.transform,
                new Vector3(0f, 0.82f, 0.22f), new Vector3(0.42f, 0.38f, 0.72f), _bodyMat);

            // 5. Dual Stepped Saddle (Rider Seat + Pillion Passenger Seat)
            GameObject riderSeat = CreatePrimitive(PrimitiveType.Cube, "Rider_Seat", visualRoot.transform,
                new Vector3(0f, 0.75f, -0.25f), new Vector3(0.36f, 0.12f, 0.42f), _seatMat);

            GameObject pillionSeat = CreatePrimitive(PrimitiveType.Cube, "Pillion_Seat", visualRoot.transform,
                new Vector3(0f, 0.82f, -0.65f), new Vector3(0.30f, 0.12f, 0.38f), _seatMat);

            // Pillion Grab Rail
            CreatePrimitive(PrimitiveType.Cube, "Pillion_GrabRail", visualRoot.transform,
                new Vector3(0f, 0.89f, -0.86f), new Vector3(0.28f, 0.05f, 0.05f), _chromeMat);

            // 6. Rear Mudguard / Fender
            CreatePrimitive(PrimitiveType.Cylinder, "Rear_Fender", visualRoot.transform,
                new Vector3(0f, 0.55f, -0.75f), new Vector3(0.32f, 0.22f, 0.58f), _bodyMat);

            // 7. Rear Taillight
            GameObject taillight = CreatePrimitive(PrimitiveType.Cylinder, "Rear_Taillight", visualRoot.transform,
                new Vector3(0f, 0.72f, -0.96f), new Vector3(0.12f, 0.05f, 0.12f), _lightMat);
            taillight.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // 8. Rear Wheel & Swingarm
            GameObject rearWheelRoot = new GameObject("RearWheel_Anchor");
            rearWheelRoot.transform.SetParent(visualRoot.transform, false);
            rearWheelRoot.transform.localPosition = new Vector3(0f, 0.35f, -0.75f);

            GameObject rTyre = CreatePrimitive(PrimitiveType.Cylinder, "Rear_Tyre", rearWheelRoot.transform,
                Vector3.zero, new Vector3(0.68f, 0.16f, 0.68f), _rubberMat);
            rTyre.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            GameObject rRim = CreatePrimitive(PrimitiveType.Cylinder, "Rear_Rim", rearWheelRoot.transform,
                Vector3.zero, new Vector3(0.48f, 0.17f, 0.48f), _chromeMat);
            rRim.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            // 9. Front Fork & Steering Assembly (Rotates with steer input)
            GameObject frontForkRoot = new GameObject("FrontFork_Root");
            frontForkRoot.transform.SetParent(visualRoot.transform, false);
            frontForkRoot.transform.localPosition = new Vector3(0f, 0.65f, 0.65f);
            frontForkRoot.transform.localRotation = Quaternion.identity;

            // Chrome Telescopic Fork Legs
            CreatePrimitive(PrimitiveType.Cylinder, "Fork_Left", frontForkRoot.transform,
                new Vector3(-0.14f, -0.25f, 0.12f), new Vector3(0.05f, 0.42f, 0.05f), _chromeMat)
                .transform.localRotation = Quaternion.Euler(18f, 0f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "Fork_Right", frontForkRoot.transform,
                new Vector3(0.14f, -0.25f, 0.12f), new Vector3(0.05f, 0.42f, 0.05f), _chromeMat)
                .transform.localRotation = Quaternion.Euler(18f, 0f, 0f);

            // Front Mudguard / Fender
            CreatePrimitive(PrimitiveType.Cylinder, "Front_Fender", frontForkRoot.transform,
                new Vector3(0f, -0.26f, 0.22f), new Vector3(0.28f, 0.18f, 0.52f), _bodyMat);

            // Wide Chrome Handlebars
            GameObject handlebar = CreatePrimitive(PrimitiveType.Cylinder, "Handlebars", frontForkRoot.transform,
                new Vector3(0f, 0.32f, 0.05f), new Vector3(0.04f, 0.46f, 0.04f), _chromeMat);
            handlebar.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            // Rubber Grips
            CreatePrimitive(PrimitiveType.Cylinder, "Grip_L", handlebar.transform,
                new Vector3(0f, 0.42f, 0f), new Vector3(1.4f, 0.12f, 1.4f), _rubberMat);
            CreatePrimitive(PrimitiveType.Cylinder, "Grip_R", handlebar.transform,
                new Vector3(0f, -0.42f, 0f), new Vector3(1.4f, 0.12f, 1.4f), _rubberMat);

            // Classic Round Headlight
            GameObject headlightHousing = CreatePrimitive(PrimitiveType.Sphere, "Headlight_Housing", frontForkRoot.transform,
                new Vector3(0f, 0.18f, 0.25f), new Vector3(0.24f, 0.24f, 0.22f), _chromeMat);

            // Real Spotlight Beam
            GameObject lightObj = new GameObject("Headlight_Spotlight");
            lightObj.transform.SetParent(frontForkRoot.transform, false);
            lightObj.transform.localPosition = new Vector3(0f, 0.18f, 0.36f);
            lightObj.transform.localRotation = Quaternion.Euler(5f, 0f, 0f);

            Light spotLight = lightObj.AddComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.color = new Color(1f, 0.98f, 0.88f); // Warm halogen beam
            spotLight.intensity = 3.5f;
            spotLight.range = 55f;
            spotLight.spotAngle = 45f;

            // Front Wheel (attached to front fork, spins with bike motion)
            GameObject frontWheelRoot = new GameObject("FrontWheel_Anchor");
            frontWheelRoot.transform.SetParent(frontForkRoot.transform, false);
            frontWheelRoot.transform.localPosition = new Vector3(0f, -0.30f, 0.22f);

            GameObject fTyre = CreatePrimitive(PrimitiveType.Cylinder, "Front_Tyre", frontWheelRoot.transform,
                Vector3.zero, new Vector3(0.68f, 0.14f, 0.68f), _rubberMat);
            fTyre.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            GameObject fRim = CreatePrimitive(PrimitiveType.Cylinder, "Front_Rim", frontWheelRoot.transform,
                Vector3.zero, new Vector3(0.50f, 0.15f, 0.50f), _chromeMat);
            fRim.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            // 10. Rider 3D Character (Helmet, Jacket, Seated posture)
            CreateRiderCharacter(visualRoot.transform);

            // 11. Pillion Passenger Character (Can be toggled)
            _passengerVisual = CreatePassengerCharacter(visualRoot.transform);

            // 12. Bind Anchors into PlayerMotorcycleController
            if (controller != null)
            {
                controller.BindVisualAnchors(
                    visualRoot.transform,
                    frontForkRoot.transform,
                    frontWheelRoot.transform,
                    rearWheelRoot.transform,
                    spotLight,
                    pillionSeat.transform
                );
            }
        }

        private void CreateRiderCharacter(Transform parent)
        {
            Material jacketMat = CreateStandardMaterial("Rider_Jacket", new Color(0.18f, 0.22f, 0.28f)); // Dark Navy Riding Jacket
            Material helmetMat = CreateStandardMaterial("Rider_Helmet", new Color(0.95f, 0.75f, 0.15f)); // Golden Yellow Full-Face Helmet
            Material visorMat = CreateStandardMaterial("Rider_Visor", new Color(0.08f, 0.08f, 0.10f));   // Tinted Black Visor

            GameObject riderRoot = new GameObject("Rider_Character");
            riderRoot.transform.SetParent(parent, false);

            // Torso (leaning slightly forward into handlebars)
            GameObject torso = CreatePrimitive(PrimitiveType.Cube, "Rider_Torso", riderRoot.transform,
                new Vector3(0f, 1.15f, -0.22f), new Vector3(0.38f, 0.48f, 0.26f), jacketMat);
            torso.transform.localRotation = Quaternion.Euler(14f, 0f, 0f);

            // Head & Helmet
            GameObject helmet = CreatePrimitive(PrimitiveType.Sphere, "Rider_Helmet", riderRoot.transform,
                new Vector3(0f, 1.52f, -0.12f), new Vector3(0.28f, 0.32f, 0.30f), helmetMat);

            // Helmet Visor
            CreatePrimitive(PrimitiveType.Cube, "Rider_Visor", helmet.transform,
                new Vector3(0f, 0.04f, 0.42f), new Vector3(0.72f, 0.32f, 0.32f), visorMat);

            // Arms extending to handlebars
            CreatePrimitive(PrimitiveType.Cylinder, "Rider_Arm_L", riderRoot.transform,
                new Vector3(-0.25f, 1.15f, 0.12f), new Vector3(0.09f, 0.35f, 0.09f), jacketMat)
                .transform.localRotation = Quaternion.Euler(45f, -22f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "Rider_Arm_R", riderRoot.transform,
                new Vector3(0.25f, 1.15f, 0.12f), new Vector3(0.09f, 0.35f, 0.09f), jacketMat)
                .transform.localRotation = Quaternion.Euler(45f, 22f, 0f);
        }

        private GameObject CreatePassengerCharacter(Transform parent)
        {
            Material dressMat = CreateStandardMaterial("Passenger_Attire", new Color(0.85f, 0.25f, 0.45f)); // Indian Festive Magenta/Pink
            Material pHelmetMat = CreateStandardMaterial("Passenger_Helmet", new Color(0.92f, 0.92f, 0.95f)); // White Gloss Helmet

            GameObject pRoot = new GameObject("Passenger_Character");
            pRoot.transform.SetParent(parent, false);

            // Torso (seated upright on pillion seat)
            CreatePrimitive(PrimitiveType.Cube, "Passenger_Torso", pRoot.transform,
                new Vector3(0f, 1.25f, -0.65f), new Vector3(0.34f, 0.44f, 0.24f), dressMat);

            // Helmet
            CreatePrimitive(PrimitiveType.Sphere, "Passenger_Helmet", pRoot.transform,
                new Vector3(0f, 1.58f, -0.65f), new Vector3(0.26f, 0.30f, 0.28f), pHelmetMat);

            // Arms holding rider's waist
            CreatePrimitive(PrimitiveType.Cylinder, "Passenger_Arm_L", pRoot.transform,
                new Vector3(-0.20f, 1.18f, -0.45f), new Vector3(0.07f, 0.28f, 0.07f), dressMat)
                .transform.localRotation = Quaternion.Euler(35f, -15f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "Passenger_Arm_R", pRoot.transform,
                new Vector3(0.20f, 1.18f, -0.45f), new Vector3(0.07f, 0.28f, 0.07f), dressMat)
                .transform.localRotation = Quaternion.Euler(35f, 15f, 0f);

            return pRoot;
        }

        public void SetPassengerVisible(bool visible)
        {
            if (_passengerVisual != null) _passengerVisual.SetActive(visible);
        }

        private GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = localScale;

            // Remove default collider on visual primitives so they don't mess up bike physics
            var collider = obj.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null && mat != null)
            {
                renderer.sharedMaterial = mat;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }

            return obj;
        }

        private void CreateMaterials()
        {
            _bodyMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Body", _bodyPaintColor, 0.7f, 0.6f);
            _chromeMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Chrome", _chromeColor, 0.95f, 0.9f);
            _engineMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Engine", _engineColor, 0.5f, 0.3f);
            _rubberMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Rubber", _rubberColor, 0.05f, 0.1f);
            _seatMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Leather", _seatColor, 0.1f, 0.2f);

            _lightMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Taillight", Color.red, 0.1f, 0.8f);
            _lightMat.EnableKeyword("_EMISSION");
            if (_lightMat.HasProperty("_EmissionColor")) _lightMat.SetColor("_EmissionColor", Color.red * 2.5f);
        }

        private Material CreateStandardMaterial(string name, Color color, float metallic = 0.5f, float smoothness = 0.5f)
        {
            return IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial(name, color, metallic, smoothness);
        }
    }
}

