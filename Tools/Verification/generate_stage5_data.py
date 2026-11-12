#!/usr/bin/env python3
import os
import hashlib
import glob

def generate_guid(filepath):
    return hashlib.md5(filepath.encode('utf-8')).hexdigest()

def ensure_meta(item_path, is_dir=False):
    meta_path = item_path + ".meta"
    if os.path.exists(meta_path):
        return

    guid = generate_guid(item_path)
    if is_dir:
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
    print(f"Created meta: {meta_path} [guid: {guid}]")

def main():
    print("Generating Stage 5 Meta files...")
    for root, dirs, files in os.walk("Assets"):
        for d in dirs:
            dir_path = os.path.join(root, d)
            ensure_meta(dir_path, is_dir=True)
        for f in files:
            if f.endswith(".meta"):
                continue
            file_path = os.path.join(root, f)
            ensure_meta(file_path, is_dir=False)

    print("Stage 5 meta file generation complete!")

    # Print relevant GUIDs for scene wiring
    targets = [
        "Assets/Game/Managers/QualitySettingsManager.cs",
        "Assets/Game/Managers/MobilePerformanceOptimizer.cs"
    ]
    for t in targets:
        meta_file = t + ".meta"
        if os.path.exists(meta_file):
            with open(meta_file) as f:
                for line in f:
                    if "guid:" in line:
                        print(f"{os.path.basename(t)}: {line.strip().split()[-1]}")

if __name__ == "__main__":
    main()

