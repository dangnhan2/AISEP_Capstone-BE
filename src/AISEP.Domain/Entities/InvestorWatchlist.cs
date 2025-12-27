namespace AISEP.Domain.Entities;

/// <summary>
/// Investor watchlist entity for tracking startups that investors are interested in
/// Allows investors to save and monitor potential investment opportunities
/// </summary>
public class InvestorWatchlist : BaseEntity
{
    public int InvestorProfileId { get; private set; }
    public int StartupProfileId { get; private set; }
    public string? Notes { get; private set; }
    public int Priority { get; private set; } = 0;
    public bool IsNotificationEnabled { get; private set; } = true;
    public DateTime? LastViewedAt { get; private set; }
    public int ViewCount { get; private set; } = 0;

    // Navigation properties
    public InvestorProfile InvestorProfile { get; private set; } = null!;
    public StartupProfile StartupProfile { get; private set; } = null!;

    private InvestorWatchlist() { }

    public static InvestorWatchlist Create(
        int investorProfileId,
        int startupProfileId,
        string? notes = null,
        int priority = 0)
    {
        return new InvestorWatchlist
        {
            InvestorProfileId = investorProfileId,
            StartupProfileId = startupProfileId,
            Notes = notes,
            Priority = priority,
            IsNotificationEnabled = true,
            ViewCount = 0
        };
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(int priority)
    {
        if (priority < 0 || priority > 5)
        {
            throw new ArgumentException("Priority must be between 0 and 5", nameof(priority));
        }

        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ToggleNotifications()
    {
        IsNotificationEnabled = !IsNotificationEnabled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableNotifications()
    {
        IsNotificationEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisableNotifications()
    {
        IsNotificationEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordView()
    {
        LastViewedAt = DateTime.UtcNow;
        ViewCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculate days since last viewed
    /// </summary>
    public int? DaysSinceLastView()
    {
        if (!LastViewedAt.HasValue)
        {
            return null;
        }

        return (int)(DateTime.UtcNow - LastViewedAt.Value).TotalDays;
    }

    /// <summary>
    /// Check if the watchlist item is stale (not viewed in specified days)
    /// </summary>
    public bool IsStale(int days = 30)
    {
        var daysSinceView = DaysSinceLastView();
        return daysSinceView.HasValue && daysSinceView.Value > days;
    }
}
