using System.IO;
using System.Xml;
using IndianBikeLife.Core.Configuration;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Managers;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests
{
    [TestFixture]
    public class OptimizationAndReleaseTests
    {
        private GameObject _holder;
        private QualitySettingsManager _qualityManager;
        private MobilePerformanceOptimizer _perfOptimizer;

        [SetUp]
        public void SetUp()
        {
            _holder = new GameObject("Test_OptimizationHolder");
            _qualityManager = _holder.AddComponent<QualitySettingsManager>();
            _perfOptimizer = _holder.AddComponent<MobilePerformanceOptimizer>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_holder != null)
            {
                Object.DestroyImmediate(_holder);
            }
        }

        [Test]
        public void Test01_QualitySettingsManager_PresetSwitching_UpdatesParameters()
        {
            _qualityManager.Initialize();

            // 1. Switch to Low Preset (Entry-level, battery saver)
            _qualityManager.SetQualityPreset(MobileQualityPreset.Low);
            Assert.AreEqual(MobileQualityPreset.Low, _qualityManager.CurrentPreset);
            Assert.AreEqual(30, _qualityManager.CurrentTargetFPS);
            Assert.AreEqual(25f, _qualityManager.CurrentShadowDistance);
            Assert.AreEqual(0.75f, _qualityManager.CurrentRenderScale);
            Assert.AreEqual(0.7f, _qualityManager.CurrentLODBias);

            // 2. Switch to Medium Preset (Mid-tier, balanced)
            _qualityManager.SetQualityPreset(MobileQualityPreset.Medium);
            Assert.AreEqual(MobileQualityPreset.Medium, _qualityManager.CurrentPreset);
            Assert.AreEqual(60, _qualityManager.CurrentTargetFPS);
            Assert.AreEqual(50f, _qualityManager.CurrentShadowDistance);
            Assert.AreEqual(1.0f, _qualityManager.CurrentRenderScale);
            Assert.AreEqual(1.0f, _qualityManager.CurrentLODBias);

            // 3. Switch to High Preset (Flagship, fidelity)
            _qualityManager.SetQualityPreset(MobileQualityPreset.High);
            Assert.AreEqual(MobileQualityPreset.High, _qualityManager.CurrentPreset);
            Assert.AreEqual(60, _qualityManager.CurrentTargetFPS);
            Assert.AreEqual(80f, _qualityManager.CurrentShadowDistance);
            Assert.AreEqual(1.0f, _qualityManager.CurrentRenderScale);
            Assert.AreEqual(1.5f, _qualityManager.CurrentLODBias);
        }

        [Test]
        public void Test02_QualityPresetChangedEvent_IsPublishedOnPresetSwitch()
        {
            _qualityManager.Initialize();
            _qualityManager.SetQualityPreset(MobileQualityPreset.Low);

            bool eventReceived = false;
            MobileQualityPreset capturedPrev = MobileQualityPreset.Low;
            MobileQualityPreset capturedNext = MobileQualityPreset.Low;

            EventBus.Subscribe<QualityPresetChangedEvent>(evt =>
            {
                eventReceived = true;
                capturedPrev = evt.PreviousPreset;
                capturedNext = evt.NewPreset;
            });

            _qualityManager.SetQualityPreset(MobileQualityPreset.High);

            Assert.IsTrue(eventReceived);
            Assert.AreEqual(MobileQualityPreset.Low, capturedPrev);
            Assert.AreEqual(MobileQualityPreset.High, capturedNext);
        }

        [Test]
        public void Test03_MobilePerformanceOptimizer_PhysicsTimestep_AndSleepThreshold()
        {
            _perfOptimizer.Initialize();

            // 50 Hz physics timestep for stable motorcycle simulation and low CPU load
            Assert.AreEqual(0.02f, Time.fixedDeltaTime, 0.001f);
            Assert.AreEqual(0.05f, Physics.sleepThreshold, 0.001f);
        }

        [Test]
        public void Test04_MobilePerformanceOptimizer_DistanceBasedTickThrottling()
        {
            _perfOptimizer.Initialize();
            Vector3 cameraPos = Vector3.zero;

            // Near agent (15 meters): ~30 Hz
            float nearTick = _perfOptimizer.GetLODTickInterval(new Vector3(15f, 0f, 0f), cameraPos);
            Assert.AreEqual(0.033f, nearTick, 0.005f);

            // Mid-distance agent (60 meters): 10 Hz
            float midTick = _perfOptimizer.GetLODTickInterval(new Vector3(60f, 0f, 0f), cameraPos);
            Assert.AreEqual(0.1f, midTick, 0.005f);

            // Far agent (120 meters): 4 Hz
            float farTick = _perfOptimizer.GetLODTickInterval(new Vector3(120f, 0f, 0f), cameraPos);
            Assert.AreEqual(0.25f, farTick, 0.005f);
        }

        [Test]
        public void Test05_MobilePerformanceOptimizer_TriggerSceneTransitionCleanup_Executes()
        {
            _perfOptimizer.Initialize();
            // Should execute without throwing exceptions
            Assert.DoesNotThrow(() => _perfOptimizer.TriggerSceneTransitionCleanup());
        }

        [Test]
        public void Test06_AndroidManifest_PermissionsAndHardwareFeatures_Validated()
        {
            string manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
            Assert.IsTrue(File.Exists(manifestPath), "AndroidManifest.xml must exist");

            string text = File.ReadAllText(manifestPath);
            Assert.IsTrue(text.Contains("package=\"com.indianbikelife.game\""));
            Assert.IsTrue(text.Contains("android.permission.INTERNET"));
            Assert.IsTrue(text.Contains("android.permission.ACCESS_NETWORK_STATE"));
            Assert.IsTrue(text.Contains("android.permission.RECORD_AUDIO"));
            Assert.IsTrue(text.Contains("android:screenOrientation=\"sensorLandscape\""));
            Assert.IsTrue(text.Contains("name=\"android.hardware.microphone\" android:required=\"false\""));
        }

        [Test]
        public void Test07_LinkXml_ProtectsCoreAssembliesFromIL2CPPStripping()
        {
            string linkPath = "Assets/link.xml";
            Assert.IsTrue(File.Exists(linkPath), "link.xml must exist to protect IL2CPP builds");

            string text = File.ReadAllText(linkPath);
            Assert.IsTrue(text.Contains("assembly fullname=\"IndianBikeLife.Core\""));
            Assert.IsTrue(text.Contains("assembly fullname=\"UnityEngine.CoreModule\""));
            Assert.IsTrue(text.Contains("assembly fullname=\"Unity.InputSystem\""));
            Assert.IsTrue(text.Contains("assembly fullname=\"Unity.RenderPipelines.Universal.Runtime\""));
        }

        [Test]
        public void Test08_URPQualityAssets_LowMedHigh_ExistAndConfigured()
        {
            Assert.IsTrue(File.Exists("Assets/Settings/URP/URP-Low.asset"));
            Assert.IsTrue(File.Exists("Assets/Settings/URP/URP-Medium.asset"));
            Assert.IsTrue(File.Exists("Assets/Settings/URP/URP-High.asset"));

            string lowText = File.ReadAllText("Assets/Settings/URP/URP-Low.asset");
            string highText = File.ReadAllText("Assets/Settings/URP/URP-High.asset");

            Assert.IsTrue(lowText.Contains("m_RenderScale: 0.75"));
            Assert.IsTrue(lowText.Contains("m_ShadowDistance: 25"));
            Assert.IsTrue(highText.Contains("m_ShadowDistance: 80"));
            Assert.IsTrue(highText.Contains("m_ShadowCascadeCount: 4"));
        }
    }
}

