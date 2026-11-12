using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.MapSystem
{
    /// <summary>
    /// Master national catalogue defining all 28 Indian States and 8 Union Territories.
    /// Provides lookups by ID, name, and regional zone without hardcoding into monolithic scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "IndiaMapCatalogue", menuName = "IndianBikeLife/Maps/India Map Catalogue")]
    public class MapCatalogue : ScriptableObject
    {
        [Header("National Hierarchy")]
        [SerializeField] private List<StateData> _states = new List<StateData>();
        [SerializeField] private List<StateData> _unionTerritories = new List<StateData>();

        public IReadOnlyList<StateData> States => _states;
        public IReadOnlyList<StateData> UnionTerritories => _unionTerritories;

        public StateData FindStateById(string stateId)
        {
            if (string.IsNullOrEmpty(stateId)) return null;

            StateData found = _states.Find(s => s != null && s.StateId.Equals(stateId, StringComparison.OrdinalIgnoreCase));
            if (found != null) return found;

            return _unionTerritories.Find(ut => ut != null && ut.StateId.Equals(stateId, StringComparison.OrdinalIgnoreCase));
        }

        public DestinationData FindDestinationById(string destinationId)
        {
            if (string.IsNullOrEmpty(destinationId)) return null;

            foreach (var state in _states)
            {
                if (state == null) continue;
                foreach (var district in state.Districts)
                {
                    if (district == null) continue;
                    foreach (var city in district.Cities)
                    {
                        if (city == null) continue;
                        foreach (var dest in city.Destinations)
                        {
                            if (dest != null && dest.DestinationId.Equals(destinationId, StringComparison.OrdinalIgnoreCase))
                            {
                                return dest;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public int TotalStatesCount => _states.Count;
        public int TotalUnionTerritoriesCount => _unionTerritories.Count;
    }
}

