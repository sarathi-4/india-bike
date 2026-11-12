using System;
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
    /// <summary>
    /// Master Game Flow UI:
    /// Provides the complete interactive interface for:
    /// 1. User Account Login & Profile (Demo Email / Guest Login)
    /// 2. Motorcycle Showroom, Garage, Colors & Upgrades (Classic 350, Bullet, Himalayan, Duke, Pulsar, Splendor)
    /// 3. India Map, 28 States & UTs, Highway/Ghat/Village Road Selection
    /// 4. Solo & Couple Passenger Riding Selection
    /// 5. Missions, Daily Login Rewards & Wallet (Silver/Gold Coins)
    /// 6. Graphics, Audio & Controls Settings
    /// </summary>
    public class MasterGameFlowUI : MonoBehaviour
    {
        private static MasterGameFlowUI _instance;
        public static MasterGameFlowUI Instance => _instance;

        public bool IsMenuOpen = true; // Starts open on play so player can configure ride

        private int _activeTab = 0; // 0=Login, 1=Garage, 2=Map, 3=Passenger, 4=Missions, 5=Settings
        private readonly string[] _tabNames = new[]
        {
            "🔑 Login & Profile",
            "🏍️ Bike Garage",
            "🗺️ India Map",
            "👫 Passenger & Mode",
            "🎯 Missions",
            "⚙️ Settings"
        };

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

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                IsMenuOpen = !IsMenuOpen;
            }
        }

        private void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;

            if (!IsMenuOpen)
            {
                // Top-Left In-Game Shortcut to open Menu/Garage
                if (GUI.Button(new Rect(15, 52, 130, 32), "🏍️ GARAGE & MENU"))
                {
                    IsMenuOpen = true;
                }
                return;
            }

            // Draw Full Menu Overlay
            GUI.Box(new Rect(0, 0, w, h), "");

            // Top Header Banner
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(20, h * 30 / 1000),
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = new Color(1f, 0.75f, 0.15f); // Indian Golden Saffron

            GUI.Label(new Rect(0, 10, w, 35), "🇮🇳 INDIAN BIKE LIFE — 3D MOTORCYCLE SIMULATOR", headerStyle);

            // Subheader: User Status & Wallet
            string riderName = _authManager != null && !string.IsNullOrEmpty(_authManager.CurrentUsername)
                ? _authManager.CurrentUsername
                : "Guest Rider";
            int silver = _economyManager != null ? _economyManager.SilverCoins : 1000;
            int gold = _economyManager != null ? _economyManager.GoldCoins : 10;
            int lvl = _progressionManager != null ? _progressionManager.CurrentLevel : 1;

            GUIStyle subStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(12, h * 17 / 1000)
            };
            subStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(0, 42, w, 24), $"Rider: <b>{riderName}</b>  |  🪙 Silver: ₹{silver:N0}  |  💰 Gold: {gold}  |  ⭐ Level: {lvl}", subStyle);

            // Tab Bar
            float tabBarW = Mathf.Min(820f, w * 0.92f);
            float tabStartX = (w - tabBarW) * 0.5f;
            float tabBtnW = tabBarW / _tabNames.Length;
            float tabH = 36f;

            for (int i = 0; i < _tabNames.Length; i++)
            {
                bool isSelected = (_activeTab == i);
                GUI.backgroundColor = isSelected ? new Color(1f, 0.65f, 0.15f) : new Color(0.25f, 0.25f, 0.28f);
                if (GUI.Button(new Rect(tabStartX + i * tabBtnW, 70, tabBtnW - 4, tabH), _tabNames[i]))
                {
                    _activeTab = i;
                }
            }
            GUI.backgroundColor = Color.white;

            // Main Content Window
            float contentW = tabBarW;
            float contentH = h - 180f;
            float contentY = 112f;
            Rect contentRect = new Rect(tabStartX, contentY, contentW, contentH);
            GUI.Box(contentRect, "");

            GUILayout.BeginArea(new Rect(contentRect.x + 20, contentRect.y + 15, contentRect.width - 40, contentRect.height - 30));

            switch (_activeTab)
            {
                case 0: DrawLoginTab(); break;
                case 1: DrawGarageTab(); break;
                case 2: DrawMapTab(); break;
                case 3: DrawPassengerTab(); break;
                case 4: DrawMissionsTab(); break;
                case 5: DrawSettingsTab(); break;
            }

            GUILayout.EndArea();

            // Bottom Big Button: START RIDE!
            float startBtnW = Mathf.Min(340f, w * 0.45f);
            float startBtnH = 46f;
            float startBtnX = (w - startBtnW) * 0.5f;
            float startBtnY = h - 60f;

            GUI.backgroundColor = new Color(0.15f, 0.78f, 0.25f); // Vibrant Green
            GUIStyle startStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Max(15, h * 22 / 1000),
                fontStyle = FontStyle.Bold
            };
            if (GUI.Button(new Rect(startBtnX, startBtnY, startBtnW, startBtnH), "🏁 START RIDE / DRIVE ▶", startStyle))
            {
                ApplyCustomizationsToBike();
                IsMenuOpen = false;
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawLoginTab()
        {
            GUILayout.Label("<b>Player Authentication & Account System:</b>", GUILayout.Height(24));
            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Email / ID:", GUILayout.Width(110));
            _loginEmail = GUILayout.TextField(_loginEmail, GUILayout.Width(280));
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Password:", GUILayout.Width(110));
            _loginPassword = GUILayout.PasswordField(_loginPassword, '*', GUILayout.Width(280));
            GUILayout.EndHorizontal();

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("🔑 Login with Email", GUILayout.Width(160), GUILayout.Height(32)))
            {
                _authManager.LoginWithEmail(_loginEmail, _loginPassword, (success, msg) =>
                {
                    _authMessage = success ? $"Login Successful! Welcome {_authManager.CurrentUsername}." : msg;
                });
            }

            if (GUILayout.Button("⚡ 1-Click Guest Login", GUILayout.Width(170), GUILayout.Height(32)))
            {
                _authManager.LoginAsGuest((success, msg) =>
                {
                    _authMessage = success ? $"Logged in as {_authManager.CurrentUsername}." : msg;
                });
            }

            if (GUILayout.Button("📝 Use Demo Credentials", GUILayout.Width(180), GUILayout.Height(32)))
            {
                _loginEmail = "demo@indianbike.com";
                _loginPassword = "IndianBike2026";
                _authManager.LoginWithEmail(_loginEmail, _loginPassword, (success, msg) =>
                {
                    _authMessage = success ? "Demo Account Loaded Successfully!" : msg;
                });
            }

            if (_authManager != null && _authManager.State != AuthState.Unauthenticated)
            {
                if (GUILayout.Button("🚪 Logout", GUILayout.Width(90), GUILayout.Height(32)))
                {
                    _authManager.Logout();
                    _authMessage = "Logged out successfully.";
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(14);
            GUIStyle msgStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
            msgStyle.normal.textColor = new Color(0.2f, 0.85f, 0.4f);
            GUILayout.Label($"Status: {_authMessage}", msgStyle);

            GUILayout.Space(10);
            GUILayout.Box("Tip: Demo Account preloaded with 'demo@indianbike.com' and 'IndianBike2026'. Or click '1-Click Guest Login' for instant access.");
        }

        private void DrawGarageTab()
        {
            GUILayout.Label("<b>Select Indian Motorcycle Model:</b>", GUILayout.Height(22));
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

            GUILayout.Space(12);
            GUILayout.Label($"<b>Selected Model:</b> {_bikeNames[_selectedBikeIndex]}");
            GUILayout.Label($"Top Speed: <b>{_bikeTopSpeeds[_selectedBikeIndex]} km/h</b>  |  Acceleration: <b>{_bikeAccels[_selectedBikeIndex]} m/s²</b>  |  Transmission: <b>5-Speed Manual</b>");

            GUILayout.Space(14);
            GUILayout.Label("<b>Motorcycle Paint Color Selection:</b>", GUILayout.Height(22));
            GUILayout.BeginHorizontal();
            for (int c = 0; c < _colorNames.Length; c++)
            {
                bool isColorSel = (_selectedColorIndex == c);
                GUI.backgroundColor = isColorSel ? Color.yellow : _bikeColors[c];
                if (GUILayout.Button(_colorNames[c], GUILayout.Height(32)))
                {
                    _selectedColorIndex = c;
                    ApplyCustomizationsToBike();
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(14);
            GUILayout.Label("<b>Performance Upgrades:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stage 1 Engine Tune (+10 km/h) [₹ 500 Silver]", GUILayout.Height(30)))
            {
                if (_economyManager != null && _economyManager.Debit(CurrencyType.SilverCoins, 500, out _))
                {
                    _bikeTopSpeeds[_selectedBikeIndex] += 10f;
                    ApplyCustomizationsToBike();
                }
            }
            if (GUILayout.Button("Ceramic Disc Brakes [₹ 300 Silver]", GUILayout.Height(30)))
            {
                if (_economyManager != null && _economyManager.Debit(CurrencyType.SilverCoins, 300, out _))
                {
                    ApplyCustomizationsToBike();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawMapTab()
        {
            GUILayout.Label("<b>Select Indian State / Territory:</b>", GUILayout.Height(22));
            GUILayout.BeginVertical();
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
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("<b>Road Corridor Type:</b>");
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

            GUILayout.Space(10);
            GUILayout.Label("<b>Time of Day & Lighting:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("☀️ Daylight (12 PM)")) { _timeController?.SetHour(12f); }
            if (GUILayout.Button("🌅 Golden Sunset (6 PM)")) { _timeController?.SetHour(18f); }
            if (GUILayout.Button("🌙 Night Cruise (11 PM)")) { _timeController?.SetHour(23f); }
            GUILayout.EndHorizontal();
        }

        private void DrawPassengerTab()
        {
            GUILayout.Label("<b>Riding Mode & Pillion Passenger:</b>", GUILayout.Height(22));
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = !_isCoupleRiding ? Color.green : Color.gray;
            if (GUILayout.Button("👤 Solo Rider (தனி சவாரி)", GUILayout.Height(34)))
            {
                _isCoupleRiding = false;
                ApplyCustomizationsToBike();
            }

            GUI.backgroundColor = _isCoupleRiding ? Color.green : Color.gray;
            if (GUILayout.Button("👫 Couple Riding: Male Rider + Female Passenger ('Priya')", GUILayout.Height(34)))
            {
                _isCoupleRiding = true;
                ApplyCustomizationsToBike();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(14);
            GUILayout.Label("<b>Passenger Helmet & Safety:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_passengerHelmet ? "⛑️ Helmet ON" : "❌ Helmet OFF", GUILayout.Width(160), GUILayout.Height(30)))
            {
                _passengerHelmet = !_passengerHelmet;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(12);
            GUILayout.Label("Indian Pillion Passenger physics automatically calculates weight balance and cornering lean dampening.");
        }

        private void DrawMissionsTab()
        {
            GUILayout.Label("<b>Daily Rewards & Active Challenges:</b>", GUILayout.Height(22));
            GUILayout.Space(6);

            if (GUILayout.Button("🎁 Claim Daily Login Reward (+500 Silver Coins)", GUILayout.Height(34)))
            {
                if (_loginRewardManager != null)
                {
                    _loginRewardManager.ClaimDailyReward();
                }
                else if (_economyManager != null)
                {
                    _economyManager.Credit(CurrencyType.SilverCoins, 500, out _);
                }
            }

            GUILayout.Space(12);
            GUILayout.Label("<b>Available Story & Sprint Missions:</b>");
            GUILayout.Box("1. ☕ Chai Express Sprint: Deliver masala chai to Sher-e-Punjab Dhaba within 90s.\n   Reward: ₹ 1,500 Silver + 150 XP");
            GUILayout.Box("2. ⚡ NH-44 Speed Trapper: Clock 120+ km/h on highway corridor for 10 consecutive seconds.\n   Reward: ₹ 2,000 Silver + 10 Gold");
            GUILayout.Box("3. 🛣️ Highway Tourer: Complete 2,000 meters along NH-44 without colliding.\n   Reward: ₹ 3,000 Silver + 250 XP");
        }

        private void DrawSettingsTab()
        {
            GUILayout.Label("<b>Graphics Quality & Performance:</b>", GUILayout.Height(22));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Low (30 FPS - Battery Saver)", GUILayout.Height(32)))
            {
                QualitySettings.SetQualityLevel(0, true);
            }
            if (GUILayout.Button("Medium (60 FPS - Balanced)", GUILayout.Height(32)))
            {
                QualitySettings.SetQualityLevel(1, true);
            }
            if (GUILayout.Button("High (60 FPS - Maximum Fidelity)", GUILayout.Height(32)))
            {
                QualitySettings.SetQualityLevel(2, true);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(14);
            GUILayout.Label("<b>Controls:</b> Keyboard (W, A, S, D, Shift, H) or On-Screen Mobile Touch HUD Buttons.");
            GUILayout.Label("<b>Version:</b> Indian Bike Life v1.0.0 (Unity 6.0 LTS URP)");
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
