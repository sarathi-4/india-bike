# Indian Bike Life — Project Setup Guide

This guide provides instructions for developers setting up **Indian Bike Life** on their development workstations.

---

## 1. Prerequisites

Before cloning and opening the project, ensure your development machine meets the following requirements:

- **OS**: Windows 10/11 (64-bit), macOS (Apple Silicon or Intel), or Linux (Ubuntu 20.04+).
- **RAM**: Minimum 16 GB (32 GB recommended).
- **GPU**: DirectX 11/12 or Vulkan-compatible dedicated GPU.
- **Git & Git LFS**: Ensure Git is installed with Git LFS (`git lfs install`).
- **Unity Hub**: Version 3.8.0 or later.
- **Unity Editor**: **Unity 2022.3 LTS** (Recommended: `2022.3.20f1` or later LTS patch) with:
  - **Android Build Support**
  - **Android SDK & NDK Tools**
  - **OpenJDK**

---

## 2. Cloning the Repository

Open your terminal or Git client:

```bash
# Clone the repository
git clone https://github.com/sarathi-4/india-bike.git

# Enter the project directory
cd india-bike

# Ensure Git LFS hooks are active
git lfs pull
```

---

## 3. Opening the Project in Unity Hub

1. Launch **Unity Hub**.
2. Click the **Projects** tab.
3. Click the **Add** dropdown at the top right and select **Add project from disk**.
4. Browse to the folder where you cloned `india-bike` and click **Select Folder**.
5. Ensure the Editor Version shows `2022.3.x (LTS)`.
6. Click the project name to open it.
7. Upon first launch, Unity will import assets, resolve packages from `Packages/manifest.json`, and generate the project library. This may take 2–5 minutes.

---

## 4. Switching Platform to Android

The project is pre-configured for Android. To verify:

1. In the Unity Editor menu, go to **File > Build Settings**.
2. Select **Android** in the Platform list.
3. If Android is not the active platform, click **Switch Platform**.
4. Verify the Scenes in Build:
   - `Assets/Scenes/Boot.unity` (Index 0)
   - `Assets/Scenes/Loading.unity` (Index 1)
   - `Assets/Scenes/MainMenu.unity` (Index 2)

---

## 5. Testing the Project in the Editor

1. Open `Assets/Scenes/Boot.unity`.
2. Click the **Play** button in the Editor toolbar.
3. Observe the Console logs:
   ```
   [IndianBikeLife] [Bootstrap] [INFO] ==================================================
   [IndianBikeLife] [Bootstrap] [INFO]    INDIAN BIKE LIFE — STARTING ENGINE BOOTSTRAP   
   [IndianBikeLife] [Bootstrap] [INFO] ==================================================
   [IndianBikeLife] [Bootstrap] [INFO] Registered 7 core services in ServiceLocator.
   ```
4. The scene will automatically advance from `Boot` to `Loading`, and then into `MainMenu`.
5. In `MainMenu`, interact with the **Development Dashboard**:
   - Adjust volume sliders to verify the `AudioManager`.
   - Toggle Graphics Presets (`Low 30 FPS`, `Medium 60 FPS`, `High 60 FPS`).
   - Click **Save to Disk** and check that `indian_bike_save.json` writes successfully.
   - Press **F3** to toggle the diagnostic Performance HUD.

---

## 6. Running Automated Tests

1. In the Unity Editor menu, go to **Window > General > Test Runner**.
2. Select the **EditMode** tab.
3. Expand `IndianBikeLife.Tests`.
4. Click **Run All** to execute the tests:
   - `EventBusTests`
   - `SaveSystemTests`
   - `ServiceLocatorTests`
   - `ConfigurationTests`
   - `InputStateTests`
5. All tests should pass with green checkmarks.

---

## 7. Android Device Deployment

1. Enable **Developer Options** and **USB Debugging** on your Android smartphone or tablet.
2. Connect your device via USB.
3. In Unity, go to **File > Build Settings**.
4. Set **Run Device** to your connected Android phone.
5. Click **Build and Run**.
6. Select an output APK filename (e.g., `IndianBikeLife_Stage1.apk`).
7. Unity will compile via IL2CPP and deploy the package directly to your Android device.

