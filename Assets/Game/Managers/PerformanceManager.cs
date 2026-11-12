using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Core.Managers
{
    /// <summary>
    /// Core diagnostic service tracking FPS, frame time ms, and memory usage.
    /// Operates on interval sampling to eliminate per-frame garbage collection.
    /// </summary>
    public class PerformanceManager : MonoBehaviour, IPerformanceDiagnostics
    {
        private const string Tag = "PerformanceManager";

        [SerializeField] private float _sampleInterval = 0.5f;
        [SerializeField] private bool _showOverlayByDefault = true;

        private float _currentFps;
        private float _currentFrameTimeMs;
        private float _allocatedMemoryMb;
        private bool _isOverlayVisible;
        private bool _isInitialized;

        private int _frameCount;
        private float _timeAccumulator;

        public bool IsInitialized => _isInitialized;
        public float CurrentFPS => _currentFps;
        public float CurrentFrameTimeMs => _currentFrameTimeMs;
        public float AllocatedMemoryMb => _allocatedMemoryMb;
        public bool IsOverlayVisible => _isOverlayVisible;

        public void Initialize()
        {
            if (_isInitialized) return;

#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
            _isOverlayVisible = false;
#else
            _isOverlayVisible = _showOverlayByDefault;
#endif

            _isInitialized = true;
            GameLogger.I(Tag, "PerformanceManager initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        private void Update()
        {
            if (!_isInitialized) return;

            _frameCount++;
            _timeAccumulator += Time.unscaledDeltaTime;

            if (_timeAccumulator >= _sampleInterval)
            {
                _currentFps = _frameCount / _timeAccumulator;
                _currentFrameTimeMs = (_timeAccumulator / _frameCount) * 1000f;
                _allocatedMemoryMb = System.GC.GetTotalMemory(false) / (1024f * 1024f);

                _frameCount = 0;
                _timeAccumulator = 0f;
            }

            // Toggle overlay with F3 or multi-finger tap on mobile
#if UNITY_EDITOR || UNITY_STANDALONE
            if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
            {
                ToggleOverlay();
            }
#elif UNITY_ANDROID
            if (UnityEngine.Input.touchCount >= 4)
            {
                ToggleOverlay();
            }
#endif
        }

        public void SetOverlayVisible(bool show)
        {
            _isOverlayVisible = show;
        }

        public void ToggleOverlay()
        {
            _isOverlayVisible = !_isOverlayVisible;
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private void OnGUI()
        {
            if (!_isOverlayVisible) return;

            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(w - 280, 10, 270, 95);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = Mathf.Max(12, h * 2 / 100);
            style.normal.textColor = _currentFps >= 50f ? Color.green : (_currentFps >= 28f ? Color.yellow : Color.red);

            string text = $"FPS: {_currentFps:0.0} ({_currentFrameTimeMs:0.0} ms)\n" +
                          $"Mem: {_allocatedMemoryMb:0.0} MB\n" +
                          $"Quality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}\n" +
                          $"Battery: {(SystemInfo.batteryLevel >= 0 ? (SystemInfo.batteryLevel * 100).ToString("0") + "%" : "AC")}";

            // Draw dark background box
            GUI.Box(rect, GUIContent.none);
            GUI.Label(rect, text, style);
        }
#endif
    }
}
