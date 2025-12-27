namespace AISEP.Domain.Entities;

/// <summary>
/// Consultation feedback entity for both startup and advisor to rate and review sessions
/// Enables quality tracking and reputation building
/// </summary>
public class ConsultationFeedback : BaseEntity
{
    public int MentorshipSessionId { get; private set; }
    public int ConsultationReportId { get; private set; }
    public int ProvidedByUserId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public bool IsHelpful { get; private set; }
    public bool WouldRecommend { get; private set; }
    public string? SpecificFeedback { get; private set; }
    public string? ImprovementSuggestions { get; private set; }
    public bool IsPublic { get; private set; } = false;

    // Navigation properties
    public MentorshipSession MentorshipSession { get; private set; } = null!;
    public ConsultationReport ConsultationReport { get; private set; } = null!;
    public User ProvidedByUser { get; private set; } = null!;

    private ConsultationFeedback() { }

    public static ConsultationFeedback Create(
        int mentorshipSessionId,
        int consultationReportId,
        int providedByUserId,
        int rating,
        bool isHelpful,
        bool wouldRecommend,
        string? comment = null,
        string? specificFeedback = null,
        string? improvementSuggestions = null,
        bool isPublic = false)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));
        }

        return new ConsultationFeedback
        {
            MentorshipSessionId = mentorshipSessionId,
            ConsultationReportId = consultationReportId,
            ProvidedByUserId = providedByUserId,
            Rating = rating,
            IsHelpful = isHelpful,
            WouldRecommend = wouldRecommend,
            Comment = comment,
            SpecificFeedback = specificFeedback,
            ImprovementSuggestions = improvementSuggestions,
            IsPublic = isPublic
        };
    }

    public void UpdateRating(int rating)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));
        }

        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateComment(string comment)
    {
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFeedback(
        int? rating = null,
        bool? isHelpful = null,
        bool? wouldRecommend = null,
        string? comment = null,
        string? specificFeedback = null,
        string? improvementSuggestions = null)
    {
        if (rating.HasValue)
        {
            if (rating.Value < 1 || rating.Value > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));
            }
            Rating = rating.Value;
        }

        if (isHelpful.HasValue)
        {
            IsHelpful = isHelpful.Value;
        }

        if (wouldRecommend.HasValue)
        {
            WouldRecommend = wouldRecommend.Value;
        }

        if (comment != null)
        {
            Comment = comment;
        }

        if (specificFeedback != null)
        {
            SpecificFeedback = specificFeedback;
        }

        if (improvementSuggestions != null)
        {
            ImprovementSuggestions = improvementSuggestions;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void MakePublic()
    {
        IsPublic = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MakePrivate()
    {
        IsPublic = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsPositive()
    {
        return Rating >= 4 && IsHelpful && WouldRecommend;
    }

    public bool IsNegative()
    {
        return Rating <= 2 || (!IsHelpful && !WouldRecommend);
    }
}
