# Google Play Release Checklist & Store Metadata

**Date:** 2026-11-12  
**Project:** Indian Bike Life (Android 3D Motorcycle Simulator)  
**Package:** `com.indianbikelife.game`  
**Version:** `1.0.0` (Version Code: 1)  
**Author:** Google Play Release Specialist & Build Engineer  

---

## 1. Release Package Configuration

| Parameter | Configuration Value | Conformance Check |
|---|---|:---:|
| **Package Name** | `com.indianbikelife.game` | **PASS** |
| **App Name** | Indian Bike Life | **PASS** |
| **Version Name** | `1.0.0` | **PASS** |
| **Version Code** | `1` | **PASS** |
| **Min SDK Version** | `26` (Android 8.0 Oreo) | **PASS** |
| **Target SDK Version** | `34` (Android 14) | **PASS** |
| **Scripting Backend** | IL2CPP | **PASS** |
| **Architectures** | `ARM64-v8a` + `ARMEABI-v7a` | **PASS** |
| **Build Artifact** | `Builds/Android/IndianBikeLife_v1.0.0_Release.aab` | **PASS** |
| **Stripping Protection** | `Assets/link.xml` verified | **PASS** |

---

## 2. Store Listing Metadata Draft

### 2.1 Title & Short Description
- **App Title (30 characters max):**  
  `Indian Bike Life: 3D Simulator` (30/30 chars)
- **Short Description (80 characters max):**  
  `Ride iconic Indian bikes across ghats, highways & cities. Multiplayer & offline!` (79/80 chars)

### 2.2 Full Description (Formatted for Google Play)
```markdown
Experience the authentic thrill of motorcycle riding across India with Indian Bike Life!

From the misty hairpin curves of the Western Ghats and congested Mumbai street traffic to scenic Goa coastal highways and rugged Ladakh mountain passes, ride iconic Indian motorcycles with realistic physics, passenger dynamics, and stunning weather.

🏍️ ICONIC MOTORCYCLES & CUSTOMIZATION
• Ride Royal Cruisers, High-Revving Street Fighters, Sport Tourers, and Daily Commuters.
• Tune your engine, suspension, brakes, and exhaust notes.
• Customize liveries, helmets, riding jackets, and authentic Indian pressure horns!

👫 TWO-PERSON PASSENGER SYSTEM
• Ride solo or take your pillion passenger on scenic long-distance road trips.
• Realistic passenger leaning into turns, inertia weight transfer, and camera angles.

🗺️ VIBRANT INDIAN ROADS & DESTINATIONS
• Explore national highways, rural village roads, ghat curves, and bustling city centers.
• Authentic roadside scenery: Highway dhabas, tea stalls, roadside temples, and lush banyan trees.
• Dynamic left-hand Indian traffic featuring auto-rickshaws, city buses, heavy trucks, and scooters.

🌦️ DYNAMIC WEATHER & DAY/NIGHT CYCLE
• Experience dynamic monsoon thunderstorms, heavy rain with wet road grip modifiers, winter fog, and clear skies.
• Seamless morning, golden sunset, and night riding with active motorcycle headlights.

🏁 15 MISSION CATEGORIES & MULTIPLAYER
• Take on highway sprints, Ghat time trials, delivery challenges, and couple riding tours.
• Host 4-player private multiplayer rooms with easy 6-character room codes.
• Real-time convoy riding, Indian quick chat phrases ("Horn OK Please!"), and voice chat.
• Full offline mode: Ride anywhere without internet!

Download Indian Bike Life today and start your journey across the roads of India!
```

---

## 3. Visual Assets Checklist

- [x] **App Icon:** $512 \times 512$ 32-bit PNG with no transparency.
- [x] **Feature Graphic:** $1024 \times 500$ 24-bit PNG/JPEG banner highlighting motorcycle convoy and Western Ghats scenery.
- [ ] **Screenshots (Minimum 4 Landscape):**
  - Screenshot 1: Royal Cruiser motorcycle on Western Ghats hairpin curve with pillion passenger.
  - Screenshot 2: City traffic navigation with auto-rickshaws and buses.
  - Screenshot 3: Motorcycle garage & tuning customization screen.
  - Screenshot 4: Monsoon rain weather effects and wet road reflections.
  - Screenshot 5: 4-Player multiplayer convoy ride on NH44 highway.

---

## 4. Google Play Data Safety Declaration

| Data Category | Data Type | Purpose | Ephemeral? | Shared with Third Parties? |
|---|---|---|:---:|:---:|
| **Identifiers** | User ID / Device Token | Account Auth & Saves | No | No |
| **Financial Info** | In-App Purchase Records | Virtual Wallet & Receipts | No | No |
| **Audio** | Voice Chat Microphone | Multiplayer In-Game Voice | **Yes** (Real-time stream, never stored) | No |
| **App Activity** | Completed Missions / XP | Progression & Cloud Saves | No | No |

- **Security Practices:**
  - All data is encrypted in transit using HTTPS/TLS 1.3.
  - Users can request complete account and data deletion in-game or via privacy URL.

---

## 5. Keystore & Signing Preparation

> [!CAUTION]
> Never commit production keystores (`.keystore` / `.jks`) or release passwords to public source control.

To generate the release signing key locally:
```bash
keytool -genkey -v -keystore indianbikelife-release.keystore \
  -alias ibl_release_key -keyalg RSA -keysize 2048 -validity 10000 \
  -dname "CN=IndianBikeLife, OU=Gaming, O=IndianBikeLife, L=Mumbai, ST=Maharashtra, C=IN"
```

To sign the AAB before Google Play Console upload:
```bash
jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 \
  -keystore indianbikelife-release.keystore \
  Builds/Android/IndianBikeLife_v1.0.0_Release.aab ibl_release_key
```

---

## 6. Pre-Launch Track Roadmap

1. **Internal Testing Track:** Upload `IndianBikeLife_v1.0.0_Release.aab` for initial QA verification on 5 physical devices.
2. **Closed Alpha Track:** 20 invited testers evaluating multiplayer connectivity across Indian telecom networks (Jio, Airtel, Vi).
3. **Open Beta Track:** Public test track for community feedback on motorcycle physics and device framerates.
4. **Production Release:** Full rollout following approval from Google Play Policy Review.

