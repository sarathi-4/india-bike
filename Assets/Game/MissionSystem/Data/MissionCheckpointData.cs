using System;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    /// <summary>
    /// Position and trigger parameters for an intermediate mission checkpoint.
    /// </summary>
    [Serializable]
    public class MissionCheckpointData
    {
        public int checkpointIndex;
        public Vector3 position;
        public float triggerRadius = 8.0f;
        public string instructionText = "Reach Checkpoint";

        public MissionCheckpointData() { }

        public MissionCheckpointData(int index, Vector3 pos, float radius = 8.0f, string instruction = "Pass through checkpoint")
        {
            checkpointIndex = index;
            position = pos;
            triggerRadius = radius;
            instructionText = instruction;
        }
    }
}

