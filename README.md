# Indian Bike Life — Stage 1: Core Game Architecture

Welcome to **Indian Bike Life**, a premium Android-only 3D motorcycle simulation game set across the vibrant highways, congested city streets, and scenic rural landscapes of India.

This repository represents **Stage 1: Core Game Architecture**, establishing an enterprise-grade, highly modular, scalable foundation engineered specifically for mobile performance and future gameplay expansions.

---

## 1. Project Purpose

The primary objective of Stage 1 is to build the core foundation without coupling features together into a monolithic structure. Future systems—such as motorcycle physics, showroom customization, traffic AI, dynamic weather, Indian road streaming, missions, character customization, and multiplayer—will integrate cleanly via decoupled interfaces, the centralized Service Locator, and the event architecture.

---

## 2. Technical Specifications & Environment

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
| **Unity Version Target** | **Unity 2022.3 LTS** (e.g. 2022.3.20f1+) or Unity 6 LTS |

---

## 3. Required Unity Packages

The project manifest (`Packages/manifest.json`) is configured with the following packages:

- **Universal Render Pipeline (`com.unity.render-pipelines.universal`)**: Mobile-optimized forward rendering with single-pass lights.
- **Unity New Input System (`com.unity.inputsystem`)**: Modern touch, tilt, and gamepad device handling.
- **TextMeshPro (`com.unity.textmeshpro`)**: Crisp dynamic text rendering.
- **Unity Test Framework (`com.unity.test-framework`)**: NUnit-based unit and integration test suite.
- **UGUI (`com.unity.ugui`)**: Runtime canvas and UI system.

---

## 4. How to Open the Project in Unity Hub

1. Ensure **Unity Hub** is installed on your computer.
2. Install **Unity 2022.3 LTS** (or Unity 6 LTS) with the **Android Build Support** module (including Android SDK & NDK tools and OpenJDK).
3. Open Unity Hub, click **Add** > **Add project from disk**.
4. Select the repository root folder (`indianbike`).
5. Open the project. Unity will import assets, resolve packages from `Packages/manifest.json`, and compile the `IndianBikeLife.Core` assembly definition.

---

## 5. How to Run the Initial Scenes

The scenes are registered in `EditorBuildSettings.asset` in the following order:

```
[0] Assets/Scenes/Boot.unity
[1] Assets/Scenes/Loading.unity
[2] Assets/Scenes/MainMenu.unity
```

### Running from Boot (Standard Flow):
1. In the Unity Project window, navigate to `Assets/Scenes/Boot.unity` and double-click to open it.
2. Press **Play** in the Unity Editor.
3. The `GameBootstrapper` executes at high priority (`executionOrder: -1000`):
   - Initializes `GameLogger`, registers all 7 services in `ServiceLocator`.
   - Initializes `SaveManager`, loads local profile and settings.
   - Applies mobile frame rate (`Application.targetFrameRate = 60`) and URP graphics preset.
   - Automatically transitions to `Loading.unity`.
4. `Loading.unity` displays the loading bar, progress percentage, and Indian riding tips.
5. Upon progress completion, `Loading.unity` smoothly transitions into `MainMenu.unity`.
6. `MainMenu.unity` displays the interactive **Stage 1 Development Dashboard**, featuring live audio volume controls, graphics preset toggle (Low 30 FPS, Medium 60 FPS, High 60 FPS), save data persistence tests, and live performance metrics.

---

## 6. Directory Structure Overview

```
Assets/
├── Art/                        # Models, Materials, Textures, Animations
├── Game/
│   ├── Audio/                  # Audio channels, sound cues, source pools
│   ├── Configuration/          # ScriptableObject configuration assets
│   ├── Core/
│   │   ├── Bootstrapping/      # Application entry point (GameBootstrapper)
│   │   ├── Services/           # Decoupled ServiceLocator
│   │   └── State/              # GameState enum & lifecycle
│   ├── Events/
│   │   ├── Core/               # Strongly-typed C# EventBus
│   │   ├── GameEvents/         # Domain event structs
│   │   └── ScriptableEvents/   # Inspector-friendly GameEvents & Listeners
│   ├── Input/                  # Decoupled motorcycle input contracts
│   ├── Interfaces/             # Contracts for all services & saveables
│   ├── Managers/               # Core managers (Game, Scene, Save, Audio, Input, Perf)
│   ├── SaveSystem/             # Schema-versioned save system & atomic storage
│   ├── SceneManagement/        # Asynchronous loading & additive streaming
│   ├── UI/                     # Loading UI & Development Dashboard
│   └── IndianBikeLife.Core.asmdef
├── Prefabs/                    # Core & UI prefabs
├── Resources/Configurations/   # Default configuration assets
├── Scenes/                     # Boot, Loading, MainMenu scenes
├── Settings/URP/               # Universal Render Pipeline settings
└── Tests/                      # Automated NUnit test suite (EventBus, Save, Config)
```

---

## 7. Next Steps: Stage 2 Readiness

Stage 1 is complete, verified, and ready. Future development can begin on **Stage 2: Motorcycle Physics, Showroom & Controls** without modifying or refactoring any core systems.
