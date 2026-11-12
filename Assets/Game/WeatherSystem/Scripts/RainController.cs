using IndianBikeLife.Core.Audio;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.WeatherSystem
{
    /// <summary>
    /// Controls rain particle systems, raindrop camera effects, and ambient rain audio.
    /// </summary>
    public class RainController : MonoBehaviour
    {
        [Header("Particle System")]
        [SerializeField] private ParticleSystem _rainParticles;
        [SerializeField] private AudioClip _rainLoopClip;

        private float _currentEmission;
        private IAudioService _audioService;

        private void Start()
        {
            ServiceLocator.TryGet(out _audioService);
        }

        public void SetRainIntensity(float emissionRate)
        {
            _currentEmission = emissionRate;

            if (_rainParticles != null)
            {
                var emission = _rainParticles.emission;
                emission.rateOverTime = emissionRate;

                if (emissionRate > 1f && !_rainParticles.isPlaying)
                {
                    _rainParticles.Play();
                }
                else if (emissionRate <= 0.1f && _rainParticles.isPlaying)
                {
                    _rainParticles.Stop();
                }
            }
        }
    }
}
