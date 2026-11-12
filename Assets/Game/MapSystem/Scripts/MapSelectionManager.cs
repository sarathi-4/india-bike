using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.MapSystem
{
    /// <summary>
    /// Coordinates UI selection across State -> District -> City -> Destination hierarchy.
    /// Manages player favourite destinations and route endpoints.
    /// </summary>
    public class MapSelectionManager : MonoBehaviour
    {
        private const string Tag = "MapSelection";

        [SerializeField] private MapCatalogue _catalogue;

        private StateData _selectedState;
        private DistrictData _selectedDistrict;
        private CityData _selectedCity;
        private DestinationData _selectedDestination;

        private readonly List<string> _favouriteDestinationIds = new List<string>();

        public MapCatalogue Catalogue => _catalogue;
        public StateData SelectedState => _selectedState;
        public DistrictData SelectedDistrict => _selectedDistrict;
        public CityData SelectedCity => _selectedCity;
        public DestinationData SelectedDestination => _selectedDestination;
        public IReadOnlyList<string> FavouriteDestinationIds => _favouriteDestinationIds;

        public void SelectState(StateData state)
        {
            _selectedState = state;
            _selectedDistrict = (state != null && state.Districts.Count > 0) ? state.Districts[0] : null;
            _selectedCity = (_selectedDistrict != null && _selectedDistrict.Cities.Count > 0) ? _selectedDistrict.Cities[0] : null;
            _selectedDestination = (_selectedCity != null && _selectedCity.Destinations.Count > 0) ? _selectedCity.Destinations[0] : null;

            GameLogger.I(Tag, $"Selected State: {state?.StateName}");
        }

        public void SelectDistrict(DistrictData district)
        {
            _selectedDistrict = district;
            _selectedCity = (district != null && district.Cities.Count > 0) ? district.Cities[0] : null;
            _selectedDestination = (_selectedCity != null && _selectedCity.Destinations.Count > 0) ? _selectedCity.Destinations[0] : null;

            GameLogger.I(Tag, $"Selected District: {district?.DistrictName}");
        }

        public void SelectCity(CityData city)
        {
            _selectedCity = city;
            _selectedDestination = (city != null && city.Destinations.Count > 0) ? city.Destinations[0] : null;

            GameLogger.I(Tag, $"Selected City: {city?.CityName}");
        }

        public void SelectDestination(DestinationData destination)
        {
            _selectedDestination = destination;
            GameLogger.I(Tag, $"Selected Destination: {destination?.DestinationName} ({destination?.PrimaryEnvironment})");
        }

        public bool ToggleFavourite(string destinationId)
        {
            if (string.IsNullOrEmpty(destinationId)) return false;

            if (_favouriteDestinationIds.Contains(destinationId))
            {
                _favouriteDestinationIds.Remove(destinationId);
                return false;
            }
            else
            {
                _favouriteDestinationIds.Add(destinationId);
                return true;
            }
        }

        public bool IsFavourite(string destinationId)
        {
            return !string.IsNullOrEmpty(destinationId) && _favouriteDestinationIds.Contains(destinationId);
        }
    }
}
