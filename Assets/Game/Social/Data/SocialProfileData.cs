using System;

namespace IndianBikeLife.Social
{
    public enum ProfilePrivacy
    {
        Public,
        FriendsOnly,
        Private
    }

    public enum ProfilePrivacyLevel
    {
        Public,
        FriendsOnly,
        Private
    }

    /// <summary>
    /// Player social profile containing level, motorcycle ownership, stats, and privacy settings.
    /// </summary>
    [Serializable]
    public class SocialProfileData
    {
        public string playerId;
        public string username = "Indian Rider";
        public string avatarId = "avatar_classic_male";
        public int playerLevel = 1;
        public int currentXP = 0;
        public string selectedMotorcycleId = "bike_classic_350";
        public float totalDistanceDrivenKm = 0f;
        public int completedMissionsCount = 0;
        public int achievementsUnlockedCount = 0;
        public string favouriteDestination = "Ooty Hill Station";
        public string bio = "Cruising Indian highways one kilometer at a time.";
        public bool isOnline = true;
        public ProfilePrivacy privacy = ProfilePrivacy.Public;

        public ProfilePrivacyLevel privacyLevel
        {
            get => (ProfilePrivacyLevel)privacy;
            set => privacy = (ProfilePrivacy)value;
        }

        public int level
        {
            get => playerLevel;
            set => playerLevel = value;
        }

        public float distanceRiddenMeters
        {
            get => totalDistanceDrivenKm * 1000f;
            set => totalDistanceDrivenKm = value / 1000f;
        }

        public SocialProfileData() { }

        public SocialProfileData(string pid, string name)
        {
            playerId = pid;
            username = name;
        }

        public SocialProfileData(string pid, string name, string motorcycle, int lvl, float distMeters)
        {
            playerId = pid;
            username = name;
            selectedMotorcycleId = motorcycle;
            playerLevel = lvl;
            totalDistanceDrivenKm = distMeters / 1000f;
        }
    }
}

