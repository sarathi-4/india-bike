using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    public enum IntersectionType
    {
        TJunction = 0,
        Crossroads = 1,
        Roundabout = 2,
        Fork = 3
    }

    /// <summary>
    /// Represents an Indian road junction or roundabout coordinating traffic branching.
    /// </summary>
    public class RoadIntersection : MonoBehaviour
    {
        [Header("Intersection Config")]
        [SerializeField] private IntersectionType _type = IntersectionType.TJunction;
        [SerializeField] private float _junctionRadius = 12f;
        [SerializeField] private List<RoadSpline> _incomingRoads = new List<RoadSpline>();

        [Header("Traffic Control")]
        [SerializeField] private bool _hasTrafficSignal = false;
        [SerializeField] private float _greenLightDuration = 15f;

        public IntersectionType Type => _type;
        public float JunctionRadius => _junctionRadius;
        public IReadOnlyList<RoadSpline> IncomingRoads => _incomingRoads;
        public bool HasTrafficSignal => _hasTrafficSignal;

        public void RegisterIncomingRoad(RoadSpline road)
        {
            if (road != null && !_incomingRoads.Contains(road))
            {
                _incomingRoads.Add(road);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _hasTrafficSignal ? Color.yellow : Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _junctionRadius);
        }
    }
}
