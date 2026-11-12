using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.EnvironmentSystem
{
    /// <summary>
    /// Manages active world chunks and distributes periodic LOD updates across roadside entities.
    /// Eliminates expensive per-frame distance recalculations on mobile.
    /// </summary>
    public class WorldChunkManager : MonoBehaviour
    {
        [SerializeField] private float _lodUpdateInterval = 0.5f;
        [SerializeField] private Transform _cameraTransform;

        private readonly List<LODController> _lodControllers = new List<LODController>();
        private float _timer;

        public void RegisterLOD(LODController controller)
        {
            if (controller != null && !_lodControllers.Contains(controller))
            {
                _lodControllers.Add(controller);
            }
        }

        public void UnregisterLOD(LODController controller)
        {
            if (controller != null)
            {
                _lodControllers.Remove(controller);
            }
        }

        public void SetCameraTarget(Transform cam)
        {
            _cameraTransform = cam;
        }

        private void Update()
        {
            if (_cameraTransform == null) return;

            _timer += Time.deltaTime;
            if (_timer >= _lodUpdateInterval)
            {
                _timer = 0f;
                Vector3 camPos = _cameraTransform.position;
                for (int i = 0; i < _lodControllers.Count; i++)
                {
                    if (_lodControllers[i] != null)
                    {
                        _lodControllers[i].UpdateLOD(camPos);
                    }
                }
            }
        }
    }
}
