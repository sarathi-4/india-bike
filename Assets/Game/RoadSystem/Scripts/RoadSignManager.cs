using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    public enum IndianRoadSignType
    {
        SpeedLimit40 = 0,
        SpeedLimit60 = 1,
        SpeedLimit80 = 2,
        SpeedLimit100 = 3,
        SpeedBreakerAhead = 4,
        HairpinBendLeft = 5,
        HairpinBendRight = 6,
        SteepAscent = 7,
        SteepDescent = 8,
        NarrowBridgeAhead = 9,
        PedestrianCrossing = 10,
        SchoolAhead = 11,
        TollPlazaAhead = 12
    }

    /// <summary>
    /// Spawns and manages authentic Indian road signposts, speed limit warnings, and caution boards.
    /// </summary>
    public class RoadSignManager : MonoBehaviour
    {
        [System.Serializable]
        public class RoadSignEntry
        {
            public IndianRoadSignType signType;
            public Vector3 position;
            public Quaternion rotation;
            public GameObject spawnedInstance;
        }

        [SerializeField] private List<RoadSignEntry> _signs = new List<RoadSignEntry>();

        public IReadOnlyList<RoadSignEntry> Signs => _signs;

        public void AddSign(IndianRoadSignType type, Vector3 position, Quaternion rotation)
        {
            _signs.Add(new RoadSignEntry
            {
                signType = type,
                position = position,
                rotation = rotation
            });
        }
    }
}

