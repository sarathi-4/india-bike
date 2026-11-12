using System.Collections.Generic;
using IndianBikeLife.MapSystem;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for geographical navigation hierarchy, map catalogues, and destination selection.
    /// </summary>
    public interface IIndiaMapManager : IGameService
    {
        MapCatalogue Catalogue { get; }
        StateData SelectedState { get; }
        DistrictData SelectedDistrict { get; }
        CityData SelectedCity { get; }
        DestinationData SelectedDestination { get; }

        void SelectState(string stateId);
        void SelectDistrict(string districtId);
        void SelectCity(string cityId);
        void SelectDestination(string destinationId);
        bool ToggleFavouriteDestination(string destinationId);
        bool IsFavourite(string destinationId);
        IReadOnlyList<string> GetFavouriteDestinations();
    }
}
