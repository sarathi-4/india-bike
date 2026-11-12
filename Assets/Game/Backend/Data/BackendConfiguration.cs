using UnityEngine;

namespace IndianBikeLife.Backend
{
    public enum BackendEnvironment
    {
        DevelopmentLocal,
        StagingCloud,
        ProductionLive
    }

    /// <summary>
    /// Configuration asset for cloud backend endpoints, timeouts, and offline fallback modes.
    /// Never stores private server API secrets or service account keys.
    /// </summary>
    [CreateAssetMenu(fileName = "BackendConfiguration", menuName = "Indian Bike Life/Backend/Backend Configuration")]
    public class BackendConfiguration : ScriptableObject
    {
        [SerializeField] private BackendEnvironment _environment = BackendEnvironment.DevelopmentLocal;
        [SerializeField] private string _apiBaseUrl = "https://api.indianbikelife.game/v1";
        [SerializeField] private float _requestTimeoutSeconds = 10.0f;
        [SerializeField] private bool _enableOfflineMockMode = true;

        public BackendEnvironment Environment => _environment;
        public string ApiBaseUrl => _apiBaseUrl;
        public float RequestTimeoutSeconds => _requestTimeoutSeconds;
        public bool EnableOfflineMockMode => _enableOfflineMockMode;
    }
}

