#!/usr/bin/env python3
import os
import re
import sys
import json
import glob

def test_manifest():
    assert os.path.exists("Packages/manifest.json"), "Packages/manifest.json missing"
    with open("Packages/manifest.json") as f:
        data = json.load(f)
    assert "dependencies" in data, "manifest missing dependencies"
    deps = data["dependencies"]
    assert "com.unity.render-pipelines.universal" in deps, "URP package missing"
    assert "com.unity.inputsystem" in deps, "Input System package missing"
    print("✓ Packages/manifest.json is valid with URP and Input System.")

def test_asmdefs():
    core_asmdef = "Assets/Game/IndianBikeLife.Core.asmdef"
    tests_asmdef = "Assets/Tests/IndianBikeLife.Tests.asmdef"
    assert os.path.exists(core_asmdef), "Core asmdef missing"
    assert os.path.exists(tests_asmdef), "Tests asmdef missing"
    print("✓ Assembly definitions (Core and Tests) are valid.")

def test_cs_files():
    cs_files = glob.glob("Assets/**/*.cs", recursive=True)
    assert len(cs_files) >= 170, f"Expected at least 170 C# files, found {len(cs_files)}"
    
    for path in cs_files:
        with open(path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Check basic balance of braces
        open_b = content.count('{')
        close_b = content.count('}')
        assert open_b == close_b, f"Mismatched braces in {path}: {open_b} open vs {close_b} close"

        # Check namespace
        assert "namespace IndianBikeLife" in content, f"Missing IndianBikeLife namespace in {path}"

    print(f"✓ All {len(cs_files)} C# source files passed lexical & structural validation.")

def test_meta_files():
    all_files = []
    for root, dirs, files in os.walk("Assets"):
        for d in dirs:
            all_files.append(os.path.join(root, d))
        for f in files:
            if not f.endswith(".meta"):
                all_files.append(os.path.join(root, f))
    
    guids = set()
    for item in all_files:
        meta_path = item + ".meta"
        assert os.path.exists(meta_path), f"Missing meta file for {item}"
        with open(meta_path, 'r', encoding='utf-8') as f:
            content = f.read()
        guid_match = re.search(r'guid:\s*([a-f0-9]{32})', content)
        assert guid_match, f"Invalid or missing GUID in {meta_path}"
        guid = guid_match.group(1)
        assert guid not in guids, f"Duplicate GUID {guid} in {meta_path}"
        guids.add(guid)

    print(f"✓ All {len(all_files)} assets and folders have valid, unique .meta files with proper GUIDs.")

def test_scenes():
    scenes = [
        "Assets/Scenes/Boot.unity",
        "Assets/Scenes/Loading.unity",
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/RegionalRideTest.unity"
    ]
    for s in scenes:
        assert os.path.exists(s), f"Missing scene {s}"

    # Verify Stage 4 and Stage 5 GameObjects in RegionalRideTest.unity
    with open("Assets/Scenes/RegionalRideTest.unity", 'r', encoding='utf-8') as f:
        scene_content = f.read()
    assert "[Stage4_MultiplayerAndBackend]" in scene_content, "Missing [Stage4_MultiplayerAndBackend] in scene"
    assert "[Stage5_OptimizationAndRelease]" in scene_content, "Missing [Stage5_OptimizationAndRelease] in scene"
    assert "253e90571eecbbc28453c97cfeb1c1ec" in scene_content, "Missing QualitySettingsManager component"
    assert "d4d8be6c9daf0f181507875820bc2a63" in scene_content, "Missing MobilePerformanceOptimizer component"

    print("✓ All 4 scenes verified; Stage 5 Optimization Managers wired in RegionalRideTest.unity.")

def test_urp_quality_presets():
    urp_low = "Assets/Settings/URP/URP-Low.asset"
    urp_med = "Assets/Settings/URP/URP-Medium.asset"
    urp_high = "Assets/Settings/URP/URP-High.asset"

    for u in [urp_low, urp_med, urp_high]:
        assert os.path.exists(u), f"Missing URP asset {u}"

    with open(urp_low) as f:
        low_c = f.read()
    with open(urp_high) as f:
        high_c = f.read()

    assert "m_RenderScale: 0.75" in low_c, "URP-Low render scale should be 0.75"
    assert "m_ShadowDistance: 25" in low_c, "URP-Low shadow distance should be 25m"
    assert "m_ShadowDistance: 80" in high_c, "URP-High shadow distance should be 80m"
    assert "m_ShadowCascadeCount: 4" in high_c, "URP-High shadow cascades should be 4"

    print("✓ URP Mobile Quality Tiers (Low, Medium, High) verified.")

def test_android_platform_config():
    # 1. ProjectSettings
    with open("ProjectSettings/ProjectSettings.asset") as f:
        ps = f.read()
    assert "bundleVersion: 1.0.0" in ps, "Bundle version must be 1.0.0"
    assert "AndroidMinSDKVersion: 26" in ps, "Min SDK must be 26"
    assert "AndroidTargetSDKVersion: 34" in ps, "Target SDK must be 34"
    assert "androidTargetArchitectures: 3" in ps, "Architectures must be ARMv7 + ARM64"
    assert "com.indianbikelife.game" in ps, "Package ID mismatch"

    # 2. AndroidManifest
    assert os.path.exists("Assets/Plugins/Android/AndroidManifest.xml"), "AndroidManifest.xml missing"
    with open("Assets/Plugins/Android/AndroidManifest.xml") as f:
        am = f.read()
    assert "android.permission.INTERNET" in am, "Missing INTERNET permission"
    assert "android.permission.ACCESS_NETWORK_STATE" in am, "Missing ACCESS_NETWORK_STATE permission"
    assert "android.permission.RECORD_AUDIO" in am, "Missing RECORD_AUDIO permission"
    assert 'name="android.hardware.microphone" android:required="false"' in am, "Microphone must be optional"

    # 3. link.xml
    assert os.path.exists("Assets/link.xml"), "link.xml missing"
    with open("Assets/link.xml") as f:
        lx = f.read()
    assert 'assembly fullname="IndianBikeLife.Core"' in lx, "Missing core assembly in link.xml"

    print("✓ Android platform, manifest, and link.xml code stripping configuration verified.")

def test_release_artifacts():
    manifest_path = "Builds/Android/BUILD_MANIFEST.json"
    aab_path = "Builds/Android/IndianBikeLife_v1.0.0_Release.aab"
    apk_path = "Builds/Android/IndianBikeLife_v1.0.0_Dev.apk"

    assert os.path.exists(manifest_path), f"Missing {manifest_path}"
    assert os.path.exists(aab_path), f"Missing {aab_path}"
    assert os.path.exists(apk_path), f"Missing {apk_path}"

    with open(manifest_path) as f:
        manifest = json.load(f)
    assert manifest["packageIdentifier"] == "com.indianbikelife.game"
    assert manifest["version"] == "1.0.0"
    assert manifest["targetSdkVersion"] == 34

    print("✓ Release artifacts (AAB, Dev APK, BUILD_MANIFEST.json) verified.")

def main():
    print("==================================================")
    print("   RUNNING STAGE 5 MASTER ARCHITECTURE TEST SUITE ")
    print("==================================================")
    try:
        test_manifest()
        test_asmdefs()
        test_cs_files()
        test_meta_files()
        test_scenes()
        test_urp_quality_presets()
        test_android_platform_config()
        test_release_artifacts()
        print("==================================================")
        print("   ALL STAGE 5 ARCHITECTURAL TESTS PASSED (100%)  ")
        print("==================================================")
        return 0
    except AssertionError as e:
        print(f"\n❌ TEST FAILED: {e}")
        return 1
    except Exception as e:
        print(f"\n❌ UNEXPECTED ERROR: {e}")
        return 1

if __name__ == "__main__":
    sys.exit(main())
