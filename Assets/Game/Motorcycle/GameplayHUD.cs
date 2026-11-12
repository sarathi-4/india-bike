using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Master in-game Mobile HUD & Touch Controls for Indian Bike Life.
    /// Delivers real-time Speedometer, RPM Bar, Gear Indicator, Nitro Meter,
    /// Landmark Location Header, and responsive on-screen touch/mouse driving pedals.
    /// </summary>
    public class GameplayHUD : MonoBehaviour
    {
        [Header("Motorcycle Target")]
        [SerializeField] private PlayerMotorcycleController _motorcycle;

        private IEconomyManager _economyManager;
        private IProgressionManager _progressionManager;
        private MotorcycleFollowCamera _followCamera;
        private MotorcycleVisualBuilder _visualBuilder;

        private bool _showTouchPedals = true;
        private float _touchThrottle = 0f;
        private float _touchSteer = 0f;
        private float _touchBrake = 0f;
        private bool _touchNitro = false;

        private void Start()
        {
            if (_motorcycle == null)
            {
                var bikeObj = GameObject.FindWithTag("Motorcycle");
                if (bikeObj != null) _motorcycle = bikeObj.GetComponent<PlayerMotorcycleController>();
            }

            if (Camera.main != null)
            {
                _followCamera = Camera.main.GetComponent<MotorcycleFollowCamera>();
            }

            if (_motorcycle != null)
            {
                _visualBuilder = _motorcycle.GetComponent<MotorcycleVisualBuilder>();
            }

            ServiceLocator.TryGet(out _economyManager);
            ServiceLocator.TryGet(out _progressionManager);
        }

        private void Update()
        {
            if (_motorcycle == null) return;

            // Pass touch inputs into motorcycle controller
            _motorcycle.SetThrottleInput(_touchThrottle);
            _motorcycle.SetSteerInput(_touchSteer);
            _motorcycle.SetBrakeInput(_touchBrake);
            _motorcycle.SetNitroInput(_touchNitro);

            // Reset touch state each frame (RepeatButton will hold it active)
            _touchThrottle = 0f;
            _touchSteer = 0f;
            _touchBrake = 0f;
            _touchNitro = false;
        }

        private void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;

            DrawTopHeader(w, h);

            if (_motorcycle != null)
            {
                DrawSpeedometerAndGears(w, h);
                DrawNitroGauge(w, h);
            }

            if (_showTouchPedals)
            {
                DrawMobileTouchControls(w, h);
            }
        }

        private void DrawTopHeader(int w, int h)
        {
            // Top Bar Background
            GUI.Box(new Rect(10, 8, w - 20, 38), "");

            // Location Banner (Center)
            GUIStyle locStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(13, h * 18 / 1000),
                fontStyle = FontStyle.Bold
            };
            locStyle.normal.textColor = new Color(1f, 0.85f, 0.25f); // Indian Golden Saffron
            GUI.Label(new Rect(10, 8, w - 20, 38), "🏍️ INDIAN BIKE LIFE  |  📍 NH-44 Express Highway Corridor", locStyle);

            // Left Side: Wallet (Silver & Gold Coins)
            int silver = _economyManager != null ? _economyManager.Wallet.SilverCoins : 1000;
            int gold = _economyManager != null ? _economyManager.Wallet.GoldCoins : 10;
            int level = _progressionManager != null ? _progressionManager.CurrentLevel : 1;

            GUIStyle statStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = Mathf.Max(11, h * 15 / 1000),
                fontStyle = FontStyle.Bold
            };
            statStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(25, 12, 350, 30), $"🪙 Silver: {silver:N0}   💰 Gold: {gold}   ⭐ Lvl: {level}", statStyle);

            // Right Side: Camera View & Passenger Toggle Buttons
            float btnW = 95f;
            float btnH = 28f;
            if (GUI.Button(new Rect(w - btnW * 2 - 35, 13, btnW, btnH), "🎥 View (V)"))
            {
                if (_followCamera != null) _followCamera.ToggleViewMode();
            }

            if (GUI.Button(new Rect(w - btnW - 25, 13, btnW, btnH), "👥 Passenger"))
            {
                if (_visualBuilder != null && _visualBuilder.PassengerVisual != null)
                {
                    bool active = _visualBuilder.PassengerVisual.activeSelf;
                    _visualBuilder.SetPassengerVisible(!active);
                }
            }
        }

        private void DrawSpeedometerAndGears(int w, int h)
        {
            // Bottom-Right Dashboard Container
            float dashW = 220f;
            float dashH = 120f;
            float dashX = w - dashW - 20f;
            float dashY = h - dashH - 25f;

            GUI.Box(new Rect(dashX, dashY, dashW, dashH), "<b>MOTORCYCLE DASHBOARD</b>");

            float speed = Mathf.Max(0f, _motorcycle.CurrentSpeedKmh);

            // Speed Display
            GUIStyle speedStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 28,
                fontStyle = FontStyle.Bold
            };
            speedStyle.normal.textColor = speed > 130f ? new Color(1f, 0.3f, 0.2f) : (speed > 80f ? Color.yellow : Color.white);

            GUI.Label(new Rect(dashX + 10, dashY + 22, dashW - 20, 36), $"{speed:0} <size=16>KM/H</size>", speedStyle);

            // Gear & RPM
            GUIStyle gearStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            gearStyle.normal.textColor = new Color(0.35f, 0.95f, 0.45f);
            GUI.Label(new Rect(dashX + 10, dashY + 56, dashW - 20, 22), $"GEAR: {_motorcycle.CurrentGear}  |  RPM: {_motorcycle.CurrentRpm:0}", gearStyle);

            // RPM Progress Bar
            float rpmRatio = Mathf.Clamp01((_motorcycle.CurrentRpm - 1000f) / 5500f);
            Rect rpmBarRect = new Rect(dashX + 20, dashY + 84, dashW - 40, 14);
            GUI.Box(rpmBarRect, "");
            Rect rpmFillRect = new Rect(rpmBarRect.x + 2, rpmBarRect.y + 2, (rpmBarRect.width - 4) * rpmRatio, rpmBarRect.height - 4);
            GUI.Box(rpmFillRect, "");
        }

        private void DrawNitroGauge(int w, int h)
        {
            // Bottom-Center Nitro Tank Gauge
            float nitroW = 240f;
            float nitroH = 34f;
            float nitroX = (w - nitroW) * 0.5f;
            float nitroY = h - nitroH - 25f;

            GUI.Box(new Rect(nitroX, nitroY, nitroW, nitroH), "");

            float nitroPercent = _motorcycle.CurrentNitroPercent;
            Rect fillRect = new Rect(nitroX + 4, nitroY + 4, (nitroW - 8) * nitroPercent, nitroH - 8);
            GUI.Box(fillRect, "");

            GUIStyle nitroText = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            nitroText.normal.textColor = _motorcycle.IsNitroActive ? new Color(1f, 0.5f, 0.1f) : Color.cyan;
            string label = _motorcycle.IsNitroActive ? "🔥 NITRO ACTIVE 🔥" : $"NITRO BOOST: {(int)(nitroPercent * 100)}% (Shift)";
            GUI.Label(new Rect(nitroX, nitroY, nitroW, nitroH), label, nitroText);
        }

        private void DrawMobileTouchControls(int w, int h)
        {
            // ---------------- RIGHT SIDE: PEDALS & NITRO ----------------
            float pedalW = 85f;
            float pedalH = 80f;
            float margin = 25f;

            // Gas / Throttle Pedal (Green)
            Rect gasRect = new Rect(w - pedalW - margin, h - pedalH - 160f, pedalW, pedalH);
            if (GUI.RepeatButton(gasRect, "<b>GAS ⬆</b>\n(W)"))
            {
                _touchThrottle = 1.0f;
            }

            // Brake / Reverse Pedal (Red)
            Rect brakeRect = new Rect(w - pedalW * 2 - margin - 15f, h - pedalH - 160f, pedalW, pedalH);
            if (GUI.RepeatButton(brakeRect, "<b>BRAKE ⬇</b>\n(S)"))
            {
                _touchBrake = 1.0f;
            }

            // Nitro Boost Button (Orange Flame)
            Rect nitroBtnRect = new Rect(w - pedalW * 1.5f - margin, h - pedalH - 255f, pedalW * 1.2f, 48f);
            if (GUI.RepeatButton(nitroBtnRect, "🔥 <b>NITRO</b> (Shift)"))
            {
                _touchNitro = true;
            }

            // ---------------- LEFT SIDE: STEERING & INDIAN HORN ----------------
            float steerW = 85f;
            float steerH = 80f;

            // Steer Left
            Rect steerLeftRect = new Rect(margin, h - steerH - 45f, steerW, steerH);
            if (GUI.RepeatButton(steerLeftRect, "<b>◀ LEFT</b>\n(A)"))
            {
                _touchSteer = -1.0f;
            }

            // Steer Right
            Rect steerRightRect = new Rect(margin + steerW + 15f, h - steerH - 45f, steerW, steerH);
            if (GUI.RepeatButton(steerRightRect, "<b>RIGHT ▶</b>\n(D)"))
            {
                _touchSteer = 1.0f;
            }

            // Indian Horn Button (Yellow - Classic "Horn OK Please")
            Rect hornRect = new Rect(margin, h - steerH - 110f, steerW * 2 + 15f, 48f);
            if (GUI.Button(hornRect, "📢 <b>HORN OK PLEASE!</b> (H)"))
            {
                _motorcycle.TriggerHorn();
            }
        }
    }
}
