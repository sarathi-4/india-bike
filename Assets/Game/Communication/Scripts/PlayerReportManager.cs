using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Communication
{
    [Serializable]
    public class PlayerReportTicket
    {
        public string ticketId;
        public string reportingPlayerId;
        public string reportedPlayerId;
        public string reason;
        public string details;
        public string timestampUtc;

        public PlayerReportTicket() { }

        public PlayerReportTicket(string reporter, string target, string reportReason, string reportDetails)
        {
            ticketId = "rpt_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            reportingPlayerId = reporter;
            reportedPlayerId = target;
            reason = reportReason;
            details = reportDetails;
            timestampUtc = DateTime.UtcNow.ToString("o");
        }
    }

    /// <summary>
    /// Processes and audits moderation report tickets submitted by players.
    /// </summary>
    public class PlayerReportManager
    {
        private const string Tag = "PlayerReportManager";

        private readonly List<PlayerReportTicket> _reports = new List<PlayerReportTicket>();

        public IReadOnlyList<PlayerReportTicket> Reports => _reports;

        public PlayerReportTicket SubmitReport(string reporterId, string targetId, string reason, string details)
        {
            var ticket = new PlayerReportTicket(reporterId, targetId, reason, details);
            _reports.Add(ticket);
            GameLogger.W(Tag, $"Moderation report lodged against player {targetId} for '{reason}': {details}");
            return ticket;
        }
    }
}
