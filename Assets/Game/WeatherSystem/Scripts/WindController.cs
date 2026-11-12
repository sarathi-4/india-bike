using UnityEngine;

namespace IndianBikeLife.WeatherSystem
{
    /// <summary>
    /// Calculates wind vectors and gusts affecting rain particles and motorcycle stability.
    /// </summary>
    public class WindController : MonoBehaviour
    {
        [SerializeField] private Vector3 _windDirection = Vector3.forward;
        [SerializeField] private float _baseWindSpeed = 3.0f;
        [SerializeField] private float _gustFrequency = 0.2f;

        public Vector3 CurrentWindVector { get; private set; }

        public void SetBaseSpeed(float speed)
        {
            _baseWindSpeed = speed;
        }

        private void Update()
        {
            float gust = Mathf.PerlinNoise(Time.time * _gustFrequency, 0f) * 2.0f;
            CurrentWindVector = _windDirection.normalized * (_baseWindSpeed + gust);
        }
    }
}
