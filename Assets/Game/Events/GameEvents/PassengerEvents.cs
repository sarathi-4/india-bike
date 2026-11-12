using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.PassengerSystem;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct PassengerMountedEvent : IGameEvent
    {
        public PassengerData Passenger { get; }
        public PassengerMountedEvent(PassengerData passenger) => Passenger = passenger;
    }

    public readonly struct PassengerDismountedEvent : IGameEvent
    {
        public PassengerData Passenger { get; }
        public PassengerDismountedEvent(PassengerData passenger) => Passenger = passenger;
    }

    public readonly struct PassengerSelectedEvent : IGameEvent
    {
        public PassengerData Passenger { get; }
        public RidingPairingMode PairingMode { get; }

        public PassengerSelectedEvent(PassengerData passenger, RidingPairingMode pairingMode)
        {
            Passenger = passenger;
            PairingMode = pairingMode;
        }
    }
}

