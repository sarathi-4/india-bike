using System;
using IndianBikeLife.PassengerSystem;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for passenger selection, mounting lifecycle, and camera synchronization.
    /// </summary>
    public interface IPassengerManager : IGameService
    {
        bool HasPassenger { get; }
        PassengerData CurrentPassengerData { get; }
        PassengerController ActivePassenger { get; }

        void SelectPassenger(PassengerData passengerData);
        void SetPassengerActive(bool active);
        void TogglePassenger();
        void MountPassenger(PassengerSeatPosition seatPosition);
        void DismountPassenger();
    }
}

