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
            // 1. Locate or create Player Motorcycle
            GameObject bikeObj = GameObject.FindWithTag("Motorcycle");
            if (bikeObj == null)
            {
                bikeObj = GameObject.Find("[PlayerMotorcycle]");
            }
            if (bikeObj == null)
            {
                bikeObj = new GameObject("[PlayerMotorcycle]");
                bikeObj.tag = "Motorcycle";
                bikeObj.transform.position = new Vector3(0f, 0.5f, 5f);
            }

            // Ensure CharacterController
            var cc = bikeObj.GetComponent<CharacterController>();
            if (cc == null)
            {
                cc = bikeObj.AddComponent<CharacterController>();
                cc.center = new Vector3(0f, 0.75f, 0f);
                cc.radius = 0.45f;
                cc.height = 1.5f;
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

            // 4. Ensure In-Game Mobile Gameplay HUD
            var gameplayHUD = FindFirstObjectByType<GameplayHUD>();
            if (gameplayHUD == null)
            {
                GameObject hudObj = new GameObject("[Gameplay_HUD]");
                gameplayHUD = hudObj.AddComponent<GameplayHUD>();
            }

            // Reposition motorcycle properly on top of the road surface
            bikeObj.transform.position = new Vector3(-2.2f, 0.6f, 12f);
            bikeObj.transform.rotation = Quaternion.identity;

            Debug.Log("[SceneGameplayBootstrapper] Full Indian Bike Life 3D Gameplay initialized successfully!");
        }
    }
}
