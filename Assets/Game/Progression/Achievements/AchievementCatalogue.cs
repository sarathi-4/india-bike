using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.Progression.Achievements
{
    /// <summary>
    /// Master catalogue of all achievements available across Indian Bike Life.
    /// </summary>
    [CreateAssetMenu(fileName = "AchievementCatalogue", menuName = "Indian Bike Life/Progression/Achievement Catalogue")]
    public class AchievementCatalogue : ScriptableObject
    {
        [SerializeField] private List<AchievementData> _achievements = new List<AchievementData>();

        public IReadOnlyList<AchievementData> Achievements => _achievements;

        public AchievementData FindById(string achievementId)
        {
            if (string.IsNullOrEmpty(achievementId)) return null;
            return _achievements.Find(a => a != null && a.AchievementId == achievementId);
        }
    }
}

