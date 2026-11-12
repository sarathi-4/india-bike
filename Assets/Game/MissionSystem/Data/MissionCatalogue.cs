using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    /// <summary>
    /// Master registry of all missions across the 15 categories.
    /// </summary>
    [CreateAssetMenu(fileName = "MissionCatalogue", menuName = "Indian Bike Life/Missions/Mission Catalogue")]
    public class MissionCatalogue : ScriptableObject
    {
        [SerializeField] private List<MissionData> _missions = new List<MissionData>();

        public IReadOnlyList<MissionData> Missions => _missions;

        public MissionData FindById(string missionId)
        {
            if (string.IsNullOrEmpty(missionId)) return null;
            return _missions.Find(m => m != null && m.MissionId == missionId);
        }

        public List<MissionData> GetMissionsByCategory(MissionCategory category)
        {
            var results = new List<MissionData>();
            for (int i = 0; i < _missions.Count; i++)
            {
                if (_missions[i] != null && _missions[i].Category == category)
                {
                    results.Add(_missions[i]);
                }
            }
            return results;
        }

        public List<MissionData> GetMissionsForLevel(int playerLevel)
        {
            var results = new List<MissionData>();
            for (int i = 0; i < _missions.Count; i++)
            {
                if (_missions[i] != null && _missions[i].RequiredPlayerLevel <= playerLevel)
                {
                    results.Add(_missions[i]);
                }
            }
            return results;
        }
    }
}

