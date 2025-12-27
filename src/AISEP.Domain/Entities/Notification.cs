using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Notification entity for tracking all system notifications
/// Supports both in-app and external notifications (email, SMS)
/// </summary>
public class Notification : BaseEntity
{
    public int UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? ActionUrl { get; private set; }
    public bool IsRead { get; private set; } = false;
    public DateTime? ReadAt { get; private set; }
    public bool IsSent { get; private set; } = false;
    public DateTime? SentAt { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public int? RelatedEntityId { get; private set; }
    public string? Metadata { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    private Notification() { }

    public static Notification Create(
        int userId,
        NotificationType type,
        string title,
        string message,
        string? actionUrl = null,
        string? relatedEntityType = null,
        int? relatedEntityId = null,
        string? metadata = null)
    {
        return new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ActionUrl = actionUrl,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId,
            Metadata = metadata,
            IsRead = false,
            IsSent = false
        };
    }

    public void MarkAsRead()
    {
        if (IsRead)
        {
            return;
        }

        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSent()
    {
        if (IsSent)
        {
            return;
        }

        IsSent = true;
        SentAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method for connection request notification
    /// </summary>
    public static Notification CreateConnectionRequest(int userId, string senderName, int connectionId)
    {
        return Create(
            userId,
            NotificationType.ConnectionRequest,
            "New Connection Request",
            $"{senderName} has sent you a connection request",
            $"/connections/{connectionId}",
            "Connection",
            connectionId);
    }

    /// <summary>
    /// Factory method for investment proposal notification
    /// </summary>
    public static Notification CreateInvestmentProposal(int userId, string investorName, int proposalId)
    {
        return Create(
            userId,
            NotificationType.InvestmentProposal,
            "New Investment Proposal",
            $"{investorName} has sent you an investment proposal",
            $"/proposals/{proposalId}",
            "InvestmentProposal",
            proposalId);
    }

    /// <summary>
    /// Factory method for score updated notification
    /// </summary>
    public static Notification CreateScoreUpdated(int userId, decimal newScore)
    {
        return Create(
            userId,
            NotificationType.ScoreUpdated,
            "Startup Score Updated",
            $"Your startup potential score has been updated to {newScore:F1}",
            "/dashboard/score");
    }
}
