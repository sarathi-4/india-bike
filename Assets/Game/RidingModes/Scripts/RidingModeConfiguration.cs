using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.RidingModes
{
    /// <summary>
    /// Master repository asset holding configuration assets for all 10 riding modes.
    /// </summary>
    [CreateAssetMenu(fileName = "RidingModeCatalogue", menuName = "IndianBikeLife/Riding Modes/Riding Mode Catalogue")]
    public class RidingModeConfiguration : ScriptableObject
    {
        [SerializeField] private List<RidingModeData> _modes = new List<RidingModeData>();

        public IReadOnlyList<RidingModeData> Modes => _modes;

        public RidingModeData GetMode(RidingModeType type)
        {
            return _modes.Find(m => m != null && m.ModeType == type);
        }
    }
}
