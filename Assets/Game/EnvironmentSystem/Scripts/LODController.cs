using UnityEngine;

namespace IndianBikeLife.EnvironmentSystem
{
    /// <summary>
    /// Mobile-friendly Level of Detail (LOD) controller.
    /// Adjusts mesh resolution and disables small decorations based on distance to player camera.
    /// </summary>
    public class LODController : MonoBehaviour
    {
        [Header("LOD Meshes")]
        [SerializeField] private GameObject _lod0HighDetail;
        [SerializeField] private GameObject _lod1MediumDetail;
        [SerializeField] private GameObject _lod2LowDetail;

        [Header("Distance Thresholds (Meters)")]
        [SerializeField] private float _lod0Distance = 35f;
        [SerializeField] private float _lod1Distance = 80f;
        [SerializeField] private float _cullDistance = 180f;

        private int _currentLOD = -1;

        public void UpdateLOD(Vector3 cameraPosition)
        {
            float distSqr = (transform.position - cameraPosition).sqrMagnitude;

            int newLOD = 3; // Culled by default
            if (distSqr <= _lod0Distance * _lod0Distance)
            {
                newLOD = 0;
            }
            else if (distSqr <= _lod1Distance * _lod1Distance)
            {
                newLOD = 1;
            }
            else if (distSqr <= _cullDistance * _cullDistance)
            {
                newLOD = 2;
            }

            if (newLOD != _currentLOD)
            {
                _currentLOD = newLOD;
                ApplyLOD(_currentLOD);
            }
        }

        private void ApplyLOD(int lod)
        {
            if (_lod0HighDetail != null) _lod0HighDetail.SetActive(lod == 0);
            if (_lod1MediumDetail != null) _lod1MediumDetail.SetActive(lod == 1);
            if (_lod2LowDetail != null) _lod2LowDetail.SetActive(lod == 2);
        }
    }
}

