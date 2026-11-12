using System.Collections.Generic;

namespace IndianBikeLife.Social
{
    /// <summary>
    /// Manages the local player's social identity and caches queried remote profiles
    /// while respecting player privacy boundaries.
    /// </summary>
    public class SocialProfileManager
    {
        private SocialProfileData _localProfile;
        private readonly Dictionary<string, SocialProfileData> _profileCache = new Dictionary<string, SocialProfileData>();

        public SocialProfileData LocalProfile => _localProfile;

        public void InitializeLocalProfile(string playerId, string playerName)
        {
            _localProfile = new SocialProfileData(playerId, playerName);
        }

        public void UpdateLocalProfile(SocialProfileData profile)
        {
            _localProfile = profile;
            CacheRemoteProfile(profile);
        }

        public SocialProfileData GetSanitizedProfile(string playerId, bool isFriend)
        {
            var profile = GetProfile(playerId);
            if (profile == null) return null;

            if (profile.privacy == ProfilePrivacy.Private || (!isFriend && profile.privacy == ProfilePrivacy.FriendsOnly))
            {
                return new SocialProfileData(profile.playerId, profile.username)
                {
                    bio = "Hidden by privacy settings.",
                    level = 0,
                    distanceRiddenMeters = 0
                };
            }

            return profile;
        }

        public void UpdateProfile(string displayName, string avatarId, string bio, ProfilePrivacy privacy)
        {
            if (_localProfile == null) return;

            if (!string.IsNullOrEmpty(displayName)) _localProfile.username = displayName;
            if (!string.IsNullOrEmpty(avatarId)) _localProfile.avatarId = avatarId;
            if (bio != null) _localProfile.bio = bio;
            _localProfile.privacy = privacy;
        }

        public SocialProfileData GetProfile(string playerId)
        {
            if (_localProfile != null && _localProfile.playerId == playerId)
            {
                return _localProfile;
            }

            if (_profileCache.TryGetValue(playerId, out var profile))
            {
                // If profile is private, sanitize stats
                if (profile.privacy == ProfilePrivacy.Private)
                {
                    return new SocialProfileData(profile.playerId, profile.username)
                    {
                        bio = "Profile is set to Private.",
                        privacy = ProfilePrivacy.Private
                    };
                }
                return profile;
            }

            return null;
        }

        public void CacheRemoteProfile(SocialProfileData profile)
        {
            if (profile != null && !string.IsNullOrEmpty(profile.playerId))
            {
                _profileCache[profile.playerId] = profile;
            }
        }
    }
}

