using System;

namespace IndianBikeLife.Progression.Leaderboards
{
    /// <summary>
    /// Represents a single ranked record in a local or online leaderboard.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public int rank;
        public string playerId;
        public string playerName;
        public long score;
        public string formattedScore;
        public string timestampUtc;
        public string metadata;

        public LeaderboardEntry() { }

        public LeaderboardEntry(int rankNum, string pid, string name, long rawScore, string formatted, string meta = "")
        {
            rank = rankNum;
            playerId = pid;
            playerName = name;
            score = rawScore;
            formattedScore = formatted;
            timestampUtc = DateTime.UtcNow.ToString("o");
            metadata = meta;
        }
    }
}

