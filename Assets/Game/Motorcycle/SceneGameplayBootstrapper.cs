using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Automatically bootstraps full gameplay in RegionalRideTest:
    /// Instantiates motorcycle physics, 3D visual builder, procedural engine sound,
    /// dynamic follow camera, 2km highway corridor, and mobile touch HUD.
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public class SceneGameplayBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitializeScene()
        {
            var existing = FindFirstObjectByType<SceneGameplayBootstrapper>();
            if (existing == null)
            {
                GameObject bootstrapperGO = new GameObject("[Gameplay_Bootstrapper]");
                existing = bootstrapperGO.AddComponent<SceneGameplayBootstrapper>();
            }
            existing.EnsureFullGameplaySetup();
        }

        private void Awake()
        {
            EnsureFullGameplaySetup();
        }

        public void EnsureFullGameplaySetup()
        {
            // Enforce URP pipeline
            if (QualitySettings.renderPipeline == null && UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline != null)
            {
                QualitySettings.renderPipeline = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
            }

            // 1. Locate or create Player Motorcycle
            GameObject bikeObj = GameObject.FindWithTag("Motorcycle") ?? GameObject.Find("[PlayerMotorcycle]");
            if (bikeObj == null)
            {
                bikeObj = new GameObject("[PlayerMotorcycle]");
                bikeObj.tag = "Motorcycle";
                bikeObj.transform.position = new Vector3(0f, 0.5f, 5f);
            }

            // Enforce uniform 1:1 scale on player motorcycle
            bikeObj.transform.localScale = Vector3.one;

            // Remove any legacy BoxCollider that blocks the CharacterController
            var legacyBox = bikeObj.GetComponent<BoxCollider>();
            if (legacyBox != null)
            {
                legacyBox.enabled = false;
                DestroyImmediate(legacyBox);
            }

            // Hide legacy untextured passenger seat anchor
            var oldSeat = bikeObj.transform.Find("PassengerSeatAnchor");
            if (oldSeat != null)
            {
                oldSeat.gameObject.SetActive(false);
            }

            // Ensure CharacterController
            var cc = bikeObj.GetComponent<CharacterController>();
            if (cc == null)
            {
                cc = bikeObj.AddComponent<CharacterController>();
                cc.center = new Vector3(0f, 0.75f, 0f);
                cc.radius = 0.45f;
                cc.height = 1.5f;
                cc.stepOffset = 0.5f;
                cc.slopeLimit = 65f;
            }

            // Ensure AudioSource
            var audio = bikeObj.GetComponent<AudioSource>();
            if (audio == null)
            {
                audio = bikeObj.AddComponent<AudioSource>();
            }

            // Ensure PlayerMotorcycleController
            var controller = bikeObj.GetComponent<PlayerMotorcycleController>();
            if (controller == null)
            {
                controller = bikeObj.AddComponent<PlayerMotorcycleController>();
            }

            // Ensure ProceduralEngineAudio
            var engineAudio = bikeObj.GetComponent<ProceduralEngineAudio>();
            if (engineAudio == null)
            {
                engineAudio = bikeObj.AddComponent<ProceduralEngineAudio>();
            }

            // Ensure MotorcycleVisualBuilder (builds 3D bike + rider + passenger)
            var visualBuilder = bikeObj.GetComponent<MotorcycleVisualBuilder>();
            if (visualBuilder == null)
            {
                visualBuilder = bikeObj.AddComponent<MotorcycleVisualBuilder>();
            }

            // 2. Ensure Main Camera Follows Motorcycle
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                var followCam = mainCam.GetComponent<MotorcycleFollowCamera>();
                if (followCam == null)
                {
                    followCam = mainCam.gameObject.AddComponent<MotorcycleFollowCamera>();
                }
                followCam.SetTarget(bikeObj.transform);
            }

            // 3. Ensure 2KM Highway Corridor & Roadside Environment
            var envBuilder = FindFirstObjectByType<RoadAndEnvironmentBuilder>();
            if (envBuilder == null)
            {
                GameObject roadEnv = new GameObject("[Highway_World_Generator]");
                envBuilder = roadEnv.AddComponent<RoadAndEnvironmentBuilder>();
            }

            // Disable legacy straight road so curved road is active
            var legacyRoad = GameObject.Find("HighwayRoad_NH44");
            if (legacyRoad != null)
            {
                legacyRoad.SetActive(false);
            }

            // 4. Ensure In-Game Mobile Gameplay HUD
            var gameplayHUD = FindFirstObjectByType<GameplayHUD>();
            if (gameplayHUD == null)
            {
                GameObject hudObj = new GameObject("[Gameplay_HUD]");
                gameplayHUD = hudObj.AddComponent<GameplayHUD>();
            }

            // 5. Ensure Master Game Flow Menu (Login, Garage, Map, Passenger, Missions)
            var masterMenu = FindFirstObjectByType<IndianBikeLife.Core.UI.MasterGameFlowUI>();
            if (masterMenu == null)
            {
                GameObject menuObj = new GameObject("[Master_Game_Flow_UI]");
                masterMenu = menuObj.AddComponent<IndianBikeLife.Core.UI.MasterGameFlowUI>();
            }

            // Suppress legacy developer HUD overlay
            var legacyDevHUD = FindFirstObjectByType<IndianBikeLife.UI.Development.RegionalRideTestHUD>();
            if (legacyDevHUD != null)
            {
                legacyDevHUD.enabled = false;
            }

            // Reposition motorcycle properly aligned along the curved road surface
            Vector3 roadPos = envBuilder.CalculateRoadPosition(12f, envBuilder.ActiveStateIndex);
            Vector3 roadDir = envBuilder.CalculateRoadDirection(12f, envBuilder.ActiveStateIndex);
            bikeObj.transform.position = roadPos + Vector3.up * 0.65f;
            bikeObj.transform.rotation = Quaternion.LookRotation(roadDir, Vector3.up);

            // Heal any pink shaders in the scene
            FixPinkMaterialsInScene();

            Debug.Log("[SceneGameplayBootstrapper] Full Indian Bike Life 3D Gameplay initialized successfully!");
        }

        public void FixPinkMaterialsInScene()
        {
            var renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            foreach (var r in renderers)
            {
                if (r == null || r.sharedMaterial == null) continue;
                var s = r.sharedMaterial.shader;
                bool isPink = s == null || !s.isSupported || s.name.Contains("Error") || s.name == "Hidden/InternalErrorShader";

                if (isPink)
                {
                    Color c = Color.gray;
                    string n = r.gameObject.name.ToLower();
                    if (n.Contains("asphalt") || n.Contains("road")) c = new Color(0.18f, 0.18f, 0.20f);
                    else if (n.Contains("grass") || n.Contains("landscape")) c = new Color(0.28f, 0.45f, 0.22f);
                    else if (n.Contains("yellow") || n.Contains("shoulder") || n.Contains("cap")) c = new Color(0.98f, 0.78f, 0.12f);
                    else if (n.Contains("white") || n.Contains("dash") || n.Contains("base")) c = new Color(0.95f, 0.95f, 0.95f);
                    else if (n.Contains("dhaba") || n.Contains("building")) c = new Color(0.92f, 0.82f, 0.65f);
                    else if (n.Contains("roof")) c = new Color(0.68f, 0.22f, 0.18f);
                    else if (n.Contains("foliage") || n.Contains("leaf") || n.Contains("tree")) c = new Color(0.18f, 0.52f, 0.18f);
                    else if (n.Contains("trunk") || n.Contains("wood") || n.Contains("pole")) c = new Color(0.35f, 0.22f, 0.14f);
                    else if (n.Contains("tank") || n.Contains("body") || n.Contains("fender")) c = new Color(0.72f, 0.08f, 0.12f);
                    else if (n.Contains("engine") || n.Contains("chrome") || n.Contains("rim") || n.Contains("fork") || n.Contains("exhaust")) c = new Color(0.92f, 0.92f, 0.95f);
                    else if (n.Contains("tyre") || n.Contains("rubber") || n.Contains("grip")) c = new Color(0.12f, 0.12f, 0.12f);
                    else if (n.Contains("seat") || n.Contains("leather")) c = new Color(0.15f, 0.12f, 0.10f);

                    r.sharedMaterial = IndianBikeLife.Core.Utilities.URPMaterialUtility.GetCachedColorMaterial(c, $"Fixed_{r.gameObject.name}");
                }
            }
        }
    }
}

