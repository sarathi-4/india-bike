using IndianBikeLife.Core.Configuration;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Service interface for querying and dynamically adjusting Android graphics quality presets.
    /// </summary>
    public interface IQualityService : IGameService
    {
        MobileQualityPreset CurrentPreset { get; }
        void SetQualityPreset(MobileQualityPreset preset);
        int CurrentTargetFPS { get; }
        float CurrentShadowDistance { get; }
        float CurrentRenderScale { get; }
        float CurrentLODBias { get; }
    }
}

