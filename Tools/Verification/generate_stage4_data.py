#!/usr/bin/env python3
import os
import hashlib

def get_deterministic_guid(seed: str) -> str:
    return hashlib.md5(seed.encode('utf-8')).hexdigest()

def ensure_meta_file(file_path: str, is_folder: bool = False) -> str:
    meta_path = file_path + ".meta"
    if os.path.exists(meta_path):
        with open(meta_path, 'r', encoding='utf-8') as f:
            for line in f:
                if line.startswith("guid:"):
                    return line.strip().split()[1]
    
    guid = get_deterministic_guid(file_path)
    if is_folder:
        content = f"""fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    else:
        content = f"""fileFormatVersion: 2
guid: {guid}
MonoImporter:
  externalObjects: {{}}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {{instanceID: 0}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    with open(meta_path, 'w', encoding='utf-8') as f:
        f.write(content)
    return guid

def main():
    print("Generating Stage 4 Meta files and Assets...")

    stage4_dirs = [
        "Assets/Game/Multiplayer",
        "Assets/Game/Multiplayer/Data",
        "Assets/Game/Multiplayer/Scripts",
        "Assets/Game/Multiplayer/UI",
        "Assets/Game/Social",
        "Assets/Game/Social/Data",
        "Assets/Game/Social/Scripts",
        "Assets/Game/Social/UI",
        "Assets/Game/Communication",
        "Assets/Game/Communication/Scripts",
        "Assets/Game/Communication/UI",
        "Assets/Game/Backend",
        "Assets/Game/Backend/Data",
        "Assets/Game/Backend/Security",
        "Assets/Game/Backend/Scripts",
        "Assets/Game/Backend/UI",
    ]
    for d in stage4_dirs:
        if os.path.isdir(d):
            ensure_meta_file(d, is_folder=True)

    for root, dirs, files in os.walk("Assets"):
        for d in dirs:
            dir_path = os.path.join(root, d)
            ensure_meta_file(dir_path, is_folder=True)
        for f in files:
            if not f.endswith(".meta"):
                file_path = os.path.join(root, f)
                ensure_meta_file(file_path, is_folder=False)

    backend_config_cs = ensure_meta_file("Assets/Game/Backend/Data/BackendConfiguration.cs", False)
    backend_config_asset = "Assets/Game/Backend/Data/BackendConfiguration.asset"
    if not os.path.exists(backend_config_asset):
        content = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {backend_config_cs}, type: 3}}
  m_Name: BackendConfiguration
  _environment: 0
  _apiBaseUrl: https://api.indianbikelife.game/v1
  _requestTimeoutSeconds: 10
  _enableOfflineMockMode: 1
"""
        with open(backend_config_asset, 'w') as f:
            f.write(content)
        ensure_meta_file(backend_config_asset, False)
        print("Created BackendConfiguration.asset")

    scripts = [
        'MultiplayerManager', 'FriendsManager', 'ChatManager', 'VoiceChatManager',
        'BackendManager', 'MultiplayerLobbyUI', 'ChatHUD'
    ]
    for s in scripts:
        for root, dirs, files in os.walk("Assets/Game"):
            for f in files:
                if f == f"{s}.cs":
                    g = ensure_meta_file(os.path.join(root, f), False)
                    print(f"{s}: {g}")

    print("Stage 4 meta file generation complete!")

if __name__ == "__main__":
    main()

