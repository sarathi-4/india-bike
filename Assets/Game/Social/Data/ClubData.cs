using System;
using System.Collections.Generic;

namespace IndianBikeLife.Social
{
    public enum ClubMemberRole
    {
        Member,
        Officer,
        Leader
    }

    [Serializable]
    public class ClubMember
    {
        public string playerId;
        public string username;
        public ClubMemberRole role;
        public string joinedDateUtc;

        public ClubMember() { }

        public ClubMember(string pid, string name, ClubMemberRole memberRole)
        {
            playerId = pid;
            username = name;
            role = memberRole;
            joinedDateUtc = DateTime.UtcNow.ToString("o");
        }
    }

    /// <summary>
    /// Represents an organized motorcycle club or riding brotherhood.
    /// </summary>
    [Serializable]
    public class ClubData
    {
        public string clubId;
        public string clubName;
        public string clubTag; // e.g. [ROYAL], [GHAT]
        public string description;
        public string leaderPlayerId;
        public int clubLevel = 1;
        public float totalClubDistanceKm = 0f;
        public List<ClubMember> members = new List<ClubMember>();

        public ClubData() { }

        public ClubData(string id, string name, string tag, string desc, string leaderId, string leaderName)
        {
            clubId = id;
            clubName = name;
            clubTag = tag;
            description = desc;
            leaderPlayerId = leaderId;
            clubLevel = 1;
            totalClubDistanceKm = 0f;
            members.Add(new ClubMember(leaderId, leaderName, ClubMemberRole.Leader));
        }

        public bool IsMember(string playerId) => members.Exists(m => m.playerId == playerId);
    }
}
