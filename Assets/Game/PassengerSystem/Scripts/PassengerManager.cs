using System;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    /// <summary>
    /// Core persistent manager implementing IPassengerManager and ISaveable.
    /// Manages passenger selection, mounting sequence, and synchronization with motorcycle physics.
    /// </summary>
    public class PassengerManager : MonoBehaviour, IPassengerManager, ISaveable
    {
        private const string Tag = "PassengerManager";

        [Header("Configuration")]
        [SerializeField] private PassengerSelection _selectionCatalog;
        [SerializeField] private GameObject _passengerPrefab;

        private PassengerData _currentPassengerData;
        private PassengerController _activePassenger;
        private bool _hasPassenger = true;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "PassengerManager";
        public bool HasPassenger => _hasPassenger;
        public PassengerData CurrentPassengerData => _currentPassengerData;
        public PassengerController ActivePassenger => _activePassenger;

        public void Initialize()
        {
            if (_isInitialized) return;

            if (_selectionCatalog != null && _selectionCatalog.SelectedPassenger != null)
            {
                _currentPassengerData = _selectionCatalog.SelectedPassenger;
            }

            _isInitialized = true;
            GameLogger.I(Tag, "PassengerManager initialized.");
        }

        public void Shutdown()
        {
            if (_activePassenger != null)
            {
                Destroy(_activePassenger.gameObject);
            }
            _isInitialized = false;
        }

        public void SelectPassenger(PassengerData passengerData)
        {
            if (passengerData == null) return;
            _currentPassengerData = passengerData;

            if (_activePassenger != null)
            {
                _activePassenger.ApplyPassengerData(passengerData);
            }

            RidingPairingMode pairing = _selectionCatalog != null ? _selectionCatalog.ActivePairing : RidingPairingMode.MaleDriver_FemalePassenger;
            EventBus.Publish(new PassengerSelectedEvent(passengerData, pairing));
            GameLogger.I(Tag, $"Selected passenger: {passengerData.DisplayName} ({passengerData.Gender})");
        }

        public void SetPassengerActive(bool active)
        {
            _hasPassenger = active;
            if (_activePassenger != null)
            {
                _activePassenger.gameObject.SetActive(active);
            }
        }

        public void TogglePassenger()
        {
            SetPassengerActive(!_hasPassenger);
        }

        public void MountPassenger(PassengerSeatPosition seatPosition)
        {
            if (!_hasPassenger || seatPosition == null) return;

            // Ensure passenger instance exists
            if (_activePassenger == null)
            {
                if (_passengerPrefab != null)
                {
                    GameObject go = Instantiate(_passengerPrefab, seatPosition.MountStandPoint.position, seatPosition.MountStandPoint.rotation);
                    _activePassenger = go.GetComponent<PassengerController>();
                    if (_activePassenger == null) _activePassenger = go.AddComponent<PassengerController>();
                }
                else
                {
                    // Procedural fallback character GameObject
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    go.name = "ProceduralPassenger";
                    go.transform.position = seatPosition.MountStandPoint.position;
                    go.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);
                    _activePassenger = go.AddComponent<PassengerController>();
                }
            }

            if (_currentPassengerData != null)
            {
                _activePassenger.ApplyPassengerData(_currentPassengerData);
            }

            _activePassenger.MountController.Mount(seatPosition, () =>
            {
                EventBus.Publish(new PassengerMountedEvent(_currentPassengerData));
                GameLogger.I(Tag, "Passenger mounted successfully.");
            });
        }

        public void DismountPassenger()
        {
            if (_activePassenger == null || !_activePassenger.MountController.IsMounted) return;

            _activePassenger.MountController.Dismount(() =>
            {
                EventBus.Publish(new PassengerDismountedEvent(_currentPassengerData));
                GameLogger.I(Tag, "Passenger dismounted.");
            });
        }

        #region ISaveable Implementation

        public void PopulateSaveData(SaveData data)
        {
            // Extensible save hook: store passenger preference in profile if desired
        }

        public void RestoreFromSaveData(SaveData data)
        {
            // Restore saved passenger preference
        }

        #endregion
    }
}

