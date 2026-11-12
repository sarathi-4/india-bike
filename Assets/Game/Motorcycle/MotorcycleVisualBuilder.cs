using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Procedurally constructs and morphs authentic 3D Indian Motorcycles:
    /// 0. Yamaha R15 V4 (Aerodynamic Supersport, Golden USD Forks, Split Racing Tail)
    /// 1. Royal Enfield Classic 350 (Teardrop Tank, Round Chrome Peak Headlamp, Sprung Saddle, Long Peashooter Exhaust)
    /// 2. Royal Enfield Bullet 350 (Heavy Cast Engine, Golden Pinstripes, Classic Heavy Cruiser Stance)
    /// 3. KTM 390 Duke ABS (Aggressive Split-LED Predator Face, Neon Orange Trellis Frame & Alloy Rims)
    /// 4. Royal Enfield Himalayan (Tall Adventure Windscreen, Tank Exo-Cages, High Beak, Upswept Off-road Exhaust)
    /// 5. Bajaj Pulsar 220F (Iconic Aerodynamic Half-Fairing, Twin Projectors, Clip-ons, Twin Blade Taillight)
    /// 
    /// Also supports Male/Female Rider Avatar customization, Helmets, Attire, and Standing/Riding poses.
    /// </summary>
    public class MotorcycleVisualBuilder : MonoBehaviour
    {
        [Header("Active Model & Styling")]
        [SerializeField] private int _currentModelIndex = 1; // Default to Royal Enfield Classic 350
        [SerializeField] private Color _bodyPaintColor = new Color(0.85f, 0.12f, 0.15f); // Racing Red
        [SerializeField] private Color _chromeColor = new Color(0.92f, 0.92f, 0.95f);
        [SerializeField] private Color _engineColor = new Color(0.20f, 0.20f, 0.22f);
        [SerializeField] private Color _rubberColor = new Color(0.12f, 0.12f, 0.12f);
        [SerializeField] private Color _seatColor = new Color(0.15f, 0.12f, 0.10f);
        [SerializeField] private Color _trellisColor = new Color(1.0f, 0.40f, 0.05f); // KTM Orange

        [Header("Avatar Customization")]
        [SerializeField] private bool _isFemaleAvatar = false;
        [SerializeField] private Color _jacketColor = new Color(0.14f, 0.18f, 0.24f); // Dark Biker Leather
        [SerializeField] private Color _helmetColor = new Color(1.0f, 0.76f, 0.08f);  // Saffron Yellow
        [SerializeField] private int _helmetStyle = 0; // 0=Full Face, 1=Classic Open, 2=Motocross Visor
        [SerializeField] private bool _isStandingInMenu = false;

        private Material _bodyMat;
        private Material _chromeMat;
        private Material _engineMat;
        private Material _rubberMat;
        private Material _seatMat;
        private Material _lightMat;
        private Material _goldForkMat;
        private Material _trellisMat;
        private Material _windshieldMat;

        private Material _riderJacketMat;
        private Material _riderPantsMat;
        private Material _riderHelmetMat;
        private Material _riderVisorMat;
        private Material _riderSkinMat;

        private GameObject _passengerVisual;
        private GameObject _riderVisual;
        private GameObject _currentChassisRoot;

        public GameObject PassengerVisual => _passengerVisual;
        public GameObject RiderVisual => _riderVisual;
        public int CurrentModelIndex => _currentModelIndex;

        private void Awake()
        {
            BuildMotorcycleModel(_currentModelIndex);
        }

        public void BuildMotorcycleModel(int modelIndex)
        {
            _currentModelIndex = Mathf.Clamp(modelIndex, 0, 5);
            CreateMaterials();

            // Destroy previous visual mesh if present
            var existingChassis = transform.Find("VisualChassis_Root");
            if (existingChassis != null)
            {
                DestroyImmediate(existingChassis.gameObject);
            }

            var controller = GetComponent<PlayerMotorcycleController>();

            // Root visual container (receives cornering lean roll)
            _currentChassisRoot = new GameObject("VisualChassis_Root");
            _currentChassisRoot.transform.SetParent(transform, false);
            _currentChassisRoot.transform.localPosition = Vector3.zero;
            _currentChassisRoot.transform.localRotation = Quaternion.identity;

            // 1. Core Frame & Engine Chassis
            BuildChassisAndEngine(_currentChassisRoot.transform, _currentModelIndex);

            // 2. Wheels & Swingarms
            Transform rearWheelAnchor = BuildRearWheelAssembly(_currentChassisRoot.transform, _currentModelIndex);

            // 3. Front Steering & Fork Assembly
            Transform frontForkRoot = BuildFrontForkAssembly(_currentChassisRoot.transform, _currentModelIndex, out Transform frontWheelAnchor, out Light headlightLight);

            // 4. Model-Specific Tank, Fairing & Styling
            Transform seatAnchor = BuildBodyworkAndTank(_currentChassisRoot.transform, frontForkRoot, _currentModelIndex);

            // 5. Rider & Passenger 3D Characters
            CreateRiderCharacter(_currentChassisRoot.transform, _currentModelIndex);
            _passengerVisual = CreatePassengerCharacter(_currentChassisRoot.transform);

            // 6. Bind Anchors to Controller
            if (controller != null)
            {
                controller.BindVisualAnchors(
                    _currentChassisRoot.transform,
                    frontForkRoot,
                    frontWheelAnchor,
                    rearWheelAnchor,
                    headlightLight,
                    seatAnchor
                );
            }
        }

        // =========================================================================
        // CHASSIS & ENGINE BUILDER
        // =========================================================================
        private void BuildChassisAndEngine(Transform root, int modelIdx)
        {
            Material frameMat = (modelIdx == 3) ? _trellisMat : _engineMat; // KTM uses Orange Trellis

            // Main Backbone
            CreatePrimitive(PrimitiveType.Cube, "Chassis_Backbone", root,
                new Vector3(0f, 0.45f, 0f), new Vector3(0.20f, 0.22f, 1.45f), frameMat);

            // KTM Exposed Trellis Subframe Bars
            if (modelIdx == 3)
            {
                CreatePrimitive(PrimitiveType.Cylinder, "Trellis_Bar_L", root,
                    new Vector3(-0.16f, 0.52f, 0.05f), new Vector3(0.04f, 0.38f, 0.04f), _trellisMat)
                    .transform.localRotation = Quaternion.Euler(35f, 0f, 25f);
                CreatePrimitive(PrimitiveType.Cylinder, "Trellis_Bar_R", root,
                    new Vector3(0.16f, 0.52f, 0.05f), new Vector3(0.04f, 0.38f, 0.04f), _trellisMat)
                    .transform.localRotation = Quaternion.Euler(35f, 0f, -25f);
            }

            // Himalayan Crash Guard Exo-Cage
            if (modelIdx == 4)
            {
                CreatePrimitive(PrimitiveType.Cylinder, "CrashGuard_L", root,
                    new Vector3(-0.25f, 0.65f, 0.35f), new Vector3(0.035f, 0.32f, 0.035f), _engineMat);
                CreatePrimitive(PrimitiveType.Cylinder, "CrashGuard_R", root,
                    new Vector3(0.25f, 0.65f, 0.35f), new Vector3(0.035f, 0.32f, 0.035f), _engineMat);
            }

            // Engine Block
            Material engBlockMat = (modelIdx == 2) ? _engineMat : _chromeMat; // Bullet uses black cast-iron
            CreatePrimitive(PrimitiveType.Cube, "Engine_Crankcase", root,
                new Vector3(0f, 0.36f, -0.05f), new Vector3(0.36f, 0.34f, 0.44f), engBlockMat);

            // Cylinder Head with Cooling Fins
            CreatePrimitive(PrimitiveType.Cylinder, "Engine_Cylinder", root,
                new Vector3(0f, 0.58f, 0.05f), new Vector3(0.25f, 0.18f, 0.25f), _engineMat);

            // Exhaust Silencer Styling
            BuildExhaust(root, modelIdx);
        }

        private void BuildExhaust(Transform root, int modelIdx)
        {
            if (modelIdx == 0) // Yamaha R15: Compact Angled Sports Muffler
            {
                GameObject r15Exhaust = CreatePrimitive(PrimitiveType.Cylinder, "Exhaust_R15", root,
                    new Vector3(0.20f, 0.25f, -0.42f), new Vector3(0.11f, 0.32f, 0.11f), _engineMat);
                r15Exhaust.transform.localRotation = Quaternion.Euler(68f, 15f, 0f);
            }
            else if (modelIdx == 3) // KTM Duke: Underbelly Shorty Exhaust
            {
                CreatePrimitive(PrimitiveType.Cube, "Exhaust_Duke_Underbelly", root,
                    new Vector3(0.12f, 0.18f, -0.15f), new Vector3(0.18f, 0.12f, 0.28f), _engineMat);
            }
            else if (modelIdx == 4) // Himalayan: Upswept 45-degree High Exhaust
            {
                GameObject himExhaust = CreatePrimitive(PrimitiveType.Cylinder, "Exhaust_Himalayan", root,
                    new Vector3(0.22f, 0.45f, -0.65f), new Vector3(0.10f, 0.48f, 0.10f), _chromeMat);
                himExhaust.transform.localRotation = Quaternion.Euler(55f, 0f, 0f);
            }
            else // Classic / Bullet / Pulsar: Long Peashooter Chrome Silencer
            {
                GameObject classicExhaust = CreatePrimitive(PrimitiveType.Cylinder, "Exhaust_Chrome", root,
                    new Vector3(0.22f, 0.24f, -0.45f), new Vector3(0.08f, 0.65f, 0.08f), _chromeMat);
                classicExhaust.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }
        }

        // =========================================================================
        // REAR WHEEL ASSEMBLY
        // =========================================================================
        private Transform BuildRearWheelAssembly(Transform root, int modelIdx)
        {
            GameObject rearAnchor = new GameObject("RearWheel_Anchor");
            rearAnchor.transform.SetParent(root, false);
            rearAnchor.transform.localPosition = new Vector3(0f, 0.35f, -0.75f);

            // Tyre
            GameObject rTyre = CreatePrimitive(PrimitiveType.Cylinder, "Rear_Tyre", rearAnchor.transform,
                Vector3.zero, new Vector3(0.68f, 0.16f, 0.68f), _rubberMat);
            rTyre.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            // Rim (KTM uses bright orange rims)
            Material rimMat = (modelIdx == 3) ? _trellisMat : _chromeMat;
            GameObject rRim = CreatePrimitive(PrimitiveType.Cylinder, "Rear_Rim", rearAnchor.transform,
                Vector3.zero, new Vector3(0.48f, 0.17f, 0.48f), rimMat);
            rRim.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            // Rear Fender
            CreatePrimitive(PrimitiveType.Cylinder, "Rear_Fender", root,
                new Vector3(0f, 0.56f, -0.75f), new Vector3(0.30f, 0.20f, 0.55f), _bodyMat);

            // Taillight
            GameObject taillight = CreatePrimitive(PrimitiveType.Cylinder, "Rear_Taillight", root,
                new Vector3(0f, 0.70f, -0.96f), new Vector3(0.12f, 0.04f, 0.12f), _lightMat);
            taillight.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            return rearAnchor.transform;
        }

        // =========================================================================
        // FRONT FORK & STEERING ASSEMBLY
        // =========================================================================
        private Transform BuildFrontForkAssembly(Transform root, int modelIdx, out Transform frontWheelAnchor, out Light headlightLight)
        {
            GameObject forkRoot = new GameObject("FrontFork_Root");
            forkRoot.transform.SetParent(root, false);
            forkRoot.transform.localPosition = new Vector3(0f, 0.65f, 0.65f);
            forkRoot.transform.localRotation = Quaternion.identity;

            // Fork Material (R15 uses golden USD forks)
            Material forkMat = (modelIdx == 0) ? _goldForkMat : _chromeMat;

            // Twin Telescopic Forks
            CreatePrimitive(PrimitiveType.Cylinder, "Fork_L", forkRoot.transform,
                new Vector3(-0.14f, -0.25f, 0.12f), new Vector3(0.05f, 0.45f, 0.05f), forkMat)
                .transform.localRotation = Quaternion.Euler(18f, 0f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "Fork_R", forkRoot.transform,
                new Vector3(0.14f, -0.25f, 0.12f), new Vector3(0.05f, 0.45f, 0.05f), forkMat)
                .transform.localRotation = Quaternion.Euler(18f, 0f, 0f);

            // Front Mudguard
            Vector3 fenderPos = (modelIdx == 4) ? new Vector3(0f, 0.10f, 0.35f) : new Vector3(0f, -0.26f, 0.22f); // Himalayan has high beak
            CreatePrimitive(PrimitiveType.Cylinder, "Front_Fender", forkRoot.transform,
                fenderPos, new Vector3(0.28f, 0.16f, 0.50f), _bodyMat);

            // Handlebars
            float barAngle = (modelIdx == 0 || modelIdx == 5) ? -12f : 0f; // R15/Pulsar clip-on drop
            GameObject handlebar = CreatePrimitive(PrimitiveType.Cylinder, "Handlebars", forkRoot.transform,
                new Vector3(0f, 0.32f, 0.05f), new Vector3(0.04f, 0.46f, 0.04f), _chromeMat);
            handlebar.transform.localRotation = Quaternion.Euler(barAngle, 0f, 90f);

            // Front Wheel Anchor
            GameObject fWheelAnchor = new GameObject("FrontWheel_Anchor");
            fWheelAnchor.transform.SetParent(forkRoot.transform, false);
            fWheelAnchor.transform.localPosition = new Vector3(0f, -0.30f, 0.22f);

            GameObject fTyre = CreatePrimitive(PrimitiveType.Cylinder, "Front_Tyre", fWheelAnchor.transform,
                Vector3.zero, new Vector3(0.68f, 0.14f, 0.68f), _rubberMat);
            fTyre.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            Material rimMat = (modelIdx == 3) ? _trellisMat : _chromeMat;
            GameObject fRim = CreatePrimitive(PrimitiveType.Cylinder, "Front_Rim", fWheelAnchor.transform,
                Vector3.zero, new Vector3(0.50f, 0.15f, 0.50f), rimMat);
            fRim.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            frontWheelAnchor = fWheelAnchor.transform;

            // Headlight System
            headlightLight = BuildHeadlight(forkRoot.transform, modelIdx);

            return forkRoot.transform;
        }

        private Light BuildHeadlight(Transform fork, int modelIdx)
        {
            if (modelIdx == 0) // Yamaha R15: Twin Sleek LED Eye Slots
            {
                CreatePrimitive(PrimitiveType.Cube, "R15_Nose_Eyes", fork,
                    new Vector3(0f, 0.22f, 0.36f), new Vector3(0.28f, 0.10f, 0.18f), _bodyMat);
            }
            else if (modelIdx == 3) // KTM Duke: Split Predator LED Face
            {
                CreatePrimitive(PrimitiveType.Cube, "KTM_Split_Headlight", fork,
                    new Vector3(0f, 0.24f, 0.32f), new Vector3(0.22f, 0.28f, 0.16f), _chromeMat);
            }
            else if (modelIdx == 5) // Pulsar 220F: Half-Fairing with Twin Projector Domes
            {
                CreatePrimitive(PrimitiveType.Cube, "Pulsar_Fairing_Nose", fork,
                    new Vector3(0f, 0.26f, 0.38f), new Vector3(0.36f, 0.34f, 0.32f), _bodyMat);
            }
            else // Classic / Bullet / Himalayan: Round Vintage Chrome Headlamp
            {
                CreatePrimitive(PrimitiveType.Sphere, "Headlight_Chrome_Dome", fork,
                    new Vector3(0f, 0.20f, 0.26f), new Vector3(0.24f, 0.24f, 0.22f), _chromeMat);
            }

            GameObject lightObj = new GameObject("Headlight_Spotlight");
            lightObj.transform.SetParent(fork, false);
            lightObj.transform.localPosition = new Vector3(0f, 0.20f, 0.38f);
            lightObj.transform.localRotation = Quaternion.Euler(5f, 0f, 0f);

            Light spotLight = lightObj.AddComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.color = new Color(1f, 0.98f, 0.88f);
            spotLight.intensity = 4.0f;
            spotLight.range = 65f;
            spotLight.spotAngle = 48f;

            return spotLight;
        }

        // =========================================================================
        // BODYWORK, TANK & WINDSHIELD
        // =========================================================================
        private Transform BuildBodyworkAndTank(Transform root, Transform fork, int modelIdx)
        {
            // 1. Fuel Tank
            if (modelIdx == 0) // R15 Muscular Supersport Tank
            {
                CreatePrimitive(PrimitiveType.Cube, "FuelTank_R15", root,
                    new Vector3(0f, 0.80f, 0.20f), new Vector3(0.38f, 0.35f, 0.65f), _bodyMat);
            }
            else if (modelIdx == 3) // KTM Duke Angular Tank Shrouds
            {
                CreatePrimitive(PrimitiveType.Cube, "FuelTank_Duke", root,
                    new Vector3(0f, 0.82f, 0.18f), new Vector3(0.35f, 0.38f, 0.58f), _bodyMat);
                // Sharp forward-pointing tank wings
                CreatePrimitive(PrimitiveType.Cube, "Duke_Wing_L", root,
                    new Vector3(-0.24f, 0.78f, 0.32f), new Vector3(0.06f, 0.26f, 0.35f), _trellisMat);
                CreatePrimitive(PrimitiveType.Cube, "Duke_Wing_R", root,
                    new Vector3(0.24f, 0.78f, 0.32f), new Vector3(0.06f, 0.26f, 0.35f), _trellisMat);
            }
            else if (modelIdx == 4) // Himalayan Adventure Tank & Tall Windscreen
            {
                CreatePrimitive(PrimitiveType.Cube, "FuelTank_Himalayan", root,
                    new Vector3(0f, 0.85f, 0.18f), new Vector3(0.40f, 0.42f, 0.62f), _bodyMat);
                // Tall Clear Windscreen
                CreatePrimitive(PrimitiveType.Cube, "Adventure_Windshield", fork,
                    new Vector3(0f, 0.52f, 0.26f), new Vector3(0.28f, 0.44f, 0.04f), _windshieldMat);
            }
            else if (modelIdx == 5) // Pulsar 220F Aerodynamic Half-Fairing
            {
                CreatePrimitive(PrimitiveType.Cube, "FuelTank_Pulsar", root,
                    new Vector3(0f, 0.80f, 0.20f), new Vector3(0.38f, 0.36f, 0.62f), _bodyMat);
                CreatePrimitive(PrimitiveType.Cube, "Pulsar_Windshield", fork,
                    new Vector3(0f, 0.48f, 0.28f), new Vector3(0.32f, 0.34f, 0.04f), _windshieldMat);
            }
            else // Classic 350 / Bullet: Iconic Teardrop Tank
            {
                CreatePrimitive(PrimitiveType.Sphere, "FuelTank_Teardrop", root,
                    new Vector3(0f, 0.82f, 0.22f), new Vector3(0.42f, 0.38f, 0.72f), _bodyMat);
            }

            // 2. Seats
            GameObject riderSeat = CreatePrimitive(PrimitiveType.Cube, "Rider_Seat", root,
                new Vector3(0f, 0.75f, -0.25f), new Vector3(0.36f, 0.12f, 0.42f), _seatMat);

            GameObject pillionSeat = CreatePrimitive(PrimitiveType.Cube, "Pillion_Seat", root,
                new Vector3(0f, 0.82f, -0.65f), new Vector3(0.30f, 0.12f, 0.38f), _seatMat);

            return pillionSeat.transform;
        }

        // =========================================================================
        // AVATAR & RIDER CHARACTER BUILDER
        // =========================================================================
        private void CreateRiderCharacter(Transform parent, int modelIdx)
        {
            _riderVisual = new GameObject("Rider_Character");
            _riderVisual.transform.SetParent(parent, false);

            float scale = _isFemaleAvatar ? 0.90f : 1.0f;
            _riderVisual.transform.localScale = Vector3.one * scale;

            float leanAngle = (modelIdx == 0 || modelIdx == 5) ? 24f : 12f; // Supersport leans more forward

            // Torso (Jacket)
            GameObject torso = CreatePrimitive(PrimitiveType.Cube, "Rider_Torso", _riderVisual.transform,
                new Vector3(0f, 1.15f, -0.22f), new Vector3(0.38f, 0.48f, 0.26f), _riderJacketMat);
            torso.transform.localRotation = Quaternion.Euler(leanAngle, 0f, 0f);

            // Helmet & Head
            GameObject helmet = CreatePrimitive(PrimitiveType.Sphere, "Rider_Helmet", _riderVisual.transform,
                new Vector3(0f, 1.54f, -0.12f), new Vector3(0.28f, 0.32f, 0.30f), _riderHelmetMat);

            // Visor
            CreatePrimitive(PrimitiveType.Cube, "Rider_Visor", helmet.transform,
                new Vector3(0f, 0.04f, 0.42f), new Vector3(0.72f, 0.32f, 0.32f), _riderVisorMat);

            // Motocross Peak Visor if style 2
            if (_helmetStyle == 2)
            {
                CreatePrimitive(PrimitiveType.Cube, "Motocross_Visor_Peak", helmet.transform,
                    new Vector3(0f, 0.28f, 0.35f), new Vector3(0.85f, 0.06f, 0.42f), _riderHelmetMat);
            }

            // Arms holding handlebars
            float armPitch = leanAngle + 30f;
            CreatePrimitive(PrimitiveType.Cylinder, "Rider_Arm_L", _riderVisual.transform,
                new Vector3(-0.25f, 1.15f, 0.12f), new Vector3(0.09f, 0.36f, 0.09f), _riderJacketMat)
                .transform.localRotation = Quaternion.Euler(armPitch, -22f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "Rider_Arm_R", _riderVisual.transform,
                new Vector3(0.25f, 1.15f, 0.12f), new Vector3(0.09f, 0.36f, 0.09f), _riderJacketMat)
                .transform.localRotation = Quaternion.Euler(armPitch, 22f, 0f);

            // Legs resting on footpegs
            CreatePrimitive(PrimitiveType.Cylinder, "Rider_Leg_L", _riderVisual.transform,
                new Vector3(-0.22f, 0.65f, -0.18f), new Vector3(0.12f, 0.45f, 0.12f), _riderPantsMat)
                .transform.localRotation = Quaternion.Euler(40f, 0f, -12f);

            CreatePrimitive(PrimitiveType.Cylinder, "Rider_Leg_R", _riderVisual.transform,
                new Vector3(0.22f, 0.65f, -0.18f), new Vector3(0.12f, 0.45f, 0.12f), _riderPantsMat)
                .transform.localRotation = Quaternion.Euler(40f, 0f, 12f);
        }

        private GameObject CreatePassengerCharacter(Transform parent)
        {
            Material dressMat = CreateStandardMaterial("Passenger_Attire", new Color(0.92f, 0.28f, 0.52f));
            Material pHelmetMat = CreateStandardMaterial("Passenger_Helmet", new Color(0.95f, 0.95f, 0.98f));

            GameObject pRoot = new GameObject("Passenger_Character");
            pRoot.transform.SetParent(parent, false);

            CreatePrimitive(PrimitiveType.Cube, "Passenger_Torso", pRoot.transform,
                new Vector3(0f, 1.25f, -0.65f), new Vector3(0.34f, 0.44f, 0.24f), dressMat);

            CreatePrimitive(PrimitiveType.Sphere, "Passenger_Helmet", pRoot.transform,
                new Vector3(0f, 1.58f, -0.65f), new Vector3(0.26f, 0.30f, 0.28f), pHelmetMat);

            CreatePrimitive(PrimitiveType.Cylinder, "Passenger_Arm_L", pRoot.transform,
                new Vector3(-0.20f, 1.18f, -0.45f), new Vector3(0.07f, 0.28f, 0.07f), dressMat)
                .transform.localRotation = Quaternion.Euler(35f, -15f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "Passenger_Arm_R", pRoot.transform,
                new Vector3(0.20f, 1.18f, -0.45f), new Vector3(0.07f, 0.28f, 0.07f), dressMat)
                .transform.localRotation = Quaternion.Euler(35f, 15f, 0f);

            return pRoot;
        }

        // =========================================================================
        // PUBLIC API FOR UI CUSTOMIZATIONS
        // =========================================================================
        public void SetBodyPaintColor(Color color)
        {
            _bodyPaintColor = color;
            if (_bodyMat != null)
            {
                if (_bodyMat.HasProperty("_BaseColor")) _bodyMat.SetColor("_BaseColor", color);
                if (_bodyMat.HasProperty("_Color")) _bodyMat.SetColor("_Color", color);
            }
        }

        public void SetRiderAvatar(bool isFemale, Color jacketColor, Color helmetColor, int helmetStyle)
        {
            _isFemaleAvatar = isFemale;
            _jacketColor = jacketColor;
            _helmetColor = helmetColor;
            _helmetStyle = helmetStyle;
            BuildMotorcycleModel(_currentModelIndex);
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
            _bodyMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Body", _bodyPaintColor, 0.75f, 0.65f);
            _chromeMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Chrome", _chromeColor, 0.95f, 0.92f);
            _engineMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Engine", _engineColor, 0.5f, 0.3f);
            _rubberMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Rubber", _rubberColor, 0.05f, 0.1f);
            _seatMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Leather", _seatColor, 0.1f, 0.2f);
            _goldForkMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_GoldFork", new Color(0.95f, 0.78f, 0.18f), 0.9f, 0.85f);
            _trellisMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Trellis", _trellisColor, 0.6f, 0.7f);
            _windshieldMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Windshield", new Color(0.85f, 0.92f, 1.0f, 0.6f), 0.1f, 0.95f);

            _lightMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Bike_Taillight", Color.red, 0.1f, 0.8f);
            _lightMat.EnableKeyword("_EMISSION");
            if (_lightMat.HasProperty("_EmissionColor")) _lightMat.SetColor("_EmissionColor", Color.red * 2.5f);

            _riderJacketMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Rider_Jacket", _jacketColor, 0.2f, 0.4f);
            _riderPantsMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Rider_Pants", new Color(0.12f, 0.15f, 0.22f), 0.1f, 0.2f);
            _riderHelmetMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Rider_Helmet", _helmetColor, 0.8f, 0.7f);
            _riderVisorMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Rider_Visor", new Color(0.06f, 0.06f, 0.08f), 0.9f, 0.9f);
            _riderSkinMat = IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial("Mat_Rider_Skin", new Color(0.82f, 0.62f, 0.48f), 0.1f, 0.2f);
        }

        private Material CreateStandardMaterial(string name, Color color, float metallic = 0.5f, float smoothness = 0.5f)
        {
            return IndianBikeLife.Core.Utilities.URPMaterialUtility.CreateURPMaterial(name, color, metallic, smoothness);
        }
    }
}
