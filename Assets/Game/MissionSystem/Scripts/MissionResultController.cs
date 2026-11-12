using System;
using IndianBikeLife.Progression.Rewards;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    [Serializable]
    public class MissionResultSummary
    {
        public string missionId;
        public string missionTitle;
        public bool isSuccess;
        public int starRating; // 1 to 3
        public float timeTakenSeconds;
        public float distanceCoveredMeters;
        public float motorcycleDamagePercent;
        public int earnedSilver;
        public int earnedGold;
        public int earnedXP;
        public string failureReason;

        public MissionResultSummary() { }
    }

    /// <summary>
    /// Evaluates player performance at mission completion, determines star ratings (1-3 stars),
    /// scales financial/XP payouts based on difficulty, and compiles result summaries.
    /// </summary>
    public static class MissionResultController
    {
        public static MissionResultSummary CalculateSuccessResult(
            MissionData mission,
            MissionDifficulty difficulty,
            float timeTaken,
            float distanceMeters,
            float damagePercent)
        {
            var summary = new MissionResultSummary
            {
                missionId = mission.MissionId,
                missionTitle = mission.Title,
                isSuccess = true,
                timeTakenSeconds = timeTaken,
                distanceCoveredMeters = distanceMeters,
                motorcycleDamagePercent = damagePercent,
                failureReason = string.Empty
            };

            // Calculate stars (1-3)
            int stars = 1;

            // Star 2: Fast completion (under 80% of time limit) or low damage
            bool fastCompletion = mission.TimeLimitSeconds > 0 && timeTaken < (mission.TimeLimitSeconds * 0.8f);
            bool lowDamage = damagePercent <= 15f;
            if (fastCompletion || lowDamage) stars = 2;

            // Star 3: All optional objectives complete AND pristine condition (damage <= 5%)
            bool allObjectivesDone = mission.CompletedObjectiveCount == mission.Objectives.Count;
            if (allObjectivesDone && damagePercent <= 5f) stars = 3;

            summary.starRating = stars;

            // Calculate reward scaling
            var diffSettings = MissionDifficultySettings.GetDefaultSettings(difficulty);
            float diffMult = diffSettings.rewardMultiplier;
            float starMult = 1.0f + (stars - 1) * 0.25f; // 1 star = 1.0x, 2 stars = 1.25x, 3 stars = 1.5x

            if (mission.Reward != null)
            {
                summary.earnedSilver = Mathf.RoundToInt(mission.Reward.SilverCoins * diffMult * starMult);
                summary.earnedGold = Mathf.RoundToInt(mission.Reward.GoldCoins * diffMult);
                summary.earnedXP = Mathf.RoundToInt(mission.Reward.XP * diffMult * starMult);
            }

            return summary;
        }

        public static MissionResultSummary CalculateFailureResult(
            MissionData mission,
            float timeTaken,
            float distanceMeters,
            float damagePercent,
            string failureReason)
        {
            return new MissionResultSummary
            {
                missionId = mission.MissionId,
                missionTitle = mission.Title,
                isSuccess = false,
                starRating = 0,
                timeTakenSeconds = timeTaken,
                distanceCoveredMeters = distanceMeters,
                motorcycleDamagePercent = damagePercent,
                earnedSilver = 0,
                earnedGold = 0,
                earnedXP = 0,
                failureReason = failureReason
            };
        }
    }
}

