using System.Collections.Generic;
using IndianBikeLife.Progression.Leaderboards;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for leaderboard score caching, rank querying, and local high-score retention.
    /// Prepares for Stage 4 backend synchronization without mocking fake online players.
    /// </summary>
    public interface ILeaderboardManager : IGameService
    {
        void SubmitScore(LeaderboardType type, string entryKey, long score, string metadata = "");
        IReadOnlyList<LeaderboardEntry> GetLeaderboard(LeaderboardType type, int maxEntries = 10);
        LeaderboardEntry GetPlayerEntry(LeaderboardType type);
    }
}
