using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.NavigationSystem
{
    /// <summary>
    /// Manages checkpoint trigger volumes along an active navigation route.
    /// Emits checkpoint arrival signals as the player rides through.
    /// </summary>
    public class CheckpointManager : MonoBehaviour
    {
        [SerializeField] private float _triggerRadius = 8.0f;
        [SerializeField] private GameObject _checkpointVisualPrefab;

        private readonly List<Vector3> _checkpointPositions = new List<Vector3>();
        private int _currentCheckpointIndex = 0;
        private GameObject _activeMarker;

        public event Action<int> OnCheckpointPassed;
        public int CurrentCheckpointIndex => _currentCheckpointIndex;
        public int TotalCheckpoints => _checkpointPositions.Count;

        public void SetCheckpoints(List<Vector3> positions)
        {
            _checkpointPositions.Clear();
            if (positions != null)
            {
                _checkpointPositions.AddRange(positions);
            }
            _currentCheckpointIndex = 0;
            UpdateActiveMarker();
        }

        public void CheckPlayerPosition(Vector3 playerPos)
        {
            if (_checkpointPositions.Count == 0 || _currentCheckpointIndex >= _checkpointPositions.Count) return;

            Vector3 target = _checkpointPositions[_currentCheckpointIndex];
            float distSqr = (playerPos - target).sqrMagnitude;

            if (distSqr <= _triggerRadius * _triggerRadius)
            {
                int passedIdx = _currentCheckpointIndex;
                _currentCheckpointIndex++;
                OnCheckpointPassed?.Invoke(passedIdx);
                UpdateActiveMarker();
            }
        }

        private void UpdateActiveMarker()
        {
            if (_activeMarker != null)
            {
                Destroy(_activeMarker);
            }

            if (_currentCheckpointIndex < _checkpointPositions.Count)
            {
                Vector3 target = _checkpointPositions[_currentCheckpointIndex];
                if (_checkpointVisualPrefab != null)
                {
                    _activeMarker = Instantiate(_checkpointVisualPrefab, target, Quaternion.identity);
                }
                else
                {
                    // Fallback visual marker cylinder
                    _activeMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    _activeMarker.name = "CheckpointMarker";
                    _activeMarker.transform.position = target + Vector3.up * 1.5f;
                    _activeMarker.transform.localScale = new Vector3(3f, 0.1f, 3f);
                    Collider col = _activeMarker.GetComponent<Collider>();
                    if (col != null) col.enabled = false;
                }
            }
        }

        private void OnDestroy()
        {
            if (_activeMarker != null) Destroy(_activeMarker);
        }
    }
}
