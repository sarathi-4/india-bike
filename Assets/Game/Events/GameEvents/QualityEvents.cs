using IndianBikeLife.Core.Configuration;
using IndianBikeLife.Core.Events.Core;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct QualityPresetChangedEvent : IGameEvent
    {
        public readonly MobileQualityPreset PreviousPreset;
        public readonly MobileQualityPreset NewPreset;
        public readonly int TargetFPS;

        public QualityPresetChangedEvent(MobileQualityPreset prev, MobileQualityPreset next, int targetFps)
        {
            PreviousPreset = prev;
            NewPreset = next;
            TargetFPS = targetFps;
        }
    }
}
