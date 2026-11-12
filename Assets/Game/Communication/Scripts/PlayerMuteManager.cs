using System.Collections.Generic;

namespace IndianBikeLife.Communication
{
    /// <summary>
    /// Manages player mute states to prevent harassment and filter unwanted incoming messages.
    /// </summary>
    public class PlayerMuteManager
    {
        private readonly HashSet<string> _mutedPlayerIds = new HashSet<string>();

        public IReadOnlyCollection<string> MutedPlayerIds => _mutedPlayerIds;

        public bool Mute(string playerId)
        {
            if (string.IsNullOrEmpty(playerId)) return false;
            return _mutedPlayerIds.Add(playerId);
        }

        public bool Unmute(string playerId)
        {
            return _mutedPlayerIds.Remove(playerId);
        }

        public bool IsMuted(string playerId)
        {
            return _mutedPlayerIds.Contains(playerId);
        }
    }
}

