using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    /// <summary>
    /// Master coordinator on the Passenger GameObject root.
    /// Manages character visuals, hold styles, and simulation updates.
    /// </summary>
    [RequireComponent(typeof(PassengerAnimationController))]
    [RequireComponent(typeof(PassengerMountController))]
    public class PassengerController : MonoBehaviour
    {
        [Header("Data & Configuration")]
        [SerializeField] private PassengerData _data;

        [Header("Visual Meshes & Tints")]
        [SerializeField] private Renderer _clothingRenderer;
        [SerializeField] private GameObject _helmetMesh;
        [SerializeField] private GameObject _accessoriesMesh;
        [SerializeField] private Transform _headTransform;

        private PassengerAnimationController _animController;
        private PassengerMountController _mountController;

        public PassengerData Data => _data;
        public PassengerAnimationController AnimController => _animController;
        public PassengerMountController MountController => _mountController;
        public Transform HeadTransform => _headTransform != null ? _headTransform : transform;

        private void Awake()
        {
            _animController = GetComponent<PassengerAnimationController>();
            _mountController = GetComponent<PassengerMountController>();

            ApplyPassengerData(_data);
        }

        public void ApplyPassengerData(PassengerData data)
        {
            if (data == null) return;
            _data = data;

            if (_clothingRenderer != null && _clothingRenderer.material != null)
            {
                _clothingRenderer.material.color = data.ClothingTint;
            }

            if (_helmetMesh != null)
            {
                _helmetMesh.SetActive(data.HasHelmet);
            }

            if (_accessoriesMesh != null)
            {
                _accessoriesMesh.SetActive(!string.IsNullOrEmpty(data.Accessories));
            }
        }

        public void UpdateRidingDynamics(float bikeRollAngle, float bikeAcceleration)
        {
            if (_animController != null && _mountController.IsMounted)
            {
                _animController.UpdateRidingDynamics(bikeRollAngle, bikeAcceleration, Time.deltaTime);
            }
        }
    }
}

