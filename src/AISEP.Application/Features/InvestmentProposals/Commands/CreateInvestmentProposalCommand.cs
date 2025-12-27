using AISEP.Application.Features.InvestmentProposals.DTOs;
using MediatR;

namespace AISEP.Application.Features.InvestmentProposals.Commands;

/// <summary>
/// Command to create a new investment proposal
/// </summary>
public class CreateInvestmentProposalCommand : IRequest<InvestmentProposalDto>
{
    public int InvestorProfileId { get; set; }
    public int StartupProfileId { get; set; }
    public decimal ProposedAmount { get; set; }
    public decimal? EquityPercentage { get; set; }
    public decimal? Valuation { get; set; }
    public string? InvestmentType { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? ResponseDeadline { get; set; }
}
