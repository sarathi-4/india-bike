using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    public enum RidingPairingMode
    {
        SoloRider = 0,               // Driver only
        MaleDriver_FemalePassenger = 1,
        MaleDriver_MalePassenger = 2,
        FemaleDriver_MalePassenger = 3,
        FemaleDriver_FemalePassenger = 4
    }

    /// <summary>
    /// Manages passenger selection catalogs, pairing combinations, and preview settings.
    /// </summary>
    [CreateAssetMenu(fileName = "PassengerSelection", menuName = "IndianBikeLife/Passenger/Passenger Selection Catalog")]
    public class PassengerSelection : ScriptableObject
    {
        [Header("Pairing Configuration")]
        [SerializeField] private RidingPairingMode _activePairing = RidingPairingMode.MaleDriver_FemalePassenger;
        [SerializeField] private CharacterGender _driverGender = CharacterGender.Male;

        [Header("Available Passenger Roster")]
        [SerializeField] private List<PassengerData> _passengerRoster = new List<PassengerData>();
        [SerializeField] private int _selectedPassengerIndex = 0;

        public RidingPairingMode ActivePairing
        {
            get => _activePairing;
            set
            {
                _activePairing = value;
                UpdateGendersFromPairing();
            }
        }

        public CharacterGender DriverGender => _driverGender;
        public IReadOnlyList<PassengerData> PassengerRoster => _passengerRoster;

        public PassengerData SelectedPassenger
        {
            get
            {
                if (_passengerRoster.Count == 0) return null;
                int idx = Mathf.Clamp(_selectedPassengerIndex, 0, _passengerRoster.Count - 1);
                return _passengerRoster[idx];
            }
        }

        public void SelectPassengerByIndex(int index)
        {
            if (index >= 0 && index < _passengerRoster.Count)
            {
                _selectedPassengerIndex = index;
            }
        }

        private void UpdateGendersFromPairing()
        {
            switch (_activePairing)
            {
                case RidingPairingMode.SoloRider:
                    break;
                case RidingPairingMode.MaleDriver_FemalePassenger:
                    _driverGender = CharacterGender.Male;
                    break;
                case RidingPairingMode.MaleDriver_MalePassenger:
                    _driverGender = CharacterGender.Male;
                    break;
                case RidingPairingMode.FemaleDriver_MalePassenger:
                    _driverGender = CharacterGender.Female;
                    break;
                case RidingPairingMode.FemaleDriver_FemalePassenger:
                    _driverGender = CharacterGender.Female;
                    break;
            }
        }
    }
}

