using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.RidingModes
{
    /// <summary>
    /// Component facilitating player selection, UI previews, and cycling of riding modes.
    /// </summary>
    public class RidingModeSelector : MonoBehaviour
    {
        [SerializeField] private RidingModeConfiguration _catalogue;
        [SerializeField] private RidingModeType _currentSelection = RidingModeType.HighwayRide;

        public event Action<RidingModeData> OnModeSelectionChanged;

        public RidingModeConfiguration Catalogue => _catalogue;
        public RidingModeType CurrentSelection => _currentSelection;

        public RidingModeData GetSelectedModeData()
        {
            return _catalogue != null ? _catalogue.GetMode(_currentSelection) : null;
        }

        public void SelectMode(RidingModeType mode)
        {
            if (_currentSelection == mode) return;
            _currentSelection = mode;

            RidingModeData data = GetSelectedModeData();
            OnModeSelectionChanged?.Invoke(data);
        }

        public void CycleNextMode()
        {
            int next = ((int)_currentSelection + 1) % 10;
            SelectMode((RidingModeType)next);
        }

        public void CyclePrevMode()
        {
            int prev = ((int)_currentSelection - 1 + 10) % 10;
            SelectMode((RidingModeType)prev);
        }
    }
}

