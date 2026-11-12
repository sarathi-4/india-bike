using System;
using IndianBikeLife.Progression.Rewards;
using UnityEngine;

namespace IndianBikeLife.Progression.Achievements
{
    public enum AchievementCategory
    {
        FirstRide,
        DistanceMilestone,
        MissionCompletion,
        HillStationExploration,
        PassengerRides,
        MotorcycleCollection,
        RidingTimeMilestone,
        ExplorationMilestone
    }

    /// <summary>
    /// Data asset defining an unlockable player achievement, milestone target, and reward.
    /// </summary>
    [CreateAssetMenu(fileName = "AchievementData", menuName = "Indian Bike Life/Progression/Achievement Data")]
    public class AchievementData : ScriptableObject
    {
        [SerializeField] private string _achievementId = "";
        [SerializeField] private string _title = "";
        [TextArea(2, 4)]
        [SerializeField] private string _description = "";
        [SerializeField] private AchievementCategory _category;
        [SerializeField] private float _targetProgress = 1.0f;
        [SerializeField] private RewardData _reward;
        [SerializeField] private bool _isSecret = false;

        public string AchievementId => string.IsNullOrEmpty(_achievementId) ? name : _achievementId;
        public string Title => _title;
        public string Description => _description;
        public AchievementCategory Category => _category;
        public float TargetProgress => _targetProgress;
        public RewardData Reward => _reward;
        public bool IsSecret => _isSecret;

        public static AchievementData CreateRuntime(string id, string tit, string desc, AchievementCategory cat, float target, RewardData rew = null)
        {
            var ach = CreateInstance<AchievementData>();
            ach._achievementId = id;
            ach._title = tit;
            ach._description = desc;
            ach._category = cat;
            ach._targetProgress = Mathf.Max(1f, target);
            ach._reward = rew;
            return ach;
        }
    }
}
