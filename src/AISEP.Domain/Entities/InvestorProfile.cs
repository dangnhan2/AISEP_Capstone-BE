namespace AISEP.Domain.Entities;

/// <summary>
/// Investor profile for investor users
/// </summary>
public class InvestorProfile : BaseEntity
{
    public int UserId { get; private set; }
    public string? OrganizationName { get; private set; }
    public string? InvestmentThesis { get; private set; }
    public string? PreferredIndustries { get; private set; } // JSON array of industries
    public string? PreferredStages { get; private set; } // JSON array of stages
    public decimal? MinInvestmentSize { get; private set; }
    public decimal? MaxInvestmentSize { get; private set; }
    public string? GeographicFocus { get; private set; } // JSON array of locations
    public string? PortfolioCompanies { get; private set; } // JSON array
    public string? Website { get; private set; }
    public bool IsPublished { get; private set; } = false;

    // Navigation properties
    public User User { get; private set; } = null!;
    public ICollection<Connection> Connections { get; private set; } = new List<Connection>();
    public ICollection<InvestmentProposal> InvestmentProposals { get; private set; } = new List<InvestmentProposal>();
    public ICollection<InvestorWatchlist> Watchlist { get; private set; } = new List<InvestorWatchlist>();

    private InvestorProfile() { }

    public static InvestorProfile Create(int userId, string? organizationName)
    {
        return new InvestorProfile
        {
            UserId = userId,
            OrganizationName = organizationName,
            IsPublished = false
        };
    }

    public void Update(
        string? organizationName,
        string? investmentThesis,
        string? preferredIndustries,
        string? preferredStages,
        decimal? minInvestmentSize,
        decimal? maxInvestmentSize,
        string? geographicFocus,
        string? website)
    {
        OrganizationName = organizationName;
        InvestmentThesis = investmentThesis;
        PreferredIndustries = preferredIndustries;
        PreferredStages = preferredStages;
        MinInvestmentSize = minInvestmentSize;
        MaxInvestmentSize = maxInvestmentSize;
        GeographicFocus = geographicFocus;
        Website = website;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        IsPublished = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        IsPublished = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
