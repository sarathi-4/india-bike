#!/usr/bin/env python3
import os
import uuid
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
    print("Generating Stage 3 Meta files and ScriptableObjects...")
    
    # 1. Ensure meta files for all new directories
    stage3_dirs = [
        "Assets/Game/MissionSystem",
        "Assets/Game/MissionSystem/Data",
        "Assets/Game/MissionSystem/Scripts",
        "Assets/Game/MissionSystem/UI",
        "Assets/Game/Progression",
        "Assets/Game/Progression/Economy",
        "Assets/Game/Progression/XP",
        "Assets/Game/Progression/Rewards",
        "Assets/Game/Progression/Achievements",
        "Assets/Game/Progression/Leaderboards",
    ]
    for d in stage3_dirs:
        if os.path.isdir(d):
            ensure_meta_file(d, is_folder=True)

    # 2. Ensure meta files for all C# files across Assets
    for root, dirs, files in os.walk("Assets"):
        for f in files:
            if f.endswith(".cs"):
                ensure_meta_file(os.path.join(root, f), is_folder=False)
            elif f.endswith(".unity"):
                meta_path = os.path.join(root, f) + ".meta"
                if not os.path.exists(meta_path):
                    g = get_deterministic_guid(os.path.join(root, f))
                    with open(meta_path, 'w') as mf:
                        mf.write(f"fileFormatVersion: 2\nguid: {g}\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n")

    # Get GUIDs of script classes
    mission_data_cs_guid = ensure_meta_file("Assets/Game/MissionSystem/Data/MissionData.cs", False)
    mission_cat_cs_guid = ensure_meta_file("Assets/Game/MissionSystem/Data/MissionCatalogue.cs", False)
    reward_data_cs_guid = ensure_meta_file("Assets/Game/Progression/Rewards/RewardData.cs", False)
    ach_data_cs_guid = ensure_meta_file("Assets/Game/Progression/Achievements/AchievementData.cs", False)
    ach_cat_cs_guid = ensure_meta_file("Assets/Game/Progression/Achievements/AchievementCatalogue.cs", False)
    level_prog_cs_guid = ensure_meta_file("Assets/Game/Progression/XP/LevelProgressionConfig.cs", False)

    print(f"MissionData.cs GUID: {mission_data_cs_guid}")
    print(f"MissionCatalogue.cs GUID: {mission_cat_cs_guid}")
    print(f"RewardData.cs GUID: {reward_data_cs_guid}")

    # 3. Create LevelProgressionConfig.asset
    level_prog_asset = "Assets/Game/Progression/XP/LevelProgressionConfig.asset"
    level_prog_content = f"""%YAML 1.1
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
  m_Script: {{fileID: 11500000, guid: {level_prog_cs_guid}, type: 3}}
  m_Name: LevelProgressionConfig
  _baseXP: 500
  _exponent: 1.25
  _flatPerLevel: 250
  _maxLevel: 50
  _unlocks:
  - level: 2
    unlockType: 1
    unlockId: CityRide
    title: City Riding Mode
    description: Master urban traffic and intersections
  - level: 5
    unlockType: 0
    unlockId: Cruiser
    title: Cruiser Motorcycle Tier
    description: Unlock Royal Enfield 350cc Cruiser Class
  - level: 10
    unlockType: 1
    unlockId: GhatRoadRide
    title: Ghat Road Ride
    description: Unlock Western Ghats Mountain Passes
"""
    with open(level_prog_asset, 'w', encoding='utf-8') as f:
        f.write(level_prog_content)
    ensure_meta_file(level_prog_asset, False)

    # 4. Create 15 MissionData Assets
    missions_info = [
        ("Mission_01_VillageMilkRun", "Village Milk Run", "Collect and deliver fresh milk cans through rural roads.", 0, 0, 1, 90.0, 500, 0, 150),
        ("Mission_02_CityPeakTraffic", "Bengaluru Peak Traffic", "Navigate through dense city intersections from Silk Board to Koramangala.", 1, 2, 2, 150.0, 1200, 2, 300),
        ("Mission_03_HighwaySprint", "NH44 Highway Sprint", "Maintain high-speed cruise along the 4-lane expressway.", 2, 1, 3, 180.0, 1500, 3, 350),
        ("Mission_04_OotyGhatClimb", "Ooty 36 Hairpins", "Conquer the 36 steep hairpin bends up the Nilgiris Ghat mountain pass.", 3, 3, 5, 240.0, 2500, 10, 600),
        ("Mission_05_ECRCoastalEndurance", "ECR Coastal Endurance", "Tour the scenic East Coast Road from Chennai to Mahabalipuram.", 4, 2, 4, 300.0, 2000, 5, 500),
        ("Mission_06_TemplePillionEscort", "Temple Pillion Escort", "Carefully transport a passenger to the hilltop temple without sharp sudden jerks.", 5, 1, 2, 160.0, 1000, 2, 250),
        ("Mission_07_SunsetCoupleCruise", "Sunset Couple Cruise", "Ride along the beach coast at sunset with your companion.", 6, 2, 3, 200.0, 1800, 5, 400),
        ("Mission_08_HeritageWaypointHunt", "Hampi Heritage Waypoints", "Locate ancient temple landmarks following GPS route coordinates.", 7, 2, 3, 220.0, 1600, 4, 380),
        ("Mission_09_WayanadForestTrail", "Wayanad Forest Trail", "Explore lush green forest trails and discover hidden roadside viewpoints.", 8, 1, 4, 250.0, 1400, 3, 320),
        ("Mission_10_PambanBridgeTimeTrial", "Pamban Sea Bridge Sprint", "Fast dash against the ocean winds across the famous sea bridge.", 9, 3, 6, 90.0, 2200, 8, 550),
        ("Mission_11_AutoRickshawSlalom", "Auto-Rickshaw Slalom", "Navigate through busy traffic without touching auto-rickshaws.", 10, 4, 7, 120.0, 3000, 15, 750),
        ("Mission_12_CenturyDistanceRide", "Century Distance Ride", "Accumulate 25 kilometers of highway endurance riding.", 11, 2, 5, 0.0, 3500, 20, 800),
        ("Mission_13_DhabaParkingPrecision", "Highway Dhaba Parking", "Carefully park your motorcycle in the marked bay outside a highway tea stall.", 12, 0, 1, 60.0, 800, 0, 200),
        ("Mission_14_ExpressChaiDelivery", "Express Chai Delivery", "Deliver hot snacks and tea before the timer expires.", 13, 3, 4, 100.0, 2000, 5, 480),
        ("Mission_15_DiwaliNightParade", "Diwali Night Parade", "Festive night tour through illuminated market streets.", 14, 2, 3, 180.0, 2500, 12, 650),
    ]

    mission_asset_guids = []

    for item in missions_info:
        file_name, title, desc, cat_idx, diff_idx, req_lvl, time_lim, silv, gold, xp = item
        asset_path = f"Assets/Game/MissionSystem/Data/{file_name}.asset"
        
        # Also create embedded reward data
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
  m_Script: {{fileID: 11500000, guid: {mission_data_cs_guid}, type: 3}}
  m_Name: {file_name}
  _missionId: {file_name}
  _title: {title}
  _description: "{desc}"
  _category: {cat_idx}
  _baseDifficulty: {diff_idx}
  _requiredPlayerLevel: {req_lvl}
  _startLocationName: Starting Point
  _startPosition: {{x: 0, y: 0, z: 0}}
  _destinationName: Destination
  _destinationPosition: {{x: 0, y: 0, z: 500}}
  _requiredBikeCategory: 0
  _passengerRequirement: {2 if cat_idx in [5, 6] else 0}
  _timeLimitSeconds: {time_lim}
  _failureConfig:
    failOnTimeExpired: {1 if time_lim > 0 else 0}
    failOnOffRoute: 1
    failOnPassengerDismount: {1 if cat_idx in [5, 6] else 0}
    failOnExcessiveDamage: 1
    maxAllowedDamagePercent: 100
    offRouteTimeoutSeconds: 15
  _objectives:
  - objectiveId: obj_dest
    type: 0
    description: Reach the final destination
    targetValue: 1
    currentProgress: 0
    isCompleted: 0
    isOptional: 0
  - objectiveId: obj_checkpoints
    type: 1
    description: Pass through all intermediate checkpoints
    targetValue: 4
    currentProgress: 0
    isCompleted: 0
    isOptional: 0
  _checkpoints:
  - checkpointIndex: 0
    position: {{x: 0, y: 0, z: 100}}
    triggerRadius: 8
    instructionText: Checkpoint 1
  - checkpointIndex: 1
    position: {{x: 5, y: 0, z: 220}}
    triggerRadius: 8
    instructionText: Checkpoint 2
  - checkpointIndex: 2
    position: {{x: -5, y: 0, z: 360}}
    triggerRadius: 8
    instructionText: Checkpoint 3
  - checkpointIndex: 3
    position: {{x: 0, y: 0, z: 500}}
    triggerRadius: 8
    instructionText: Destination Checkpoint
  _reward: {{fileID: 0}}
"""
        with open(asset_path, 'w', encoding='utf-8') as f:
            f.write(content)
        guid = ensure_meta_file(asset_path, False)
        mission_asset_guids.append(guid)
        print(f"Created {file_name}.asset (GUID: {guid})")

    # 5. Create MissionCatalogue.asset aggregating all 15 missions
    catalogue_path = "Assets/Game/MissionSystem/Data/MissionCatalogue.asset"
    catalogue_content = f"""%YAML 1.1
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
  m_Script: {{fileID: 11500000, guid: {mission_cat_cs_guid}, type: 3}}
  m_Name: MissionCatalogue
  _missions:
"""
    for g in mission_asset_guids:
        catalogue_content += f"  - {{fileID: 11400000, guid: {g}, type: 2}}\n"

    with open(catalogue_path, 'w', encoding='utf-8') as f:
        f.write(catalogue_content)
    cat_guid = ensure_meta_file(catalogue_path, False)
    print(f"Created MissionCatalogue.asset with 15 missions (GUID: {cat_guid})")

    # 6. Create AchievementCatalogue.asset
    ach_cat_path = "Assets/Game/Progression/Achievements/AchievementCatalogue.asset"
    ach_cat_content = f"""%YAML 1.1
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
  m_Script: {{fileID: 11500000, guid: {ach_cat_cs_guid}, type: 3}}
  m_Name: AchievementCatalogue
  _achievements: []
"""
    with open(ach_cat_path, 'w', encoding='utf-8') as f:
        f.write(ach_cat_content)
    ensure_meta_file(ach_cat_path, False)
    print("Created AchievementCatalogue.asset")

    print("\nStage 3 Asset generation complete!")

if __name__ == "__main__":
    main()
