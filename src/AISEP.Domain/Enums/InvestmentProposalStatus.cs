namespace AISEP.Domain.Enums;

/// <summary>
/// Status of investment proposal from investor to startup
/// </summary>
public enum InvestmentProposalStatus
{
    Draft,
    Submitted,
    UnderReview,
    Negotiating,
    DueDiligence,
    TermSheetSent,
    Accepted,
    Declined,
    Withdrawn,
    Closed
}
