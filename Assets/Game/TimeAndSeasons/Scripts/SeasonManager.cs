using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.TimeAndSeasons
{
    /// <summary>
    /// Governs seasonal cycles (Summer, Monsoon, Winter) tailored to Indian tropical and subtropical climates.
    /// Strictly excludes geographically inaccurate phenomena like snowfall in South Indian plains.
    /// </summary>
    public class SeasonManager : MonoBehaviour
    {
        private const string Tag = "SeasonManager";

        [SerializeField] private IndianSeason _currentSeason = IndianSeason.Summer;

        public event Action<IndianSeason> OnSeasonChanged;
        public IndianSeason CurrentSeason => _currentSeason;

        public void SetSeason(IndianSeason season)
        {
            if (_currentSeason == season) return;
            _currentSeason = season;
            OnSeasonChanged?.Invoke(_currentSeason);
            GameLogger.I(Tag, $"Season changed to: {_currentSeason}");
        }
    }
}

