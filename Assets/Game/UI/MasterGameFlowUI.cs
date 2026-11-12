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
    public enum GameFlowScreen
    {
        SplashAndLoading = 0,       // Animated Splash & Loading screen (Screenshots 1 & 2)
        TermsWelcome = 1,           // "Welcome to Simulator - Continue" screen (Screenshot 4)
        MainMenuGarage = 2,          // Main 3D Garage Hub (Screenshot 5)
        PlayerProfile = 3,           // Leaderboards & Player Profile Dashboard (Screenshots 3 & 7)
        CharacterCustomization = 4,  // Male/Female & Attire/Helmet Customizer (Screenshot 6)
        BikeGarageShowroom = 5,      // Indian Motorcycle Showroom & Specs
        MapSelection = 6,            // 28 States & UTs Route Selection
        InGameRiding = 7             // Active 3D Motorcycle Simulator
    }

    /// <summary>
    /// Master Game Flow UI implementing the authentic AAA mobile flow:
    /// 1. Splash & Loading Screen with animated Progress Bar (Screenshots 1 & 2)
    /// 2. Welcome & Terms Continue screen (Screenshot 4)
    /// 3. Main Menu 3D Garage Hub with bottom navigation bar & Play button (Screenshot 5)
    /// 4. Character Customization Studio (Male/Female, Helmet, Jacket colors, Sliders) (Screenshot 6)
    /// 5. Player Profile & Leaderboard Dashboard (Stats, Medals, Distance, Demo Login) (Screenshots 3 & 7)
    /// 6. Seamless transition into active 3D riding with in-game menu shortcut.
    /// </summary>
    public class MasterGameFlowUI : MonoBehaviour
    {
        private static MasterGameFlowUI _instance;
        public static MasterGameFlowUI Instance => _instance;

        [Header("Active Screen State")]
        public GameFlowScreen CurrentScreen = GameFlowScreen.SplashAndLoading;
        public bool IsMenuOpen => CurrentScreen != GameFlowScreen.InGameRiding;

        // Loading animation state
        private float _loadingProgress = 0f;
        private bool _isLoadingComplete = false;
        private string _loadingStatusText = "LOADING ASSETS & SHADERS...";

        // Authentication state
        private AuthenticationManager _authManager;
        private string _loginEmail = "demo@indianbike.com";
        private string _loginPassword = "IndianBike2026";
        private string _authMessage = "Ready to ride. Use Demo or 1-Click Guest Login.";

        // Selected Bike & Customization
        private int _selectedBikeIndex = 0;
        private readonly string[] _bikeNames = new[]
        {
            "Royal Enfield Classic 350",
            "Royal Enfield Bullet 350",
            "Royal Enfield Himalayan 411",
            "KTM 390 Duke ABS",
            "Bajaj Pulsar 220F DTS-i",
            "Hero Splendor Plus i3S"
        };
        private readonly float[] _bikeTopSpeeds = new[] { 130f, 125f, 140f, 165f, 138f, 95f };
        private readonly float[] _bikeAccels = new[] { 7.5f, 7.0f, 8.2f, 12.5f, 9.0f, 5.0f };

        private int _selectedColorIndex = 0;
        private readonly string[] _colorNames = new[]
        {
            "Royal Maroon Red",
            "Stealth Matte Black",
            "Brilliant Chrome Silver",
            "Desert Storm Sand",
            "British Racing Green"
        };
        private readonly Color[] _bikeColors = new[]
        {
            new Color(0.72f, 0.08f, 0.12f),
            new Color(0.12f, 0.12f, 0.14f),
            new Color(0.92f, 0.92f, 0.96f),
            new Color(0.85f, 0.72f, 0.45f),
            new Color(0.12f, 0.42f, 0.18f)
        };

        // Character Customization State
        private bool _isFemaleRider = false;
        private int _selectedHelmetIndex = 0;
        private readonly string[] _helmetStyles = new[]
        {
            "Full-Face Aerodynamic Helmet",
            "Classic Royal Cruiser Half-Helmet",
            "Street Biker Bandana & Mask",
            "No Helmet (Open Wind)"
        };

        private int _selectedJacketIndex = 0;
        private readonly string[] _jacketStyles = new[]
        {
            "Navy Technical Riding Armor",
            "Vibrant Magenta Street Jacket",
            "White Racing Tracksuit (Pro)",
            "Traditional Indian Kurta / Saree"
        };

        private readonly Color[] _characterColors = new[]
        {
            new Color(0.92f, 0.15f, 0.45f), // Neon Pink
            new Color(0.95f, 0.75f, 0.15f), // Golden Yellow
            new Color(0.15f, 0.55f, 0.95f), // Electric Blue
            new Color(0.85f, 0.12f, 0.12f), // Crimson Red
            new Color(0.12f, 0.12f, 0.15f), // Matte Black
            new Color(0.95f, 0.95f, 0.98f), // Pure White
            new Color(0.18f, 0.55f, 0.22f)  // Emerald Green
        };
        private int _selectedJacketColorIdx = 1;
        private int _selectedHelmetColorIdx = 1;
        private float _riderMuscleScale = 1.0f;

        // Selected State & Road
        private int _selectedStateIndex = 0;
        private readonly string[] _stateNames = new[]
        {
            "Tamil Nadu (NH-44 Corridor & Ooty Ghats)",
            "Maharashtra (Mumbai-Pune Expressway)",
            "Goa (Coastal Marine Highway)",
            "Himachal Pradesh (Manali-Rohtang Pass)",
            "Ladakh (Khardung La High Desert)",
            "Kerala (Munnar Tea Hills)",
            "Punjab & Haryana (Grand Trunk Road)",
            "Delhi NCR (Ring Road & India Gate)"
        };

        private int _selectedRoadTypeIndex = 0;
        private readonly string[] _roadTypes = new[]
        {
            "National Express Highway (Asphalt 6-Lane)",
            "Western Ghats / Mountain Hairpin Curves",
            "Rural Village Road (Chai Dhaba Trail)",
            "Coastal Beach Highway"
        };

        // Passenger selection
        private bool _isCoupleRiding = false;
        private bool _passengerHelmet = true;

        // Player Stats (matching Screenshots 3 & 7)
        private float _distanceRiddenKm = 8.84f;
        private int _contractsCompleted = 7;
        private int _challengersDefeated = 3;
        private int _multiplayerMatches = 0;
        private float _timePlayedHours = 0.7f;

        // References to scene systems
        private PlayerMotorcycleController _motorcycle;
        private MotorcycleVisualBuilder _visualBuilder;
        private IEconomyManager _economyManager;
        private IProgressionManager _progressionManager;
        private LoginRewardManager _loginRewardManager;
        private TimeOfDayController _timeController;
        private SeasonManager _seasonManager;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _authManager = new AuthenticationManager();
        }

        private void Start()
        {
            LocateSceneSystems();
            ApplyCustomizationsToBike();
            StartCoroutine(SimulateSplashLoadingRoutine());
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
            ServiceLocator.TryGet(out _loginRewardManager);

            _timeController = FindFirstObjectByType<TimeOfDayController>();
            _seasonManager = FindFirstObjectByType<SeasonManager>();
        }

        private IEnumerator SimulateSplashLoadingRoutine()
        {
            _loadingProgress = 0f;
            _loadingStatusText = "INITIALIZING 3D ENGINE...";
            yield return new WaitForSeconds(0.4f);

            _loadingProgress = 0.35f;
            _loadingStatusText = "LOADING NH-44 HIGHWAY & ENVIRONMENT...";
            yield return new WaitForSeconds(0.5f);

            _loadingProgress = 0.75f;
            _loadingStatusText = "TUNING 350cc PROCEDURAL EXHAUST SOUND...";
            yield return new WaitForSeconds(0.4f);

            _loadingProgress = 1.0f;
            _loadingStatusText = "READY TO RIDE!";
            _isLoadingComplete = true;
            yield return new WaitForSeconds(0.3f);

            CurrentScreen = GameFlowScreen.TermsWelcome;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                if (CurrentScreen == GameFlowScreen.InGameRiding)
                {
                    CurrentScreen = GameFlowScreen.MainMenuGarage;
                }
                else if (CurrentScreen != GameFlowScreen.SplashAndLoading && CurrentScreen != GameFlowScreen.TermsWelcome)
                {
                    CurrentScreen = GameFlowScreen.InGameRiding;
                }
            }

            if (CurrentScreen == GameFlowScreen.InGameRiding && _motorcycle != null)
            {
                _distanceRiddenKm += (_motorcycle.CurrentSpeedKmh / 3600f) * Time.deltaTime;
                _timePlayedHours += (Time.deltaTime / 3600f);
            }
        }

        private void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;

            switch (CurrentScreen)
            {
                case GameFlowScreen.SplashAndLoading:
                    DrawSplashScreen(w, h);
                    break;

                case GameFlowScreen.TermsWelcome:
                    DrawTermsWelcomeScreen(w, h);
                    break;

                case GameFlowScreen.MainMenuGarage:
                    DrawMainMenuGarageScreen(w, h);
                    break;

                case GameFlowScreen.PlayerProfile:
                    DrawPlayerProfileScreen(w, h);
                    break;

                case GameFlowScreen.CharacterCustomization:
                    DrawCharacterCustomizationScreen(w, h);
                    break;

                case GameFlowScreen.BikeGarageShowroom:
                    DrawBikeGarageShowroomScreen(w, h);
                    break;

                case GameFlowScreen.MapSelection:
                    DrawMapSelectionScreen(w, h);
                    break;

                case GameFlowScreen.InGameRiding:
                    DrawInGameTopBar(w, h);
                    break;
            }
        }

        // =========================================================================
        // SCREEN 1: SPLASH & LOADING (Matches Screenshots 1 & 2)
        // =========================================================================
        private void DrawSplashScreen(int w, int h)
        {
            GUI.Box(new Rect(0, 0, w, h), "");

            // Top Game Logo
            GUIStyle logoStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(32, h * 55 / 1000),
                fontStyle = FontStyle.Bold
            };
            logoStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(0, h * 0.22f, w, 60), "INDIAN 3D BIKE DRIVING", logoStyle);

            GUIStyle subStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(16, h * 24 / 1000),
                fontStyle = FontStyle.Bold
            };
            subStyle.normal.textColor = new Color(1f, 0.85f, 0.15f);
            GUI.Label(new Rect(0, h * 0.22f + 55, w, 35), "ROYAL ENFIELD & INDIAN HIGHWAY SIMULATOR", subStyle);

            // Center Percentage Indicator (Screenshot 2: 75%)
            int percentInt = Mathf.RoundToInt(_loadingProgress * 100f);
            GUIStyle percentStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(36, h * 50 / 1000),
                fontStyle = FontStyle.Bold
            };
            percentStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(0, h * 0.68f, w, 45), $"{percentInt}%", percentStyle);

            // Bottom Animated Progress Bar (Screenshot 1 & 2)
            float barW = Mathf.Min(560f, w * 0.75f);
            float barH = 14f;
            float barX = (w - barW) * 0.5f;
            float barY = h * 0.77f;

            // Background Track
            GUI.backgroundColor = new Color(0.18f, 0.18f, 0.22f);
            GUI.Box(new Rect(barX, barY, barW, barH), "");

            // Animated Fill Bar (Vibrant Orange / Amber)
            GUI.backgroundColor = new Color(1f, 0.55f, 0.05f);
            GUI.Box(new Rect(barX, barY, barW * _loadingProgress, barH), "");
            GUI.backgroundColor = Color.white;

            // Status Text
            GUIStyle statusStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(11, h * 15 / 1000)
            };
            statusStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
            GUI.Label(new Rect(0, barY + 20, w, 25), _loadingStatusText, statusStyle);
        }

        // =========================================================================
        // SCREEN 2: WELCOME & TERMS CONTINUE (Matches Screenshot 4)
        // =========================================================================
        private void DrawTermsWelcomeScreen(int w, int h)
        {
            GUI.Box(new Rect(0, 0, w, h), "");

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(26, h * 42 / 1000),
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(0, h * 0.25f, w, 40), "WELCOME TO", titleStyle);

            GUIStyle mainTitle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(32, h * 55 / 1000),
                fontStyle = FontStyle.BoldAndItalic
            };
            mainTitle.normal.textColor = new Color(1f, 0.82f, 0.2f); // Golden Saffron
            GUI.Label(new Rect(0, h * 0.32f, w, 50), "INDIAN BIKE LIFE 3D", mainTitle);

            // Green Continue Button (Screenshot 4)
            float btnW = Mathf.Min(360f, w * 0.5f);
            float btnH = 50f;
            float btnX = (w - btnW) * 0.5f;
            float btnY = h * 0.54f;

            GUI.backgroundColor = new Color(0.2f, 0.85f, 0.35f); // Vibrant Light Green
            GUIStyle continueStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Max(17, h * 24 / 1000),
                fontStyle = FontStyle.Bold
            };
            continueStyle.normal.textColor = Color.black;

            if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "CONTINUE ▶", continueStyle))
            {
                CurrentScreen = GameFlowScreen.MainMenuGarage;
            }
            GUI.backgroundColor = Color.white;

            // Terms note
            GUIStyle termStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(11, h * 15 / 1000)
            };
            termStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            GUI.Label(new Rect(0, btnY + 65, w, 40), "By continuing, you accept the Indian Bike Life terms.\nPrivacy Policy | Terms and Conditions", termStyle);
        }

        // =========================================================================
        // SCREEN 3: 3D GARAGE & MAIN MENU (Matches Screenshot 5)
        // =========================================================================
        private void DrawMainMenuGarageScreen(int w, int h)
        {
            // 1. Top Bar (Avatar, Name, Coins, Settings, Profile)
            float topH = 55f;
            GUI.Box(new Rect(0, 0, w, topH), "");

            string riderName = _authManager != null && !string.IsNullOrEmpty(_authManager.CurrentUsername)
                ? _authManager.CurrentUsername
                : "IndianRider";
            int silver = _economyManager != null ? _economyManager.SilverCoins : 1000;
            int gold = _economyManager != null ? _economyManager.GoldCoins : 10;
            int level = _progressionManager != null ? _progressionManager.CurrentLevel : 1;

            // Avatar & Name on Left
            GUIStyle nameStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Max(14, h * 20 / 1000),
                fontStyle = FontStyle.Bold
            };
            nameStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(20, 8, 260, 24), $"👤 {riderName} (Lvl {level})", nameStyle);

            GUIStyle levelSub = new GUIStyle(GUI.skin.label) { fontSize = 11 };
            levelSub.normal.textColor = new Color(0.3f, 0.85f, 0.4f);
            GUI.Label(new Rect(20, 29, 260, 20), "XP: 350 / 500  [Veteran Cruiser]", levelSub);

            // Center: Wallet Badges (Screenshot 5: Money 5000 / Tokens 1)
            float walletX = w * 0.42f;
            GUIStyle walletStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Max(13, h * 18 / 1000),
                fontStyle = FontStyle.Bold
            };
            walletStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(walletX, 15, 300, 28), $"🪙 Silver: ₹{silver:N0}   💰 Gold: {gold}", walletStyle);

            // Top Right: Profile and Settings buttons
            if (GUI.Button(new Rect(w - 220, 12, 105, 32), "👤 Profile"))
            {
                CurrentScreen = GameFlowScreen.PlayerProfile;
            }
            if (GUI.Button(new Rect(w - 105, 12, 85, 32), "⚙️ Options"))
            {
                CurrentScreen = GameFlowScreen.BikeGarageShowroom;
            }

            // 2. Right Side Floating Cards (Screenshot 5: Spin, Daily Rewards, Social)
            float cardW = Mathf.Min(260f, w * 0.28f);
            float cardX = w - cardW - 20f;
            float cardY = 80f;

            // Route Card
            GUI.Box(new Rect(cardX, cardY, cardW, 110), "");
            GUI.Label(new Rect(cardX + 10, cardY + 10, cardW - 20, 25), "<b>📍 SELECTED CORRIDOR</b>");
            GUI.Label(new Rect(cardX + 10, cardY + 32, cardW - 20, 45), $"{_stateNames[_selectedStateIndex]}\n{_roadTypes[_selectedRoadTypeIndex]}");
            if (GUI.Button(new Rect(cardX + 10, cardY + 75, cardW - 20, 26), "🗺️ Change State / Map"))
            {
                CurrentScreen = GameFlowScreen.MapSelection;
            }

            // Daily Rewards Card (Screenshot 5)
            cardY += 125f;
            GUI.Box(new Rect(cardX, cardY, cardW, 85), "");
            GUI.Label(new Rect(cardX + 10, cardY + 8, cardW - 20, 22), "<b>🎁 DAILY REWARDS</b>");
            GUI.Label(new Rect(cardX + 10, cardY + 28, cardW - 20, 20), "Streak: Day 1 (Claimed)");
            if (GUI.Button(new Rect(cardX + 10, cardY + 50, cardW - 20, 26), "+500 Silver Bonus"))
            {
                _economyManager?.AddCurrency(CurrencyType.SilverCoins, 500, "Daily Bonus");
            }

            // 3. Bottom Navigation Bar (Screenshot 5: MOTORPASS | MISSIONS | SHOP | GARAGE | [PLAY])
            float barH = 65f;
            float barY = h - barH;
            GUI.Box(new Rect(0, barY, w, barH), "");

            float navBtnW = Mathf.Min(140f, w * 0.16f);
            float navBtnH = 46f;
            float navY = barY + 10f;

            if (GUI.Button(new Rect(20, navY, navBtnW, navBtnH), "🎯 MISSIONS"))
            {
                CurrentScreen = GameFlowScreen.PlayerProfile;
            }
            if (GUI.Button(new Rect(25 + navBtnW, navY, navBtnW, navBtnH), "🏍️ GARAGE"))
            {
                CurrentScreen = GameFlowScreen.BikeGarageShowroom;
            }
            if (GUI.Button(new Rect(30 + navBtnW * 2, navY, navBtnW, navBtnH), "👤 CHARACTER"))
            {
                CurrentScreen = GameFlowScreen.CharacterCustomization;
            }
            if (GUI.Button(new Rect(35 + navBtnW * 3, navY, navBtnW, navBtnH), "🗺️ MAP"))
            {
                CurrentScreen = GameFlowScreen.MapSelection;
            }

            // BIG GOLDEN PLAY BUTTON (Screenshot 5: Right corner)
            float playBtnW = Mathf.Min(220f, w * 0.25f);
            float playBtnX = w - playBtnW - 20f;

            GUI.backgroundColor = new Color(1f, 0.75f, 0.15f); // Vibrant Golden Yellow
            GUIStyle playStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Max(18, h * 26 / 1000),
                fontStyle = FontStyle.Bold
            };
            playStyle.normal.textColor = Color.black;

            if (GUI.Button(new Rect(playBtnX, navY - 4, playBtnW, navBtnH + 8), "PLAY ▶", playStyle))
            {
                ApplyCustomizationsToBike();
                CurrentScreen = GameFlowScreen.InGameRiding;
            }
            GUI.backgroundColor = Color.white;
        }

        // =========================================================================
        // SCREEN 4: PLAYER PROFILE & LEADERBOARDS (Matches Screenshots 3 & 7)
        // =========================================================================
        private void DrawPlayerProfileScreen(int w, int h)
        {
            GUI.Box(new Rect(0, 0, w, h), "");

            // Top Header: "PLAYER PROFILE"
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = Mathf.Max(18, h * 26 / 1000),
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = new Color(0.2f, 0.85f, 0.5f);
            GUI.Label(new Rect(25, 12, 300, 30), "■ LEADERBOARDS / PLAYER PROFILE", headerStyle);

            string riderName = _authManager != null && !string.IsNullOrEmpty(_authManager.CurrentUsername)
                ? _authManager.CurrentUsername
                : "IndianRider";
            int silver = _economyManager != null ? _economyManager.SilverCoins : 7180;
            int gold = _economyManager != null ? _economyManager.GoldCoins : 15;

            // Stats Top Bar (Screenshot 3 & 7: Level 29, Name, Coins 7180, Time 0.7 Hours)
            GUIStyle topStatStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight, fontSize = 13 };
            topStatStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(w - 450, 12, 420, 30), $"⭐ Lvl 29  <b>{riderName}</b>  |  🪙 ₹{silver:N0}  💰 {gold}  |  ⏱️ Time: {_timePlayedHours:F1} Hrs", topStatStyle);

            // Left Panel: 6 Featured Stat Cards (Screenshot 3)
            float panelW = Mathf.Min(580f, w * 0.62f);
            float startY = 55f;

            GUI.Box(new Rect(25, startY, panelW, h - startY - 70), "FEATURED STATS & PROGRESSION");

            float cardGridY = startY + 30f;
            float gridItemW = (panelW - 30) * 0.5f;
            float gridItemH = 68f;

            // 1. League Progress
            DrawStatCard(35, cardGridY, gridItemW, gridItemH, "LEAGUE PROGRESS", "Indian Rookies (Grand Finale)");
            // 2. Contracts Completed
            DrawStatCard(45 + gridItemW, cardGridY, gridItemW, gridItemH, "CONTRACTS COMPLETED", $"{_contractsCompleted}");

            cardGridY += gridItemH + 8;
            // 3. Leaderboard
            DrawStatCard(35, cardGridY, gridItemW, gridItemH, "HALL OF LEADERBOARDS", "TOP 25% (Rank 132)");
            // 4. Distance Ridden
            DrawStatCard(45 + gridItemW, cardGridY, gridItemW, gridItemH, "DISTANCE RIDDEN", $"{_distanceRiddenKm:F2} km");

            cardGridY += gridItemH + 8;
            // 5. Challengers Defeated
            DrawStatCard(35, cardGridY, gridItemW, gridItemH, "CHALLENGERS DEFEATED", $"{_challengersDefeated}");
            // 6. Multiplayer Matches
            DrawStatCard(45 + gridItemW, cardGridY, gridItemW, gridItemH, "MULTIPLAYER MATCHES", $"{_multiplayerMatches} Rounds");

            cardGridY += gridItemH + 16;
            // Medals Row (Screenshot 3 & 7: Bronze x7, Silver x6, Gold x3)
            GUI.Label(new Rect(35, cardGridY, panelW - 40, 22), "<b>MEDALS & TROPHIES:</b>");
            GUI.Label(new Rect(35, cardGridY + 22, panelW - 40, 24), "🥉 Bronze x 7     🥈 Silver x 6     🥇 Gold x 3     👑 Platinum x 1");

            // Right Panel: Account & Demo Login
            float rightX = 35 + panelW;
            float rightW = w - rightX - 25;
            GUI.Box(new Rect(rightX, startY, rightW, h - startY - 70), "ACCOUNT & DEMO LOGIN");

            GUILayout.BeginArea(new Rect(rightX + 15, startY + 30, rightW - 30, h - startY - 120));
            GUILayout.Label("<b>Login with Demo Account:</b>");
            _loginEmail = GUILayout.TextField(_loginEmail);
            _loginPassword = GUILayout.PasswordField(_loginPassword, '*');

            GUILayout.Space(8);
            if (GUILayout.Button("🔑 Login / Load Account", GUILayout.Height(30)))
            {
                _authManager.LoginWithEmail(_loginEmail, _loginPassword, (ok, msg) => _authMessage = msg);
            }
            if (GUILayout.Button("⚡ 1-Click Guest Login", GUILayout.Height(30)))
            {
                _authManager.LoginAsGuest((ok, msg) => _authMessage = msg);
            }
            if (GUILayout.Button("🚪 Logout", GUILayout.Height(28)))
            {
                _authManager.Logout();
                _authMessage = "Logged out.";
            }
            GUILayout.Space(8);
            GUILayout.Label($"Status: {_authMessage}");
            GUILayout.EndArea();

            // Bottom Back Button
            if (GUI.Button(new Rect(25, h - 55, 180, 40), "◀ BACK TO MENU"))
            {
                CurrentScreen = GameFlowScreen.MainMenuGarage;
            }
        }

        private void DrawStatCard(float x, float y, float w, float h, string title, string val)
        {
            GUI.Box(new Rect(x, y, w, h), "");
            GUIStyle tStyle = new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Bold };
            tStyle.normal.textColor = new Color(0.7f, 0.7f, 0.75f);
            GUI.Label(new Rect(x + 10, y + 6, w - 20, 16), title, tStyle);

            GUIStyle vStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
            vStyle.normal.textColor = new Color(1f, 0.65f, 0.15f); // Golden orange
            GUI.Label(new Rect(x + 10, y + 26, w - 20, 30), val, vStyle);
        }

        // =========================================================================
        // SCREEN 5: CHARACTER CUSTOMIZATION (Matches Screenshot 6)
        // =========================================================================
        private void DrawCharacterCustomizationScreen(int w, int h)
        {
            GUI.Box(new Rect(0, 0, w, h), "");

            // Top Header
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(18, h * 28 / 1000),
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = new Color(1f, 0.75f, 0.15f);
            GUI.Label(new Rect(0, 10, w, 32), "CHARACTER CUSTOMIZATION STUDIO", headerStyle);

            // Gender Toggle: [MALE] [FEMALE] (Screenshot 6: Top Left)
            float genderW = 110f;
            GUI.backgroundColor = !_isFemaleRider ? Color.cyan : Color.gray;
            if (GUI.Button(new Rect(30, 48, genderW, 32), "MALE"))
            {
                _isFemaleRider = false;
                ApplyCustomizationsToBike();
            }
            GUI.backgroundColor = _isFemaleRider ? Color.cyan : Color.gray;
            if (GUI.Button(new Rect(35 + genderW, 48, genderW, 32), "FEMALE"))
            {
                _isFemaleRider = true;
                ApplyCustomizationsToBike();
            }
            GUI.backgroundColor = Color.white;

            // Left Panel: Body Sliders (Screenshot 6: Muscle, Scale, Leaning)
            float sliderPanelW = Mathf.Min(340f, w * 0.35f);
            GUI.Box(new Rect(30, 90, sliderPanelW, h - 160), "BODY SCALING & PHYSIQUE");

            GUILayout.BeginArea(new Rect(45, 115, sliderPanelW - 30, h - 200));
            GUILayout.Label($"Rider Muscle Build: <b>{_riderMuscleScale:F2}x</b>");
            float newScale = GUILayout.HorizontalSlider(_riderMuscleScale, 0.75f, 1.35f);
            if (Mathf.Abs(newScale - _riderMuscleScale) > 0.01f)
            {
                _riderMuscleScale = newScale;
                _visualBuilder?.SetRiderBuildScale(_riderMuscleScale);
            }

            GUILayout.Space(14);
            GUILayout.Label("<b>Headgear & Helmet Selection:</b>");
            for (int i = 0; i < _helmetStyles.Length; i++)
            {
                bool isSel = (_selectedHelmetIndex == i);
                GUI.backgroundColor = isSel ? new Color(1f, 0.75f, 0.15f) : Color.gray;
                if (GUILayout.Button(_helmetStyles[i], GUILayout.Height(30)))
                {
                    _selectedHelmetIndex = i;
                }
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(14);
            GUILayout.Label("<b>Riding Jacket & Attire:</b>");
            for (int j = 0; j < _jacketStyles.Length; j++)
            {
                bool isJacketSel = (_selectedJacketIndex == j);
                GUI.backgroundColor = isJacketSel ? new Color(0.2f, 0.85f, 0.4f) : Color.gray;
                if (GUILayout.Button(_jacketStyles[j], GUILayout.Height(30)))
                {
                    _selectedJacketIndex = j;
                    if (j == 1) _visualBuilder?.SetRiderJacketColor(new Color(0.92f, 0.15f, 0.45f)); // Magenta
                    else if (j == 2) _visualBuilder?.SetRiderJacketColor(new Color(0.95f, 0.95f, 0.98f)); // White
                    else _visualBuilder?.SetRiderJacketColor(new Color(0.18f, 0.22f, 0.28f)); // Navy
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndArea();

            // Right Panel: Color Palette Grid (Screenshot 6: Right side color swatches)
            float rightW = Mathf.Min(340f, w * 0.35f);
            float rightX = w - rightW - 30;
            GUI.Box(new Rect(rightX, 90, rightW, h - 160), "COLOR PALETTE & GEAR PREVIEW");

            GUILayout.BeginArea(new Rect(rightX + 15, 115, rightW - 30, h - 200));
            GUILayout.Label("<b>Jacket Color Palette:</b>");
            GUILayout.BeginHorizontal();
            for (int c = 0; c < _characterColors.Length; c++)
            {
                GUI.backgroundColor = _characterColors[c];
                if (GUILayout.Button("", GUILayout.Width(34), GUILayout.Height(34)))
                {
                    _selectedJacketColorIdx = c;
                    _visualBuilder?.SetRiderJacketColor(_characterColors[c]);
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(16);
            GUILayout.Label("<b>Helmet Color Palette:</b>");
            GUILayout.BeginHorizontal();
            for (int c = 0; c < _characterColors.Length; c++)
            {
                GUI.backgroundColor = _characterColors[c];
                if (GUILayout.Button("", GUILayout.Width(34), GUILayout.Height(34)))
                {
                    _selectedHelmetColorIdx = c;
                    _visualBuilder?.SetRiderHelmetColor(_characterColors[c]);
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(25);
            GUILayout.Box("Live 3D Customizer: All color and build modifications are applied instantly to the 3D Rider character model on the motorcycle.");
            GUILayout.EndArea();

            // Bottom Buttons
            if (GUI.Button(new Rect(30, h - 55, 180, 42), "◀ BACK TO MENU"))
            {
                CurrentScreen = GameFlowScreen.MainMenuGarage;
            }

            GUI.backgroundColor = new Color(0.2f, 0.85f, 0.4f);
            if (GUI.Button(new Rect(w - 220, h - 55, 190, 42), "✓ CONFIRM & SAVE"))
            {
                ApplyCustomizationsToBike();
                CurrentScreen = GameFlowScreen.MainMenuGarage;
            }
            GUI.backgroundColor = Color.white;
        }

        // =========================================================================
        // SCREEN 6: BIKE GARAGE & SHOWROOM
        // =========================================================================
        private void DrawBikeGarageShowroomScreen(int w, int h)
        {
            GUI.Box(new Rect(0, 0, w, h), "");

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(18, h * 28 / 1000),
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = new Color(1f, 0.75f, 0.15f);
            GUI.Label(new Rect(0, 10, w, 32), "INDIAN MOTORCYCLE GARAGE & TUNING", headerStyle);

            float panelW = Mathf.Min(780f, w * 0.85f);
            float startX = (w - panelW) * 0.5f;

            GUI.Box(new Rect(startX, 55, panelW, h - 120), "");
            GUILayout.BeginArea(new Rect(startX + 20, 70, panelW - 40, h - 150));

            GUILayout.Label("<b>Select Indian Motorcycle:</b>");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _bikeNames.Length; i++)
            {
                bool isSel = (_selectedBikeIndex == i);
                GUI.backgroundColor = isSel ? new Color(1f, 0.65f, 0.15f) : Color.gray;
                if (GUILayout.Button(_bikeNames[i], GUILayout.Height(36)))
                {
                    _selectedBikeIndex = i;
                    ApplyCustomizationsToBike();
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(14);
            GUILayout.Label($"<b>Selected:</b> {_bikeNames[_selectedBikeIndex]}");
            GUILayout.Label($"Top Speed: <b>{_bikeTopSpeeds[_selectedBikeIndex]} km/h</b>  |  Acceleration: <b>{_bikeAccels[_selectedBikeIndex]} m/s²</b>  |  5-Speed Manual");

            GUILayout.Space(14);
            GUILayout.Label("<b>Paint Color Customization:</b>");
            GUILayout.BeginHorizontal();
            for (int c = 0; c < _colorNames.Length; c++)
            {
                bool isColorSel = (_selectedColorIndex == c);
                GUI.backgroundColor = isColorSel ? Color.yellow : _bikeColors[c];
                if (GUILayout.Button(_colorNames[c], GUILayout.Height(34)))
                {
                    _selectedColorIndex = c;
                    ApplyCustomizationsToBike();
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(16);
            GUILayout.Label("<b>Performance Tuning & Upgrades:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stage 1 Engine Tune (+10 km/h) [₹ 500 Silver]", GUILayout.Height(32)))
            {
                if (_economyManager != null && _economyManager.SpendCurrency(CurrencyType.SilverCoins, 500, "Stage 1 Engine Tune"))
                {
                    _bikeTopSpeeds[_selectedBikeIndex] += 10f;
                    ApplyCustomizationsToBike();
                }
            }
            if (GUILayout.Button("Ceramic Disc Brakes [₹ 300 Silver]", GUILayout.Height(32)))
            {
                if (_economyManager != null && _economyManager.SpendCurrency(CurrencyType.SilverCoins, 300, "Ceramic Disc Brakes"))
                {
                    ApplyCustomizationsToBike();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (GUI.Button(new Rect(startX, h - 55, 180, 42), "◀ BACK TO MENU"))
            {
                CurrentScreen = GameFlowScreen.MainMenuGarage;
            }
        }

        // =========================================================================
        // SCREEN 7: MAP & ROUTE SELECTION
        // =========================================================================
        private void DrawMapSelectionScreen(int w, int h)
        {
            GUI.Box(new Rect(0, 0, w, h), "");

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(18, h * 28 / 1000),
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = new Color(1f, 0.75f, 0.15f);
            GUI.Label(new Rect(0, 10, w, 32), "INDIA MAP & CORRIDOR SELECTION (28 STATES & 8 UTs)", headerStyle);

            float panelW = Mathf.Min(780f, w * 0.85f);
            float startX = (w - panelW) * 0.5f;

            GUI.Box(new Rect(startX, 55, panelW, h - 120), "");
            GUILayout.BeginArea(new Rect(startX + 20, 70, panelW - 40, h - 150));

            GUILayout.Label("<b>Select Indian State / Corridor:</b>");
            for (int s = 0; s < _stateNames.Length; s++)
            {
                bool isSel = (_selectedStateIndex == s);
                GUI.backgroundColor = isSel ? new Color(1f, 0.75f, 0.2f) : Color.gray;
                if (GUILayout.Button(_stateNames[s], GUILayout.Height(28)))
                {
                    _selectedStateIndex = s;
                }
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(12);
            GUILayout.Label("<b>Road Terrain Type:</b>");
            GUILayout.BeginHorizontal();
            for (int r = 0; r < _roadTypes.Length; r++)
            {
                bool isRoadSel = (_selectedRoadTypeIndex == r);
                GUI.backgroundColor = isRoadSel ? Color.cyan : Color.gray;
                if (GUILayout.Button(_roadTypes[r], GUILayout.Height(30)))
                {
                    _selectedRoadTypeIndex = r;
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(12);
            GUILayout.Label("<b>Time & Weather:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("☀️ Daylight (12 PM)")) { _timeController?.SetHour(12f); }
            if (GUILayout.Button("🌅 Sunset (6 PM)")) { _timeController?.SetHour(18f); }
            if (GUILayout.Button("🌙 Night (11 PM)")) { _timeController?.SetHour(23f); }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (GUI.Button(new Rect(startX, h - 55, 180, 42), "◀ BACK TO MENU"))
            {
                CurrentScreen = GameFlowScreen.MainMenuGarage;
            }
        }

        // =========================================================================
        // IN-GAME TOP BAR SHORTCUT
        // =========================================================================
        private void DrawInGameTopBar(int w, int h)
        {
            if (GUI.Button(new Rect(15, 52, 130, 32), "🏍️ GARAGE & MENU"))
            {
                CurrentScreen = GameFlowScreen.MainMenuGarage;
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
                _visualBuilder.SetBodyPaintColor(_bikeColors[_selectedColorIndex]);
                _visualBuilder.SetPassengerVisible(_isCoupleRiding);
                _visualBuilder.SetRiderBuildScale(_riderMuscleScale);
                _visualBuilder.SetRiderJacketColor(_characterColors[_selectedJacketColorIdx]);
                _visualBuilder.SetRiderHelmetColor(_characterColors[_selectedHelmetColorIdx]);
            }

            if (_motorcycle != null)
            {
                _motorcycle.ApplyBikeTuning(
                    _bikeNames[_selectedBikeIndex],
                    _bikeTopSpeeds[_selectedBikeIndex],
                    _bikeAccels[_selectedBikeIndex],
                    28f
                );
            }
        }
    }
}
