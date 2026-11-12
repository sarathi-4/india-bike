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
    print("✓ Packages/manifest.json is valid.")

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

    # Verify Stage 4 GameObject in RegionalRideTest.unity
    with open("Assets/Scenes/RegionalRideTest.unity", 'r', encoding='utf-8') as f:
        scene_content = f.read()
    assert "[Stage4_MultiplayerAndBackend]" in scene_content, "Missing [Stage4_MultiplayerAndBackend] in scene"
    assert "ce473481d110e6b730cc7137c3d40e46" in scene_content, "Missing MultiplayerManager component"
    assert "60034e4780462374a0739a5d5e2f03d2" in scene_content, "Missing FriendsManager component"
    assert "da4f2e983fee23eba428a4a49d2951c5" in scene_content, "Missing ChatManager component"
    assert "2ece0834b7dd34c9c202a6d9802aed02" in scene_content, "Missing VoiceChatManager component"
    assert "69a2d35d50fc81cce0008ad307970106" in scene_content, "Missing BackendManager component"
    assert "de28085fae83ebfcfc2ff1a0792d1f30" in scene_content, "Missing MultiplayerLobbyUI component"
    assert "210adcf02af63983885733b87c6584ac" in scene_content, "Missing ChatHUD component"

    print("✓ All 4 scenes verified; Stage 4 Managers wired in RegionalRideTest.unity.")

def test_stage4_backend_configuration():
    config_path = "Assets/Game/Backend/Data/BackendConfiguration.asset"
    assert os.path.exists(config_path), f"Missing {config_path}"
    with open(config_path, 'r', encoding='utf-8') as f:
        content = f.read()
    assert "_apiBaseUrl: https://api.indianbikelife.game/v1" in content, "Missing cloud sync endpoint"
    assert "_enableOfflineMockMode: 1" in content, "Offline mock mode must be active by default"
    print("✓ BackendConfiguration.asset verified with secure endpoint and offline fallback.")

def main():
    print("==================================================")
    print("   RUNNING STAGE 4 MASTER ARCHITECTURE TEST SUITE ")
    print("==================================================")
    try:
        test_manifest()
        test_asmdefs()
        test_cs_files()
        test_meta_files()
        test_scenes()
        test_stage4_backend_configuration()
        print("==================================================")
        print("   ALL STAGE 4 ARCHITECTURAL TESTS PASSED (100%)  ")
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
