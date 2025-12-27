namespace AISEP.Domain.Entities;

/// <summary>
/// Consultation report entity for documenting outcomes of mentorship sessions
/// Separate from MentorshipSession to track detailed report and action items
/// </summary>
public class ConsultationReport : BaseEntity
{
    public int MentorshipSessionId { get; private set; }
    public int ConsultationRequestId { get; private set; }
    public string Summary { get; private set; } = string.Empty;
    public string? KeyDiscussionPoints { get; private set; }
    public string? ActionItems { get; private set; }
    public string? RecommendedResources { get; private set; }
    public string? FollowUpNotes { get; private set; }
    public DateTime? NextSessionSuggested { get; private set; }
    public bool IsSharedWithStartup { get; private set; } = true;
    public DateTime? SharedAt { get; private set; }
    public int CreatedByUserId { get; private set; }

    // Navigation properties
    public MentorshipSession MentorshipSession { get; private set; } = null!;
    public ConsultationRequest ConsultationRequest { get; private set; } = null!;
    public User CreatedByUser { get; private set; } = null!;

    private ConsultationReport() { }

    public static ConsultationReport Create(
        int mentorshipSessionId,
        int consultationRequestId,
        int createdByUserId,
        string summary,
        string? keyDiscussionPoints = null,
        string? actionItems = null,
        string? recommendedResources = null,
        string? followUpNotes = null,
        DateTime? nextSessionSuggested = null)
    {
        return new ConsultationReport
        {
            MentorshipSessionId = mentorshipSessionId,
            ConsultationRequestId = consultationRequestId,
            CreatedByUserId = createdByUserId,
            Summary = summary,
            KeyDiscussionPoints = keyDiscussionPoints,
            ActionItems = actionItems,
            RecommendedResources = recommendedResources,
            FollowUpNotes = followUpNotes,
            NextSessionSuggested = nextSessionSuggested,
            IsSharedWithStartup = true
        };
    }

    public void UpdateReport(
        string summary,
        string? keyDiscussionPoints = null,
        string? actionItems = null,
        string? recommendedResources = null,
        string? followUpNotes = null)
    {
        Summary = summary;
        KeyDiscussionPoints = keyDiscussionPoints;
        ActionItems = actionItems;
        RecommendedResources = recommendedResources;
        FollowUpNotes = followUpNotes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ShareWithStartup()
    {
        if (IsSharedWithStartup)
        {
            return;
        }

        IsSharedWithStartup = true;
        SharedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConsultationReportSharedEvent
    }

    public void UnshareFromStartup()
    {
        IsSharedWithStartup = false;
        SharedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddActionItems(string actionItems)
    {
        ActionItems = string.IsNullOrWhiteSpace(ActionItems)
            ? actionItems
            : $"{ActionItems}\n{actionItems}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRecommendedResource(string resource)
    {
        RecommendedResources = string.IsNullOrWhiteSpace(RecommendedResources)
            ? resource
            : $"{RecommendedResources}\n{resource}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SuggestNextSession(DateTime suggestedDate)
    {
        NextSessionSuggested = suggestedDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
