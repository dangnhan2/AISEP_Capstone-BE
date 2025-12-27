namespace AISEP.Domain.Entities;

/// <summary>
/// Mentorship session between advisor and startup
/// </summary>
public class MentorshipSession : BaseEntity
{
    public int ConnectionId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public string? Topic { get; private set; }
    public string? SessionNotes { get; private set; }
    public string? ActionItems { get; private set; } // JSON array
    public int? ProductivityRating { get; private set; } // 1-5 scale
    public bool IsCompleted { get; private set; } = false;
    public DateTime? CompletedAt { get; private set; }

    // Navigation properties
    public Connection Connection { get; private set; } = null!;
    public ConsultationRequest? ConsultationRequest { get; private set; }
    public ConsultationReport? ConsultationReport { get; private set; }
    public ICollection<ConsultationFeedback> Feedbacks { get; private set; } = new List<ConsultationFeedback>();

    private MentorshipSession() { }

    public static MentorshipSession Create(
        int connectionId,
        DateTime scheduledAt,
        int durationMinutes,
        string? topic)
    {
        if (durationMinutes <= 0)
            throw new ArgumentException("Duration must be positive", nameof(durationMinutes));

        if (scheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled time must be in the future", nameof(scheduledAt));

        return new MentorshipSession
        {
            ConnectionId = connectionId,
            ScheduledAt = scheduledAt,
            DurationMinutes = durationMinutes,
            Topic = topic,
            IsCompleted = false
        };
    }

    public void Complete(string? sessionNotes, string? actionItems, int? productivityRating)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Session is already completed");

        if (productivityRating.HasValue && (productivityRating < 1 || productivityRating > 5))
            throw new ArgumentException("Rating must be between 1 and 5", nameof(productivityRating));

        SessionNotes = sessionNotes;
        ActionItems = actionItems;
        ProductivityRating = productivityRating;
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event MentorshipSessionCompletedEvent
    }

    public void Reschedule(DateTime newScheduledAt)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Cannot reschedule completed session");

        if (newScheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("New scheduled time must be in the future", nameof(newScheduledAt));

        ScheduledAt = newScheduledAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTopic(string topic)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Cannot update topic of completed session");

        Topic = topic;
        UpdatedAt = DateTime.UtcNow;
    }
}
