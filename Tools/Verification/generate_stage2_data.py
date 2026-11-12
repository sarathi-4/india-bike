import os
import hashlib
import json

WORKSPACE = "/run/media/sarathi/2-250/indianbike"

def get_guid(rel_path):
    rel_path = rel_path.replace("\\", "/")
    if rel_path == "Assets/Scenes/RegionalRideTest.unity":
        return "e0000000000000000000000000000004"
    return hashlib.md5(rel_path.encode('utf-8')).hexdigest()

def write_asset(rel_path, content):
    full_path = os.path.join(WORKSPACE, rel_path)
    os.makedirs(os.path.dirname(full_path), exist_ok=True)
    with open(full_path, "w", encoding="utf-8") as f:
        f.write(content)

# 1. Generate Passenger Assets
priya_content = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: ca9978a88b71bdb5eed4b4cef5a33b4a, type: 3}
  m_Name: Priya
  _passengerId: passenger_priya
  _displayName: Priya
  _gender: 1
  _outfitStyle: Casual Kurti & Jeans
  _hasHelmet: 1
  _helmetStyle: Glossy Crimson Full-Face
  _accessories: College Backpack
  _clothingTint: {r: 0.9, g: 0.25, b: 0.35, a: 1}
  _preferredHoldStyle: 0
  _weightKg: 54
  _leanReactionMultiplier: 1.05
"""

rahul_content = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: ca9978a88b71bdb5eed4b4cef5a33b4a, type: 3}
  m_Name: Rahul
  _passengerId: passenger_rahul
  _displayName: Rahul
  _gender: 0
  _outfitStyle: Denim Jacket & Cargo Pants
  _hasHelmet: 1
  _helmetStyle: Matte Charcoal Full-Face
  _accessories: Riding Gloves
  _clothingTint: {r: 0.2, g: 0.4, b: 0.7, a: 1}
  _preferredHoldStyle: 1
  _weightKg: 68
  _leanReactionMultiplier: 1.0
"""

ananya_content = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: ca9978a88b71bdb5eed4b4cef5a33b4a, type: 3}
  m_Name: Ananya
  _passengerId: passenger_ananya
  _displayName: Ananya
  _gender: 1
  _outfitStyle: Leather Touring Jacket
  _hasHelmet: 1
  _helmetStyle: White Graphic Visor
  _accessories: Camera Sling
  _clothingTint: {r: 0.95, g: 0.75, b: 0.2, a: 1}
  _preferredHoldStyle: 0
  _weightKg: 52
  _leanReactionMultiplier: 1.1
"""

vikram_content = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: ca9978a88b71bdb5eed4b4cef5a33b4a, type: 3}
  m_Name: Vikram
  _passengerId: passenger_vikram
  _displayName: Vikram
  _gender: 0
  _outfitStyle: Casual Hoodie & Boots
  _hasHelmet: 1
  _helmetStyle: Sport Modular Helmet
  _accessories: Touring Tail Bag
  _clothingTint: {r: 0.3, g: 0.6, b: 0.35, a: 1}
  _preferredHoldStyle: 1
  _weightKg: 72
  _leanReactionMultiplier: 0.95
"""

write_asset("Assets/Game/PassengerSystem/Data/Priya.asset", priya_content)
write_asset("Assets/Game/PassengerSystem/Data/Rahul.asset", rahul_content)
write_asset("Assets/Game/PassengerSystem/Data/Ananya.asset", ananya_content)
write_asset("Assets/Game/PassengerSystem/Data/Vikram.asset", vikram_content)

priya_guid = get_guid("Assets/Game/PassengerSystem/Data/Priya.asset")
rahul_guid = get_guid("Assets/Game/PassengerSystem/Data/Rahul.asset")
ananya_guid = get_guid("Assets/Game/PassengerSystem/Data/Ananya.asset")
vikram_guid = get_guid("Assets/Game/PassengerSystem/Data/Vikram.asset")

passenger_selection_content = f"""%YAML 1.1
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
  m_Script: {{fileID: 11500000, guid: e00504de2901af4ca0ce9ae2725dbf52, type: 3}}
  m_Name: PassengerSelection
  _activePairing: 1
  _driverGender: 0
  _passengerRoster:
  - {{fileID: 11400000, guid: {priya_guid}, type: 2}}
  - {{fileID: 11400000, guid: {rahul_guid}, type: 2}}
  - {{fileID: 11400000, guid: {ananya_guid}, type: 2}}
  - {{fileID: 11400000, guid: {vikram_guid}, type: 2}}
  _selectedPassengerIndex: 0
"""
write_asset("Assets/Game/PassengerSystem/Data/PassengerSelection.asset", passenger_selection_content)

# 2. Generate Map Destinations and Priorities
def make_dest(dest_id, name, desc, env_type, elev, dist, scene="RegionalRideTest", playable=1):
    return f"""%YAML 1.1
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
  m_Script: {{fileID: 11500000, guid: 9ce818f97737f6214f1a3b9ec17b23f2, type: 3}}
  m_Name: {name}
  _destinationId: {dest_id}
  _destinationName: {name}
  _description: {desc}
  _primaryEnvironment: {env_type}
  _elevationMeters: {elev}
  _distanceFromStateCapitalKm: {dist}
  _worldAnchorPosition: {{x: 0, y: {elev * 0.1}, z: {dist * 10}}}
  _sceneName: {scene}
  _isPlayableInStage2: {playable}
  _recommendedBikes: Classic 350, Himalayan, Duke 390
"""

# Tamil Nadu Destinations
d_ooty = make_dest("ooty_ghat", "Ooty Hill Ghat", "Famous 36 hairpin bends through eucalyptus forests", 3, 2240, 535)
d_ecr = make_dest("ecr_coastal", "ECR Coastal Highway", "Scenic coastal expressway from Chennai to Mahabalipuram", 4, 10, 55)
d_kodai = make_dest("kodaikanal_ghat", "Kodaikanal Ghat", "Misty pine ridges and scenic waterfalls", 3, 2133, 520)
d_yercaud = make_dest("yercaud_loop", "Yercaud 20 Hairpins", "20 winding hairpin bends ascending Shevaroy Hills", 3, 1515, 360)
d_ramesh = make_dest("rameswaram_bridge", "Pamban Sea Bridge", "Spectacular road bridge across the Indian Ocean", 4, 5, 560)

# Kerala Destinations
d_munnar = make_dest("munnar_tea", "Munnar Tea Estate Ghat", "Lush emerald rolling hills and winding fog tracks", 3, 1600, 280)
d_wayanad = make_dest("wayanad_pass", "Thamarassery Ghat Pass", "Challenging 9 hairpin bends through rainforest canopy", 3, 900, 390)
d_varkala = make_dest("varkala_cliff", "Varkala Cliff Road", "Red laterite cliffs overlooking the Arabian Sea", 4, 30, 45)

# Karnataka Destinations
d_coorg = make_dest("coorg_coffee", "Coorg Coffee Trails", "Winding roads through aromatic coffee plantations", 3, 1150, 250)
d_hampi = make_dest("hampi_ruins", "Hampi Boulder Highway", "Ancient UNESCO stone temples and granite boulders", 5, 467, 340)

# Andhra Pradesh Destinations
d_araku = make_dest("araku_valley", "Araku Valley Ghat", "Eastern Ghats misty curves and tribal coffee gardens", 3, 910, 640)

# Telangana Destinations
d_hyd_orr = make_dest("hyderabad_orr", "Hyderabad Outer Ring Road", "8-lane modern expressway encircling the tech city", 1, 540, 15)

write_asset("Assets/Game/MapSystem/Regions/TamilNadu_Ooty.asset", d_ooty)
write_asset("Assets/Game/MapSystem/Regions/TamilNadu_ECR.asset", d_ecr)
write_asset("Assets/Game/MapSystem/Regions/TamilNadu_Kodaikanal.asset", d_kodai)
write_asset("Assets/Game/MapSystem/Regions/TamilNadu_Yercaud.asset", d_yercaud)
write_asset("Assets/Game/MapSystem/Regions/TamilNadu_Rameswaram.asset", d_ramesh)
write_asset("Assets/Game/MapSystem/Regions/Kerala_Munnar.asset", d_munnar)
write_asset("Assets/Game/MapSystem/Regions/Kerala_Wayanad.asset", d_wayanad)
write_asset("Assets/Game/MapSystem/Regions/Kerala_Varkala.asset", d_varkala)
write_asset("Assets/Game/MapSystem/Regions/Karnataka_Coorg.asset", d_coorg)
write_asset("Assets/Game/MapSystem/Regions/Karnataka_Hampi.asset", d_hampi)
write_asset("Assets/Game/MapSystem/Regions/Andhra_Araku.asset", d_araku)
write_asset("Assets/Game/MapSystem/Regions/Telangana_HyderabadORR.asset", d_hyd_orr)

# Generate 5 Priority State Assets and 23 other State Assets
all_states = [
    ("tamil_nadu", "Tamil Nadu", 0, "Chennai", 1),
    ("kerala", "Kerala", 0, "Thiruvananthapuram", 1),
    ("karnataka", "Karnataka", 0, "Bengaluru", 1),
    ("andhra_pradesh", "Andhra Pradesh", 0, "Amaravati", 1),
    ("telangana", "Telangana", 0, "Hyderabad", 1),
    ("maharashtra", "Maharashtra", 2, "Mumbai", 0),
    ("goa", "Goa", 2, "Panaji", 0),
    ("gujarat", "Gujarat", 2, "Gandhinagar", 0),
    ("rajasthan", "Rajasthan", 1, "Jaipur", 0),
    ("punjab", "Punjab", 1, "Chandigarh", 0),
    ("haryana", "Haryana", 1, "Chandigarh", 0),
    ("himachal_pradesh", "Himachal Pradesh", 1, "Shimla", 0),
    ("uttarakhand", "Uttarakhand", 1, "Dehradun", 0),
    ("uttar_pradesh", "Uttar Pradesh", 1, "Lucknow", 0),
    ("madhya_pradesh", "Madhya Pradesh", 4, "Bhopal", 0),
    ("chhattisgarh", "Chhattisgarh", 4, "Raipur", 0),
    ("bihar", "Bihar", 3, "Patna", 0),
    ("jharkhand", "Jharkhand", 3, "Ranchi", 0),
    ("odisha", "Odisha", 3, "Bhubaneswar", 0),
    ("west_bengal", "West Bengal", 3, "Kolkata", 0),
    ("sikkim", "Sikkim", 5, "Gangtok", 0),
    ("assam", "Assam", 5, "Dispur", 0),
    ("meghalaya", "Meghalaya", 5, "Shillong", 0),
    ("arunachal_pradesh", "Arunachal Pradesh", 5, "Itanagar", 0),
    ("nagaland", "Nagaland", 5, "Kohima", 0),
    ("manipur", "Manipur", 5, "Imphal", 0),
    ("mizoram", "Mizoram", 5, "Aizawl", 0),
    ("tripura", "Tripura", 5, "Agartala", 0),
]

all_uts = [
    ("delhi", "Delhi NCR", 6, "New Delhi"),
    ("jammu_kashmir", "Jammu and Kashmir", 6, "Srinagar"),
    ("ladakh", "Ladakh", 6, "Leh"),
    ("puducherry", "Puducherry", 6, "Puducherry"),
    ("chandigarh", "Chandigarh", 6, "Chandigarh"),
    ("andaman_nicobar", "Andaman and Nicobar Islands", 6, "Port Blair"),
    ("dadra_nagar_haveli_daman_diu", "Dadra and Nagar Haveli and Daman and Diu", 6, "Daman"),
    ("lakshadweep", "Lakshadweep", 6, "Kavaratti"),
]

state_guids = []
for s_id, s_name, zone, capital, priority in all_states:
    path = f"Assets/Game/MapSystem/Regions/State_{s_id}.asset"
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
  m_Script: {{fileID: 11500000, guid: b528c84ae0bd13daf247d8c5554fe1b3, type: 3}}
  m_Name: {s_name}
  _stateId: {s_id}
  _stateName: {s_name}
  _zone: {zone}
  _capitalName: {capital}
  _isPriorityStage2State: {priority}
  _districts: []
"""
    write_asset(path, content)
    state_guids.append(get_guid(path))

ut_guids = []
for ut_id, ut_name, zone, capital in all_uts:
    path = f"Assets/Game/MapSystem/Regions/UT_{ut_id}.asset"
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
  m_Script: {{fileID: 11500000, guid: b528c84ae0bd13daf247d8c5554fe1b3, type: 3}}
  m_Name: {ut_name}
  _stateId: {ut_id}
  _stateName: {ut_name}
  _zone: {zone}
  _capitalName: {capital}
  _isPriorityStage2State: 0
  _districts: []
"""
    write_asset(path, content)
    ut_guids.append(get_guid(path))

# Master Map Catalogue
map_cat_lines = [
    "%YAML 1.1",
    "%TAG !u! tag:unity3d.com,2011:",
    "--- !u!114 &11400000",
    "MonoBehaviour:",
    "  m_ObjectHideFlags: 0",
    "  m_CorrespondingSourceObject: {fileID: 0}",
    "  m_PrefabInstance: {fileID: 0}",
    "  m_PrefabAsset: {fileID: 0}",
    "  m_GameObject: {fileID: 0}",
    "  m_Enabled: 1",
    "  m_EditorHideFlags: 0",
    "  m_Script: {fileID: 11500000, guid: bcf77a8f59dc2a475094e2186149868b, type: 3}",
    "  m_Name: IndiaMapCatalogue",
    "  _states:"
]
for g in state_guids:
    map_cat_lines.append(f"  - {{fileID: 11400000, guid: {g}, type: 2}}")

map_cat_lines.append("  _unionTerritories:")
for g in ut_guids:
    map_cat_lines.append(f"  - {{fileID: 11400000, guid: {g}, type: 2}}")

write_asset("Assets/Game/MapSystem/Data/IndiaMapCatalogue.asset", "\n".join(map_cat_lines) + "\n")

# 3. Generate Riding Modes
modes = [
    (0, "Mode_VillageRide", "Village Ride", "Narrow village roads, huts, slow traffic", 0, 4, 0, 10, 0, 0, "Classic 350"),
    (1, "Mode_CityRide", "City Ride", "Dense urban intersections, auto-rickshaws, traffic lights", 1, 1, 2, 12, 0, 0, "Pulsar 200"),
    (2, "Mode_HighwayRide", "Highway Ride", "High-speed national highways, dhabas, heavy trucks", 3, 0, 1, 14, 0, 0, "Interceptor 650"),
    (3, "Mode_HillStationGhatRide", "Hill Station Ghat", "Curved mountain roads, hairpin bends, misty valleys", 4, 1, 1, 9, 0, 0, "Himalayan 450"),
    (4, "Mode_LongDistanceTrip", "Long-Distance Trip", "Multi-city highway tour with checkpoints", 3, 0, 1, 7, 0, 0, "Super Meteor 650"),
    (5, "Mode_FreeRide", "Free Ride", "Open unconstrained exploration", 0, 0, 1, 15, 0, 0, "Duke 390"),
    (6, "Mode_OffRoadRide", "Off-Road Ride", "Dirt trails, gravel, rugged terrain", 5, 5, 0, 16, 0, 0, "Xpulse 200 4V"),
    (7, "Mode_NightRide", "Night Ride", "Midnight highways, dark winding roads, headlights", 3, 0, 0, 23, 0, 0, "Continental GT"),
    (8, "Mode_RaceTrackRide", "Race Track Ride", "Enclosed circuit track, lap timer foundations", 1, 0, 0, 11, 0, 0, "RC 390"),
    (9, "Mode_CoupleRide", "Couple Ride", "Romantic scenic ride with passenger integration", 4, 0, 1, 17, 0, 1, "Classic 350"),
]

mode_guids = []
for m_idx, m_file, m_title, m_desc, theme, surface, density, hour, rain, pass_req, bike in modes:
    path = f"Assets/Game/RidingModes/Data/{m_file}.asset"
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
  m_Script: {{fileID: 11500000, guid: b1c6d3bc89115b9c0ba9e02c525f2ee3, type: 3}}
  m_Name: {m_title}
  _modeType: {m_idx}
  _modeTitle: {m_title}
  _description: {m_desc}
  _theme: {theme}
  _surfaceType: {surface}
  _trafficDensity: {density}
  _defaultHourOfDay: {hour}
  _enableRainByDefault: {rain}
  _requiresPassenger: {pass_req}
  _recommendedMotorcycle: {bike}
  _isImplementedInStage2: 1
"""
    write_asset(path, content)
    mode_guids.append(get_guid(path))

mode_cat_lines = [
    "%YAML 1.1",
    "%TAG !u! tag:unity3d.com,2011:",
    "--- !u!114 &11400000",
    "MonoBehaviour:",
    "  m_ObjectHideFlags: 0",
    "  m_CorrespondingSourceObject: {fileID: 0}",
    "  m_PrefabInstance: {fileID: 0}",
    "  m_PrefabAsset: {fileID: 0}",
    "  m_GameObject: {fileID: 0}",
    "  m_Enabled: 1",
    "  m_EditorHideFlags: 0",
    "  m_Script: {fileID: 11500000, guid: 2bbf3f07be807e3bc57b295c25ba47ba, type: 3}",
    "  m_Name: RidingModeCatalogue",
    "  _modes:"
]
for g in mode_guids:
    mode_cat_lines.append(f"  - {{fileID: 11400000, guid: {g}, type: 2}}")

write_asset("Assets/Game/RidingModes/Data/RidingModeCatalogue.asset", "\n".join(mode_cat_lines) + "\n")

# 4. Generate Weather Configuration Asset
weather_conf_content = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 3684a325163cec9d2d00a25d1c315bf2, type: 3}
  m_Name: WeatherConfiguration
  _settings:
  - weatherType: 0
    rainEmissionRate: 0
    fogDensity: 0.003
    fogColor: {r: 0.7, g: 0.8, b: 0.9, a: 1}
    windSpeedMps: 2.5
    roadGripModifier: 1.0
    hasThunder: 0
    cloudAlpha: 0.1
  - weatherType: 1
    rainEmissionRate: 0
    fogDensity: 0.008
    fogColor: {r: 0.65, g: 0.7, b: 0.75, a: 1}
    windSpeedMps: 4.5
    roadGripModifier: 0.95
    hasThunder: 0
    cloudAlpha: 0.5
  - weatherType: 3
    rainEmissionRate: 150
    fogDensity: 0.015
    fogColor: {r: 0.55, g: 0.6, b: 0.65, a: 1}
    windSpeedMps: 6.0
    roadGripModifier: 0.80
    hasThunder: 0
    cloudAlpha: 0.7
  - weatherType: 4
    rainEmissionRate: 500
    fogDensity: 0.025
    fogColor: {r: 0.45, g: 0.5, b: 0.55, a: 1}
    windSpeedMps: 10.0
    roadGripModifier: 0.65
    hasThunder: 0
    cloudAlpha: 0.9
  - weatherType: 5
    rainEmissionRate: 750
    fogDensity: 0.030
    fogColor: {r: 0.35, g: 0.4, b: 0.45, a: 1}
    windSpeedMps: 15.0
    roadGripModifier: 0.60
    hasThunder: 1
    cloudAlpha: 0.95
  - weatherType: 6
    rainEmissionRate: 0
    fogDensity: 0.045
    fogColor: {r: 0.75, g: 0.8, b: 0.85, a: 1}
    windSpeedMps: 1.5
    roadGripModifier: 0.90
    hasThunder: 0
    cloudAlpha: 0.8
"""
write_asset("Assets/Game/WeatherSystem/Data/WeatherConfiguration.asset", weather_conf_content)

# 5. Generate Traffic Configuration Asset
traffic_conf_content = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: c6e0c052a7aed916c5193e2c421590f1, type: 3}
  m_Name: TrafficConfiguration
  _lowDensityMaxVehicles: 8
  _mediumDensityMaxVehicles: 16
  _highDensityMaxVehicles: 28
  _minSpawnDistanceAhead: 40
  _maxSpawnDistanceAhead: 120
  _despawnDistanceBehind: 60
  _vehicleConfigs:
  - vehicleType: 0
    maxSpeedKmph: 45
    acceleration: 2.8
    brakeDeceleration: 6.5
    safeFollowDistanceMeters: 6
    lengthMeters: 2.6
    widthMeters: 1.4
  - vehicleType: 1
    maxSpeedKmph: 55
    acceleration: 2.0
    brakeDeceleration: 5.5
    safeFollowDistanceMeters: 10
    lengthMeters: 10.5
    widthMeters: 2.5
  - vehicleType: 2
    maxSpeedKmph: 50
    acceleration: 1.8
    brakeDeceleration: 5.0
    safeFollowDistanceMeters: 12
    lengthMeters: 9.0
    widthMeters: 2.5
  - vehicleType: 3
    maxSpeedKmph: 65
    acceleration: 4.2
    brakeDeceleration: 7.5
    safeFollowDistanceMeters: 7
    lengthMeters: 3.8
    widthMeters: 1.7
  - vehicleType: 5
    maxSpeedKmph: 50
    acceleration: 3.8
    brakeDeceleration: 7.0
    safeFollowDistanceMeters: 5
    lengthMeters: 1.9
    widthMeters: 0.7
"""
write_asset("Assets/Game/TrafficSystem/Data/TrafficConfiguration.asset", traffic_conf_content)

# 6. Generate Playable Regional Test Scene: RegionalRideTest.unity
def get_scene_header():
    return """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestHole: 0.25
    smallestOccluder: 5
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 1
  m_FogColor: {r: 0.75, g: 0.8, b: 0.85, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.005
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.25, g: 0.28, b: 0.32, a: 1}
  m_AmbientEquatorColor: {r: 0.15, g: 0.18, b: 0.20, a: 1}
  m_AmbientGroundColor: {r: 0.08, g: 0.08, b: 0.08, a: 1}
  m_AmbientIntensity: 1
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 202}
  m_IndirectSpecularColor: {r: 0.45, g: 0.50, b: 0.58, a: 1}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 0
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 3
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    buildHeightMesh: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {fileID: 0}
"""

regional_scene_content = get_scene_header() + """--- !u!1 &100
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 101}
  - component: {fileID: 102}
  - component: {fileID: 103}
  - component: {fileID: 104}
  m_Layer: 0
  m_Name: Main Camera
  m_TagString: MainCamera
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &101
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 100}
  m_LocalRotation: {x: 0.1, y: 0, z: 0, w: 0.995}
  m_LocalPosition: {x: 0, y: 1.8, z: -4.5}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {x: 11.5, y: 0, z: 0}
--- !u!20 &102
Camera:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 100}
  m_Enabled: 1
  serializedVersion: 2
  m_ClearFlags: 1
  m_BackGroundColor: {r: 0.2, g: 0.2, b: 0.2, a: 1}
  m_projectionMatrixMode: 1
  m_GateFitMode: 2
  m_FOVAxisMode: 0
  m_SensorSize: {x: 36, y: 24}
  m_LensShift: {x: 0, y: 0}
  m_FocalLength: 50
  m_NormalizedViewPortRect:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1
    height: 1
  near clip plane: 0.3
  far clip plane: 1500
  field of view: 60
  orthographic: 0
  orthographic size: 5
  m_Depth: -1
  m_CullingMask:
    serializedVersion: 2
    m_Bits: 4294967295
  m_RenderingPath: -1
  m_TargetTexture: {fileID: 0}
  m_TargetDisplay: 0
  m_TargetEye: 3
  m_HDR: 0
  m_AllowMSAA: 1
  m_AllowDynamicResolution: 0
  m_ForceIntoRT: 0
  m_OcclusionCulling: 1
  m_StereoConvergence: 10
  m_StereoSeparation: 0.022
--- !u!81 &103
AudioListener:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 100}
  m_Enabled: 1
--- !u!114 &104
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 100}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: f9fe717499124aa6688ec437beb5afd6, type: 3}
  m_Name: 
  _motorcycleTarget: {fileID: 301}
  _passengerHeadTarget: {fileID: 401}
  _currentView: 2
  _followDistance: 3.8
  _followHeight: 1.5
  _positionSmoothing: 8.0
  _rotationSmoothing: 6.0
--- !u!1 &200
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 201}
  - component: {fileID: 202}
  - component: {fileID: 203}
  m_Layer: 0
  m_Name: Directional Light (Sun/Moon)
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &201
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 200}
  m_LocalRotation: {x: 0.40821788, y: -0.23456968, z: 0.10938163, w: 0.8754261}
  m_LocalPosition: {x: 0, y: 50, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 1
  m_LocalEulerAnglesHint: {x: 50, y: -30, z: 0}
--- !u!108 &202
Light:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 200}
  m_Enabled: 1
  serializedVersion: 10
  m_Type: 1
  m_Shape: 0
  m_Color: {r: 1, g: 0.95, b: 0.9, a: 1}
  m_Intensity: 1.2
  m_Range: 10
  m_SpotAngle: 30
  m_InnerSpotAngle: 21.80208
  m_CookieSize: 10
  m_Shadows:
    m_Type: 2
    m_Resolution: -1
    m_CustomResolution: -1
    m_Strength: 1
    m_Bias: 0.05
    m_NormalBias: 0.4
    m_NearPlane: 0.2
    m_CullingMatrixOverride:
      e00: 1
      e01: 0
      e02: 0
      e03: 0
      e10: 0
      e11: 1
      e12: 0
      e13: 0
      e20: 0
      e21: 0
      e22: 1
      e23: 0
      e30: 0
      e31: 0
      e32: 0
      e33: 1
    m_UseCullingMatrixOverride: 0
  m_Cookie: {fileID: 0}
  m_DrawHalo: 0
  m_Flare: {fileID: 0}
  m_RenderMode: 0
  m_CullingMask:
    serializedVersion: 2
    m_Bits: 4294967295
  m_RenderingLayerMask: 1
  m_Lightmapping: 4
  m_LightShadowCasterMode: 0
  m_AreaSize: {x: 1, y: 1}
  m_BounceIntensity: 1
  m_ColorTemperature: 6570
  m_UseColorTemperature: 0
  m_BoundingSphereOverride: {x: 0, y: 0, z: 0, w: 0}
  m_UseBoundingSphereOverride: 0
  m_UseViewFrustumForShadowCasterCull: 1
  m_ShadowRadius: 0
  m_ShadowAngle: 0
--- !u!114 &203
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 200}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: a20188c60ca1926395175dbcdac07b71, type: 3}
  m_Name: 
  _celestialLight: {fileID: 202}
  _dayIntensity: 1.2
  _nightIntensity: 0.25
--- !u!1 &300
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 301}
  - component: {fileID: 302}
  - component: {fileID: 303}
  m_Layer: 0
  m_Name: '[PlayerMotorcycle]'
  m_TagString: Motorcycle
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &301
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 300}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: -1.8, y: 0.5, z: 10}
  m_LocalScale: {x: 0.8, y: 1.1, z: 2.1}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {fileID: 401}
  m_Father: {fileID: 0}
  m_RootOrder: 2
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!65 &302
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 300}
  m_Material: {fileID: 0}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 0
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {x: 1, y: 1, z: 1}
  m_Center: {x: 0, y: 0, z: 0}
--- !u!114 &303
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 300}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: eaf1204e092a71761755166f00d5877a, type: 3}
  m_Name: 
  _seatAnchor: {fileID: 401}
--- !u!1 &400
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 401}
  - component: {fileID: 402}
  - component: {fileID: 403}
  - component: {fileID: 404}
  m_Layer: 0
  m_Name: PassengerSeatAnchor
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &401
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 400}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0.35, z: -0.4}
  m_LocalScale: {x: 0.7, y: 0.9, z: 0.7}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 301}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &402
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 400}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 165a6b2e95fb76407ae90d2d7259ee91, type: 3}
  m_Name: 
--- !u!114 &403
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 400}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: c44509890b0f138938e73c333ebd3d70, type: 3}
  m_Name: 
--- !u!114 &404
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 400}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 7ca629f0c873c006a8cd6034b2f3a9f7, type: 3}
  m_Name: 
--- !u!1 &500
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 501}
  - component: {fileID: 502}
  - component: {fileID: 503}
  - component: {fileID: 504}
  - component: {fileID: 505}
  - component: {fileID: 506}
  m_Layer: 0
  m_Name: HighwayRoad_NH44
  m_TagString: Road
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &501
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 500}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 3
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &502
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 500}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 6da5382cbc5376b255fbd7e4ef2bffc3, type: 3}
  m_Name: 
  _nodes:
  - position: {x: 0, y: 0, z: 0}
    width: 8
    rollAngle: 0
    speedLimitKmph: 80
  - position: {x: 0, y: 0, z: 100}
    width: 8
    rollAngle: 0
    speedLimitKmph: 80
  - position: {x: 35, y: 12, z: 200}
    width: 7.5
    rollAngle: 4.5
    speedLimitKmph: 50
  - position: {x: 90, y: 28, z: 320}
    width: 7.0
    rollAngle: 8.0
    speedLimitKmph: 40
  _isLoop: 0
  _surfaceType: 0
  _laneCount: 2
  _laneWidth: 3.8
--- !u!33 &503
MeshFilter:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 500}
  m_Mesh: {fileID: 0}
--- !u!23 &504
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 500}
  m_Enabled: 1
  m_CastShadows: 1
  m_ReceiveShadows: 1
  m_DynamicOccludee: 1
  m_StaticShadowCaster: 0
  m_MotionVectors: 1
  m_LightProbeUsage: 1
  m_ReflectionProbeUsage: 1
  m_RayTracingMode: 2
  m_RayTraceProcedural: 0
  m_RenderingLayerMask: 1
  m_RendererPriority: 0
  m_Materials: []
  m_StaticBatchInfo:
    firstSubMesh: 0
    subMeshCount: 0
  m_CheckPreviousWindow: 0
--- !u!114 &505
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 500}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: a2495ed4a260a554c578f5afc30d1273, type: 3}
  m_Name: 
  _segmentsPerCurve: 25
--- !u!114 &506
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 500}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 77dc271fd2e94df33a67bf4396b614b5, type: 3}
  m_Name: 
  _surfaceType: 0
  _speedLimitKmph: 80
--- !u!1 &600
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 601}
  - component: {fileID: 602}
  - component: {fileID: 603}
  - component: {fileID: 604}
  - component: {fileID: 605}
  - component: {fileID: 606}
  - component: {fileID: 607}
  - component: {fileID: 608}
  - component: {fileID: 609}
  - component: {fileID: 610}
  m_Layer: 0
  m_Name: '[Stage2_Managers]'
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &601
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 4
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &602
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 0e5bc492d1a87228d3d23646e735d0fc, type: 3}
  m_Name: 
--- !u!114 &603
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 4e8c9f6e3dda85be1c3b35faf66e4b61, type: 3}
  m_Name: 
--- !u!114 &604
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: ccb318c76fea54a0fb14c778f68ba36b, type: 3}
  m_Name: 
  _playerMotorcycle: {fileID: 301}
--- !u!114 &605
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: b6a96a405df2474a40526b77d87b3afb, type: 3}
  m_Name: 
--- !u!114 &606
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: ea07453f4f8166d880cef3824271ebd3, type: 3}
  m_Name: 
--- !u!114 &607
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 0a0f5896a2ca5cb85848bb017770b0be, type: 3}
  m_Name: 
--- !u!114 &608
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 37ba8e886508cfb92a3d0153f08a1da4, type: 3}
  m_Name: 
--- !u!114 &609
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 2a1ee2e18edb8e77b1d7bed71b114d68, type: 3}
  m_Name: 
--- !u!114 &610
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 600}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 19a1087a6d5dad937f613bc063956eb3, type: 3}
  m_Name: 
  _cameraController: {fileID: 104}
  _motorcycleSeat: {fileID: 303}
"""
write_asset("Assets/Scenes/RegionalRideTest.unity", regional_scene_content)

# 7. Update EditorBuildSettings.asset to include RegionalRideTest.unity at Index 3
build_settings_content = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1045 &1
EditorBuildSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Scenes:
  - m_Enabled: 1
    m_Path: Assets/Scenes/Boot.unity
    m_Guid: e0000000000000000000000000000001
  - m_Enabled: 1
    m_Path: Assets/Scenes/Loading.unity
    m_Guid: e0000000000000000000000000000002
  - m_Enabled: 1
    m_Path: Assets/Scenes/MainMenu.unity
    m_Guid: e0000000000000000000000000000003
  - m_Enabled: 1
    m_Path: Assets/Scenes/RegionalRideTest.unity
    m_Guid: e0000000000000000000000000000004
  m_configObjects: {}
"""
write_asset("ProjectSettings/EditorBuildSettings.asset", build_settings_content)

# 8. Generate .meta files for all new assets and folders
def generate_all_metas():
    assets_dir = os.path.join(WORKSPACE, "Assets")
    count = 0
    for root, dirs, files in os.walk(assets_dir):
        rel_root = os.path.relpath(root, WORKSPACE).replace("\\", "/")
        if rel_root != "Assets":
            meta_path = root + ".meta"
            if not os.path.exists(meta_path):
                guid = get_guid(rel_root)
                with open(meta_path, "w", encoding="utf-8") as f:
                    f.write(f"""fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
""")
                count += 1

        for f in files:
            if f.endswith(".meta"):
                continue
            fpath = os.path.join(root, f)
            meta_path = fpath + ".meta"
            if not os.path.exists(meta_path):
                rel_f = os.path.relpath(fpath, WORKSPACE).replace("\\", "/")
                guid = get_guid(rel_f)
                if f.endswith(".cs"):
                    meta_content = f"""fileFormatVersion: 2
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
                elif f.endswith(".unity"):
                    meta_content = f"""fileFormatVersion: 2
guid: {guid}
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
                elif f.endswith(".asset"):
                    meta_content = f"""fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
                else:
                    meta_content = f"""fileFormatVersion: 2
guid: {guid}
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
                with open(meta_path, "w", encoding="utf-8") as mf:
                    mf.write(meta_content)
                count += 1
    print(f"Generated {count} new .meta files for Stage 2.")

generate_all_metas()
print("Stage 2 assets, configurations, and scenes generated successfully.")
