using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Social
{
    [Serializable]
    public class GroupRidePlan
    {
        public string rideId;
        public string rideTitle;
        public string destinationName;
        public Vector3 meetingPoint;
        public string scheduledTimeUtc;
        public List<string> invitedPlayerIds = new List<string>();
        public List<string> confirmedPlayerIds = new List<string>();

        public GroupRidePlan() { }

        public GroupRidePlan(string id, string title, string dest, Vector3 meet)
        {
            rideId = id;
            rideTitle = title;
            destinationName = dest;
            meetingPoint = meet;
            scheduledTimeUtc = DateTime.UtcNow.AddMinutes(30).ToString("o");
        }
    }

    /// <summary>
    /// Schedules and coordinates club group rides and destination road trips.
    /// </summary>
    public class GroupRideManager
    {
        private const string Tag = "GroupRideManager";

        private readonly List<GroupRidePlan> _activeRides = new List<GroupRidePlan>();

        public IReadOnlyList<GroupRidePlan> ActiveRides => _activeRides;

        public GroupRidePlan CreateRidePlan(string title, string destination, Vector3 meetingPoint, string organizerId)
        {
            string id = "ride_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var plan = new GroupRidePlan(id, title, destination, meetingPoint);
            plan.confirmedPlayerIds.Add(organizerId);

            _activeRides.Add(plan);
            GameLogger.I(Tag, $"Created group ride plan '{title}' towards {destination}");
            return plan;
        }

        public bool ConfirmAttendance(string rideId, string playerId)
        {
            var plan = _activeRides.Find(r => r.rideId == rideId);
            if (plan != null && !plan.confirmedPlayerIds.Contains(playerId))
            {
                plan.confirmedPlayerIds.Add(playerId);
                GameLogger.I(Tag, $"Player {playerId} confirmed attendance for ride {plan.rideTitle}");
                return true;
            }
            return false;
        }
    }
}

