using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Startup profile for founder users
/// </summary>
public class StartupProfile : BaseEntity
{
    public int UserId { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public string Industry { get; private set; } = string.Empty;
    public StartupStage Stage { get; private set; }
    public DateTime FoundingDate { get; private set; }
    public int TeamSize { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string? Description { get; private set; }
    public decimal? FundingAmountSought { get; private set; }
    public bool IsPublished { get; private set; } = false;

    // Navigation properties
    public User User { get; private set; } = null!;
    public ICollection<Document> Documents { get; private set; } = new List<Document>();
    public ICollection<StartupScore> Scores { get; private set; } = new List<StartupScore>();
    public ICollection<Connection> Connections { get; private set; } = new List<Connection>();
    public ICollection<InvestmentProposal> InvestmentProposals { get; private set; } = new List<InvestmentProposal>();
    public ICollection<InvestorWatchlist> WatchedBy { get; private set; } = new List<InvestorWatchlist>();
    public ICollection<ConsultationRequest> ConsultationRequests { get; private set; } = new List<ConsultationRequest>();

    private StartupProfile() { }

    public static StartupProfile Create(
        int userId,
        string companyName,
        string industry,
        StartupStage stage,
        DateTime foundingDate,
        int teamSize,
        string location)
    {
        var profile = new StartupProfile
        {
            UserId = userId,
            CompanyName = companyName,
            Industry = industry,
            Stage = stage,
            FoundingDate = foundingDate,
            TeamSize = teamSize,
            Location = location,
            IsPublished = false
        };

        return profile;
    }

    public void Update(
        string companyName,
        string industry,
        StartupStage stage,
        int teamSize,
        string location,
        string? website,
        string? description,
        decimal? fundingAmountSought)
    {
        CompanyName = companyName;
        Industry = industry;
        Stage = stage;
        TeamSize = teamSize;
        Location = location;
        Website = website;
        Description = description;
        FundingAmountSought = fundingAmountSought;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLogo(string logoUrl)
    {
        LogoUrl = logoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCoverImage(string coverImageUrl)
    {
        CoverImageUrl = coverImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (string.IsNullOrEmpty(Description))
            throw new InvalidOperationException("Cannot publish profile without description");

        IsPublished = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        IsPublished = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
