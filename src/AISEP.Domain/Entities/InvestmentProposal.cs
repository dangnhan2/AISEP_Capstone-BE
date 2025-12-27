using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Investment proposal entity representing investment offers from investors to startups
/// Core entity for tracking the investment deal flow
/// </summary>
public class InvestmentProposal : BaseEntity
{
    public int InvestorProfileId { get; private set; }
    public int StartupProfileId { get; private set; }
    public InvestmentProposalStatus Status { get; private set; }
    public decimal ProposedAmount { get; private set; }
    public decimal? EquityPercentage { get; private set; }
    public decimal? Valuation { get; private set; }
    public string? InvestmentType { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string? Terms { get; private set; }
    public string? TermSheetUrl { get; private set; }
    public DateTime? DueDiligenceDeadline { get; private set; }
    public DateTime? ResponseDeadline { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? DeclinedAt { get; private set; }
    public string? DeclineReason { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public InvestorProfile InvestorProfile { get; private set; } = null!;
    public StartupProfile StartupProfile { get; private set; } = null!;

    private InvestmentProposal() { }

    public static InvestmentProposal Create(
        int investorProfileId,
        int startupProfileId,
        decimal proposedAmount,
        string message,
        decimal? equityPercentage = null,
        decimal? valuation = null,
        string? investmentType = null,
        DateTime? responseDeadline = null)
    {
        return new InvestmentProposal
        {
            InvestorProfileId = investorProfileId,
            StartupProfileId = startupProfileId,
            Status = InvestmentProposalStatus.Draft,
            ProposedAmount = proposedAmount,
            EquityPercentage = equityPercentage,
            Valuation = valuation,
            InvestmentType = investmentType,
            Message = message,
            ResponseDeadline = responseDeadline ?? DateTime.UtcNow.AddDays(14)
        };
    }

    public void Submit()
    {
        if (Status != InvestmentProposalStatus.Draft)
        {
            throw new InvalidOperationException("Only draft proposals can be submitted");
        }

        Status = InvestmentProposalStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event InvestmentProposalSubmittedEvent
    }

    public void Accept()
    {
        if (Status != InvestmentProposalStatus.Submitted && 
            Status != InvestmentProposalStatus.Negotiating &&
            Status != InvestmentProposalStatus.TermSheetSent)
        {
            throw new InvalidOperationException("Only submitted, negotiating, or term sheet sent proposals can be accepted");
        }

        Status = InvestmentProposalStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event InvestmentProposalAcceptedEvent
    }

    public void Decline(string reason)
    {
        if (Status == InvestmentProposalStatus.Accepted || 
            Status == InvestmentProposalStatus.Closed)
        {
            throw new InvalidOperationException($"Cannot decline proposal with status {Status}");
        }

        Status = InvestmentProposalStatus.Declined;
        DeclinedAt = DateTime.UtcNow;
        DeclineReason = reason;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event InvestmentProposalDeclinedEvent
    }

    public void MoveToNegotiation()
    {
        if (Status != InvestmentProposalStatus.Submitted && 
            Status != InvestmentProposalStatus.UnderReview)
        {
            throw new InvalidOperationException("Only submitted or under review proposals can move to negotiation");
        }

        Status = InvestmentProposalStatus.Negotiating;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MoveToDueDiligence(DateTime deadline)
    {
        if (Status != InvestmentProposalStatus.Negotiating)
        {
            throw new InvalidOperationException("Only negotiating proposals can move to due diligence");
        }

        Status = InvestmentProposalStatus.DueDiligence;
        DueDiligenceDeadline = deadline;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SendTermSheet(string termSheetUrl)
    {
        if (Status != InvestmentProposalStatus.DueDiligence)
        {
            throw new InvalidOperationException("Only proposals in due diligence can have term sheets sent");
        }

        Status = InvestmentProposalStatus.TermSheetSent;
        TermSheetUrl = termSheetUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTerms(
        decimal? proposedAmount = null,
        decimal? equityPercentage = null,
        decimal? valuation = null,
        string? terms = null)
    {
        if (Status == InvestmentProposalStatus.Accepted || 
            Status == InvestmentProposalStatus.Closed ||
            Status == InvestmentProposalStatus.Declined)
        {
            throw new InvalidOperationException("Cannot update terms of finalized proposals");
        }

        if (proposedAmount.HasValue)
        {
            ProposedAmount = proposedAmount.Value;
        }

        if (equityPercentage.HasValue)
        {
            EquityPercentage = equityPercentage.Value;
        }

        if (valuation.HasValue)
        {
            Valuation = valuation.Value;
        }

        if (terms != null)
        {
            Terms = terms;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status != InvestmentProposalStatus.Accepted)
        {
            throw new InvalidOperationException("Only accepted proposals can be closed");
        }

        Status = InvestmentProposalStatus.Closed;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event InvestmentProposalClosedEvent
    }

    public void Withdraw()
    {
        if (Status == InvestmentProposalStatus.Accepted || 
            Status == InvestmentProposalStatus.Closed)
        {
            throw new InvalidOperationException($"Cannot withdraw proposal with status {Status}");
        }

        Status = InvestmentProposalStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event InvestmentProposalWithdrawnEvent
    }

    public bool IsActive()
    {
        return Status != InvestmentProposalStatus.Declined &&
               Status != InvestmentProposalStatus.Withdrawn &&
               Status != InvestmentProposalStatus.Closed;
    }
}
