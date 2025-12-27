using AISEP.Domain.Enums;

namespace AISEP.Application.Features.InvestmentProposals.DTOs;

/// <summary>
/// DTO for Investment Proposal details
/// </summary>
public class InvestmentProposalDto
{
    public int Id { get; set; }
    public int InvestorProfileId { get; set; }
    public int StartupProfileId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ProposedAmount { get; set; }
    public decimal? EquityPercentage { get; set; }
    public decimal? Valuation { get; set; }
    public string? InvestmentType { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Terms { get; set; }
    public string? TermSheetUrl { get; set; }
    public DateTime? DueDiligenceDeadline { get; set; }
    public DateTime? ResponseDeadline { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }
    public string? DeclineReason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (optional, can be included on demand)
    public InvestorProfileSummaryDto? InvestorProfile { get; set; }
    public StartupProfileSummaryDto? StartupProfile { get; set; }
}

/// <summary>
/// Summary DTO for Investor Profile (nested in proposal)
/// </summary>
public class InvestorProfileSummaryDto
{
    public int Id { get; set; }
    public string? OrganizationName { get; set; }
    public string? InvestmentThesis { get; set; }
}

/// <summary>
/// Summary DTO for Startup Profile (nested in proposal)
/// </summary>
public class StartupProfileSummaryDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
}
