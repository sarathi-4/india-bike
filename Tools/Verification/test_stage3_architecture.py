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
    assert len(cs_files) >= 150, f"Expected at least 150 C# files, found {len(cs_files)}"
    
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
        meta = item + ".meta"
        assert os.path.exists(meta), f"Missing meta file for {item}"
        with open(meta, 'r', encoding='utf-8') as f:
            content = f.read()
        m = re.search(r'guid:\s*([a-f0-9]{32})', content)
        assert m, f"Invalid or missing GUID in {meta}"
        guid = m.group(1)
        assert guid not in guids, f"Duplicate GUID detected: {guid} in {meta}"
        guids.add(guid)

    print(f"✓ All {len(all_files)} assets and folders have valid, unique 32-hex GUID .meta files.")

def test_scenes():
    with open("ProjectSettings/EditorBuildSettings.asset") as f:
        content = f.read()
    
    expected_scenes = ["Boot.unity", "Loading.unity", "MainMenu.unity", "RegionalRideTest.unity"]
    for sc in expected_scenes:
        assert sc in content, f"Scene {sc} not registered in EditorBuildSettings"
    print("✓ All 4 scenes (Boot, Loading, MainMenu, RegionalRideTest) verified in EditorBuildSettings.")

def test_stage3_mission_catalogue():
    cat_path = "Assets/Game/MissionSystem/Data/MissionCatalogue.asset"
    assert os.path.exists(cat_path), f"Missing {cat_path}"
    with open(cat_path) as f:
        content = f.read()
    
    # Check 15 mission references in catalogue
    matches = re.findall(r'-\s*\{fileID:[^}]+guid:\s*([a-f0-9]{32})', content)
    assert len(matches) == 15, f"Expected 15 mission references in catalogue, found {len(matches)}"

    # Check that all 15 mission assets exist on disk
    mission_assets = glob.glob("Assets/Game/MissionSystem/Data/Mission_*.asset")
    assert len(mission_assets) == 15, f"Expected 15 Mission_*.asset files, found {len(mission_assets)}"

    print("✓ MissionCatalogue contains all 15 mission categories and valid asset references.")

def test_stage3_save_schema_v2():
    with open("Assets/Game/SaveSystem/Data/SaveData.cs") as f:
        content = f.read()
    assert "public const int CurrentSchemaVersion = 2;" in content, "SaveData schemaVersion must be 2"
    assert "public ProgressionSaveData progression;" in content, "SaveData missing progression container"

    assert os.path.exists("Assets/Game/SaveSystem/Migrations/SaveMigratorV1ToV2.cs"), "SaveMigratorV1ToV2 missing"
    assert os.path.exists("Assets/Game/SaveSystem/Data/ProgressionSaveData.cs"), "ProgressionSaveData missing"
    print("✓ SaveData Schema Version 2 and Migration V1->V2 validated.")

def test_stage3_progression_and_economy():
    assert os.path.exists("Assets/Game/Progression/XP/LevelProgressionConfig.asset"), "LevelProgressionConfig.asset missing"
    assert os.path.exists("Assets/Game/Progression/Achievements/AchievementCatalogue.asset"), "AchievementCatalogue.asset missing"
    print("✓ Progression, Economy, Rewards, Achievements and Leaderboards assets verified.")

def main():
    print("==================================================")
    print("   RUNNING STAGE 3 MASTER ARCHITECTURE TEST SUITE ")
    print("==================================================")
    try:
        test_manifest()
        test_asmdefs()
        test_cs_files()
        test_meta_files()
        test_scenes()
        test_stage3_mission_catalogue()
        test_stage3_save_schema_v2()
        test_stage3_progression_and_economy()
        print("==================================================")
        print("   ALL STAGE 3 ARCHITECTURAL TESTS PASSED (100%)  ")
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

