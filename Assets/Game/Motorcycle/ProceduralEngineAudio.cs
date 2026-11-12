using UnityEngine;

namespace IndianBikeLife.Motorcycle
{
    /// <summary>
    /// Procedurally synthesizes realistic Indian motorcycle exhaust rumble (single-cylinder 350cc thump)
    /// and Indian classic dual-tone horn audio in real-time.
    /// Operates without requiring external .wav or .mp3 audio asset files.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PlayerMotorcycleController))]
    public class ProceduralEngineAudio : MonoBehaviour
    {
        private PlayerMotorcycleController _controller;
        private AudioSource _audioSource;

        private float _samplingRate = 44100f;
        private float _enginePhase;
        private float _hornPhase1;
        private float _hornPhase2;

        private float _hornTimer = 0f;
        private bool _isHonking = false;

        private void Awake()
        {
            _controller = GetComponent<PlayerMotorcycleController>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = true;
            _audioSource.loop = true;
            _audioSource.spatialBlend = 0.35f; // Slight 3D presence
            _samplingRate = AudioSettings.outputSampleRate;

            _controller.OnHornHonked += TriggerHornAudio;
        }

        private void OnDestroy()
        {
            if (_controller != null)
            {
                _controller.OnHornHonked -= TriggerHornAudio;
            }
        }

        public void TriggerHornAudio()
        {
            _hornTimer = 0.55f; // Honk for 0.55 seconds
            _isHonking = true;
        }

        private void Update()
        {
            if (_isHonking)
            {
                _hornTimer -= Time.deltaTime;
                if (_hornTimer <= 0f)
                {
                    _isHonking = false;
                }
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (_controller == null) return;

            float rpm = _controller.CurrentRpm;
            // 350cc single-cylinder 4-stroke thumps at RPM / 120 Hz
            float baseEngineFreq = Mathf.Lerp(28f, 135f, Mathf.InverseLerp(1000f, 6500f, rpm));
            float engineVolume = Mathf.Lerp(0.12f, 0.28f, Mathf.InverseLerp(1000f, 6500f, rpm));

            // Indian dual-tone horn frequencies (Classic Roots Vibrosonic horn ~ 370 Hz & 440 Hz)
            float hornFreq1 = 370f;
            float hornFreq2 = 440f;
            float hornVolume = _isHonking ? 0.35f : 0f;

            for (int i = 0; i < data.Length; i += channels)
            {
                // 1. Engine exhaust pulse (triangular/sawtooth harmonic single-cylinder thump)
                _enginePhase += (baseEngineFreq / _samplingRate);
                if (_enginePhase > 1f) _enginePhase -= 1f;

                float engineWave = Mathf.Sin(_enginePhase * 2f * Mathf.PI);
                // Add second harmonic for throaty exhaust growl
                engineWave += 0.5f * Mathf.Sin(_enginePhase * 4f * Mathf.PI);
                engineWave *= engineVolume;

                // 2. Dual-tone Indian horn
                float hornWave = 0f;
                if (_isHonking)
                {
                    _hornPhase1 += (hornFreq1 / _samplingRate);
                    _hornPhase2 += (hornFreq2 / _samplingRate);
                    if (_hornPhase1 > 1f) _hornPhase1 -= 1f;
                    if (_hornPhase2 > 1f) _hornPhase2 -= 1f;

                    hornWave = (Mathf.Sin(_hornPhase1 * 2f * Mathf.PI) + Mathf.Sin(_hornPhase2 * 2f * Mathf.PI)) * 0.5f * hornVolume;
                }

                float sample = Mathf.Clamp(engineWave + hornWave, -1f, 1f);

                for (int c = 0; c < channels; c++)
                {
                    data[i + c] = sample;
                }
            }
        }
    }
}

