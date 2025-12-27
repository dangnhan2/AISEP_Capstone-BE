namespace AISEP.Domain.Enums;

/// <summary>
/// Status of consultation request from startup to advisor
/// </summary>
public enum ConsultationRequestStatus
{
    Pending,
    Accepted,
    Declined,
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}
