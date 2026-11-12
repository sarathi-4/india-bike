using System;
using System.Collections;
using IndianBikeLife.Backend;
using IndianBikeLife.Core.Configuration;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Managers;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Motorcycle;
using IndianBikeLife.PassengerSystem;
using IndianBikeLife.Progression.Economy;
using IndianBikeLife.Progression.Rewards;
using IndianBikeLife.TimeAndSeasons;
using IndianBikeLife.WeatherSystem;
using UnityEngine;

namespace IndianBikeLife.Core.UI
{
    public enum GameScreen
    {
        SplashScreen = 1,       // 1. Splash Screen
        MainMenu = 2,           // 2. Main Menu
        Garage = 3,             // 3. Garage
        IndiaMap = 4,           // 4. India Map
        RidingHUD = 5,          // 5. Riding HUD
        Missions = 6,           // 6. Missions
        Multiplayer = 7,        // 7. Multiplayer
        Settings = 8,           // 8. Settings
        RewardScreen = 9,       // 9. Reward Screen
        AvatarStudio = 10       // 10. Rider Avatar Customization Studio
    }

    /// <summary>
    /// Master UI System strictly matching the 9-Screen Indian Bike Life Visual Design Target:
    /// Screen 1: Splash Screen with Tricolor Indian Bike Life Logo and 85% Progress Bar
    /// Screen 2: Main Menu with Player001, Lv.5, Wallet, Big Yellow PLAY Button and Side Navigation
    /// Screen 3: Garage with Revolving 3D Showroom, Spec Bars (Speed, Handling, Braking, Fuel), Upgrade/Customize and Bike Carousel
    /// Screen 4: India Map with 28 States & UTs, 3D Geographical Pin and Tamil Nadu Card (Districts: 38, Locations: 250+)
    /// Screen 5: Riding HUD with GPS Mini-map, Speedometer (120 km/h, Gear 5), Fuel Bar, Throttle & Brake Pedals
    /// Screen 6: Missions with Story/Daily/Weekly Tabs, Chennai Run, Rewards (500 XP, 1000 Coins) and GO buttons
    /// Screen 7: Multiplayer with Online: 2456 Pill, Online Ride, Create Room, and Join Room Cards
    /// Screen 8: Settings with Graphics (Ultra/High/Med/Low, 30/60 FPS, Bloom, Shadows), Audio, Controls, Language, Account
    /// Screen 9: Reward Screen with Golden Winged "MISSION COMPLETE!", Coin Stack, Star XP, New Bike Unlocked and Continue
    /// </summary>
    public class MasterGameFlowUI : MonoBehaviour
    {
        private static MasterGameFlowUI _instance;
        public static MasterGameFlowUI Instance => _instance;

        [Header("Active Screen State")]
        public GameScreen CurrentScreen = GameScreen.SplashScreen;
        public bool IsMenuOpen
        {
            get => CurrentScreen != GameScreen.RidingHUD;
            set => CurrentScreen = value ? GameScreen.MainMenu : GameScreen.RidingHUD;
        }

        // 1. Splash Screen State
        private float _splashProgress = 0.85f;

        // 2. Main Menu & Profile State
        private string _playerName = "Player001";
        private int _playerLevel = 5;
        private int _silverCoins = 1250;
        private int _goldCoins = 75;

        // 3. Garage & Motorcycle State
        private int _selectedBikeIndex = 0;
        private readonly string[] _bikeModelNames = new[]
        {
            "Yamaha R15 V4",
            "Royal Enfield Classic 350",
            "Royal Enfield Bullet 350",
            "KTM 390 Duke ABS",
            "Royal Enfield Himalayan",
            "Bajaj Pulsar 220F"
        };
        private readonly float[] _bikeSpeeds = new[] { 0.85f, 0.72f, 0.68f, 0.94f, 0.78f, 0.82f };
        private readonly float[] _bikeHandlings = new[] { 0.80f, 0.65f, 0.60f, 0.88f, 0.82f, 0.74f };
        private readonly float[] _bikeBrakings = new[] { 0.75f, 0.70f, 0.65f, 0.92f, 0.76f, 0.78f };
        private readonly float[] _bikeFuels = new[] { 0.90f, 0.85f, 0.82f, 0.75f, 0.95f, 0.80f };

        private int _selectedColorIndex = 0;
        private bool _isCoupleRiding = false;
        private readonly Color[] _bikeColors = new[]
        {
            new Color(0.85f, 0.12f, 0.15f), // Racing Red (Yamaha/Enfield)
            new Color(0.12f, 0.35f, 0.85f), // Metallic Blue
            new Color(0.95f, 0.95f, 0.98f), // Pearl White
            new Color(0.12f, 0.12f, 0.15f), // Matte Black
            new Color(1.00f, 0.45f, 0.05f)  // KTM Neon Orange
        };

        // 4. India Map State
        private int _selectedStateIndex = 0;
        private readonly string[] _stateNames = new[]
        {
            "Tamil Nadu",
            "Maharashtra",
            "Goa",
            "Himachal Pradesh",
            "Ladakh",
            "Kerala",
            "Punjab",
            "Karnataka"
        };
        private readonly string[] _stateDescriptions = new[]
        {
            "Districts: 38\nLocations: 250+\nCorridors: NH-44 Express & Ooty Ghats",
            "Districts: 36\nLocations: 210+\nCorridors: Mumbai-Pune Expressway",
            "Districts: 2\nLocations: 80+\nCorridors: Coastal Beach Marine Drive",
            "Districts: 12\nLocations: 120+\nCorridors: Manali-Rohtang Snow Pass",
            "Districts: 2\nLocations: 65+\nCorridors: Khardung La 17,582 ft High Pass",
            "Districts: 14\nLocations: 140+\nCorridors: Munnar Tea Garden Hairpin Bends",
            "Districts: 23\nLocations: 160+\nCorridors: Grand Trunk Road & Sher-e-Punjab",
            "Districts: 31\nLocations: 190+\nCorridors: Bangalore-Mysore Highway"
        };

        // 6. Missions State
        private int _missionTab = 0; // 0=Story, 1=Daily, 2=Weekly
        private bool _mission1Claimed = false;
        private bool _mission2Claimed = false;
        private bool _mission3Claimed = false;

        // 7. Multiplayer & Social Invite State
        private string _roomCodeInput = "TN-44-ROYAL";
        private string _myGeneratedRoomCode = "TN-44-ROYAL";
        private string _multiplayerStatus = "Ready to connect to Indian Highway Convoy.";
        private bool _isRoomCodeCopied = false;
        private readonly string[] _convoyFriends = new[] { "Arun_Rider99 (Online)", "Kavitha_Bullet (Online)", "Siva_Duke390 (In-Ride)", "Vijay_Pulsar (Online)" };

        // 10. Avatar Customization State
        private bool _isFemaleAvatar = false;
        private int _selectedJacketIdx = 0;
        private readonly string[] _jacketNames = new[] { "Dark Leather", "Racing Red", "Royal Navy", "Desert Tan", "KTM Neon" };
        private readonly Color[] _jacketColors = new[]
        {
            new Color(0.14f, 0.18f, 0.24f),
            new Color(0.85f, 0.15f, 0.18f),
            new Color(0.12f, 0.35f, 0.82f),
            new Color(0.55f, 0.40f, 0.25f),
            new Color(1.00f, 0.45f, 0.05f)
        };
        private int _selectedHelmetIdx = 0;
        private readonly string[] _helmetColorNames = new[] { "Saffron Yellow", "Gloss White", "Matte Black", "Racer Red" };
        private readonly Color[] _helmetColors = new[]
        {
            new Color(1.0f, 0.76f, 0.08f),
            new Color(0.95f, 0.95f, 0.98f),
            new Color(0.12f, 0.12f, 0.15f),
            new Color(0.85f, 0.15f, 0.18f)
        };
        private int _selectedHelmetStyle = 0; // 0=Full Face, 1=Classic Open Face, 2=Motocross Visor

        // 8. Settings State
        private int _settingsTab = 0; // 0=Graphics, 1=Audio, 2=Controls, 3=Language, 4=Account
        private int _graphicsQualityIndex = 3; // 0=Low, 1=Med, 2=High, 3=Ultra
        private int _frameRateIndex = 1; // 0=30 FPS, 1=60 FPS
        private bool _bloomEnabled = true;
        private bool _shadowsEnabled = true;
        private float _masterVolume = 1.0f;
        private float _engineVolume = 1.0f;
        private float _hornVolume = 1.0f;
        private int _selectedLanguageIndex = 0; // 0=English, 1=Tamil (தமிழ்)

        // Account Auth
        private AuthenticationManager _authManager;
        private string _loginEmail = "demo@indianbike.com";
        private string _loginPassword = "IndianBike2026";
        private string _authMessage = "Signed in as Player001 (demo@indianbike.com).";

        // References to 3D simulation
        private PlayerMotorcycleController _motorcycle;
        private MotorcycleVisualBuilder _visualBuilder;
        private IEconomyManager _economyManager;
        private IProgressionManager _progressionManager;
        private TimeOfDayController _timeController;

        // UI Procedural Textures
        private Texture2D _blueHeaderTex;
        private Texture2D _goldBtnTex;
        private Texture2D _darkPanelTex;
        private Texture2D _lightPanelTex;
        private Texture2D _greenBtnTex;
        private Texture2D _blueBtnTex;
        private Texture2D _barFillTex;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _authManager = new AuthenticationManager();
            CreateProceduralTextures();
        }

        private void Start()
        {
            LocateSceneSystems();
            ApplyCustomizationsToBike();
            StartCoroutine(SplashLoadingSequenceRoutine());
        }

        private void CreateProceduralTextures()
        {
            _blueHeaderTex = MakeColorTex(new Color(0.05f, 0.18f, 0.44f, 0.96f));
            _goldBtnTex = MakeColorTex(new Color(1.0f, 0.76f, 0.08f, 1.0f));
            _darkPanelTex = MakeColorTex(new Color(0.04f, 0.09f, 0.18f, 0.90f));
            _lightPanelTex = MakeColorTex(new Color(0.08f, 0.18f, 0.32f, 0.85f));
            _greenBtnTex = MakeColorTex(new Color(0.12f, 0.72f, 0.28f, 1.0f));
            _blueBtnTex = MakeColorTex(new Color(0.12f, 0.48f, 0.95f, 1.0f));
            _barFillTex = MakeColorTex(new Color(0.15f, 0.65f, 1.0f, 1.0f));
        }

        private Texture2D MakeColorTex(Color col)
        {
            Texture2D tex = new Texture2D(2, 2);
            Color[] pix = new Color[4];
            for (int i = 0; i < 4; i++) pix[i] = col;
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }

        private void LocateSceneSystems()
        {
            if (_motorcycle == null)
            {
                var bikeObj = GameObject.FindWithTag("Motorcycle") ?? GameObject.Find("[PlayerMotorcycle]");
                if (bikeObj != null)
                {
                    _motorcycle = bikeObj.GetComponent<PlayerMotorcycleController>();
                    _visualBuilder = bikeObj.GetComponent<MotorcycleVisualBuilder>();
                }
            }

            ServiceLocator.TryGet(out _economyManager);
            ServiceLocator.TryGet(out _progressionManager);
            _timeController = FindFirstObjectByType<TimeOfDayController>();

            if (_economyManager != null)
            {
                _silverCoins = _economyManager.SilverCoins;
                _goldCoins = _economyManager.GoldCoins;
            }
        }

        private IEnumerator SplashLoadingSequenceRoutine()
        {
            _splashProgress = 0.15f;
            yield return new WaitForSeconds(0.4f);
            _splashProgress = 0.50f;
            yield return new WaitForSeconds(0.5f);
            _splashProgress = 0.85f;
            yield return new WaitForSeconds(0.6f);
            _splashProgress = 1.0f;
            yield return new WaitForSeconds(0.3f);

            // Automatically advance to Main Menu
            CurrentScreen = GameScreen.MainMenu;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                if (CurrentScreen == GameScreen.RidingHUD)
                {
                    CurrentScreen = GameScreen.MainMenu;
                }
                else if (CurrentScreen != GameScreen.SplashScreen)
                {
                    CurrentScreen = GameScreen.RidingHUD;
                }
            }

            // Sync live wallet
            if (_economyManager != null)
            {
                _silverCoins = _economyManager.SilverCoins;
                _goldCoins = _economyManager.GoldCoins;
            }
        }

        public void OpenMenu(GameScreen screen = GameScreen.MainMenu)
        {
            CurrentScreen = screen;
        }

        public void CloseMenuToRiding()
        {
            CurrentScreen = GameScreen.RidingHUD;
        }

        private void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;

            switch (CurrentScreen)
            {
                case GameScreen.SplashScreen:
                    DrawScreen1_Splash(w, h);
                    break;

                case GameScreen.MainMenu:
                    DrawScreen2_MainMenu(w, h);
                    break;

                case GameScreen.Garage:
                    DrawScreen3_Garage(w, h);
                    break;

                case GameScreen.IndiaMap:
                    DrawScreen4_IndiaMap(w, h);
                    break;

                case GameScreen.RidingHUD:
                    DrawScreen5_RidingHUD(w, h);
                    break;

                case GameScreen.Missions:
                    DrawScreen6_Missions(w, h);
                    break;

                case GameScreen.Multiplayer:
                    DrawScreen7_Multiplayer(w, h);
                    break;

                case GameScreen.Settings:
                    DrawScreen8_Settings(w, h);
                    break;

                case GameScreen.RewardScreen:
                    DrawScreen9_RewardScreen(w, h);
                    break;

                case GameScreen.AvatarStudio:
                    DrawScreen10_AvatarStudio(w, h);
                    break;
            }
        }

        // =========================================================================
        // SCREEN 1: SPLASH SCREEN (Matches Reference 1. Splash Screen)
        // =========================================================================
        private void DrawScreen1_Splash(int w, int h)
        {
            // Dark vignette background
            GUI.Box(new Rect(0, 0, w, h), "");

            // Screen Tag
            DrawScreenTag("1. Splash Screen");

            // Big Speedometer Arc Title: "INDIAN BIKE LIFE"
            GUIStyle logoStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(38, h * 65 / 1000),
                fontStyle = FontStyle.Bold
            };
            logoStyle.normal.textColor = new Color(1.0f, 0.55f, 0.05f); // Saffron Orange
            GUI.Label(new Rect(0, h * 0.25f, w, 65), "INDIAN BIKE LIFE", logoStyle);

            // Subtitle with Indian Tricolor Accent
            GUIStyle subStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(16, h * 24 / 1000),
                fontStyle = FontStyle.Bold
            };
            subStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(0, h * 0.25f + 65, w, 32), "🇮🇳  RIDE · EXPLORE · LIVE  🇮🇳", subStyle);

            // Bottom Progress Bar: "Loading... 85%"
            float barW = Mathf.Min(560f, w * 0.72f);
            float barH = 14f;
            float barX = (w - barW) * 0.5f;
            float barY = h * 0.82f;

            // Background Track
            GUI.DrawTexture(new Rect(barX, barY, barW, barH), _darkPanelTex);

            // Gold/Orange Progress Fill
            int pct = Mathf.RoundToInt(_splashProgress * 100f);
            GUI.DrawTexture(new Rect(barX, barY, barW * _splashProgress, barH), _goldBtnTex);

            // Label above/under bar
            GUIStyle pctStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(13, h * 18 / 1000),
                fontStyle = FontStyle.Bold
            };
            pctStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(0, barY + 18, w, 26), $"Loading... {pct}%", pctStyle);
        }

        // =========================================================================
        // SCREEN 2: MAIN MENU (Matches Reference 2. Main Menu)
        // =========================================================================
        private void DrawScreen2_MainMenu(int w, int h)
        {
            DrawScreenTag("2. Main Menu");

            // Top-Left: Player Profile Badge
            GUI.DrawTexture(new Rect(20, 15, 175, 52), _darkPanelTex);
            GUIStyle nameStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 14 };
            nameStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(32, 19, 150, 22), $"👤 {_playerName}", nameStyle);

            GUIStyle lvlStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
            lvlStyle.normal.textColor = new Color(1.0f, 0.80f, 0.20f);
            GUI.Label(new Rect(32, 40, 150, 18), $"Lv. {_playerLevel}", lvlStyle);

            // Top-Right: Wallet (1250 Silver, 75 Gold, Settings gear)
            DrawTopWallet(w);

            // Right-Side Main Action Stack (Reference 2)
            float rightStackW = Mathf.Min(220f, w * 0.26f);
            float rightStackX = w - rightStackW - 25f;
            float rightStackY = h * 0.22f;

            // Big Indian Bike Life Logo on right
            GUIStyle rightLogo = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(22, h * 32 / 1000),
                fontStyle = FontStyle.Bold
            };
            rightLogo.normal.textColor = new Color(1.0f, 0.65f, 0.12f);
            GUI.Label(new Rect(rightStackX - 20, rightStackY - 45, rightStackW + 40, 38), "INDIAN BIKE LIFE", rightLogo);

            // HUGE BRIGHT YELLOW CTA BUTTON: PLAY ▶
            GUI.DrawTexture(new Rect(rightStackX, rightStackY, rightStackW, 58), _goldBtnTex);
            GUIStyle playBtnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Max(22, h * 32 / 1000),
                fontStyle = FontStyle.Bold
            };
            playBtnStyle.normal.textColor = Color.black;
            if (GUI.Button(new Rect(rightStackX, rightStackY, rightStackW, 58), "PLAY ▶", playBtnStyle))
            {
                ApplyCustomizationsToBike();
                CurrentScreen = GameScreen.RidingHUD;
            }

            // Vertical Menu Navigation Stack
            float navY = rightStackY + 70f;
            float navBtnH = 40f;

            DrawMenuNavButton(rightStackX, navY, rightStackW, navBtnH, "🏍️ GARAGE", () => CurrentScreen = GameScreen.Garage);
            navY += navBtnH + 8;
            DrawMenuNavButton(rightStackX, navY, rightStackW, navBtnH, "🗺️ MAP", () => CurrentScreen = GameScreen.IndiaMap);
            navY += navBtnH + 8;
            DrawMenuNavButton(rightStackX, navY, rightStackW, navBtnH, "🎯 MISSIONS", () => CurrentScreen = GameScreen.Missions);
            navY += navBtnH + 8;
            DrawMenuNavButton(rightStackX, navY, rightStackW, navBtnH, "👥 MULTIPLAYER", () => CurrentScreen = GameScreen.Multiplayer);

            // Bottom-Left Buttons: PROFILE & ACHIEVEMENTS (Reference 2)
            float bottomY = h - 55f;
            GUI.DrawTexture(new Rect(20, bottomY, 120, 38), _darkPanelTex);
            if (GUI.Button(new Rect(20, bottomY, 120, 38), "👤 PROFILE"))
            {
                CurrentScreen = GameScreen.Settings;
                _settingsTab = 4; // Account/Profile
            }

            GUI.DrawTexture(new Rect(148, bottomY, 140, 38), _darkPanelTex);
            if (GUI.Button(new Rect(148, bottomY, 140, 38), "🎨 AVATAR"))
            {
                CurrentScreen = GameScreen.AvatarStudio;
            }

            GUI.DrawTexture(new Rect(296, bottomY, 150, 38), _darkPanelTex);
            if (GUI.Button(new Rect(296, bottomY, 150, 38), "⭐ ACHIEVEMENTS"))
            {
                CurrentScreen = GameScreen.RewardScreen;
            }
        }

        // =========================================================================
        // SCREEN 3: GARAGE (Matches Reference 3. Garage)
        // =========================================================================
        private void DrawScreen3_Garage(int w, int h)
        {
            DrawScreenTag("3. Garage");

            // Top Bar: [⬅] GARAGE + Wallet
            DrawHeaderBar(w, "GARAGE", () => CurrentScreen = GameScreen.MainMenu);
            DrawTopWallet(w);

            // Right Side Spec Panel: Bike Name, Speed, Handling, Braking, Fuel, UPGRADE, CUSTOMIZE
            float panelW = Mathf.Min(280f, w * 0.30f);
            float panelX = w - panelW - 25f;
            float panelY = 70f;

            GUI.DrawTexture(new Rect(panelX, panelY, panelW, h - panelY - 95), _darkPanelTex);
            GUILayout.BeginArea(new Rect(panelX + 16, panelY + 16, panelW - 32, h - panelY - 120));

            // Motorcycle Name
            GUIStyle bikeTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold
            };
            bikeTitleStyle.normal.textColor = Color.white;
            GUILayout.Label(_bikeModelNames[_selectedBikeIndex], bikeTitleStyle);
            GUILayout.Space(12);

            // Stat Bars with filled blue indicators
            DrawSpecBar("Speed", _bikeSpeeds[_selectedBikeIndex]);
            DrawSpecBar("Handling", _bikeHandlings[_selectedBikeIndex]);
            DrawSpecBar("Braking", _bikeBrakings[_selectedBikeIndex]);
            DrawSpecBar("Fuel", _bikeFuels[_selectedBikeIndex]);

            GUILayout.Space(16);

            // Big Yellow UPGRADE Button
            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            GUIStyle upStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 15 };
            upStyle.normal.textColor = Color.black;
            if (GUILayout.Button("UPGRADE (₹500)", upStyle, GUILayout.Height(44)))
            {
                if (_economyManager != null && _economyManager.SpendCurrency(CurrencyType.SilverCoins, 500, "Bike Upgrade"))
                {
                    _bikeSpeeds[_selectedBikeIndex] = Mathf.Min(1.0f, _bikeSpeeds[_selectedBikeIndex] + 0.05f);
                    ApplyCustomizationsToBike();
                }
            }

            GUILayout.Space(6);
            // Blue CUSTOMIZE COLOR Button
            GUI.backgroundColor = new Color(0.12f, 0.50f, 0.98f);
            GUIStyle custStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 13 };
            if (GUILayout.Button("🎨 BIKE COLOR", custStyle, GUILayout.Height(32)))
            {
                _selectedColorIndex = (_selectedColorIndex + 1) % _bikeColors.Length;
                ApplyCustomizationsToBike();
            }

            GUILayout.Space(4);
            // Orange AVATAR STUDIO Button
            GUI.backgroundColor = new Color(1.0f, 0.55f, 0.12f);
            if (GUILayout.Button("👤 RIDER AVATAR", custStyle, GUILayout.Height(32)))
            {
                CurrentScreen = GameScreen.AvatarStudio;
            }

            GUILayout.Space(4);
            // Passenger Toggle (Couple Riding)
            GUI.backgroundColor = _isCoupleRiding ? new Color(0.85f, 0.35f, 0.65f) : new Color(0.35f, 0.45f, 0.60f);
            if (GUILayout.Button(_isCoupleRiding ? "👥 COUPLE: ON" : "👤 SOLO RIDER", GUILayout.Height(30)))
            {
                _isCoupleRiding = !_isCoupleRiding;
                ApplyCustomizationsToBike();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();

            // Bottom Bike Carousel Selector with < and > arrows (Reference 3)
            float carouselH = 75f;
            float carouselY = h - carouselH - 10f;
            float carouselW = w - 50f;
            float carouselX = 25f;

            GUI.DrawTexture(new Rect(carouselX, carouselY, carouselW, carouselH), _darkPanelTex);

            // Left Arrow <
            if (GUI.Button(new Rect(carouselX + 8, carouselY + 12, 45, carouselH - 24), "◀"))
            {
                _selectedBikeIndex = (_selectedBikeIndex - 1 + _bikeModelNames.Length) % _bikeModelNames.Length;
                ApplyCustomizationsToBike();
            }

            // Bike Thumbnails in Center
            float slotW = (carouselW - 130f) / _bikeModelNames.Length;
            for (int b = 0; b < _bikeModelNames.Length; b++)
            {
                float slotX = carouselX + 60f + b * slotW;
                bool isCur = (_selectedBikeIndex == b);

                if (isCur)
                {
                    GUI.DrawTexture(new Rect(slotX, carouselY + 8, slotW - 6, carouselH - 16), _goldBtnTex);
                }
                else
                {
                    GUI.DrawTexture(new Rect(slotX, carouselY + 8, slotW - 6, carouselH - 16), _lightPanelTex);
                }

                GUIStyle slotStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = Mathf.Max(10, h * 13 / 1000),
                    fontStyle = FontStyle.Bold
                };
                slotStyle.normal.textColor = isCur ? Color.black : Color.white;

                if (GUI.Button(new Rect(slotX, carouselY + 8, slotW - 6, carouselH - 16), $"🏍️\n{_bikeModelNames[b]}", slotStyle))
                {
                    _selectedBikeIndex = b;
                    ApplyCustomizationsToBike();
                }
            }

            // Right Arrow >
            if (GUI.Button(new Rect(carouselX + carouselW - 53, carouselY + 12, 45, carouselH - 24), "▶"))
            {
                _selectedBikeIndex = (_selectedBikeIndex + 1) % _bikeModelNames.Length;
                ApplyCustomizationsToBike();
            }
        }

        private void DrawSpecBar(string label, float fraction)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(75));

            Rect r = GUILayoutUtility.GetRect(130, 14);
            GUI.DrawTexture(r, _lightPanelTex);
            GUI.DrawTexture(new Rect(r.x, r.y, r.width * Mathf.Clamp01(fraction), r.height), _barFillTex);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        // =========================================================================
        // SCREEN 4: INDIA MAP (Matches Reference 4. India Map)
        // =========================================================================
        private void DrawScreen4_IndiaMap(int w, int h)
        {
            DrawScreenTag("4. India Map");

            // Top Bar: [⬅] INDIA MAP
            DrawHeaderBar(w, "INDIA MAP", () => CurrentScreen = GameScreen.MainMenu);

            // Left Side: 3D Geographical Map Card with Pinned States
            float mapW = Mathf.Min(560f, w * 0.58f);
            float mapY = 70f;
            float mapH = h - mapY - 25f;

            GUI.DrawTexture(new Rect(25, mapY, mapW, mapH), _darkPanelTex);

            GUIStyle mapHeader = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            mapHeader.normal.textColor = new Color(1f, 0.75f, 0.15f);
            GUI.Label(new Rect(25, mapY + 12, mapW, 28), "🗺️ 28 STATES & 8 UNION TERRITORIES OF INDIA", mapHeader);

            // State Selection Grid inside Map
            float gridY = mapY + 48f;
            float colW = (mapW - 35) / 2f;
            for (int s = 0; s < _stateNames.Length; s++)
            {
                int col = s % 2;
                int row = s / 2;
                float sx = 35 + col * (colW + 10);
                float sy = gridY + row * 44;

                bool isSel = (_selectedStateIndex == s);
                GUI.backgroundColor = isSel ? new Color(1f, 0.76f, 0.08f) : new Color(0.12f, 0.28f, 0.55f);
                GUIStyle sBtn = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 13 };
                sBtn.normal.textColor = isSel ? Color.black : Color.white;

                if (GUI.Button(new Rect(sx, sy, colW, 38), $"📍 {_stateNames[s]}", sBtn))
                {
                    _selectedStateIndex = s;
                    var roadGen = FindFirstObjectByType<RoadAndEnvironmentBuilder>();
                    roadGen?.LoadStateEnvironment(_selectedStateIndex);
                }
            }
            GUI.backgroundColor = Color.white;

            // Right Side Card: "Tamil Nadu", Districts: 38, Locations: 250+, SELECT STATE ▶ (Reference 4)
            float cardW = w - mapW - 65f;
            float cardX = 35 + mapW + 10f;
            float cardH = mapH;

            GUI.DrawTexture(new Rect(cardX, mapY, cardW, cardH), _darkPanelTex);

            GUILayout.BeginArea(new Rect(cardX + 16, mapY + 18, cardW - 32, cardH - 36));

            // State Name Title (Reference 4: Tamil Nadu)
            GUIStyle stateTitle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
            stateTitle.normal.textColor = new Color(1.0f, 0.82f, 0.20f);
            GUILayout.Label(_stateNames[_selectedStateIndex], stateTitle);

            GUILayout.Space(12);
            // Temple Thumbnail Banner Box (Reference 4)
            Rect thumbRect = GUILayoutUtility.GetRect(cardW - 32, 130);
            GUI.DrawTexture(thumbRect, _lightPanelTex);
            GUIStyle thumbStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            thumbStyle.normal.textColor = Color.white;
            GUI.Label(thumbRect, $"🏛️ HISTORIC CORRIDOR\n{_stateNames[_selectedStateIndex]}", thumbStyle);

            GUILayout.Space(14);
            // Districts & Locations
            GUIStyle descStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
            descStyle.normal.textColor = Color.white;
            GUILayout.Label(_stateDescriptions[_selectedStateIndex], descStyle);

            GUILayout.FlexibleSpace();

            // Big Yellow CTA Button: SELECT STATE ▶ (Reference 4)
            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            GUIStyle selStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold
            };
            selStyle.normal.textColor = Color.black;

            if (GUILayout.Button("SELECT STATE ▶", selStyle, GUILayout.Height(50)))
            {
                var roadGen = FindFirstObjectByType<RoadAndEnvironmentBuilder>();
                roadGen?.LoadStateEnvironment(_selectedStateIndex);
                if (_motorcycle != null)
                {
                    _motorcycle.RespawnOnRoad();
                }
                ApplyCustomizationsToBike();
                CurrentScreen = GameScreen.RidingHUD;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();
        }

        // =========================================================================
        // SCREEN 5: RIDING HUD (Matches Reference 5. Riding HUD)
        // =========================================================================
        private void DrawScreen5_RidingHUD(int w, int h)
        {
            DrawScreenTag("5. Riding HUD");

            // Top-Left: Circular GPS Mini-Map (Reference 5)
            float mapSize = Mathf.Min(110f, h * 0.24f);
            GUI.DrawTexture(new Rect(20, 20, mapSize, mapSize), _darkPanelTex);
            GUIStyle gpsStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            gpsStyle.normal.textColor = new Color(0.2f, 0.9f, 0.4f);
            GUI.Label(new Rect(20, 20, mapSize, mapSize), "🧭 GPS\nNH-44", gpsStyle);

            // Top-Center: Fuel Bar (Reference 5)
            float fuelW = 180f;
            float fuelH = 16f;
            float fuelX = (w - fuelW) * 0.5f;
            GUI.DrawTexture(new Rect(fuelX, 15, fuelW, fuelH), _darkPanelTex);
            GUI.DrawTexture(new Rect(fuelX, 15, fuelW * 0.88f, fuelH), _greenBtnTex);
            GUI.Label(new Rect(fuelX - 25, 12, 22, 20), "⛽");

            // Top-Right: Digital Speedometer "120 km/h" with Red Gear Badge "5" (Reference 5)
            float curSpeed = _motorcycle != null ? _motorcycle.CurrentSpeedKmh : 120f;
            int curGear = _motorcycle != null ? _motorcycle.CurrentGear : 5;

            float speedBoxW = 165f;
            float speedBoxX = w - speedBoxW - 20f;
            GUI.DrawTexture(new Rect(speedBoxX, 15, speedBoxW, 55), _darkPanelTex);

            GUIStyle spdStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 24,
                fontStyle = FontStyle.Bold
            };
            spdStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(speedBoxX + 10, 18, 90, 32), $"{Mathf.RoundToInt(curSpeed)}", spdStyle);

            GUIStyle kmhStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
            kmhStyle.normal.textColor = Color.gray;
            GUI.Label(new Rect(speedBoxX + 104, 26, 45, 18), "km/h", kmhStyle);

            // Red Gear Badge: 5
            GUI.backgroundColor = new Color(0.85f, 0.15f, 0.15f);
            GUI.Box(new Rect(speedBoxX + speedBoxW - 24, 18, 20, 22), $"{curGear}");
            GUI.backgroundColor = Color.white;

            // Right Vertical Control Icons: Camera View 📷, Horn 📢, Settings/Menu ⚙️ (Reference 5)
            float iconX = w - 55f;
            float iconY = 85f;
            if (GUI.Button(new Rect(iconX, iconY, 40, 40), "📷"))
            {
                var cam = Camera.main?.GetComponent<MotorcycleFollowCamera>();
                cam?.ToggleViewMode();
            }
            if (GUI.Button(new Rect(iconX, iconY + 50, 40, 40), "📢"))
            {
                _motorcycle?.TriggerHorn();
            }
            if (GUI.Button(new Rect(iconX, iconY + 95, 40, 40), "👥"))
            {
                _isCoupleRiding = !_isCoupleRiding;
                ApplyCustomizationsToBike();
            }
            if (GUI.Button(new Rect(iconX, iconY + 140, 40, 40), "⚙️"))
            {
                CurrentScreen = GameScreen.MainMenu;
            }

            // Bottom-Left: Steering Touch Arrows ◀ ▶ (Reference 5)
            float steerY = h - 90f;
            if (GUI.RepeatButton(new Rect(30, steerY, 70, 70), "◀"))
            {
                _motorcycle?.SetSteerInput(-0.85f);
            }
            if (GUI.RepeatButton(new Rect(115, steerY, 70, 70), "▶"))
            {
                _motorcycle?.SetSteerInput(0.85f);
            }

            // Center Upright Re-center Button & 360-degree look hint
            if (GUI.Button(new Rect(w * 0.5f - 60f, h - 45f, 120f, 32f), "🔄 RE-CENTER"))
            {
                _motorcycle?.RespawnOnRoad();
            }

            GUIStyle hintStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            hintStyle.normal.textColor = new Color(1f, 1f, 1f, 0.70f);
            GUI.Label(new Rect(w * 0.5f - 150f, h - 72f, 300f, 22f), "👆 Swipe / Drag Screen for 360° View", hintStyle);

            // Bottom-Right: Brake Pedal & Gas Throttle Pedal (Reference 5)
            float pedalY = h - 100f;
            float brakeX = w - 180f;
            float gasX = w - 95f;

            // Brake Pedal
            GUI.backgroundColor = new Color(0.7f, 0.2f, 0.2f);
            if (GUI.RepeatButton(new Rect(brakeX, pedalY + 15, 65, 75), "BRAKE\n🛑"))
            {
                _motorcycle?.SetBrakeInput(1f);
            }

            // Gas Throttle Pedal (Tread Pattern)
            GUI.backgroundColor = new Color(0.2f, 0.75f, 0.3f);
            if (GUI.RepeatButton(new Rect(gasX, pedalY, 70, 90), "GAS\n⚡"))
            {
                _motorcycle?.SetThrottleInput(1f);
            }
            GUI.backgroundColor = Color.white;

            // Top-Left In-Game Garage / Menu Shortcut Button
            if (GUI.Button(new Rect(145, 20, 110, 32), "🏍️ GARAGE"))
            {
                CurrentScreen = GameScreen.Garage;
            }
        }

        // =========================================================================
        // SCREEN 6: MISSIONS (Matches Reference 6. Missions)
        // =========================================================================
        private void DrawScreen6_Missions(int w, int h)
        {
            DrawScreenTag("6. Missions");

            // Top Bar: [⬅] MISSIONS
            DrawHeaderBar(w, "MISSIONS", () => CurrentScreen = GameScreen.MainMenu);

            // Left Side: 3D Rider Silhouette / Preview
            float leftW = Mathf.Min(320f, w * 0.35f);
            float panelY = 70f;
            float panelH = h - panelY - 25f;

            GUI.DrawTexture(new Rect(25, panelY, leftW, panelH), _darkPanelTex);

            GUIStyle riderInfo = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            riderInfo.normal.textColor = Color.white;
            GUI.Label(new Rect(25, panelY + 25, leftW, 30), "🏍️ TOURING MISSIONS", riderInfo);

            GUIStyle rewardBox = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12 };
            rewardBox.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
            GUI.Label(new Rect(25, panelY + 65, leftW, 60), "Complete Indian Highway\nMilestones to earn Silver,\nGold & XP Trophies!", rewardBox);

            // Right Side: Missions Board with Story/Daily/Weekly Tabs (Reference 6)
            float boardX = 35 + leftW + 10f;
            float boardW = w - boardX - 25f;

            GUI.DrawTexture(new Rect(boardX, panelY, boardW, panelH), _darkPanelTex);

            // Vertical / Horizontal Tabs: [Story] [Daily] [Weekly]
            float tabW = 85f;
            float tabH = 34f;
            float tabX = boardX + 15f;
            float tabY = panelY + 12f;

            DrawMissionTab(tabX, tabY, tabW, tabH, "Story", 0);
            DrawMissionTab(tabX + tabW + 5, tabY, tabW, tabH, "Daily", 1);
            DrawMissionTab(tabX + (tabW + 5) * 2, tabY, tabW, tabH, "Weekly", 2);

            // Mission Rows (Reference 6)
            float rowY = panelY + 58f;
            float rowH = 50f;
            float rowW = boardW - 30f;

            DrawMissionRow(boardX + 15, rowY, rowW, rowH, "Reach Chennai via NH-44", "0 / 1", "🪙 500", ref _mission1Claimed);
            rowY += rowH + 10;
            DrawMissionRow(boardX + 15, rowY, rowW, rowH, "Complete 5 Rides", "0 / 5", "🪙 300", ref _mission2Claimed);
            rowY += rowH + 10;
            DrawMissionRow(boardX + 15, rowY, rowW, rowH, "Travel 100 km Tour", "0 / 100", "🪙 1000", ref _mission3Claimed);

            // Bottom Rewards Summary Strip (Reference 6: REWARDS: [XP 500] [🪙 1000] [🎁 Mystery Box])
            float bottomStripY = panelY + panelH - 55f;
            GUI.DrawTexture(new Rect(boardX + 15, bottomStripY, rowW, 42), _lightPanelTex);

            GUIStyle rewSummary = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            rewSummary.normal.textColor = Color.white;
            GUI.Label(new Rect(boardX + 15, bottomStripY, rowW, 42), "REWARDS:   ⭐ XP 500     🪙 1,000 Coins     🎁 Mystery Gift Box", rewSummary);
        }

        private void DrawMissionTab(float x, float y, float w, float h, string title, int index)
        {
            bool isCur = (_missionTab == index);
            if (isCur)
            {
                GUI.DrawTexture(new Rect(x, y, w, h), _goldBtnTex);
            }
            else
            {
                GUI.DrawTexture(new Rect(x, y, w, h), _lightPanelTex);
            }

            GUIStyle tStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 13 };
            tStyle.normal.textColor = isCur ? Color.black : Color.white;
            if (GUI.Button(new Rect(x, y, w, h), title, tStyle))
            {
                _missionTab = index;
            }
        }

        private void DrawMissionRow(float x, float y, float w, float h, string missionName, string progress, string reward, ref bool claimed)
        {
            GUI.DrawTexture(new Rect(x, y, w, h), _lightPanelTex);

            // Mission Name
            GUIStyle mStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
            mStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(x + 15, y + 8, w * 0.45f, 22), missionName, mStyle);

            // Progress & Reward
            GUIStyle sub = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            sub.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
            GUI.Label(new Rect(x + 15, y + 27, w * 0.45f, 18), $"Progress: {progress}   |   Reward: {reward}", sub);

            // Yellow GO Button (Reference 6)
            float goW = 75f;
            float goH = 34f;
            float goX = x + w - goW - 12f;
            float goY = y + (h - goH) * 0.5f;

            GUI.DrawTexture(new Rect(goX, goY, goW, goH), _goldBtnTex);
            GUIStyle goStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 14 };
            goStyle.normal.textColor = Color.black;

            if (GUI.Button(new Rect(goX, goY, goW, goH), "GO", goStyle))
            {
                claimed = true;
                CurrentScreen = GameScreen.RewardScreen;
            }
        }

        // =========================================================================
        // SCREEN 7: MULTIPLAYER (Matches Reference 7. Multiplayer)
        // =========================================================================
        private void DrawScreen7_Multiplayer(int w, int h)
        {
            DrawScreenTag("7. Multiplayer");

            // Top Bar: [⬅] MULTIPLAYER + Green Pill "🟢 Online: 2456" (Reference 7)
            DrawHeaderBar(w, "MULTIPLAYER", () => CurrentScreen = GameScreen.MainMenu);

            // Green Online Pill
            float pillW = 140f;
            float pillX = w - pillW - 25f;
            GUI.DrawTexture(new Rect(pillX, 12, pillW, 34), _greenBtnTex);
            GUIStyle pillStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            pillStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(pillX, 12, pillW, 34), "🟢 Online: 2456", pillStyle);

            // 3 Bottom Feature Cards (Reference 7: Online Ride, Create Room, Join Room)
            float cardSpacing = 20f;
            float cardW = (w - 50f - cardSpacing * 2) / 3f;
            float cardH = Mathf.Min(220f, h * 0.44f);
            float cardY = h - cardH - 85f;

            // Card 1: Online Ride
            DrawMultiplayerCard(25, cardY, cardW, cardH, "🌐", "Online Ride", "Instant Matchmaking\nNH-44 Express Convoy", "Join Now", () =>
            {
                _multiplayerStatus = "Matched with 3 Indian Riders! Convoy starting.";
                CurrentScreen = GameScreen.RidingHUD;
            });

            // Card 2: Create Room & Invite
            DrawMultiplayerCard(25 + cardW + cardSpacing, cardY, cardW, cardH, "👥", "Create Room", $"Host Private 4-Player Convoy\nRoom: {_myGeneratedRoomCode}", "Host & Invite", () =>
            {
                _multiplayerStatus = $"Created Room {_myGeneratedRoomCode}! Code copied to clipboard.";
                GUIUtility.systemCopyBuffer = _myGeneratedRoomCode;
                _isRoomCodeCopied = true;
            });

            // Card 3: Join Room
            DrawMultiplayerCard(25 + (cardW + cardSpacing) * 2, cardY, cardW, cardH, "🚪", "Join Room", "Enter Room Invite Code\nbelow to join friends", "Join Room", () =>
            {
                _multiplayerStatus = "Joined Convoy Room " + _roomCodeInput + "!";
                CurrentScreen = GameScreen.RidingHUD;
            });

            // Bottom Social Friends & Room Code Invite Bar
            float barY = h - 70f;
            GUI.DrawTexture(new Rect(25, barY, w - 50f, 55f), _darkPanelTex);

            GUIStyle invStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
            invStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(35, barY + 8, 220, 20), $"Invite Code: <b>{_myGeneratedRoomCode}</b>", invStyle);

            if (GUI.Button(new Rect(35, barY + 28, 140, 24), _isRoomCodeCopied ? "✓ CODE COPIED!" : "📋 COPY INVITE CODE"))
            {
                GUIUtility.systemCopyBuffer = _myGeneratedRoomCode;
                _isRoomCodeCopied = true;
                _multiplayerStatus = $"Room code {_myGeneratedRoomCode} copied! Share with friends to ride together.";
            }

            // Quick Friend Invites
            GUI.Label(new Rect(210, barY + 8, 120, 20), "Online Friends:", invStyle);
            float fx = 320f;
            for (int f = 0; f < _convoyFriends.Length; f++)
            {
                if (fx + 140f > w - 180f) break;
                if (GUI.Button(new Rect(fx, barY + 12, 135, 30), $"+ {_convoyFriends[f]}"))
                {
                    _multiplayerStatus = $"Invited {_convoyFriends[f]} to convoy {_myGeneratedRoomCode}!";
                }
                fx += 145f;
            }

            // Room Code Input on Right
            float inX = w - 190f;
            GUI.Label(new Rect(inX, barY + 6, 120, 18), "Enter Room Code:", invStyle);
            _roomCodeInput = GUI.TextField(new Rect(inX, barY + 26, 155, 24), _roomCodeInput);

            // Status message
            GUIStyle statStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            statStyle.normal.textColor = new Color(1f, 0.85f, 0.2f);
            GUI.Label(new Rect(0, cardY - 32, w, 24), _multiplayerStatus, statStyle);
        }

        private void DrawMultiplayerCard(float x, float y, float w, float h, string icon, string title, string desc, string btnText, Action onAction)
        {
            GUI.DrawTexture(new Rect(x, y, w, h), _darkPanelTex);

            GUILayout.BeginArea(new Rect(x + 12, y + 14, w - 24, h - 28));

            GUIStyle icStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 28 };
            GUILayout.Label(icon, icStyle);

            GUIStyle tStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold };
            tStyle.normal.textColor = Color.white;
            GUILayout.Label(title, tStyle);

            GUIStyle dStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 11 };
            dStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.Label(desc, dStyle);

            GUILayout.FlexibleSpace();

            // Yellow Button (Reference 7)
            GUI.DrawTexture(GUILayoutUtility.GetRect(w - 24, 38), _goldBtnTex);
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 14 };
            btnStyle.normal.textColor = Color.black;

            if (GUI.Button(GUILayoutUtility.GetLastRect(), btnText, btnStyle))
            {
                onAction?.Invoke();
            }

            GUILayout.EndArea();
        }

        // =========================================================================
        // SCREEN 8: SETTINGS (Matches Reference 8. Settings)
        // =========================================================================
        private void DrawScreen8_Settings(int w, int h)
        {
            DrawScreenTag("8. Settings");

            // Top Bar: [⬅] SETTINGS
            DrawHeaderBar(w, "SETTINGS", () => CurrentScreen = GameScreen.MainMenu);

            float panelY = 70f;
            float panelH = h - panelY - 25f;

            // Left Category Tabs: Graphics, Audio, Controls, Language, Account (Reference 8)
            float tabListW = Mathf.Min(220f, w * 0.26f);
            GUI.DrawTexture(new Rect(25, panelY, tabListW, panelH), _darkPanelTex);

            float tabBtnH = 44f;
            float ty = panelY + 12f;

            DrawSettingsTabBtn(35, ty, tabListW - 20, tabBtnH, "🖥️ Graphics", 0);
            ty += tabBtnH + 6;
            DrawSettingsTabBtn(35, ty, tabListW - 20, tabBtnH, "🔊 Audio", 1);
            ty += tabBtnH + 6;
            DrawSettingsTabBtn(35, ty, tabListW - 20, tabBtnH, "🎮 Controls", 2);
            ty += tabBtnH + 6;
            DrawSettingsTabBtn(35, ty, tabListW - 20, tabBtnH, "🌐 Language", 3);
            ty += tabBtnH + 6;
            DrawSettingsTabBtn(35, ty, tabListW - 20, tabBtnH, "👤 Account", 4);

            // Right Settings Configuration Panel (Reference 8)
            float setW = w - tabListW - 65f;
            float setX = 35 + tabListW + 10f;
            GUI.DrawTexture(new Rect(setX, panelY, setW, panelH), _darkPanelTex);

            GUILayout.BeginArea(new Rect(setX + 20, panelY + 16, setW - 40, panelH - 32));

            switch (_settingsTab)
            {
                case 0: DrawSettingsGraphics(); break;
                case 1: DrawSettingsAudio(); break;
                case 2: DrawSettingsControls(); break;
                case 3: DrawSettingsLanguage(); break;
                case 4: DrawSettingsAccount(); break;
            }

            GUILayout.FlexibleSpace();

            // Bottom Right Yellow [Apply] Button (Reference 8)
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            GUIStyle appStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 15 };
            appStyle.normal.textColor = Color.black;
            if (GUILayout.Button("Apply", appStyle, GUILayout.Width(130), GUILayout.Height(38)))
            {
                ApplySettingsChanges();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawSettingsTabBtn(float x, float y, float w, float h, string title, int index)
        {
            bool isCur = (_settingsTab == index);
            if (isCur)
            {
                GUI.DrawTexture(new Rect(x, y, w, h), _goldBtnTex);
            }
            else
            {
                GUI.DrawTexture(new Rect(x, y, w, h), _lightPanelTex);
            }

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 13 };
            btnStyle.normal.textColor = isCur ? Color.black : Color.white;
            if (GUI.Button(new Rect(x, y, w, h), title, btnStyle))
            {
                _settingsTab = index;
            }
        }

        private void DrawSettingsGraphics()
        {
            // Graphics Quality: [Low] [Medium] [High] [Ultra] (Reference 8)
            GUILayout.Label("<b>Graphics Quality:</b>");
            GUILayout.BeginHorizontal();
            string[] qNames = new[] { "Low", "Medium", "High", "Ultra" };
            for (int q = 0; q < qNames.Length; q++)
            {
                bool isQ = (_graphicsQualityIndex == q);
                GUI.backgroundColor = isQ ? new Color(1f, 0.76f, 0.08f) : Color.gray;
                if (GUILayout.Button(qNames[q], GUILayout.Height(32)))
                {
                    _graphicsQualityIndex = q;
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(14);

            // Frame Rate: [30 FPS] [60 FPS] (Reference 8)
            GUILayout.Label("<b>Frame Rate:</b>");
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = (_frameRateIndex == 0) ? new Color(1f, 0.76f, 0.08f) : Color.gray;
            if (GUILayout.Button("30 FPS", GUILayout.Width(110), GUILayout.Height(32))) _frameRateIndex = 0;

            GUI.backgroundColor = (_frameRateIndex == 1) ? new Color(1f, 0.76f, 0.08f) : Color.gray;
            if (GUILayout.Button("60 FPS", GUILayout.Width(110), GUILayout.Height(32))) _frameRateIndex = 1;
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(14);

            // Bloom Switch (Reference 8)
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bloom:", GUILayout.Width(140));
            GUI.backgroundColor = _bloomEnabled ? new Color(0.2f, 0.8f, 0.35f) : Color.gray;
            if (GUILayout.Button(_bloomEnabled ? "ON" : "OFF", GUILayout.Width(80), GUILayout.Height(28)))
            {
                _bloomEnabled = !_bloomEnabled;
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Shadows Switch (Reference 8)
            GUILayout.BeginHorizontal();
            GUILayout.Label("Shadows:", GUILayout.Width(140));
            GUI.backgroundColor = _shadowsEnabled ? new Color(0.2f, 0.8f, 0.35f) : Color.gray;
            if (GUILayout.Button(_shadowsEnabled ? "ON" : "OFF", GUILayout.Width(80), GUILayout.Height(28)))
            {
                _shadowsEnabled = !_shadowsEnabled;
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        private void DrawSettingsAudio()
        {
            GUILayout.Label("<b>Master Volume:</b>");
            _masterVolume = GUILayout.HorizontalSlider(_masterVolume, 0f, 1f);

            GUILayout.Space(10);
            GUILayout.Label("<b>350cc Engine Thump Volume:</b>");
            _engineVolume = GUILayout.HorizontalSlider(_engineVolume, 0f, 1f);

            GUILayout.Space(10);
            GUILayout.Label("<b>Roots Vibrosonic Horn Volume:</b>");
            _hornVolume = GUILayout.HorizontalSlider(_hornVolume, 0f, 1f);
        }

        private void DrawSettingsControls()
        {
            GUILayout.Label("<b>Control Scheme for Android / PC:</b>");
            GUILayout.Box("✓ On-Screen Mobile Touch HUD (Gas, Brake, Steer, Horn, Nitro)\n✓ Keyboard Support (W, A, S, D, Space, Shift, H)\n✓ Accelerometer Tilt Steering Supported");
        }

        private void DrawSettingsLanguage()
        {
            GUILayout.Label("<b>Select Game Language:</b>");
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = (_selectedLanguageIndex == 0) ? new Color(1f, 0.76f, 0.08f) : Color.gray;
            if (GUILayout.Button("English (Default)", GUILayout.Height(36))) _selectedLanguageIndex = 0;

            GUI.backgroundColor = (_selectedLanguageIndex == 1) ? new Color(1f, 0.76f, 0.08f) : Color.gray;
            if (GUILayout.Button("தமிழ் (Tamil)", GUILayout.Height(36))) _selectedLanguageIndex = 1;
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        private void DrawSettingsAccount()
        {
            GUILayout.Label("<b>Player Authentication & Account:</b>");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Email:", GUILayout.Width(80));
            _loginEmail = GUILayout.TextField(_loginEmail, GUILayout.Width(220));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Password:", GUILayout.Width(80));
            _loginPassword = GUILayout.PasswordField(_loginPassword, '*', GUILayout.Width(220));
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🔑 Login", GUILayout.Width(100), GUILayout.Height(30)))
            {
                _authManager.LoginWithEmail(_loginEmail, _loginPassword, (ok, msg) => _authMessage = msg);
            }
            if (GUILayout.Button("⚡ 1-Click Guest Login", GUILayout.Width(160), GUILayout.Height(30)))
            {
                _authManager.LoginAsGuest((ok, msg) => _authMessage = msg);
            }
            GUILayout.EndHorizontal();
            GUILayout.Label($"Status: {_authMessage}");
        }

        private void ApplySettingsChanges()
        {
            QualitySettings.SetQualityLevel(Mathf.Clamp(_graphicsQualityIndex, 0, 2), true);
            Application.targetFrameRate = (_frameRateIndex == 0) ? 30 : 60;
        }

        // =========================================================================
        // SCREEN 9: REWARD SCREEN (Matches Reference 9. Reward Screen)
        // =========================================================================
        private void DrawScreen9_RewardScreen(int w, int h)
        {
            DrawScreenTag("9. Reward Screen");

            // Dark Festive Background
            GUI.Box(new Rect(0, 0, w, h), "");

            // Golden Winged Crest Banner: "MISSION COMPLETE!" (Reference 9)
            GUIStyle crestStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(30, h * 50 / 1000),
                fontStyle = FontStyle.Bold
            };
            crestStyle.normal.textColor = new Color(1.0f, 0.82f, 0.18f); // Golden Glowing Yellow
            GUI.Label(new Rect(0, h * 0.18f, w, 55), "🦅  MISSION COMPLETE!  🦅", crestStyle);

            // 3 Center Reward Cards: [+1000 Coins] [+500 XP] [New Bike Unlocked] (Reference 9)
            float totalCardsW = Mathf.Min(660f, w * 0.78f);
            float cardSpacing = 16f;
            float cW = (totalCardsW - cardSpacing * 2) / 3f;
            float cardH = 145f;
            float cardX = (w - totalCardsW) * 0.5f;
            float cardY = h * 0.38f;

            // Card 1: Gold Coin Stack +1000 Coins
            GUI.DrawTexture(new Rect(cardX, cardY, cW, cardH), _darkPanelTex);
            GUIStyle rwVal = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 28 };
            GUI.Label(new Rect(cardX, cardY + 15, cW, 35), "🪙🪙", rwVal);
            GUIStyle rwT1 = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold };
            rwT1.normal.textColor = new Color(1.0f, 0.82f, 0.18f);
            GUI.Label(new Rect(cardX, cardY + 60, cW, 25), "+1000", rwT1);
            GUIStyle rwSub = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12 };
            rwSub.normal.textColor = Color.gray;
            GUI.Label(new Rect(cardX, cardY + 88, cW, 20), "Coins", rwSub);

            // Card 2: Blue Star +500 XP
            float cx2 = cardX + cW + cardSpacing;
            GUI.DrawTexture(new Rect(cx2, cardY, cW, cardH), _darkPanelTex);
            GUI.Label(new Rect(cx2, cardY + 15, cW, 35), "⭐", rwVal);
            GUIStyle rwT2 = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold };
            rwT2.normal.textColor = new Color(0.2f, 0.85f, 1.0f);
            GUI.Label(new Rect(cx2, cardY + 60, cW, 25), "+500", rwT2);
            GUI.Label(new Rect(cx2, cardY + 88, cW, 20), "XP", rwSub);

            // Card 3: New Bike Unlocked
            float cx3 = cardX + (cW + cardSpacing) * 2;
            GUI.DrawTexture(new Rect(cx3, cardY, cW, cardH), _darkPanelTex);
            GUI.Label(new Rect(cx3, cardY + 15, cW, 35), "🏍️", rwVal);
            GUIStyle rwT3 = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 14, fontStyle = FontStyle.Bold };
            rwT3.normal.textColor = Color.white;
            GUI.Label(new Rect(cx3, cardY + 60, cW, 25), "New Bike", rwT3);
            GUI.Label(new Rect(cx3, cardY + 88, cW, 20), "Unlocked", rwSub);

            // Big Yellow CTA Button: Continue ▶ (Reference 9)
            float conW = Mathf.Min(260f, w * 0.35f);
            float conH = 48f;
            float conX = (w - conW) * 0.5f;
            float conY = cardY + cardH + 35f;

            GUI.DrawTexture(new Rect(conX, conY, conW, conH), _goldBtnTex);
            GUIStyle conStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 17 };
            conStyle.normal.textColor = Color.black;

            if (GUI.Button(new Rect(conX, conY, conW, conH), "Continue ▶", conStyle))
            {
                // Credit rewards to player wallet
                _silverCoins += 1000;
                _economyManager?.AddCurrency(CurrencyType.SilverCoins, 1000, "Mission Complete Reward");
                CurrentScreen = GameScreen.MainMenu;
            }
        }

        // =========================================================================
        // SHARED UI HELPERS & NAVIGATION
        // =========================================================================
        private void DrawScreenTag(string label)
        {
            GUIStyle tagStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            tagStyle.normal.textColor = new Color(1.0f, 0.85f, 0.20f); // Yellow tag
            GUI.Label(new Rect(15, 4, 250, 18), label, tagStyle);
        }

        private void DrawHeaderBar(int w, string title, Action onBack)
        {
            // Blue Header Bar (Reference 3, 4, 6, 7, 8)
            GUI.DrawTexture(new Rect(0, 0, w, 52), _blueHeaderTex);

            // Blue Square Back Button [ ⬅ ]
            if (GUI.Button(new Rect(12, 10, 44, 32), "⬅"))
            {
                onBack?.Invoke();
            }

            // Title
            GUIStyle tStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold
            };
            tStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(68, 14, 250, 26), title, tStyle);
        }

        private void DrawTopWallet(int w)
        {
            float wx = w - 240f;
            GUI.DrawTexture(new Rect(wx, 12, 185, 34), _darkPanelTex);

            GUIStyle wStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
            wStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(wx + 10, 19, 175, 20), $"🪙 {_silverCoins}    💰 {_goldCoins}", wStyle);

            // Plus Button [+]
            GUI.backgroundColor = new Color(0.12f, 0.75f, 0.35f);
            if (GUI.Button(new Rect(wx + 155, 15, 24, 26), "+"))
            {
                _silverCoins += 500;
                _economyManager?.AddCurrency(CurrencyType.SilverCoins, 500, "Bonus Coins");
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawMenuNavButton(float x, float y, float w, float h, string title, Action onClick)
        {
            GUI.DrawTexture(new Rect(x, y, w, h), _darkPanelTex);
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            btnStyle.normal.textColor = Color.white;

            if (GUI.Button(new Rect(x, y, w, h), "   " + title, btnStyle))
            {
                onClick?.Invoke();
            }
        }

        // =========================================================================
        // SCREEN 10: AVATAR & RIDER STUDIO
        // =========================================================================
        private void DrawScreen10_AvatarStudio(int w, int h)
        {
            DrawScreenTag("10. Rider Avatar Studio");
            DrawHeaderBar(w, "RIDER AVATAR & ATTIRE", () => CurrentScreen = GameScreen.Garage);
            DrawTopWallet(w);

            float panelW = Mathf.Min(340f, w * 0.38f);
            float panelX = w - panelW - 25f;
            float panelY = 70f;
            float panelH = h - panelY - 30f;

            GUI.DrawTexture(new Rect(panelX, panelY, panelW, panelH), _darkPanelTex);
            GUILayout.BeginArea(new Rect(panelX + 16, panelY + 16, panelW - 32, panelH - 32));

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
            titleStyle.normal.textColor = new Color(1f, 0.78f, 0.12f);
            GUILayout.Label("👤 RIDER CUSTOMIZATION", titleStyle);
            GUILayout.Space(10);

            // Gender Selection
            GUILayout.Label("<b>Rider Character Gender:</b>");
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = !_isFemaleAvatar ? new Color(1f, 0.76f, 0.08f) : Color.gray;
            if (GUILayout.Button("👨 MALE RIDER", GUILayout.Height(34)))
            {
                _isFemaleAvatar = false;
                ApplyCustomizationsToBike();
            }
            GUI.backgroundColor = _isFemaleAvatar ? new Color(1f, 0.76f, 0.08f) : Color.gray;
            if (GUILayout.Button("👩 FEMALE RIDER", GUILayout.Height(34)))
            {
                _isFemaleAvatar = true;
                ApplyCustomizationsToBike();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            // Jacket Color Selection
            GUILayout.Label("<b>Leather Riding Jacket:</b>");
            for (int j = 0; j < _jacketNames.Length; j++)
            {
                bool isSel = (_selectedJacketIdx == j);
                GUI.backgroundColor = isSel ? new Color(1f, 0.76f, 0.08f) : Color.gray;
                if (GUILayout.Button($"🧥 {_jacketNames[j]}", GUILayout.Height(28)))
                {
                    _selectedJacketIdx = j;
                    ApplyCustomizationsToBike();
                }
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(12);

            // Helmet Style Selection
            GUILayout.Label("<b>Helmet Selection:</b>");
            string[] styles = { "🪖 Full-Face Sport", "🪖 Classic Retro Open", "🪖 Motocross Peak Visor" };
            for (int hs = 0; hs < styles.Length; hs++)
            {
                bool isSel = (_selectedHelmetStyle == hs);
                GUI.backgroundColor = isSel ? new Color(1f, 0.76f, 0.08f) : Color.gray;
                if (GUILayout.Button(styles[hs], GUILayout.Height(28)))
                {
                    _selectedHelmetStyle = hs;
                    ApplyCustomizationsToBike();
                }
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(12);

            // Helmet Color Selection
            GUILayout.Label("<b>Helmet Color:</b>");
            GUILayout.BeginHorizontal();
            for (int hc = 0; hc < _helmetColorNames.Length; hc++)
            {
                bool isSel = (_selectedHelmetIdx == hc);
                GUI.backgroundColor = isSel ? new Color(1f, 0.76f, 0.08f) : Color.gray;
                if (GUILayout.Button(_helmetColorNames[hc], GUILayout.Height(28)))
                {
                    _selectedHelmetIdx = hc;
                    ApplyCustomizationsToBike();
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            GUIStyle saveStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 15 };
            saveStyle.normal.textColor = Color.black;
            if (GUILayout.Button("SAVE & RETURN ▶", saveStyle, GUILayout.Height(42)))
            {
                ApplyCustomizationsToBike();
                CurrentScreen = GameScreen.Garage;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();
        }

        private void ApplyCustomizationsToBike()
        {
            if (_motorcycle == null || _visualBuilder == null)
            {
                LocateSceneSystems();
            }

            if (_visualBuilder != null)
            {
                if (_visualBuilder.CurrentModelIndex != _selectedBikeIndex)
                {
                    _visualBuilder.BuildMotorcycleModel(_selectedBikeIndex);
                }
                _visualBuilder.SetBodyPaintColor(_bikeColors[_selectedColorIndex]);
                _visualBuilder.SetPassengerVisible(_isCoupleRiding);
                _visualBuilder.SetRiderAvatar(_isFemaleAvatar, _jacketColors[_selectedJacketIdx], _helmetColors[_selectedHelmetIdx], _selectedHelmetStyle);
            }

            if (_motorcycle != null)
            {
                float baseSpeed = _bikeSpeeds[_selectedBikeIndex] * 165f;
                _motorcycle.ApplyBikeTuning(
                    _bikeModelNames[_selectedBikeIndex],
                    baseSpeed,
                    8.5f,
                    28f
                );
            }
        }
    }
}
