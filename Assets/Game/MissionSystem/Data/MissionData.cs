using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Progression.Rewards;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    [Serializable]
    public class MissionFailureConfig
    {
        public bool failOnTimeExpired = true;
        public bool failOnOffRoute = true;
        public bool failOnPassengerDismount = false;
        public bool failOnExcessiveDamage = true;
        public float maxAllowedDamagePercent = 100f;
        public float offRouteTimeoutSeconds = 15.0f;
    }

    /// <summary>
    /// Master ScriptableObject defining an individual mission profile, route, objectives, and rewards.
    /// </summary>
    [CreateAssetMenu(fileName = "MissionData", menuName = "Indian Bike Life/Missions/Mission Data")]
    public class MissionData : ScriptableObject
    {
        [Header("Metadata")]
        [SerializeField] private string _missionId = "";
        [SerializeField] private string _title = "";
        [TextArea(2, 4)]
        [SerializeField] private string _description = "";
        [SerializeField] private MissionCategory _category = MissionCategory.Highway;
        [SerializeField] private MissionDifficulty _baseDifficulty = MissionDifficulty.Normal;
        [SerializeField] private int _requiredPlayerLevel = 1;

        [Header("Locations & Route")]
        [SerializeField] private string _startLocationName = "Start Location";
        [SerializeField] private Vector3 _startPosition = Vector3.zero;
        [SerializeField] private string _destinationName = "Destination";
        [SerializeField] private Vector3 _destinationPosition = new Vector3(0, 0, 500);

        [Header("Vehicle & Passenger Rules")]
        [SerializeField] private MotorcycleCategoryRequirement _requiredBikeCategory = MotorcycleCategoryRequirement.Any;
        [SerializeField] private PassengerRequirement _passengerRequirement = PassengerRequirement.None;

        [Header("Rules & Limits")]
        [SerializeField] private float _timeLimitSeconds = 120.0f; // <=0 for untimed
        [SerializeField] private MissionFailureConfig _failureConfig = new MissionFailureConfig();

        [Header("Objectives & Checkpoints")]
        [SerializeField] private List<MissionObjective> _objectives = new List<MissionObjective>();
        [SerializeField] private List<MissionCheckpointData> _checkpoints = new List<MissionCheckpointData>();

        [Header("Rewards")]
        [SerializeField] private RewardData _reward;

        // Runtime status
        [NonSerialized] private MissionStatus _status = MissionStatus.NotStarted;
        [NonSerialized] private float _runtimeElapsedTime = 0f;

        public string MissionId => string.IsNullOrEmpty(_missionId) ? name : _missionId;
        public string Title => _title;
        public string Description => _description;
        public MissionCategory Category => _category;
        public MissionDifficulty BaseDifficulty => _baseDifficulty;
        public int RequiredPlayerLevel => _requiredPlayerLevel;
        public string StartLocationName => _startLocationName;
        public Vector3 StartPosition => _startPosition;
        public string DestinationName => _destinationName;
        public Vector3 DestinationPosition => _destinationPosition;
        public MotorcycleCategoryRequirement RequiredBikeCategory => _requiredBikeCategory;
        public PassengerRequirement PassengerRule => _passengerRequirement;
        public float TimeLimitSeconds => _timeLimitSeconds;
        public MissionFailureConfig FailureConfig => _failureConfig;
        public IReadOnlyList<MissionObjective> Objectives => _objectives;
        public IReadOnlyList<MissionCheckpointData> Checkpoints => _checkpoints;
        public RewardData Reward => _reward;

        public MissionStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public float RuntimeElapsedTime
        {
            get => _runtimeElapsedTime;
            set => _runtimeElapsedTime = value;
        }

        /// <summary>
        /// Creates a deep runtime instance so objective progression doesn't modify the source asset.
        /// </summary>
        public MissionData CloneRuntimeInstance()
        {
            var clone = Instantiate(this);
            clone.name = name;
            clone._status = MissionStatus.NotStarted;
            clone._runtimeElapsedTime = 0f;

            // Deep copy objectives
            clone._objectives = new List<MissionObjective>();
            for (int i = 0; i < _objectives.Count; i++)
            {
                var orig = _objectives[i];
                clone._objectives.Add(new MissionObjective(orig.objectiveId, orig.type, orig.description, orig.targetValue, orig.isOptional));
            }

            // Deep copy checkpoints
            clone._checkpoints = new List<MissionCheckpointData>();
            for (int i = 0; i < _checkpoints.Count; i++)
            {
                var origCp = _checkpoints[i];
                clone._checkpoints.Add(new MissionCheckpointData(origCp.checkpointIndex, origCp.position, origCp.triggerRadius, origCp.instructionText));
            }

            return clone;
        }

        public bool AreAllRequiredObjectivesComplete()
        {
            if (_objectives == null || _objectives.Count == 0) return true;

            for (int i = 0; i < _objectives.Count; i++)
            {
                if (!_objectives[i].isOptional && !_objectives[i].isCompleted)
                {
                    return false;
                }
            }
            return true;
        }

        public int CompletedObjectiveCount
        {
            get
            {
                int c = 0;
                for (int i = 0; i < _objectives.Count; i++)
                {
                    if (_objectives[i].isCompleted) c++;
                }
                return c;
            }
        }
    }
}

