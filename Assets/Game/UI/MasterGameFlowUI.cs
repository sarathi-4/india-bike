using System;
using System.Collections;
using System.IO;
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
        WelcomeAndLogin = 0,    // 1. Welcome & Login Screen (Matches Image 1)
        LoadingScreen = 1,      // 2. Loading Screen with Progress Bar (Matches Image 2)
        MainMenu = 2,           // 3. Main Play Hub / Open World Menu (Matches Image 3)
        Garage = 3,             // 4. Garage Turntable with 3D Models (Reference 3)
        IndiaMap = 4,           // 5. India Map State Selector (Reference 4)
        RidingHUD = 5,          // 6. In-Game 3D Riding HUD (Reference 5)
        Missions = 6,           // 7. Missions Catalogue (Reference 6)
        Multiplayer = 7,        // 8. Multiplayer Convoy (Reference 7)
        Settings = 8,           // 9. Settings (Reference 8)
        RewardScreen = 9,       // 10. Reward Screen (Reference 9)
        AvatarStudio = 10       // 11. Rider Avatar Studio
    }

    /// <summary>
    /// Master UI System strictly matching the user-provided visual targets:
    /// - Screen 1: Welcome Screen on Game Open with Coins, Diamonds, START RIDE, Sign Up & Login Features (Image 1)
    /// - Screen 2: Loading Screen with 75% Progress Bar, MADE IN INDIA, Tips, and TAMIL GAME STUDIO (Image 2)
    /// - Screen 3: Play Menu with 3D Open World, Player Profile Lv.1, and Right Action Stack (Image 3)
    /// - Complete connected Garage, India Map, 3D Riding Simulation, Missions, Multiplayer & Settings
    /// </summary>
    public class MasterGameFlowUI : MonoBehaviour
    {
        private static MasterGameFlowUI _instance;
        public static MasterGameFlowUI Instance => _instance;

        [Header("Active Screen State")]
        public GameScreen CurrentScreen = GameScreen.WelcomeAndLogin;
        public bool IsMenuOpen
        {
            get => CurrentScreen != GameScreen.RidingHUD;
            set => CurrentScreen = value ? GameScreen.MainMenu : GameScreen.RidingHUD;
        }

        // Texture Backdrops
        private Texture2D _welcomeTex;
        private Texture2D _loadingTex;
        private Texture2D _mainMenuTex;

        // 1. Welcome Screen & Auth State (Image 1)
        private bool _showLoginModal = true;
        private int _loginTab = 0; // 0=Sign In, 1=Create Account, 2=Guest Ride
        private string _playerName = "Player_001";
        private int _playerLevel = 1;
        private int _playerXP = 0;
        private int _silverCoins = 125750;
        private int _diamonds = 2560;
        private string _inputRiderName = "Player_001";
        private string _inputEmail = "rider@indianbike.com";
        private string _inputPassword = "********";
        private string _authMessage = "Signed in as Player_001 (Guest Ride)";
        private bool _rememberMe = true;
        private bool _isAuthenticated = false;

        // 2. Loading Screen State (Image 2)
        private float _loadingProgress = 0.05f;
        private int _currentTipIndex = 0;
        private readonly string[] _loadingTips = new[]
        {
            "💡 Tip: Use GPS to find the best route!",
            "💡 Tip: Touch ◀ ▶ or use A / D keys to lean smoothly into sharp highway curves!",
            "💡 Tip: Swipe or drag anywhere on the screen for full 360° camera orbit view!",
            "💡 Tip: Switch between 6 authentic Indian motorcycles in the Garage!",
            "💡 Tip: Enable Couple Riding to ride with a passenger across Tamil Nadu NH-44!",
            "💡 Tip: Upgrade engine, handling and brakes in the Garage for peak top speed!"
        };

        // 3. Sound & Audio Settings (Image 3)
        private bool _soundEnabled = true;
        private bool _musicEnabled = true;

        // 4. Garage & Motorcycle State
        private int _selectedBikeIndex = 3; // Default to KTM 390 Duke matching reference hero
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

        private int _selectedColorIndex = 4; // KTM Orange
        private bool _isCoupleRiding = false;
        private readonly Color[] _bikeColors = new[]
        {
            new Color(0.85f, 0.12f, 0.15f), // Racing Red
            new Color(0.12f, 0.35f, 0.85f), // Metallic Blue
            new Color(0.95f, 0.95f, 0.98f), // Pearl White
            new Color(0.12f, 0.12f, 0.15f), // Matte Black
            new Color(1.00f, 0.45f, 0.05f)  // KTM Neon Orange
        };

        // 5. India Map State
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
            "Districts: 38
Locations: 250+
Corridors: NH-44 Express & Ooty Ghats",
            "Districts: 36
Locations: 210+
Corridors: Mumbai-Pune Expressway",
            "Districts: 2
Locations: 80+
Corridors: Coastal Beach Marine Drive",
            "Districts: 12
Locations: 120+
Corridors: Manali-Rohtang Snow Pass",
            "Districts: 2
Locations: 65+
Corridors: Khardung La 17,582 ft High Pass",
            "Districts: 14
Locations: 140+
Corridors: Munnar Tea Garden Hairpin Bends",
            "Districts: 23
Locations: 160+
Corridors: Grand Trunk Road & Sher-e-Punjab",
            "Districts: 31
Locations: 190+
Corridors: Bangalore-Mysore Highway"
        };

        // 6. Missions State
        private int _missionTab = 0;
        private bool _mission1Claimed = false;
        private bool _mission2Claimed = false;
        private bool _mission3Claimed = false;

        // 7. Multiplayer State
        private string _roomCodeInput = "TN-44-ROYAL";
        private string _myGeneratedRoomCode = "TN-44-ROYAL";
        private string _multiplayerStatus = "Ready to connect to Indian Highway Convoy.";
        private bool _isRoomCodeCopied = false;
        private readonly string[] _convoyFriends = new[] { "Arun_Rider99 (Online)", "Kavitha_Bullet (Online)", "Siva_Duke390 (In-Ride)", "Vijay_Pulsar (Online)" };

        // 8. Settings State
        private int _settingsTab = 0;
        private int _graphicsQualityIndex = 3;
        private int _frameRateIndex = 1;
        private bool _bloomEnabled = true;
        private bool _shadowsEnabled = true;
        private float _masterVolume = 1.0f;
        private float _engineVolume = 1.0f;
        private float _hornVolume = 1.0f;
        private int _selectedLanguageIndex = 0;

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
            new Color(1.0f, 0.45f, 0.05f)
        };
        private int _selectedHelmetIdx = 0;
        private readonly string[] _helmetColorNames = new[] { "Saffron Gold", "Matte Black", "Pearl White", "Crimson Red" };
        private readonly Color[] _helmetColors = new[]
        {
            new Color(1.0f, 0.76f, 0.08f),
            new Color(0.12f, 0.12f, 0.14f),
            new Color(0.95f, 0.95f, 0.98f),
            new Color(0.85f, 0.15f, 0.18f)
        };
        private int _selectedHelmetStyle = 0;

        // Backend & Simulation References
        private AuthenticationManager _authManager;
        private PlayerMotorcycleController _motorcycle;
        private MotorcycleVisualBuilder _visualBuilder;
        private IEconomyManager _economyManager;
        private IProgressionManager _progressionManager;
        private TimeOfDayController _timeController;

        // Procedural Textures
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
            LoadArtworkTextures();
        }

        private void Start()
        {
            LocateSceneSystems();
            ApplyCustomizationsToBike();

            // Load saved rider authentication if present
            if (PlayerPrefs.HasKey("IBL_RiderName"))
            {
                _playerName = PlayerPrefs.GetString("IBL_RiderName", "Player_001");
                _inputRiderName = _playerName;
                _isAuthenticated = true;
                _showLoginModal = false;
            }

            // Always start on Screen 1: Welcome & Login (Image 1)
            CurrentScreen = GameScreen.WelcomeAndLogin;
        }

        private void LoadArtworkTextures()
        {
            _welcomeTex = LoadTextureFile("Screen1_Welcome.jpg");
            _loadingTex = LoadTextureFile("Screen2_Loading.jpg");
            _mainMenuTex = LoadTextureFile("Screen3_MainMenu.jpg");
        }

        private Texture2D LoadTextureFile(string filename)
        {
            try
            {
                string fullPath = Path.Combine(Application.dataPath, "Game/UI/Textures", filename);
                if (File.Exists(fullPath))
                {
                    byte[] bytes = File.ReadAllBytes(fullPath);
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
                    if (tex.LoadImage(bytes))
                    {
                        tex.filterMode = FilterMode.Bilinear;
                        tex.wrapMode = TextureWrapMode.Clamp;
                        return tex;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[MasterGameFlowUI] Could not load texture {filename}: {e.Message}");
            }
            return null;
        }

        private void CreateProceduralTextures()
        {
            _blueHeaderTex = MakeColorTex(new Color(0.05f, 0.18f, 0.44f, 0.96f));
            _goldBtnTex = MakeColorTex(new Color(1.0f, 0.76f, 0.08f, 1.0f));
            _darkPanelTex = MakeColorTex(new Color(0.04f, 0.09f, 0.18f, 0.92f));
            _lightPanelTex = MakeColorTex(new Color(0.08f, 0.18f, 0.32f, 0.88f));
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
            }
        }

        public void LaunchLoadingToRide()
        {
            StartCoroutine(LoadingSequenceRoutine(launchDirectRiding: true));
        }

        public void LaunchLoadingToMenu()
        {
            StartCoroutine(LoadingSequenceRoutine(launchDirectRiding: false));
        }

        private IEnumerator LoadingSequenceRoutine(bool launchDirectRiding)
        {
            CurrentScreen = GameScreen.LoadingScreen;
            _loadingProgress = 0.05f;
            _currentTipIndex = UnityEngine.Random.Range(0, _loadingTips.Length);

            float elapsed = 0f;
            float duration = 1.6f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _loadingProgress = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            _loadingProgress = 1.0f;
            yield return new WaitForSeconds(0.2f);

            ApplyCustomizationsToBike();

            if (launchDirectRiding)
            {
                CurrentScreen = GameScreen.RidingHUD;
            }
            else
            {
                CurrentScreen = GameScreen.MainMenu;
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                if (CurrentScreen == GameScreen.RidingHUD)
                {
                    CurrentScreen = GameScreen.MainMenu;
                }
                else if (CurrentScreen != GameScreen.LoadingScreen && CurrentScreen != GameScreen.WelcomeAndLogin)
                {
                    CurrentScreen = GameScreen.RidingHUD;
                }
            }

            // Sync wallet
            if (_economyManager != null)
            {
                _silverCoins = _economyManager.SilverCoins;
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
                case GameScreen.WelcomeAndLogin:
                    DrawScreen1_WelcomeAndLogin(w, h);
                    break;

                case GameScreen.LoadingScreen:
                    DrawScreen2_Loading(w, h);
                    break;

                case GameScreen.MainMenu:
                    DrawScreen3_MainMenu(w, h);
                    break;

                case GameScreen.Garage:
                    DrawScreen4_Garage(w, h);
                    break;

                case GameScreen.IndiaMap:
                    DrawScreen5_IndiaMap(w, h);
                    break;

                case GameScreen.RidingHUD:
                    DrawScreen6_RidingHUD(w, h);
                    break;

                case GameScreen.Missions:
                    DrawScreen7_Missions(w, h);
                    break;

                case GameScreen.Multiplayer:
                    DrawScreen8_Multiplayer(w, h);
                    break;

                case GameScreen.Settings:
                    DrawScreen9_Settings(w, h);
                    break;

                case GameScreen.RewardScreen:
                    DrawScreen10_RewardScreen(w, h);
                    break;

                case GameScreen.AvatarStudio:
                    DrawScreen11_AvatarStudio(w, h);
                    break;
            }
        }

        // =========================================================================
        // SCREEN 1: WELCOME & LOGIN SCREEN (Matches Image 1)
        // =========================================================================
        private void DrawScreen1_WelcomeAndLogin(int w, int h)
        {
            // Draw Full Background Artwork (Image 1)
            if (_welcomeTex != null)
            {
                GUI.DrawTexture(new Rect(0, 0, w, h), _welcomeTex, ScaleMode.ScaleAndCrop);
            }
            else
            {
                GUI.Box(new Rect(0, 0, w, h), "");
            }

            DrawScreenTag("1. Welcome & Login");

            // Top Bar: Coins (125,750), Diamonds (2,560), Settings (⚙️), Bell (🔔)
            DrawTopBar_Welcome(w, h);

            // Left Navigation Stack (PLAY, GARAGE, SHOWROOM, MAP, MISSIONS, MULTIPLAYER)
            DrawLeftMenu_Welcome(w, h);

            // Bottom Left Navigation (PROFILE, ACHIEVEMENTS, REWARDS, WALLET)
            DrawBottomNav_Welcome(w, h);

            // Top-Right Promo Card (NEW BIKE UNLOCK NOW!)
            DrawPromoCard_Welcome(w, h);

            // Bottom-Right Giant Glowing CTA: START RIDE >>
            DrawStartRideButton_Welcome(w, h);

            // Prominent Rider Authentication Modal (Sign Up / Login / Guest)
            if (_showLoginModal)
            {
                DrawLoginAndSignupModal(w, h);
            }
        }

        private void DrawTopBar_Welcome(int w, int h)
        {
            float barY = 15f;

            // Left: Player Profile / Auth Pill
            GUI.DrawTexture(new Rect(20, barY, 200, 38), _darkPanelTex);
            GUIStyle riderStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 };
            riderStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(28, barY + 4, 150, 20), $"👤 {_playerName}", riderStyle);

            GUIStyle authBtnStyle = new GUIStyle(GUI.skin.button) { fontSize = 10, fontStyle = FontStyle.Bold };
            if (GUI.Button(new Rect(155, barY + 5, 60, 28), _isAuthenticated ? "LOGOUT" : "LOGIN", authBtnStyle))
            {
                _showLoginModal = !_showLoginModal;
            }

            // Right: Currency Bar (Coins 125,750 +, Diamonds 2,560 +, Settings, Bell)
            float curX = w - 420f;

            // Coins Badge
            GUI.DrawTexture(new Rect(curX, barY, 140, 36), _darkPanelTex);
            GUIStyle curStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13, alignment = TextAnchor.MiddleLeft };
            curStyle.normal.textColor = new Color(1.0f, 0.85f, 0.20f);
            GUI.Label(new Rect(curX + 8, barY + 4, 100, 26), $"🪙 {_silverCoins:N0}", curStyle);
            if (GUI.Button(new Rect(curX + 110, barY + 4, 24, 26), "+"))
            {
                _silverCoins += 5000;
                if (_economyManager != null) _economyManager.AddCurrency(CurrencyType.SilverCoins, 5000, "Bonus");
            }

            // Diamonds Badge
            curX += 148f;
            GUI.DrawTexture(new Rect(curX, barY, 125, 36), _darkPanelTex);
            curStyle.normal.textColor = new Color(0.35f, 0.75f, 1.0f);
            GUI.Label(new Rect(curX + 8, barY + 4, 90, 26), $"💎 {_diamonds:N0}", curStyle);
            if (GUI.Button(new Rect(curX + 96, barY + 4, 24, 26), "+"))
            {
                _diamonds += 250;
            }

            // Settings ⚙️
            curX += 134f;
            if (GUI.Button(new Rect(curX, barY, 38, 36), "⚙️"))
            {
                CurrentScreen = GameScreen.Settings;
            }

            // Notification Bell 🔔
            curX += 44f;
            if (GUI.Button(new Rect(curX, barY, 38, 36), "🔔"))
            {
                _authMessage = "Daily Bonus Ready! Claimed +500 Silver Coins.";
                _silverCoins += 500;
            }
        }

        private void DrawLeftMenu_Welcome(int w, int h)
        {
            float menuX = 20f;
            float menuY = h * 0.32f;
            float btnW = Mathf.Min(210f, w * 0.24f);
            float btnH = 40f;
            float gap = 8f;

            // Big Yellow PLAY Button
            GUI.backgroundColor = new Color(1.0f, 0.78f, 0.08f);
            GUIStyle bigPlayStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 18 };
            bigPlayStyle.normal.textColor = Color.black;
            if (GUI.Button(new Rect(menuX, menuY, btnW, 48), "▶  PLAY", bigPlayStyle))
            {
                LaunchLoadingToRide();
            }
            GUI.backgroundColor = Color.white;
            menuY += 56f;

            // Nav buttons
            DrawMenuNavButton(menuX, menuY, btnW, btnH, "🏠  GARAGE", () => CurrentScreen = GameScreen.Garage);
            menuY += btnH + gap;
            DrawMenuNavButton(menuX, menuY, btnW, btnH, "🛒  SHOWROOM", () => CurrentScreen = GameScreen.Garage);
            menuY += btnH + gap;
            DrawMenuNavButton(menuX, menuY, btnW, btnH, "📍  MAP", () => CurrentScreen = GameScreen.IndiaMap);
            menuY += btnH + gap;
            DrawMenuNavButton(menuX, menuY, btnW, btnH, "🏆  MISSIONS", () => CurrentScreen = GameScreen.Missions);
            menuY += btnH + gap;
            DrawMenuNavButton(menuX, menuY, btnW, btnH, "👥  MULTIPLAYER", () => CurrentScreen = GameScreen.Multiplayer);
        }

        private void DrawBottomNav_Welcome(int w, int h)
        {
            float navY = h - 52f;
            float navX = 20f;
            float btnW = 105f;
            float btnH = 36f;
            float gap = 10f;

            DrawMenuNavButton(navX, navY, btnW, btnH, "👑 PROFILE", () => { CurrentScreen = GameScreen.Settings; _settingsTab = 4; });
            navX += btnW + gap;
            DrawMenuNavButton(navX, navY, btnW + 15, btnH, "⭐ AWARDS", () => CurrentScreen = GameScreen.RewardScreen);
            navX += btnW + gap + 15;
            DrawMenuNavButton(navX, navY, btnW, btnH, "🎁 REWARDS", () => { _silverCoins += 1000; _authMessage = "Claimed +1,000 Rewards!"; });
            navX += btnW + gap;
            DrawMenuNavButton(navX, navY, btnW, btnH, "👛 WALLET", () => { _showLoginModal = true; _loginTab = 0; });
        }

        private void DrawPromoCard_Welcome(int w, int h)
        {
            float cardW = 190f;
            float cardH = 95f;
            float cardX = w - cardW - 25f;
            float cardY = 70f;

            GUI.DrawTexture(new Rect(cardX, cardY, cardW, cardH), _darkPanelTex);
            GUIStyle promoStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 12 };
            promoStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(cardX, cardY + 8, cardW, 20), "NEW BIKE UNLOCKED!", promoStyle);

            GUI.backgroundColor = new Color(0.85f, 0.15f, 0.15f);
            if (GUI.Button(new Rect(cardX + 15, cardY + cardH - 36, cardW - 30, 26), "UNLOCK NOW >>"))
            {
                _selectedBikeIndex = 0; // Yamaha R15 V4
                CurrentScreen = GameScreen.Garage;
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawStartRideButton_Welcome(int w, int h)
        {
            float btnW = Mathf.Min(260f, w * 0.32f);
            float btnH = 58f;
            float btnX = w - btnW - 25f;
            float btnY = h - btnH - 22f;

            GUI.backgroundColor = new Color(1.0f, 0.76f, 0.08f);
            GUIStyle rideStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Max(20, h * 28 / 1000),
                fontStyle = FontStyle.Bold
            };
            rideStyle.normal.textColor = Color.black;

            if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "START RIDE >>", rideStyle))
            {
                LaunchLoadingToRide();
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawLoginAndSignupModal(int w, int h)
        {
            // Dim background overlay
            GUI.Box(new Rect(0, 0, w, h), "");

            float modalW = Mathf.Min(480f, w * 0.88f);
            float modalH = Mathf.Min(390f, h * 0.85f);
            float modalX = (w - modalW) * 0.5f;
            float modalY = (h - modalH) * 0.5f;

            // Modal Container
            GUI.DrawTexture(new Rect(modalX, modalY, modalW, modalH), _darkPanelTex);

            // Modal Header
            GUI.DrawTexture(new Rect(modalX, modalY, modalW, 46), _blueHeaderTex);
            GUIStyle headStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };
            headStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(modalX, modalY + 10, modalW, 26), "🇮🇳 INDIAN BIKE LIFE — RIDER AUTHENTICATION", headStyle);

            // Close button
            if (GUI.Button(new Rect(modalX + modalW - 36, modalY + 8, 28, 28), "✖"))
            {
                _showLoginModal = false;
            }

            // Tab bar: [SIGN IN] | [CREATE ACCOUNT] | [GUEST PLAY]
            float tabY = modalY + 54f;
            float tabW = (modalW - 40f) / 3f;

            for (int t = 0; t < 3; t++)
            {
                string tabTitle = t == 0 ? "🔑 SIGN IN" : (t == 1 ? "📝 SIGN UP" : "⚡ GUEST");
                bool isSel = (_loginTab == t);
                GUI.backgroundColor = isSel ? new Color(1.0f, 0.78f, 0.12f) : new Color(0.2f, 0.28f, 0.40f);
                if (GUI.Button(new Rect(modalX + 20 + t * tabW, tabY, tabW - 4, 34), tabTitle))
                {
                    _loginTab = t;
                }
            }
            GUI.backgroundColor = Color.white;

            // Form Area
            float formY = tabY + 48f;
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 12 };
            labelStyle.normal.textColor = Color.white;

            if (_loginTab == 0) // SIGN IN
            {
                GUI.Label(new Rect(modalX + 25, formY, 150, 20), "Email or Mobile Number:", labelStyle);
                _inputEmail = GUI.TextField(new Rect(modalX + 25, formY + 22, modalW - 50, 32), _inputEmail);

                formY += 62f;
                GUI.Label(new Rect(modalX + 25, formY, 150, 20), "Password:", labelStyle);
                _inputPassword = GUI.PasswordField(new Rect(modalX + 25, formY + 22, modalW - 50, 32), _inputPassword, '*');

                formY += 62f;
                _rememberMe = GUI.Toggle(new Rect(modalX + 25, formY, 180, 22), _rememberMe, " Remember Me");

                formY += 30f;
                GUI.backgroundColor = new Color(1.0f, 0.76f, 0.08f);
                GUIStyle btnStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 15 };
                btnStyle.normal.textColor = Color.black;
                if (GUI.Button(new Rect(modalX + 25, formY, modalW - 50, 42), "SIGN IN ▶", btnStyle))
                {
                    _playerName = string.IsNullOrEmpty(_inputEmail) ? "Player_001" : _inputEmail.Split('@')[0];
                    _isAuthenticated = true;
                    PlayerPrefs.SetString("IBL_RiderName", _playerName);
                    PlayerPrefs.Save();
                    _showLoginModal = false;
                }
                GUI.backgroundColor = Color.white;
            }
            else if (_loginTab == 1) // SIGN UP
            {
                GUI.Label(new Rect(modalX + 25, formY, 150, 20), "Rider Screen Name:", labelStyle);
                _inputRiderName = GUI.TextField(new Rect(modalX + 25, formY + 22, modalW - 50, 30), _inputRiderName);

                formY += 58f;
                GUI.Label(new Rect(modalX + 25, formY, 150, 20), "Email or Mobile:", labelStyle);
                _inputEmail = GUI.TextField(new Rect(modalX + 25, formY + 22, modalW - 50, 30), _inputEmail);

                formY += 58f;
                GUI.Label(new Rect(modalX + 25, formY, 150, 20), "Password:", labelStyle);
                _inputPassword = GUI.PasswordField(new Rect(modalX + 25, formY + 22, modalW - 50, 30), _inputPassword, '*');

                formY += 60f;
                GUI.backgroundColor = new Color(0.12f, 0.75f, 0.35f);
                GUIStyle signUpStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 14 };
                if (GUI.Button(new Rect(modalX + 25, formY, modalW - 50, 42), "CREATE ACCOUNT ▶ (+2,500 COINS)", signUpStyle))
                {
                    _playerName = string.IsNullOrEmpty(_inputRiderName) ? "Rider_Pro" : _inputRiderName;
                    _silverCoins += 2500;
                    _isAuthenticated = true;
                    PlayerPrefs.SetString("IBL_RiderName", _playerName);
                    PlayerPrefs.Save();
                    _showLoginModal = false;
                }
                GUI.backgroundColor = Color.white;
            }
            else // GUEST PLAY
            {
                GUIStyle guestDesc = new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 13 };
                guestDesc.normal.textColor = new Color(0.9f, 0.9f, 0.95f);
                GUI.Label(new Rect(modalX + 25, formY + 15, modalW - 50, 70),
                    "Ride immediately across India without an account! Your high scores, tuned bikes and silver coins will be saved locally on this device.", guestDesc);

                formY += 95f;
                GUI.backgroundColor = new Color(1.0f, 0.78f, 0.08f);
                GUIStyle guestBtn = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 15 };
                guestBtn.normal.textColor = Color.black;
                if (GUI.Button(new Rect(modalX + 25, formY, modalW - 50, 46), "CONTINUE AS GUEST ▶", guestBtn))
                {
                    _playerName = "Guest_Rider";
                    _isAuthenticated = false;
                    _showLoginModal = false;
                }
                GUI.backgroundColor = Color.white;
            }
        }

        // =========================================================================
        // SCREEN 2: LOADING SCREEN (Matches Image 2)
        // =========================================================================
        private void DrawScreen2_Loading(int w, int h)
        {
            // Draw Full Background Artwork (Image 2)
            if (_loadingTex != null)
            {
                GUI.DrawTexture(new Rect(0, 0, w, h), _loadingTex, ScaleMode.ScaleAndCrop);
            }
            else
            {
                GUI.Box(new Rect(0, 0, w, h), "");
            }

            DrawScreenTag("2. Loading Screen");

            // Dynamic Animated Loading Bar (Positioned exactly matching Image 2 at bottom center)
            float barW = Mathf.Min(540f, w * 0.52f);
            float barH = 20f;
            float barX = (w - barW) * 0.5f;
            float barY = h * 0.835f;

            // Background Track
            GUI.DrawTexture(new Rect(barX - 2, barY - 2, barW + 4, barH + 4), _darkPanelTex);

            // Glowing Golden-Orange Progress Fill
            int pct = Mathf.Clamp(Mathf.RoundToInt(_loadingProgress * 100f), 0, 100);
            GUI.DrawTexture(new Rect(barX, barY, barW * _loadingProgress, barH), _goldBtnTex);

            // Percentage Label: Loading... 75%
            GUIStyle pctStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(14, h * 20 / 1000),
                fontStyle = FontStyle.Bold
            };
            pctStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(barX, barY - 28, barW, 26), $"Loading... {pct}%", pctStyle);

            // Cycling Tip under progress bar
            GUIStyle tipStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(12, h * 16 / 1000),
                fontStyle = FontStyle.Bold
            };
            tipStyle.normal.textColor = new Color(1.0f, 0.90f, 0.20f);
            GUI.Label(new Rect(barX - 80, barY + barH + 6, barW + 160, 26), _loadingTips[_currentTipIndex], tipStyle);
        }

        // =========================================================================
        // SCREEN 3: MAIN MENU / PLAY HUB (Matches Image 3)
        // =========================================================================
        private void DrawScreen3_MainMenu(int w, int h)
        {
            // Draw Full Background Artwork (Image 3)
            if (_mainMenuTex != null)
            {
                GUI.DrawTexture(new Rect(0, 0, w, h), _mainMenuTex, ScaleMode.ScaleAndCrop);
            }
            else
            {
                GUI.Box(new Rect(0, 0, w, h), "");
            }

            DrawScreenTag("3. Main Play Hub");

            // Top-Right Audio & Settings Quick Buttons (Sound, Music, Settings)
            float trX = w - 145f;
            float trY = 18f;

            if (GUI.Button(new Rect(trX, trY, 36, 36), _soundEnabled ? "🔊" : "🔇"))
            {
                _soundEnabled = !_soundEnabled;
            }
            if (GUI.Button(new Rect(trX + 42, trY, 36, 36), _musicEnabled ? "🎵" : "🔈"))
            {
                _musicEnabled = !_musicEnabled;
            }
            if (GUI.Button(new Rect(trX + 84, trY, 36, 36), "⚙️"))
            {
                CurrentScreen = GameScreen.Settings;
            }

            // Bottom-Left Player Profile Card (Player_001, Lv. 1, 0/100 XP)
            float profX = 25f;
            float profY = h - 75f;
            GUI.DrawTexture(new Rect(profX, profY, 210, 56), _darkPanelTex);

            GUIStyle nameStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 14 };
            nameStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(profX + 12, profY + 6, 180, 20), $"👤 {_playerName}", nameStyle);

            GUIStyle lvStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
            lvStyle.normal.textColor = new Color(1.0f, 0.80f, 0.20f);
            GUI.Label(new Rect(profX + 12, profY + 28, 180, 18), $"Lv. {_playerLevel}   XP: {_playerXP} / 100", lvStyle);

            // Right-Side Main Action Stack (PLAY, GARAGE, MAP, MISSIONS, MULTIPLAYER, SETTINGS)
            float rightStackW = Mathf.Min(220f, w * 0.26f);
            float rightStackX = w - rightStackW - 25f;
            float rightStackY = h * 0.38f;
            float btnH = 42f;
            float gap = 7f;

            // Big Yellow PLAY Button
            GUI.backgroundColor = new Color(1.0f, 0.78f, 0.08f);
            GUIStyle playBtnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Max(20, h * 28 / 1000),
                fontStyle = FontStyle.Bold
            };
            playBtnStyle.normal.textColor = Color.black;
            if (GUI.Button(new Rect(rightStackX, rightStackY, rightStackW, 52), "▶  PLAY", playBtnStyle))
            {
                ApplyCustomizationsToBike();
                CurrentScreen = GameScreen.RidingHUD;
            }
            GUI.backgroundColor = Color.white;
            rightStackY += 58f;

            DrawMenuNavButton(rightStackX, rightStackY, rightStackW, btnH, "🏠  GARAGE", () => CurrentScreen = GameScreen.Garage);
            rightStackY += btnH + gap;
            DrawMenuNavButton(rightStackX, rightStackY, rightStackW, btnH, "📍  MAP", () => CurrentScreen = GameScreen.IndiaMap);
            rightStackY += btnH + gap;
            DrawMenuNavButton(rightStackX, rightStackY, rightStackW, btnH, "🏆  MISSIONS", () => CurrentScreen = GameScreen.Missions);
            rightStackY += btnH + gap;
            DrawMenuNavButton(rightStackX, rightStackY, rightStackW, btnH, "👥  MULTIPLAYER", () => CurrentScreen = GameScreen.Multiplayer);
            rightStackY += btnH + gap;
            DrawMenuNavButton(rightStackX, rightStackY, rightStackW, btnH, "⚙️  SETTINGS", () => CurrentScreen = GameScreen.Settings);
        }

        // =========================================================================
        // SCREEN 4: GARAGE (Matches Reference 3)
        // =========================================================================
        private void DrawScreen4_Garage(int w, int h)
        {
            DrawScreenTag("4. Garage");

            // Header Back Button & Title
            DrawHeaderWithBack(w, "GARAGE & SHOWROOM", () => CurrentScreen = GameScreen.MainMenu);
            DrawTopWallet(w);

            // Right Motorcycle Spec & Action Card (Reference 3)
            float panelW = Mathf.Min(320f, w * 0.32f);
            float panelX = w - panelW - 25f;
            float panelY = 70f;

            GUI.DrawTexture(new Rect(panelX, panelY, panelW, h - panelY - 95), _darkPanelTex);
            GUILayout.BeginArea(new Rect(panelX + 16, panelY + 16, panelW - 32, h - panelY - 120));

            GUIStyle bikeTitleStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold };
            bikeTitleStyle.normal.textColor = Color.white;
            GUILayout.Label(_bikeModelNames[_selectedBikeIndex], bikeTitleStyle);
            GUILayout.Space(10);

            // Specs
            DrawSpecBar("Speed", _bikeSpeeds[_selectedBikeIndex]);
            DrawSpecBar("Handling", _bikeHandlings[_selectedBikeIndex]);
            DrawSpecBar("Braking", _bikeBrakings[_selectedBikeIndex]);
            DrawSpecBar("Fuel", _bikeFuels[_selectedBikeIndex]);

            GUILayout.Space(14);

            // UPGRADE Button
            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            GUIStyle upStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 14 };
            upStyle.normal.textColor = Color.black;
            if (GUILayout.Button("UPGRADE (₹500)", upStyle, GUILayout.Height(38)))
            {
                if (_silverCoins >= 500)
                {
                    _silverCoins -= 500;
                    _bikeSpeeds[_selectedBikeIndex] = Mathf.Min(1.0f, _bikeSpeeds[_selectedBikeIndex] + 0.04f);
                    ApplyCustomizationsToBike();
                }
            }

            GUILayout.Space(5);
            // BIKE COLOR Button
            GUI.backgroundColor = new Color(0.12f, 0.50f, 0.98f);
            if (GUILayout.Button("🎨 BIKE COLOR", GUILayout.Height(30)))
            {
                _selectedColorIndex = (_selectedColorIndex + 1) % _bikeColors.Length;
                ApplyCustomizationsToBike();
            }

            GUILayout.Space(5);
            // AVATAR Button
            GUI.backgroundColor = new Color(1.0f, 0.55f, 0.12f);
            if (GUILayout.Button("👤 RIDER AVATAR", GUILayout.Height(30)))
            {
                CurrentScreen = GameScreen.AvatarStudio;
            }

            GUILayout.Space(5);
            // Couple Riding Toggle
            GUI.backgroundColor = _isCoupleRiding ? new Color(0.85f, 0.35f, 0.65f) : new Color(0.35f, 0.45f, 0.60f);
            if (GUILayout.Button(_isCoupleRiding ? "👥 COUPLE: ON" : "👤 SOLO RIDER", GUILayout.Height(30)))
            {
                _isCoupleRiding = !_isCoupleRiding;
                ApplyCustomizationsToBike();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();

            // Bottom Carousel
            float carouselH = 75f;
            float carouselY = h - carouselH - 10f;
            float carouselW = w - 50f;
            float carouselX = 25f;

            GUI.DrawTexture(new Rect(carouselX, carouselY, carouselW, carouselH), _darkPanelTex);

            if (GUI.Button(new Rect(carouselX + 8, carouselY + 12, 45, carouselH - 24), "◀"))
            {
                _selectedBikeIndex = (_selectedBikeIndex - 1 + _bikeModelNames.Length) % _bikeModelNames.Length;
                ApplyCustomizationsToBike();
            }

            float slotW = (carouselW - 130f) / _bikeModelNames.Length;
            for (int b = 0; b < _bikeModelNames.Length; b++)
            {
                float slotX = carouselX + 60f + b * slotW;
                bool isCur = (_selectedBikeIndex == b);
                GUI.DrawTexture(new Rect(slotX, carouselY + 8, slotW - 6, carouselH - 16), isCur ? _goldBtnTex : _lightPanelTex);

                GUIStyle slotStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = Mathf.Max(10, h * 13 / 1000),
                    fontStyle = FontStyle.Bold
                };
                slotStyle.normal.textColor = isCur ? Color.black : Color.white;
                if (GUI.Button(new Rect(slotX, carouselY + 8, slotW - 6, carouselH - 16), _bikeModelNames[b], slotStyle))
                {
                    _selectedBikeIndex = b;
                    ApplyCustomizationsToBike();
                }
            }

            if (GUI.Button(new Rect(carouselX + carouselW - 53, carouselY + 12, 45, carouselH - 24), "▶"))
            {
                _selectedBikeIndex = (_selectedBikeIndex + 1) % _bikeModelNames.Length;
                ApplyCustomizationsToBike();
            }
        }

        // =========================================================================
        // SCREEN 5: INDIA MAP (Matches Reference 4)
        // =========================================================================
        private void DrawScreen5_IndiaMap(int w, int h)
        {
            DrawScreenTag("5. India Map");
            DrawHeaderWithBack(w, "INDIA HIGHWAY MAP", () => CurrentScreen = GameScreen.MainMenu);

            // Left States Grid
            float gridW = Mathf.Min(400f, w * 0.44f);
            float startY = 70f;
            GUI.DrawTexture(new Rect(25, startY, gridW, h - startY - 20), _darkPanelTex);

            GUILayout.BeginArea(new Rect(35, startY + 12, gridW - 20, h - startY - 35));
            GUILayout.Label("<b>SELECT HIGHWAY CORRIDOR</b>", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUILayout.Space(8);

            for (int i = 0; i < _stateNames.Length; i++)
            {
                bool isSel = (_selectedStateIndex == i);
                GUI.backgroundColor = isSel ? new Color(1f, 0.76f, 0.08f) : new Color(0.15f, 0.28f, 0.48f);
                if (GUILayout.Button(_stateNames[i], GUILayout.Height(36)))
                {
                    _selectedStateIndex = i;
                    var env = FindFirstObjectByType<RoadAndEnvironmentBuilder>();
                    if (env != null) env.LoadStateEnvironment(_selectedStateIndex);
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndArea();

            // Right State Details Card
            float cardX = 25 + gridW + 20f;
            float cardW = w - cardX - 25f;
            GUI.DrawTexture(new Rect(cardX, startY, cardW, h - startY - 20), _darkPanelTex);

            GUILayout.BeginArea(new Rect(cardX + 20, startY + 20, cardW - 40, h - startY - 45));
            GUIStyle stHead = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
            stHead.normal.textColor = new Color(1f, 0.80f, 0.20f);
            GUILayout.Label(_stateNames[_selectedStateIndex], stHead);
            GUILayout.Space(12);

            GUIStyle descStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            descStyle.normal.textColor = Color.white;
            GUILayout.Label(_stateDescriptions[_selectedStateIndex], descStyle);

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            GUIStyle selBtn = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 18 };
            selBtn.normal.textColor = Color.black;
            if (GUILayout.Button("SELECT STATE ▶", selBtn, GUILayout.Height(52)))
            {
                var env = FindFirstObjectByType<RoadAndEnvironmentBuilder>();
                if (env != null) env.LoadStateEnvironment(_selectedStateIndex);
                ApplyCustomizationsToBike();
                CurrentScreen = GameScreen.RidingHUD;
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndArea();
        }

        // =========================================================================
        // SCREEN 6: RIDING HUD (Matches Reference 5)
        // =========================================================================
        private void DrawScreen6_RidingHUD(int w, int h)
        {
            // Top-Left GPS Radar
            float mapSize = 85f;
            GUI.DrawTexture(new Rect(20, 20, mapSize, mapSize), _darkPanelTex);
            GUIStyle gpsStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            gpsStyle.normal.textColor = new Color(0.2f, 0.95f, 0.45f);
            GUI.Label(new Rect(20, 20, mapSize, mapSize), $"🧭 GPS
{_stateNames[_selectedStateIndex]}", gpsStyle);

            // Top-Center: Fuel Bar
            float fuelW = 180f;
            float fuelH = 16f;
            float fuelX = (w - fuelW) * 0.5f;
            GUI.DrawTexture(new Rect(fuelX, 15, fuelW, fuelH), _darkPanelTex);
            GUI.DrawTexture(new Rect(fuelX, 15, fuelW * 0.90f, fuelH), _greenBtnTex);
            GUI.Label(new Rect(fuelX - 25, 12, 22, 20), "⛽");

            // Top-Right: Speedometer & Gear
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

            // Red Gear Badge
            GUI.backgroundColor = new Color(0.85f, 0.15f, 0.15f);
            GUI.Box(new Rect(speedBoxX + speedBoxW - 24, 18, 20, 22), $"{curGear}");
            GUI.backgroundColor = Color.white;

            // Right Vertical Control Icons: Camera 📷, Horn 📢, Couple 👥, Menu ⚙️
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

            // Bottom-Left: Steering Touch Arrows ◀ ▶
            float steerY = h - 90f;
            if (GUI.RepeatButton(new Rect(30, steerY, 70, 70), "◀"))
            {
                _motorcycle?.SetSteerInput(-0.85f);
            }
            if (GUI.RepeatButton(new Rect(115, steerY, 70, 70), "▶"))
            {
                _motorcycle?.SetSteerInput(0.85f);
            }

            // Center Upright Re-center Button & 360 Hint
            if (GUI.Button(new Rect(w * 0.5f - 65f, h - 45f, 130f, 32f), "🔄 RE-CENTER"))
            {
                _motorcycle?.RespawnOnRoad();
            }

            GUIStyle hintStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            hintStyle.normal.textColor = new Color(1f, 1f, 1f, 0.75f);
            GUI.Label(new Rect(w * 0.5f - 160f, h - 72f, 320f, 22f), "👆 Swipe / Drag Screen for 360° View", hintStyle);

            // Bottom-Right: Brake & Gas Pedals
            float pedalY = h - 100f;
            float brakeX = w - 180f;
            float gasX = w - 95f;

            GUI.backgroundColor = new Color(0.7f, 0.2f, 0.2f);
            if (GUI.RepeatButton(new Rect(brakeX, pedalY + 15, 65, 75), "BRAKE
🛑"))
            {
                _motorcycle?.SetBrakeInput(1f);
            }

            GUI.backgroundColor = new Color(0.2f, 0.75f, 0.3f);
            if (GUI.RepeatButton(new Rect(gasX, pedalY, 70, 90), "GAS
⚡"))
            {
                _motorcycle?.SetThrottleInput(1f);
            }
            GUI.backgroundColor = Color.white;
        }

        // =========================================================================
        // SCREEN 7: MISSIONS (Matches Reference 6)
        // =========================================================================
        private void DrawScreen7_Missions(int w, int h)
        {
            DrawScreenTag("7. Missions");
            DrawHeaderWithBack(w, "MISSIONS & CHALLENGES", () => CurrentScreen = GameScreen.MainMenu);

            float cardW = Mathf.Min(680f, w * 0.75f);
            float cardX = (w - cardW) * 0.5f;
            float startY = 70f;

            GUI.DrawTexture(new Rect(cardX, startY, cardW, h - startY - 20), _darkPanelTex);

            GUILayout.BeginArea(new Rect(cardX + 20, startY + 15, cardW - 40, h - startY - 35));

            // Tabs: Story, Daily, Weekly
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Story Missions", GUILayout.Height(34))) _missionTab = 0;
            if (GUILayout.Button("Daily Tasks", GUILayout.Height(34))) _missionTab = 1;
            if (GUILayout.Button("Weekly Challenges", GUILayout.Height(34))) _missionTab = 2;
            GUILayout.EndHorizontal();

            GUILayout.Space(16);

            // Mission List
            DrawMissionRow("Reach Chennai Express Toll", "Story", "0 / 1", 500, ref _mission1Claimed);
            GUILayout.Space(10);
            DrawMissionRow("Complete 5 Highway Rides", "Daily", "0 / 5", 300, ref _mission2Claimed);
            GUILayout.Space(10);
            DrawMissionRow("Travel 100 KM on NH-44", "Weekly", "0 / 100 km", 1000, ref _mission3Claimed);

            GUILayout.EndArea();
        }

        private void DrawMissionRow(string title, string tag, string progress, int coins, ref bool claimed)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.BeginVertical();
            GUILayout.Label($"<b>{title}</b> ({tag})");
            GUILayout.Label($"Progress: {progress}    Reward: 🪙 {coins}");
            GUILayout.EndVertical();

            if (claimed)
            {
                GUI.enabled = false;
                GUILayout.Button("CLAIMED", GUILayout.Width(90), GUILayout.Height(36));
                GUI.enabled = true;
            }
            else
            {
                GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
                if (GUILayout.Button("GO ▶", GUILayout.Width(90), GUILayout.Height(36)))
                {
                    claimed = true;
                    _silverCoins += coins;
                    CurrentScreen = GameScreen.RidingHUD;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();
        }

        // =========================================================================
        // SCREEN 8: MULTIPLAYER (Matches Reference 7)
        // =========================================================================
        private void DrawScreen8_Multiplayer(int w, int h)
        {
            DrawScreenTag("8. Multiplayer");
            DrawHeaderWithBack(w, "MULTIPLAYER HIGHWAY CONVOY", () => CurrentScreen = GameScreen.MainMenu);

            float cardW = Mathf.Min(700f, w * 0.80f);
            float cardX = (w - cardW) * 0.5f;
            float startY = 70f;

            GUI.DrawTexture(new Rect(cardX, startY, cardW, h - startY - 20), _darkPanelTex);

            GUILayout.BeginArea(new Rect(cardX + 25, startY + 20, cardW - 50, h - startY - 45));

            GUILayout.Label("<b>Online Riders: 2,456</b>  |  Convoy Ping: 24 ms", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Convoy Room Code:", GUILayout.Width(140));
            _roomCodeInput = GUILayout.TextField(_roomCodeInput, GUILayout.Width(160));

            GUI.backgroundColor = new Color(0.12f, 0.72f, 0.28f);
            if (GUILayout.Button("JOIN ROOM ▶", GUILayout.Height(28)))
            {
                _multiplayerStatus = $"Joined Convoy {_roomCodeInput}! 4 Riders Ready.";
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(15);
            GUILayout.Label($"<b>Status:</b> {_multiplayerStatus}");
            GUILayout.Space(15);

            GUILayout.Label("<b>Online Friends Ready to Ride:</b>");
            for (int f = 0; f < _convoyFriends.Length; f++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_convoyFriends[f], GUILayout.Width(220));
                if (GUILayout.Button("INVITE", GUILayout.Width(80)))
                {
                    _multiplayerStatus = $"Invited {_convoyFriends[f]} to your convoy!";
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1.0f, 0.76f, 0.08f);
            GUIStyle goBtn = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 17 };
            goBtn.normal.textColor = Color.black;
            if (GUILayout.Button("START CONVOY RIDE ▶", goBtn, GUILayout.Height(46)))
            {
                CurrentScreen = GameScreen.RidingHUD;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();
        }

        // =========================================================================
        // SCREEN 9: SETTINGS (Matches Reference 8)
        // =========================================================================
        private void DrawScreen9_Settings(int w, int h)
        {
            DrawScreenTag("9. Settings");
            DrawHeaderWithBack(w, "GAME SETTINGS", () => CurrentScreen = GameScreen.MainMenu);

            float cardW = Mathf.Min(650f, w * 0.78f);
            float cardX = (w - cardW) * 0.5f;
            float startY = 70f;

            GUI.DrawTexture(new Rect(cardX, startY, cardW, h - startY - 20), _darkPanelTex);

            GUILayout.BeginArea(new Rect(cardX + 25, startY + 20, cardW - 50, h - startY - 45));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Graphics", GUILayout.Height(32))) _settingsTab = 0;
            if (GUILayout.Button("Audio", GUILayout.Height(32))) _settingsTab = 1;
            if (GUILayout.Button("Controls", GUILayout.Height(32))) _settingsTab = 2;
            if (GUILayout.Button("Account", GUILayout.Height(32))) _settingsTab = 4;
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (_settingsTab == 0) // Graphics
            {
                GUILayout.Label("Graphics Quality:");
                GUILayout.BeginHorizontal();
                string[] q = new[] { "Low", "Medium", "High", "Ultra" };
                for (int i = 0; i < q.Length; i++)
                {
                    if (GUILayout.Toggle(_graphicsQualityIndex == i, q[i], "Button")) _graphicsQualityIndex = i;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(14);
                GUILayout.Label("Frame Rate:");
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(_frameRateIndex == 0, "30 FPS", "Button")) _frameRateIndex = 0;
                if (GUILayout.Toggle(_frameRateIndex == 1, "60 FPS", "Button")) _frameRateIndex = 1;
                GUILayout.EndHorizontal();

                GUILayout.Space(14);
                _bloomEnabled = GUILayout.Toggle(_bloomEnabled, " Bloom & Sun Shafts");
                _shadowsEnabled = GUILayout.Toggle(_shadowsEnabled, " Realtime Shadows");
            }
            else if (_settingsTab == 1) // Audio
            {
                GUILayout.Label($"Master Volume: {Mathf.RoundToInt(_masterVolume * 100)}%");
                _masterVolume = GUILayout.HorizontalSlider(_masterVolume, 0f, 1f);

                GUILayout.Space(12);
                GUILayout.Label($"Motorcycle Engine: {Mathf.RoundToInt(_engineVolume * 100)}%");
                _engineVolume = GUILayout.HorizontalSlider(_engineVolume, 0f, 1f);

                GUILayout.Space(12);
                GUILayout.Label($"Highway Horn: {Mathf.RoundToInt(_hornVolume * 100)}%");
                _hornVolume = GUILayout.HorizontalSlider(_hornVolume, 0f, 1f);
            }
            else if (_settingsTab == 2) // Controls
            {
                GUILayout.Label("Steering Mode: Touch Buttons (◀ ▶) + Tilt Gyro + Keyboard WASD");
                GUILayout.Label("Sensitivity: Adaptive high-speed damping enabled");
                GUILayout.Label("Camera: 360° Touch Orbit + Auto-recenter enabled");
            }
            else // Account
            {
                GUILayout.Label($"<b>Rider Profile:</b> {_playerName}");
                GUILayout.Label($"<b>Wallet:</b> 🪙 {_silverCoins:N0} Silver  |  💎 {_diamonds:N0} Diamonds");
                GUILayout.Space(12);
                if (GUILayout.Button("SWITCH ACCOUNT / RE-LOGIN", GUILayout.Height(36)))
                {
                    _showLoginModal = true;
                    CurrentScreen = GameScreen.WelcomeAndLogin;
                }
            }

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            if (GUILayout.Button("APPLY SETTINGS ▶", GUILayout.Height(40)))
            {
                QualitySettings.SetQualityLevel(_graphicsQualityIndex, true);
                Application.targetFrameRate = _frameRateIndex == 1 ? 60 : 30;
                CurrentScreen = GameScreen.MainMenu;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();
        }

        // =========================================================================
        // SCREEN 10: REWARDS (Matches Reference 9)
        // =========================================================================
        private void DrawScreen10_RewardScreen(int w, int h)
        {
            DrawScreenTag("10. Rewards");

            float cardW = Mathf.Min(600f, w * 0.72f);
            float cardH = Mathf.Min(380f, h * 0.75f);
            float cardX = (w - cardW) * 0.5f;
            float cardY = (h - cardH) * 0.5f;

            GUI.DrawTexture(new Rect(cardX, cardY, cardW, cardH), _darkPanelTex);

            GUILayout.BeginArea(new Rect(cardX + 25, cardY + 25, cardW - 50, cardH - 50));

            GUIStyle winStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 26,
                fontStyle = FontStyle.Bold
            };
            winStyle.normal.textColor = new Color(1.0f, 0.85f, 0.15f);
            GUILayout.Label("🏆 MISSION COMPLETE! 🏆", winStyle);

            GUILayout.Space(20);
            GUILayout.Label("+1,000 Silver Coins Credited!", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16 });
            GUILayout.Label("+500 XP Added to Rider Progression!", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16 });

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1f, 0.76f, 0.08f);
            GUIStyle contStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold, fontSize = 17 };
            contStyle.normal.textColor = Color.black;
            if (GUILayout.Button("CONTINUE ▶", contStyle, GUILayout.Height(46)))
            {
                CurrentScreen = GameScreen.MainMenu;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();
        }

        // =========================================================================
        // SCREEN 11: AVATAR STUDIO
        // =========================================================================
        private void DrawScreen11_AvatarStudio(int w, int h)
        {
            DrawScreenTag("11. Rider Avatar Studio");
            DrawHeaderWithBack(w, "RIDER AVATAR CUSTOMIZER", () => CurrentScreen = GameScreen.Garage);

            float panelW = Mathf.Min(340f, w * 0.35f);
            float panelX = w - panelW - 25f;
            float panelY = 70f;

            GUI.DrawTexture(new Rect(panelX, panelY, panelW, h - panelY - 20), _darkPanelTex);

            GUILayout.BeginArea(new Rect(panelX + 20, panelY + 20, panelW - 40, h - panelY - 40));

            GUILayout.Label("<b>RIDER GENDER:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(!_isFemaleAvatar, "Male", "Button", GUILayout.Height(30)))
            {
                _isFemaleAvatar = false;
                ApplyCustomizationsToBike();
            }
            if (GUILayout.Toggle(_isFemaleAvatar, "Female", "Button", GUILayout.Height(30)))
            {
                _isFemaleAvatar = true;
                ApplyCustomizationsToBike();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(14);
            GUILayout.Label("<b>RIDING JACKET:</b>");
            for (int j = 0; j < _jacketNames.Length; j++)
            {
                if (GUILayout.Toggle(_selectedJacketIdx == j, _jacketNames[j], "Button"))
                {
                    _selectedJacketIdx = j;
                    ApplyCustomizationsToBike();
                }
            }

            GUILayout.Space(14);
            GUILayout.Label("<b>HELMET COLOR:</b>");
            for (int hc = 0; hc < _helmetColorNames.Length; hc++)
            {
                if (GUILayout.Toggle(_selectedHelmetIdx == hc, _helmetColorNames[hc], "Button"))
                {
                    _selectedHelmetIdx = hc;
                    ApplyCustomizationsToBike();
                }
            }

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1.0f, 0.76f, 0.08f);
            if (GUILayout.Button("SAVE AVATAR ▶", GUILayout.Height(40)))
            {
                CurrentScreen = GameScreen.Garage;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();
        }

        // =========================================================================
        // HELPERS & PROCEDURAL DRAWERS
        // =========================================================================
        private void DrawScreenTag(string label)
        {
            GUIStyle tagStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            tagStyle.normal.textColor = new Color(1f, 1f, 1f, 0.50f);
            GUI.Label(new Rect(8, 2, 220, 18), label, tagStyle);
        }

        private void DrawHeaderWithBack(int w, string title, Action onBack)
        {
            float headerH = 46f;
            GUI.DrawTexture(new Rect(0, 0, w, headerH), _blueHeaderTex);

            if (GUI.Button(new Rect(15, 8, 80, 30), "◀ BACK"))
            {
                onBack?.Invoke();
            }

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(100, 8, w - 200, 30), title, titleStyle);
        }

        private void DrawTopWallet(int w)
        {
            float wallW = 200f;
            float wallX = w - wallW - 20f;
            GUI.DrawTexture(new Rect(wallX, 8, wallW, 30), _darkPanelTex);

            GUIStyle coinStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 12 };
            coinStyle.normal.textColor = new Color(1f, 0.85f, 0.2f);
            GUI.Label(new Rect(wallX + 12, 12, wallW - 24, 22), $"🪙 {_silverCoins:N0}   💎 {_diamonds:N0}", coinStyle);
        }

        private void DrawSpecBar(string specName, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(specName, GUILayout.Width(75));

            Rect r = GUILayoutUtility.GetRect(140, 16);
            GUI.DrawTexture(r, _darkPanelTex);
            GUI.DrawTexture(new Rect(r.x, r.y, r.width * Mathf.Clamp01(value), r.height), _barFillTex);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        private void DrawMenuNavButton(float x, float y, float w, float h, string label, Action onClick)
        {
            GUI.DrawTexture(new Rect(x, y, w, h), _darkPanelTex);
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            btnStyle.normal.textColor = Color.white;
            if (GUI.Button(new Rect(x, y, w, h), $"  {label}", btnStyle))
            {
                onClick?.Invoke();
            }
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
                float accel = 24f + (_selectedBikeIndex * 2.5f);
                _motorcycle.ApplyBikeTuning(
                    _bikeModelNames[_selectedBikeIndex],
                    baseSpeed,
                    accel,
                    36f
                );
            }
        }
    }
}
