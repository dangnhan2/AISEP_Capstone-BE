using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Consultation request entity for startups requesting mentorship from advisors
/// Represents the initial request phase of the consultation workflow
/// </summary>
public class ConsultationRequest : BaseEntity
{
    public int StartupProfileId { get; private set; }
    public int AdvisorProfileId { get; private set; }
    public ConsultationRequestStatus Status { get; private set; }
    public string Topic { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ExpectedOutcome { get; private set; }
    public string? PreferredSchedule { get; private set; }
    public DateTime? RequestedDate { get; private set; }
    public DateTime? ScheduledDate { get; private set; }
    public int EstimatedDurationMinutes { get; private set; } = 60;
    public string? MeetingLink { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? DeclinedAt { get; private set; }
    public string? DeclineReason { get; private set; }
    public int? MentorshipSessionId { get; private set; }

    // Navigation properties
    public StartupProfile StartupProfile { get; private set; } = null!;
    public AdvisorProfile AdvisorProfile { get; private set; } = null!;
    public MentorshipSession? MentorshipSession { get; private set; }

    private ConsultationRequest() { }

    public static ConsultationRequest Create(
        int startupProfileId,
        int advisorProfileId,
        string topic,
        string description,
        string? expectedOutcome = null,
        string? preferredSchedule = null,
        DateTime? requestedDate = null,
        int estimatedDurationMinutes = 60)
    {
        return new ConsultationRequest
        {
            StartupProfileId = startupProfileId,
            AdvisorProfileId = advisorProfileId,
            Status = ConsultationRequestStatus.Pending,
            Topic = topic,
            Description = description,
            ExpectedOutcome = expectedOutcome,
            PreferredSchedule = preferredSchedule,
            RequestedDate = requestedDate,
            EstimatedDurationMinutes = estimatedDurationMinutes
        };
    }

    public void Accept(DateTime scheduledDate, string? meetingLink = null)
    {
        if (Status != ConsultationRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be accepted");
        }

        Status = ConsultationRequestStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        ScheduledDate = scheduledDate;
        MeetingLink = meetingLink;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConsultationRequestAcceptedEvent
    }

    public void Schedule(DateTime scheduledDate, string meetingLink)
    {
        if (Status != ConsultationRequestStatus.Accepted)
        {
            throw new InvalidOperationException("Only accepted requests can be scheduled");
        }

        Status = ConsultationRequestStatus.Scheduled;
        ScheduledDate = scheduledDate;
        MeetingLink = meetingLink;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConsultationScheduledEvent
    }

    public void Decline(string reason)
    {
        if (Status != ConsultationRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be declined");
        }

        Status = ConsultationRequestStatus.Declined;
        DeclinedAt = DateTime.UtcNow;
        DeclineReason = reason;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConsultationRequestDeclinedEvent
    }

    public void MarkAsInProgress()
    {
        if (Status != ConsultationRequestStatus.Scheduled)
        {
            throw new InvalidOperationException("Only scheduled consultations can be marked as in progress");
        }

        Status = ConsultationRequestStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(int mentorshipSessionId)
    {
        if (Status != ConsultationRequestStatus.InProgress && 
            Status != ConsultationRequestStatus.Scheduled)
        {
            throw new InvalidOperationException("Only in-progress or scheduled consultations can be completed");
        }

        Status = ConsultationRequestStatus.Completed;
        MentorshipSessionId = mentorshipSessionId;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConsultationCompletedEvent
    }

    public void Cancel()
    {
        if (Status == ConsultationRequestStatus.Completed ||
            Status == ConsultationRequestStatus.Declined)
        {
            throw new InvalidOperationException("Completed or declined consultations cannot be cancelled");
        }

        Status = ConsultationRequestStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConsultationCancelledEvent
    }

    public void MarkAsNoShow()
    {
        if (Status != ConsultationRequestStatus.Scheduled &&
            Status != ConsultationRequestStatus.InProgress)
        {
            throw new InvalidOperationException("Only scheduled or in-progress consultations can be marked as no-show");
        }

        Status = ConsultationRequestStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSchedule(DateTime newScheduledDate, string? newMeetingLink = null)
    {
        if (Status != ConsultationRequestStatus.Scheduled)
        {
            throw new InvalidOperationException("Only scheduled consultations can be rescheduled");
        }

        ScheduledDate = newScheduledDate;
        if (newMeetingLink != null)
        {
            MeetingLink = newMeetingLink;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
