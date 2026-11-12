# Indian Bike Life — Master Architecture & Systems

Welcome to **Indian Bike Life**, a premium Android-only 3D motorcycle simulation game set across the vibrant highways, congested city streets, hill station ghat roads, and scenic rural landscapes of India.

---

## 1. Project Overview & Progress

- **Stage 1 (Parts 1–14)**: Core Game Architecture, Service Locator, Event Bus, Async Scene Loading, Save System, Audio Foundation, Input Contracts, and Android URP Configuration.
- **Stage 2 (Parts 15–23)**: Passengers, Indian Maps & Environment Systems.
  - **Part 15**: Passenger Riding System (Two-person riding, procedural lean, inertia pitch, seat positioning, cameras).
  - **Part 16**: Indian Map System (28 States + 8 Union Territories catalogue, 5 priority states, chunk streaming).
  - **Part 17**: Real Road System (Procedural splines, left-hand traffic, intersections, surface friction properties).
  - **Part 18**: Navigation System (Turn-by-turn guidance, distance & time estimation, checkpoints, GPS simulator).
  - **Part 19**: Environment System (Indian dhabas, tea stalls, temples, bus stops, palms, banyans, LOD controller).
  - **Part 20**: Traffic AI System (Auto-rickshaws, buses, trucks, cars, scooters, collision avoidance, traffic lights).
  - **Part 21**: 10 Selectable Riding Modes (Village, City, Highway, Hill Station, Long-Distance, Free Ride, Off-Road, Night, Race Track, Couple Ride).
  - **Part 22**: Dynamic Weather System (Sunny, Rainy, Thunderstorm, Fog, Wind, dynamic wet road grip modifier).
  - **Part 23**: Day/Night & Seasons (Sun/moon cycle, night streetlighting, Indian seasonal variations: Summer, Monsoon, Winter).
- **Stage 3 (Parts 24–25)**: Missions, Progression & Rewards.
  - **Part 24**: Data-Driven Mission System supporting 15 categories, 5 difficulty tiers (Beginner to Expert), variable objectives, countdown timers, checkpoint triggers, off-route/damage failure conditions, and daily/weekly challenge managers.
  - **Part 25**: Virtual Economy (Silver & Gold Coins), non-linear XP curve, 7-day consecutive login calendar with anti-cheat clock detection, 8 milestone achievement categories, and local leaderboard ranking cache.
- **Stage 4 (Parts 26–29)**: Multiplayer, Social & Backend.
  - **Part 26**: 4-Player Private Multiplayer Rooms, alphanumeric 6-character room codes, host migration, 15–20 Hz state snapshot broadcast, dead reckoning, client interpolation, and anti-teleport / speedhack validation ($>48\text{ m/s}$ rejected).
  - **Part 27**: Social Profiles (Public, FriendsOnly, Private), friend request lifecycle with spam prevention and blocklists, Riding Clubs with member hierarchy (Leader, Officer, Member), and scheduled group rides with meetup points and RSVPs.
  - **Part 28**: In-game communication with 8 authentic Indian Quick Chat riding phrases, client-side profanity regex filtering, rate limiting (max 2 msgs/sec), player mute/report managers, and Vivox-compatible voice chat abstraction with Android mic permission handling.
  - **Part 29**: Cloud backend supporting 16 Firestore/document collections, guest device ID auth, salted SHA256 email registration/login, account deletion GDPR compliance, cloud saves with timestamp conflict resolution, 50-action FIFO offline sync queue, and transaction HMAC-SHA256 signature verification.

---

## 2. Technical Specifications & Target Platform

| Parameter | Specification |
| :--- | :--- |
| **Project Name** | Indian Bike Life |
| **Package Identifier** | `com.indianbikelife.game` |
| **Target Platform** | Android Only |
| **Target Architecture** | ARM64 (`arm64-v8a`) & ARMv7 (`armeabi-v7a`) |
| **Scripting Backend** | IL2CPP |
| **Target API Level** | Android 14 / 15 (API 34+) |
| **Minimum API Level** | Android 8.0 Oreo (API 26+) |
| **Default Orientation** | Landscape (Sensor Landscape Left & Right) |
| **Render Pipeline** | Universal Render Pipeline (URP 14.x+) |
| **Target Frame Rate** | 60 FPS (Fidelity/Balanced) / 30 FPS (Battery Saver) |
| **Unity Version Target** | **Unity 2022.3 LTS** (or Unity 6 LTS) |

---

## 3. Scenes in Build

Configured in `EditorBuildSettings.asset`:

```
[0] Assets/Scenes/Boot.unity              - Engine Bootstrap & Service Registration
[1] Assets/Scenes/Loading.unity           - Asynchronous Loading Screen & Indian Riding Tips
[2] Assets/Scenes/MainMenu.unity          - Stage 1 Development & Audio/Save Dashboard
[3] Assets/Scenes/RegionalRideTest.unity  - Stage 2, 3 & 4 Playable Environment with Multiplayer & Backend
```

---

## 4. How to Open and Run the Project

1. Open **Unity Hub** and select **Add > Add project from disk**.
2. Select the repository root folder (`indianbike`).
3. Open with **Unity 2022.3 LTS** with Android Build Support installed.
4. **Running the Regional Test Scene (Stage 2, 3 & 4)**:
   - In Unity Project view, open `Assets/Scenes/RegionalRideTest.unity`.
   - Press **Play**.
   - Use the on-screen **Stage 2, 3 & 4 System Controls** panel:
     - **Multiplayer Controls**: Host a 4-player room, join with room code, toggle Ready status, simulate network ride.
     - **Communication Controls**: Trigger Quick Chat phrases (*"Horn OK Please!"*, *"Fuel stop at the Dhaba!"*), toggle mic state.
     - **Backend & Cloud**: Authenticate as Guest, trigger cloud sync, test offline transaction queuing.
     - **Mission Controls**: Start sample Highway or Village missions, complete or fail active missions.
     - **Economy & Progression**: Credit +500 Silver, spend 200 Silver, add +350 XP, claim daily streak rewards.
     - **Mount / Dismount Passenger**: Observe procedural leaning into turns and seat positioning.
     - **Camera Angle**: Toggle between Driver POV, Passenger-focused, Third-Person Chase, and Cinematic views.
     - **Weather Conditions**: Switch between Sunny, Heavy Rain, and Fog; notice wet road friction response.
     - **Time of Day**: Switch between Morning, Noon, Sunset, and Night; observe sun angles and streetlights turning on.
5. **Running from Boot (Full Flow)**:
   - Open `Assets/Scenes/Boot.unity` and press **Play**. The game will bootstrap services, load save profiles, display the loading screen, and advance to `MainMenu`.

---

## 5. Running Automated Tests

### Unity Test Runner:
1. In the Unity Editor menu, select **Window > General > Test Runner**.
2. Under the **EditMode** tab, click **Run All**.
3. All test suites covering EventBus, SaveSystem, ServiceLocator, Configuration, PassengerSystem, MapSystem, RoadSystem, Navigation, Traffic, Weather, MissionSystem, Economy, Multiplayer, Social, Communication, and Backend will execute and pass.

### CLI Verification Suites:
```bash
# 1. Master Architectural Verification Suite (228 C# files, 438 assets)
python3 Tools/Verification/test_stage4_architecture.py

# 2. Stage 4 Functional Simulation Test Runner (22 of 22 Tests)
python3 Tools/Verification/run_stage4_simulation_tests.py

# 3. Stage 3 Functional Simulation Test Runner (18 of 18 Tests)
python3 Tools/Verification/run_stage3_simulation_tests.py
```
