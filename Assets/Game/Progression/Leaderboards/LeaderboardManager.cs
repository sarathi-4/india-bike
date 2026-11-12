using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Progression.Leaderboards
{
    /// <summary>
    /// Local leaderboard management service implementing ILeaderboardManager and IGameService.
    /// Provides local offline high-score rankings and an explicit contract for Stage 4 cloud integration.
    /// Does NOT simulate fake online bot players.
    /// </summary>
    public class LeaderboardManager : MonoBehaviour, ILeaderboardManager
    {
        private const string Tag = "LeaderboardManager";

        private readonly Dictionary<LeaderboardType, List<LeaderboardEntry>> _boardEntries 
            = new Dictionary<LeaderboardType, List<LeaderboardEntry>>();

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            // Initialize collections for all types
            foreach (LeaderboardType type in Enum.GetValues(typeof(LeaderboardType)))
            {
                _boardEntries[type] = new List<LeaderboardEntry>();
            }

            ServiceLocator.Register<ILeaderboardManager>(this);
            _isInitialized = true;
            GameLogger.I(Tag, "LeaderboardManager initialized (local ranking cache ready for Stage 4 backend sync).");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;
            ServiceLocator.Unregister<ILeaderboardManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public void SubmitScore(LeaderboardType type, string entryKey, long score, string metadata = "")
        {
            if (!_boardEntries.ContainsKey(type))
            {
                _boardEntries[type] = new List<LeaderboardEntry>();
            }

            var entries = _boardEntries[type];

            // For time trial: lower score (seconds/ms) is better. For distance/XP: higher is better.
            bool lowerIsBetter = type == LeaderboardType.MissionTimeTrial;

            string formatted = lowerIsBetter ? $"{score / 1000f:0.00}s" : score.ToString("N0");

            // Look for existing entry from this player
            var existing = entries.Find(e => e.playerId == entryKey);
            if (existing != null)
            {
                bool isImprovement = lowerIsBetter ? score < existing.score : score > existing.score;
                if (isImprovement)
                {
                    existing.score = score;
                    existing.formattedScore = formatted;
                    existing.metadata = metadata;
                    existing.timestampUtc = DateTime.UtcNow.ToString("o");
                }
            }
            else
            {
                entries.Add(new LeaderboardEntry(0, entryKey, "Local Rider", score, formatted, metadata));
            }

            // Sort and recalculate ranks
            entries.Sort((a, b) => lowerIsBetter ? a.score.CompareTo(b.score) : b.score.CompareTo(a.score));

            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].rank = i + 1;
            }

            GameLogger.I(Tag, $"Score submitted to {type}: {score} ({formatted}). Local Rank: {GetPlayerRank(type, entryKey)}");
        }

        public IReadOnlyList<LeaderboardEntry> GetLeaderboard(LeaderboardType type, int maxEntries = 10)
        {
            if (!_boardEntries.ContainsKey(type)) return Array.Empty<LeaderboardEntry>();

            var list = _boardEntries[type];
            int count = Mathf.Min(list.Count, maxEntries);
            return list.GetRange(0, count);
        }

        public LeaderboardEntry GetPlayerEntry(LeaderboardType type)
        {
            if (!_boardEntries.ContainsKey(type)) return null;
            return _boardEntries[type].Find(e => e.playerId == "local_player" || e.playerId == "player_profile");
        }

        private int GetPlayerRank(LeaderboardType type, string playerId)
        {
            if (!_boardEntries.ContainsKey(type)) return -1;
            var entry = _boardEntries[type].Find(e => e.playerId == playerId);
            return entry != null ? entry.rank : -1;
        }
    }
}

