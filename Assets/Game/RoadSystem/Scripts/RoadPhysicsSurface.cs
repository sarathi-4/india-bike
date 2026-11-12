using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    /// <summary>
    /// Component placed on road colliders to supply surface friction, speed limits,
    /// and surface feedback to motorcycle physics and wheel colliders.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RoadPhysicsSurface : MonoBehaviour
    {
        [Header("Surface Properties")]
        [SerializeField] private RoadSurfaceType _surfaceType = RoadSurfaceType.WornTarmac;
        [SerializeField] private RoadSurface _surfaceSettings;

        [Header("Road Features")]
        [SerializeField] private bool _hasSpeedBreakers;
        [SerializeField] private float _speedLimitKmph = 60f;
        [SerializeField] private bool _isWet;

        public RoadSurfaceType SurfaceType => _surfaceType;
        public float Friction => _isWet ? _surfaceSettings.frictionCoefficient * 0.7f : _surfaceSettings.frictionCoefficient;
        public float RollingResistance => _surfaceSettings.rollingResistance;
        public float Bumpiness => _surfaceSettings.bumpiness;
        public float SpeedLimitKmph => _speedLimitKmph;
        public bool HasSpeedBreakers => _hasSpeedBreakers;
        public bool IsWet => _isWet;

        private void Awake()
        {
            if (_surfaceSettings == null)
            {
                _surfaceSettings = RoadSurface.GetDefault(_surfaceType);
            }
            if (_speedLimitKmph <= 0f)
            {
                _speedLimitKmph = _surfaceSettings.defaultSpeedLimitKmph;
            }
        }

        public void SetWet(bool wet)
        {
            _isWet = wet;
        }
    }
}
