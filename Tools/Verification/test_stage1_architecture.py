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
    print("✓ Packages/manifest.json is valid and contains URP, Input System, and Test Framework.")

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
    
    assert len(cs_files) >= 30, f"Expected at least 30 C# files, found {len(cs_files)}"
    
    errors = []
    for cs in cs_files:
        with open(cs, "r", encoding="utf-8") as f:
            content = f.read()
        
        # Check brace balance
        # Remove comments and string literals to accurately count braces
        cleaned = re.sub(r'//.*', '', content)
        cleaned = re.sub(r'/\*.*?\*/', '', cleaned, flags=re.DOTALL)
        cleaned = re.sub(r'"(\\.|[^"\\])*"', '', cleaned)
        cleaned = re.sub(r"'(\\.|[^'\\])*'", '', cleaned)
        
        open_braces = cleaned.count('{')
        close_braces = cleaned.count('}')
        if open_braces != close_braces:
            errors.append(f"{cs}: Brace mismatch ({{: {open_braces}, }}: {close_braces})")
            
        # Check namespace
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

def test_scene_and_build_settings(all_guids):
    build_settings_path = os.path.join(WORKSPACE, "ProjectSettings/EditorBuildSettings.asset")
    with open(build_settings_path, "r", encoding="utf-8") as f:
        build_text = f.read()
        
    expected_scenes = [
        ("Assets/Scenes/Boot.unity", "e0000000000000000000000000000001"),
        ("Assets/Scenes/Loading.unity", "e0000000000000000000000000000002"),
        ("Assets/Scenes/MainMenu.unity", "e0000000000000000000000000000003")
    ]
    
    for path, guid in expected_scenes:
        assert os.path.exists(os.path.join(WORKSPACE, path)), f"Scene missing: {path}"
        assert guid in build_text, f"Build settings missing scene GUID {guid} for {path}"
        assert guid in all_guids, f"GUID {guid} not found in meta registry"
        
    print("✓ Scene registration in EditorBuildSettings verified with correct GUIDs and paths.")

if __name__ == "__main__":
    print("==================================================")
    print("   RUNNING STAGE 1 CORE ARCHITECTURE VALIDATION   ")
    print("==================================================")
    test_manifest()
    test_asmdefs()
    test_csharp_syntax()
    guids = test_meta_integrity()
    test_scene_and_build_settings(guids)
    print("==================================================")
    print("   ALL ARCHITECTURE AND STRUCTURAL TESTS PASSED   ")
    print("==================================================")
