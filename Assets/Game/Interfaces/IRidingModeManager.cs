using System.Collections.Generic;
using IndianBikeLife.RidingModes;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for selecting, configuring, and applying motorcycle riding modes.
    /// </summary>
    public interface IRidingModeManager : IGameService
    {
        RidingModeType ActiveModeType { get; }
        RidingModeData ActiveModeData { get; }
        IReadOnlyList<RidingModeData> AllModes { get; }

        void SelectMode(RidingModeType modeType);
        void ApplyActiveModeSettings();
    }
}
