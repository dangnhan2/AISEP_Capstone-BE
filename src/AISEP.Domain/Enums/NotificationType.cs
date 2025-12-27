namespace AISEP.Domain.Enums;

/// <summary>
/// Types of notifications that can be sent to users
/// </summary>
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    ConnectionRequest,
    ConnectionAccepted,
    ConnectionDeclined,
    InvestmentProposal,
    DocumentApproved,
    DocumentRejected,
    ScoreUpdated,
    MentorshipRequest,
    MentorshipAccepted,
    SessionScheduled,
    SessionReminder,
    MessageReceived,
    SystemAlert
}
