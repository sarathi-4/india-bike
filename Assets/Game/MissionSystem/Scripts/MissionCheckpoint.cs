using System;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    /// <summary>
    /// Individual checkpoint spatial volume. Detects player motorcycle passage and provides visual cues.
    /// </summary>
    public class MissionCheckpoint : MonoBehaviour
    {
        private int _index;
        private float _triggerRadius = 8.0f;
        private Action<int> _onPassedCallback;
        private bool _isTriggered;

        public int Index => _index;
        public bool IsTriggered => _isTriggered;

        public void Initialize(int index, float radius, Action<int> onPassedCallback)
        {
            _index = index;
            _triggerRadius = radius;
            _onPassedCallback = onPassedCallback;
            _isTriggered = false;

            // Setup trigger sphere if no collider exists
            var col = GetComponent<SphereCollider>();
            if (col == null)
            {
                col = gameObject.AddComponent<SphereCollider>();
            }
            col.isTrigger = true;
            col.radius = _triggerRadius;
        }

        public bool CheckProximity(Vector3 playerPosition)
        {
            if (_isTriggered) return false;

            float distSqr = (playerPosition - transform.position).sqrMagnitude;
            if (distSqr <= _triggerRadius * _triggerRadius)
            {
                Trigger();
                return true;
            }
            return false;
        }

        public void Trigger()
        {
            if (_isTriggered) return;
            _isTriggered = true;
            _onPassedCallback?.Invoke(_index);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;

            // Check if player or motorcycle triggered
            if (other.CompareTag("Player") || other.name.Contains("Motorcycle") || other.name.Contains("Bike"))
            {
                Trigger();
            }
        }
    }
}

