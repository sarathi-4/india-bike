using System;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    public enum MissionObjectiveType
    {
        ReachDestination,
        HitCheckpoints,
        MaintainSpeed,
        AvoidCollisions,
        TransportPassenger,
        DeliveryDropoff,
        FuelEconomy,
        ExplorePOIs
    }

    public enum PassengerRequirement
    {
        None,
        Optional,
        Required,
        CoupleRiding
    }

    public enum MotorcycleCategoryRequirement
    {
        Any,
        Commuter,
        Cruiser,
        Sport,
        Adventure,
        Scooter
    }

    /// <summary>
    /// Represents a discrete goal within a mission with individual completion tracking.
    /// Supports both required and optional (bonus reward) objectives.
    /// </summary>
    [Serializable]
    public class MissionObjective
    {
        public string objectiveId;
        public MissionObjectiveType type;
        public string description;
        public float targetValue = 1.0f;
        public float currentProgress = 0f;
        public bool isCompleted = false;
        public bool isOptional = false;

        public MissionObjective() { }

        public MissionObjective(string id, MissionObjectiveType objType, string desc, float target, bool optional = false)
        {
            objectiveId = id;
            type = objType;
            description = desc;
            targetValue = Mathf.Max(1f, target);
            currentProgress = 0f;
            isCompleted = false;
            isOptional = optional;
        }

        public void AddProgress(float delta)
        {
            if (isCompleted || delta <= 0f) return;

            currentProgress = Mathf.Min(targetValue, currentProgress + delta);
            if (currentProgress >= targetValue)
            {
                isCompleted = true;
            }
        }

        public void SetProgress(float val)
        {
            currentProgress = Mathf.Clamp(val, 0f, targetValue);
            isCompleted = currentProgress >= targetValue;
        }

        public void ResetProgress()
        {
            currentProgress = 0f;
            isCompleted = false;
        }

        public float NormalizedProgress => targetValue > 0f ? Mathf.Clamp01(currentProgress / targetValue) : 0f;
    }
}
