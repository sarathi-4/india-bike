using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.MapSystem
{
    public enum IndianZone
    {
        South = 0,
        North = 1,
        West = 2,
        East = 3,
        Central = 4,
        NorthEast = 5,
        UnionTerritory = 6
    }

    /// <summary>
    /// Represents an Indian State or Union Territory.
    /// </summary>
    [CreateAssetMenu(fileName = "NewState", menuName = "IndianBikeLife/Maps/State Data")]
    public class StateData : ScriptableObject
    {
        [Header("State Info")]
        [SerializeField] private string _stateId;
        [SerializeField] private string _stateName;
        [SerializeField] private IndianZone _zone = IndianZone.South;
        [SerializeField] private string _capitalName;
        [SerializeField] private bool _isPriorityStage2State = false;

        [Header("Districts")]
        [SerializeField] private List<DistrictData> _districts = new List<DistrictData>();

        public string StateId => _stateId;
        public string StateName => _stateName;
        public IndianZone Zone => _zone;
        public string CapitalName => _capitalName;
        public bool IsPriorityStage2State => _isPriorityStage2State;
        public IReadOnlyList<DistrictData> Districts => _districts;
    }
}

