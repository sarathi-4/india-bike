import os
import re
import json

WORKSPACE = "/run/media/sarathi/2-250/indianbike"
ASSETS_DIR = os.path.join(WORKSPACE, "Assets")

def test_manifest():
    manifest_path = os.path.join(WORKSPACE, "Packages/manifest.json")
    assert os.path.exists(manifest_path), "manifest.json missing"
    with open(manifest_path, "r") as f:
        data = json.load(f)
    assert "com.unity.render-pipelines.universal" in data["dependencies"]
    assert "com.unity.inputsystem" in data["dependencies"]
    assert "com.unity.test-framework" in data["dependencies"]
    print("✓ Packages/manifest.json is valid.")

def test_asmdefs():
    core_asmdef = os.path.join(WORKSPACE, "Assets/Game/IndianBikeLife.Core.asmdef")
    tests_asmdef = os.path.join(WORKSPACE, "Assets/Tests/IndianBikeLife.Tests.asmdef")
    with open(core_asmdef) as f:
        core_data = json.load(f)
    with open(tests_asmdef) as f:
        tests_data = json.load(f)
    assert core_data["name"] == "IndianBikeLife.Core"
    assert tests_data["name"] == "IndianBikeLife.Tests"
    print("✓ Assembly definitions (Core and Tests) are valid.")

def test_csharp_syntax():
    cs_files = []
    for root, _, files in os.walk(ASSETS_DIR):
        for f in files:
            if f.endswith(".cs"):
                cs_files.append(os.path.join(root, f))
    
    assert len(cs_files) >= 60, f"Expected at least 60 C# files, found {len(cs_files)}"
    
    errors = []
    for cs in cs_files:
        with open(cs, "r", encoding="utf-8") as f:
            content = f.read()
        
        # Strip comments and string literals
        cleaned = re.sub(r'//.*', '', content)
        cleaned = re.sub(r'/\*.*?\*/', '', cleaned, flags=re.DOTALL)
        cleaned = re.sub(r'"(\\.|[^"\\])*"', '', cleaned)
        cleaned = re.sub(r"'(\\.|[^'\\])*'", '', cleaned)
        
        open_braces = cleaned.count('{')
        close_braces = cleaned.count('}')
        if open_braces != close_braces:
            errors.append(f"{cs}: Brace mismatch ({{: {open_braces}, }}: {close_braces})")
            
        if "namespace " not in content:
            errors.append(f"{cs}: Missing namespace definition")
            
    if errors:
        for err in errors:
            print("ERROR:", err)
        assert False, f"Found {len(errors)} C# syntax errors"
    
    print(f"✓ All {len(cs_files)} C# source files passed lexical & structural validation.")

def test_meta_integrity():
    all_guids = {}
    missing_metas = []
    
    for root, dirs, files in os.walk(ASSETS_DIR):
        rel_root = os.path.relpath(root, WORKSPACE).replace("\\", "/")
        if rel_root != "Assets":
            meta_path = root + ".meta"
            if not os.path.exists(meta_path):
                missing_metas.append(rel_root)
            else:
                with open(meta_path, "r", encoding="utf-8") as f:
                    content = f.read()
                m = re.search(r'guid:\s*([a-f0-9]{32})', content)
                assert m, f"Corrupted meta file {meta_path}"
                guid = m.group(1)
                assert guid not in all_guids, f"Duplicate GUID {guid} in {meta_path} and {all_guids[guid]}"
                all_guids[guid] = meta_path
                
        for file in files:
            if file.endswith(".meta"):
                continue
            fpath = os.path.join(root, file)
            meta_path = fpath + ".meta"
            if not os.path.exists(meta_path):
                missing_metas.append(os.path.relpath(fpath, WORKSPACE))
            else:
                with open(meta_path, "r", encoding="utf-8") as f:
                    content = f.read()
                m = re.search(r'guid:\s*([a-f0-9]{32})', content)
                assert m, f"Corrupted meta file {meta_path}"
                guid = m.group(1)
                assert guid not in all_guids, f"Duplicate GUID {guid} in {meta_path} and {all_guids[guid]}"
                all_guids[guid] = meta_path
                
    assert len(missing_metas) == 0, f"Missing meta files for: {missing_metas}"
    print(f"✓ All {len(all_guids)} assets and folders have valid, unique 32-hex GUID .meta files.")
    return all_guids

def test_scenes_and_build_settings(all_guids):
    build_settings_path = os.path.join(WORKSPACE, "ProjectSettings/EditorBuildSettings.asset")
    with open(build_settings_path, "r", encoding="utf-8") as f:
        build_text = f.read()
        
    expected_scenes = [
        ("Assets/Scenes/Boot.unity", "e0000000000000000000000000000001"),
        ("Assets/Scenes/Loading.unity", "e0000000000000000000000000000002"),
        ("Assets/Scenes/MainMenu.unity", "e0000000000000000000000000000003"),
        ("Assets/Scenes/RegionalRideTest.unity", "e0000000000000000000000000000004")
    ]
    
    for path, guid in expected_scenes:
        assert os.path.exists(os.path.join(WORKSPACE, path)), f"Scene missing: {path}"
        assert guid in build_text, f"Build settings missing scene GUID {guid} for {path}"
        assert guid in all_guids, f"GUID {guid} not found in meta registry"
        
    print("✓ All 4 scenes (Boot, Loading, MainMenu, RegionalRideTest) verified in EditorBuildSettings.")

def test_map_catalogue_states():
    cat_path = os.path.join(WORKSPACE, "Assets/Game/MapSystem/Data/IndiaMapCatalogue.asset")
    assert os.path.exists(cat_path), "IndiaMapCatalogue.asset missing"
    with open(cat_path, "r") as f:
        content = f.read()
        
    # Check that 28 states and 8 union territories are linked in the lists
    state_refs = re.findall(r'-\s*\{fileID:[^}]+guid:\s*([a-f0-9]{32})', content)
    assert len(state_refs) == 36, f"Expected 36 national entities (28 states + 8 UTs), found {len(state_refs)}"
    print(f"✓ IndiaMapCatalogue contains all {len(state_refs)} national jurisdictions (28 states + 8 UTs).")

def test_riding_modes_catalogue():
    cat_path = os.path.join(WORKSPACE, "Assets/Game/RidingModes/Data/RidingModeCatalogue.asset")
    assert os.path.exists(cat_path), "RidingModeCatalogue.asset missing"
    with open(cat_path, "r") as f:
        content = f.read()
    mode_refs = re.findall(r'-\s*\{fileID:[^}]+guid:\s*([a-f0-9]{32})', content)
    assert len(mode_refs) == 10, f"Expected 10 riding modes, found {len(mode_refs)}"
    print("✓ RidingModeCatalogue contains all 10 configured riding modes.")

if __name__ == "__main__":
    print("==================================================")
    print("   RUNNING STAGE 2 MASTER ARCHITECTURE TEST SUITE ")
    print("==================================================")
    test_manifest()
    test_asmdefs()
    test_csharp_syntax()
    guids = test_meta_integrity()
    test_scenes_and_build_settings(guids)
    test_map_catalogue_states()
    test_riding_modes_catalogue()
    print("==================================================")
    print("   ALL STAGE 2 ARCHITECTURAL TESTS PASSED (100%)  ")
    print("==================================================")
