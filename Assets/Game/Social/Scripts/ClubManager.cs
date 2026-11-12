using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Social
{
    /// <summary>
    /// Coordinates motorcycle clubs, member permissions, and collective riding stats.
    /// </summary>
    public class ClubManager
    {
        private const string Tag = "ClubManager";

        private ClubData _myClub;
        private readonly Dictionary<string, ClubData> _allClubs = new Dictionary<string, ClubData>();

        public ClubData MyClub => _myClub;
        public bool HasClub => _myClub != null;

        public ClubData CreateClub(string clubName, string description, string tag, string leaderId, string leaderName)
        {
            string id = "club_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var club = new ClubData(id, clubName, tag, description, leaderId, leaderName);

            _allClubs[id] = club;
            _myClub = club;

            GameLogger.I(Tag, $"Created club '{clubName}' [{tag}] with leader {leaderName}");
            return club;
        }

        public bool JoinClub(string clubId, string playerId, string playerName)
        {
            if (_allClubs.TryGetValue(clubId, out var club))
            {
                if (!club.IsMember(playerId))
                {
                    club.members.Add(new ClubMember(playerId, playerName, ClubMemberRole.Member));
                    _myClub = club;
                    GameLogger.I(Tag, $"Player {playerName} joined club {club.clubName}");
                    return true;
                }
            }
            return false;
        }

        public bool LeaveClub(string playerId)
        {
            if (_myClub == null) return false;

            var member = _myClub.members.Find(m => m.playerId == playerId);
            if (member != null)
            {
                _myClub.members.Remove(member);
                GameLogger.I(Tag, $"Player {member.username} left club {_myClub.clubName}");

                if (_myClub.members.Count == 0)
                {
                    _allClubs.Remove(_myClub.clubId);
                }
                _myClub = null;
                return true;
            }
            return false;
        }
    }
}
