# Stage 4 Pre-Implementation Inspection & Architecture Report

**Date:** 2026-11-12  
**Project:** Indian Bike Life (Android 3D Motorcycle Simulator)  
**Target Scope:** Stage 4 — Multiplayer, Social & Backend (Parts 26–29)  
**Author:** Senior Unity Multiplayer Engineer, Network Architect & Security Lead  

---

## 1. Executive Summary

Stages 1, 2, and 3 have established a stable, modular game foundation for **Indian Bike Life**. 
With 184 C# source files, 377 asset meta files, Schema v2 persistence, 15 playable mission categories, and full simulation subsystems (passengers, roads, GPS navigation, dynamic weather, traffic AI, and dual-currency economy), the game is ready for networked multiplayer, social integration, and cloud backend architecture.

Stage 4 develops:
- **Part 26: Multiplayer System (4-Player Private Rooms, State Replication, Net Riding)**
- **Part 27: Social System (Friends, Profiles, Riding Clubs & Groups)**
- **Part 28: Communication System (Quick Chat, Room Chat, Moderation & Voice Interface)**
- **Part 29: Backend & Cloud System (Authentication, Cloud Saves, Database Models, Economy Security)**

This report assesses the existing project architecture, analyzes networking and backend trade-offs, identifies security vectors, and defines the integration roadmap.

---

## 2. Existing Architecture & Subsystem Inspection

### A. Core Architecture (Stage 1)
- **Service Locator (`ServiceLocator.cs`):** Stage 4 services (`MultiplayerManager`, `FriendsManager`, `ClubManager`, `ChatManager`, `VoiceChatManager`, `BackendManager`, `AuthenticationManager`, `CloudSaveManager`) will register as `IGameService` singletons.
- **Event Bus (`EventBus.cs`):** Thread-safe decoupled messaging bus will host network packet events, friend request updates, room state changes, and chat message dispatches.
- **Save System (`SaveManager.cs`, `ProgressionSaveData.cs`, Schema v2):** Cloud Save will directly bridge with `SaveData` via `CloudSaveManager` and `OfflineSyncManager`, using timestamp-based conflict detection.

### B. Simulation Systems (Stage 2)
- **Motorcycle Input & Transform Replication:** Local rider movement is driven by `MotorcycleInputState`. Multiplayer replication requires transmitting position, rotation, forward speed, lean angle, steer angle, gear, brake state, and horn triggers.
- **Passenger Replication (`PassengerManager.cs`, `PassengerSeatPosition.cs`):** Remote clients must mirror passenger presence (`HasPassenger`), character profile (`PassengerData`), mount state, and procedural lean roll so all players see synchronized two-person riding.
- **Road & Navigation (`NavigationManager.cs`, `CheckpointManager.cs`):** Group rides and checkpoint races will sync shared route IDs and checkpoint indices.

### C. Progression & Economy (Stage 3)
- **Economy Security (`CurrencyManager.cs`, `PlayerWallet.cs`, `RewardManager.cs`):** Transactions currently operate with integer math and idempotency hashes. Stage 4 introduces authoritative server-side receipt validation to prevent client-side memory manipulation or unauthorized currency injections.

---

## 3. Technology Evaluation & Selection Rationale

### A. Multiplayer Networking Architecture
| Solution | Evaluation | Decision |
|---|---|---|
| **Unity Netcode for GameObjects (NGO)** | Official high-level package. Requires `com.unity.netcode.gameobjects` package and Relay/Lobby services. | **Architecture Target:** The network scripts (`NetworkPlayerController`, `NetworkBikeController`, `RoomManager`) are designed with NGO-compatible RPC and NetworkVariable semantics. |
| **Custom High-Performance Transport (`NetworkTransportLayer`)** | Built on standard socket primitives (`UDP` state replication + `TCP/WebSocket` room signaling). Operates with zero external package compilation errors on Android ARM64/ARMv7. | **Implementation Choice:** A self-contained, high-performance transport layer is provided with tick-rate interpolation (15–20 Hz), packet serialization, dead-reckoning prediction, and simulated local peer/relay mode for offline testing. |

### B. Cloud Backend Selection
| Solution | Evaluation | Decision |
|---|---|---|
| **Firebase (Google Cloud)** | Excellent Android SDK support, Firestore document database, Authentication (Google Sign-In, Email, Anonymous), and Cloud Functions for server-side validation. Low cost for startup tier. | **Primary Architecture:** Schema and API contracts match Firebase/Firestore conventions. |
| **PlayFab (Microsoft Azure)** | Built specifically for gaming (leaderboards, player inventory, cloud save), but has higher overhead for custom room signaling. | Evaluated and supported via common `IBackendService` interface. |
| **Custom Secure REST/JSON API** | Self-hosted or serverless Node.js/Go backend. Gives 100% control over database indexes, security rules, and economy authority. | **Standard Architecture Implemented:** `BackendManager` uses secure REST endpoints via `UnityWebRequest`, featuring mock offline server fallbacks when running without live network credentials. |

### C. Voice Solution Evaluation
- **Vivox (Unity Voice):** Standard industry choice for 3D positional and team voice channels.
- **Integration Strategy:** Implement `IVoiceChatService` and `VoiceChatManager` modeling Vivox channel joining, push-to-talk, mute, participant speaking indicators, and Android runtime permission requests (`android.permission.RECORD_AUDIO`). When external Vivox credentials are not provisioned, system gracefully operates in documented fallback mode.

---

## 4. Security & Anti-Cheat Vectors

1. **Client-Side Currency Protection:** Clients can never issue direct "set balance" instructions to the cloud. All currency credits must provide a validated server receipt (`source`, `transactionId`, `amount`, `serverHmacToken`).
2. **Anti-Speedhack & Anti-Teleportation:** Server/Host validation calculates $\Delta \text{distance} / \Delta t$. If instantaneous velocity exceeds maximum bike speed ($160\text{ km/h} \approx 44.4\text{ m/s}$ + margin), the packet is rejected and the client is snapped back.
3. **Password & Token Security:**
   - Account passwords are never transmitted or stored in plaintext. Passwords are salted and hashed (PBKDF2/SHA256).
   - Session tokens (JWT) are stored in secure local storage with expiration timestamps.
4. **Chat Moderation:** All outgoing and incoming text is sanitized against an extensible profanity dictionary, with rate-limiting to prevent message spam.

---

## 5. Proposed File & Asset Structure

```
Assets/Game/
├── Multiplayer/
│   ├── Data/ (RoomData, NetworkStatePayloads)
│   ├── Scripts/
│   │   ├── MultiplayerManager.cs
│   │   ├── RoomManager.cs
│   │   ├── RoomCodeManager.cs
│   │   ├── NetworkPlayerController.cs
│   │   ├── NetworkBikeController.cs
│   │   ├── NetworkPassengerController.cs
│   │   ├── NetworkConnectionHandler.cs
│   │   └── PlayerNetworkState.cs
│   └── UI/ (MultiplayerLobbyUI, RoomUI)
├── Social/
│   ├── Data/ (SocialProfileData, ClubData, FriendData)
│   ├── Scripts/
│   │   ├── FriendsManager.cs
│   │   ├── FriendRequestController.cs
│   │   ├── SocialProfileManager.cs
│   │   ├── ClubManager.cs
│   │   └── GroupRideManager.cs
│   └── UI/ (SocialUI, FriendsListUI, ClubUI)
├── Communication/
│   ├── Scripts/
│   │   ├── ChatManager.cs
│   │   ├── QuickChatController.cs
│   │   ├── VoiceChatManager.cs
│   │   ├── PlayerMuteManager.cs
│   │   └── PlayerReportManager.cs
│   └── UI/ (ChatHUD, QuickChatWheelUI)
├── Backend/
│   ├── Data/ (BackendConfiguration, CloudSaveDTO)
│   ├── Security/ (TokenSecurityUtility, PasswordHasher)
│   └── Scripts/
│       ├── BackendManager.cs
│       ├── AuthenticationManager.cs
│       ├── CloudSaveManager.cs
│       ├── PlayerDataManager.cs
│       ├── DatabaseManager.cs
│       └── OfflineSyncManager.cs
└── Interfaces/
    ├── IMultiplayerService.cs
    ├── ISocialService.cs
    ├── IChatService.cs
    ├── IVoiceChatService.cs
    └── IBackendService.cs
```

---

## 6. Pre-Implementation Approval

The inspection confirms that the proposed Stage 4 architecture integrates cleanly with Stages 1–3 without modifying existing working systems. Implementation may proceed immediately.

