using System;

namespace IndianBikeLife.Social
{
    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    [Serializable]
    public class FriendEntry
    {
        public string playerId;
        public string username;
        public int playerLevel;
        public bool isOnline;
        public string currentActivity = "In Main Menu";
        public string friendshipDateUtc;

        public FriendEntry() { }

        public FriendEntry(string pid, string name, int lvl, bool online = true)
        {
            playerId = pid;
            username = name;
            playerLevel = lvl;
            isOnline = online;
            friendshipDateUtc = DateTime.UtcNow.ToString("o");
        }
    }

    [Serializable]
    public class FriendRequest
    {
        public string requestId;
        public string fromPlayerId;
        public string fromPlayerName;
        public string timestampUtc;
        public string message;
        public FriendRequestStatus status;

        public FriendRequest() { }

        public FriendRequest(string reqId, string fromId, string fromName)
        {
            requestId = reqId;
            fromPlayerId = fromId;
            fromPlayerName = fromName;
            message = "";
            timestampUtc = DateTime.UtcNow.ToString("o");
            status = FriendRequestStatus.Pending;
        }

        public FriendRequest(string reqId, string fromId, string fromName, string msg)
        {
            requestId = reqId;
            fromPlayerId = fromId;
            fromPlayerName = fromName;
            message = msg;
            timestampUtc = DateTime.UtcNow.ToString("o");
            status = FriendRequestStatus.Pending;
        }
    }
}

