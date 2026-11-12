using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Utilities.Logger;
using IndianBikeLife.MapSystem.Streaming;
using UnityEngine;

namespace IndianBikeLife.MapSystem
{
    /// <summary>
    /// Core manager for Indian geographical map selection, hierarchy routing, and chunk streaming.
    /// Implements IIndiaMapManager, IGameService, and ISaveable.
    /// </summary>
    [RequireComponent(typeof(MapSelectionManager))]
    public class IndiaMapManager : MonoBehaviour, IIndiaMapManager, ISaveable
    {
        private const string Tag = "IndiaMapManager";

        [SerializeField] private MapCatalogue _catalogue;
        [SerializeField] private MapStreamingManager _streamingManager;

        private MapSelectionManager _selectionManager;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "IndiaMapManager";

        public MapCatalogue Catalogue => _catalogue;
        public StateData SelectedState => _selectionManager.SelectedState;
        public DistrictData SelectedDistrict => _selectionManager.SelectedDistrict;
        public CityData SelectedCity => _selectionManager.SelectedCity;
        public DestinationData SelectedDestination => _selectionManager.SelectedDestination;

        public void Initialize()
        {
            if (_isInitialized) return;

            _selectionManager = GetComponent<MapSelectionManager>();
            if (_streamingManager == null)
            {
                _streamingManager = GetComponentInChildren<MapStreamingManager>();
            }

            // Default selection: Tamil Nadu (priority state 1)
            if (_catalogue != null && _catalogue.States.Count > 0)
            {
                SelectState(_catalogue.States[0].StateId);
            }

            _isInitialized = true;
            GameLogger.I(Tag, "IndiaMapManager initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void SelectState(string stateId)
        {
            if (_catalogue == null) return;
            StateData state = _catalogue.FindStateById(stateId);
            if (state != null)
            {
                _selectionManager.SelectState(state);
            }
        }

        public void SelectDistrict(string districtId)
        {
            if (SelectedState == null) return;
            DistrictData district = null;
            foreach (var d in SelectedState.Districts)
            {
                if (d != null && d.DistrictId.Equals(districtId, StringComparison.OrdinalIgnoreCase))
                {
                    district = d;
                    break;
                }
            }
            if (district != null)
            {
                _selectionManager.SelectDistrict(district);
            }
        }

        public void SelectCity(string cityId)
        {
            if (SelectedDistrict == null) return;
            CityData city = null;
            foreach (var c in SelectedDistrict.Cities)
            {
                if (c != null && c.CityId.Equals(cityId, StringComparison.OrdinalIgnoreCase))
                {
                    city = c;
                    break;
                }
            }
            if (city != null)
            {
                _selectionManager.SelectCity(city);
            }
        }

        public void SelectDestination(string destinationId)
        {
            if (_catalogue == null) return;
            DestinationData dest = _catalogue.FindDestinationById(destinationId);
            if (dest != null)
            {
                _selectionManager.SelectDestination(dest);
            }
        }

        public bool ToggleFavouriteDestination(string destinationId)
        {
            return _selectionManager.ToggleFavourite(destinationId);
        }

        public bool IsFavourite(string destinationId)
        {
            return _selectionManager.IsFavourite(destinationId);
        }

        public IReadOnlyList<string> GetFavouriteDestinations()
        {
            return _selectionManager.FavouriteDestinationIds;
        }

        #region ISaveable Implementation

        public void PopulateSaveData(SaveData data)
        {
            // Future save integration for favourite destinations
        }

        public void RestoreFromSaveData(SaveData data)
        {
            // Restore favourite destinations
        }

        #endregion
    }
}

