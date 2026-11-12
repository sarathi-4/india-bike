using System;
using System.Collections.Generic;

namespace IndianBikeLife.Social
{
    /// <summary>
    /// Evaluates and manages pending friend requests, preventing spam and duplicates.
    /// </summary>
    public class FriendRequestController
    {
        private readonly List<FriendRequest> _incomingRequests = new List<FriendRequest>();
        private readonly HashSet<string> _blockedPlayerIds = new HashSet<string>();

        public IReadOnlyList<FriendRequest> IncomingRequests => _incomingRequests;
        public IReadOnlyCollection<string> BlockedPlayerIds => _blockedPlayerIds;

        public bool CanSendRequest(string targetPlayerId, IReadOnlyList<FriendEntry> existingFriends)
        {
            if (string.IsNullOrEmpty(targetPlayerId)) return false;
            if (_blockedPlayerIds.Contains(targetPlayerId)) return false;

            // Check if already friends
            if (existingFriends != null)
            {
                for (int i = 0; i < existingFriends.Count; i++)
                {
                    if (existingFriends[i].playerId == targetPlayerId) return false;
                }
            }

            // Check if request already pending
            return !_incomingRequests.Exists(r => r.fromPlayerId == targetPlayerId);
        }

        public void AddIncomingRequest(FriendRequest req)
        {
            if (req == null || _blockedPlayerIds.Contains(req.fromPlayerId)) return;
            if (!_incomingRequests.Exists(r => r.requestId == req.requestId))
            {
                _incomingRequests.Add(req);
            }
        }

        public FriendRequest ResolveRequest(string requestId, bool accept)
        {
            var req = _incomingRequests.Find(r => r.requestId == requestId);
            if (req != null)
            {
                req.status = accept ? FriendRequestStatus.Accepted : FriendRequestStatus.Rejected;
                _incomingRequests.Remove(req);
                return req;
            }
            return null;
        }

        public bool BlockPlayer(string playerId)
        {
            if (string.IsNullOrEmpty(playerId)) return false;

            // Remove any pending requests
            _incomingRequests.RemoveAll(r => r.fromPlayerId == playerId);
            return _blockedPlayerIds.Add(playerId);
        }

        public bool UnblockPlayer(string playerId)
        {
            return _blockedPlayerIds.Remove(playerId);
        }

        public bool IsBlocked(string playerId)
        {
            return _blockedPlayerIds.Contains(playerId);
        }
    }
}

