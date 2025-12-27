using AISEP.Application.Features.InvestmentProposals.DTOs;
using AISEP.Domain.Entities;
using AISEP.Domain.Enums;
using AISEP.Domain.Interfaces;
using MediatR;

namespace AISEP.Application.Features.InvestmentProposals.Commands;

/// <summary>
/// Handler for CreateInvestmentProposalCommand
/// Creates a new investment proposal following CQRS pattern
/// </summary>
public class CreateInvestmentProposalCommandHandler
    : IRequestHandler<CreateInvestmentProposalCommand, InvestmentProposalDto>
{
    private readonly IRepository<InvestmentProposal> _proposalRepository;
    private readonly IRepository<InvestorProfile> _investorRepository;
    private readonly IRepository<StartupProfile> _startupRepository;

    public CreateInvestmentProposalCommandHandler(
        IRepository<InvestmentProposal> proposalRepository,
        IRepository<InvestorProfile> investorRepository,
        IRepository<StartupProfile> startupRepository)
    {
        _proposalRepository = proposalRepository ?? throw new ArgumentNullException(nameof(proposalRepository));
        _investorRepository = investorRepository ?? throw new ArgumentNullException(nameof(investorRepository));
        _startupRepository = startupRepository ?? throw new ArgumentNullException(nameof(startupRepository));
    }

    public async Task<InvestmentProposalDto> Handle(
        CreateInvestmentProposalCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate that investor profile exists
        var investorExists = await _investorRepository.AnyAsync(
            inv => inv.Id == request.InvestorProfileId,
            cancellationToken);

        if (!investorExists)
        {
            throw new InvalidOperationException($"Investor profile with ID {request.InvestorProfileId} not found");
        }

        // 2. Validate that startup profile exists
        var startupExists = await _startupRepository.AnyAsync(
            s => s.Id == request.StartupProfileId,
            cancellationToken);

        if (!startupExists)
        {
            throw new InvalidOperationException($"Startup profile with ID {request.StartupProfileId} not found");
        }

        // 3. Check if there's already an active proposal between these parties
        // IsActive means: Status != Declined && Status != Withdrawn && Status != Closed
        var existingProposal = await _proposalRepository.FirstOrDefaultAsync(
            p => p.InvestorProfileId == request.InvestorProfileId
                && p.StartupProfileId == request.StartupProfileId
                && p.Status != InvestmentProposalStatus.Declined
                && p.Status != InvestmentProposalStatus.Withdrawn
                && p.Status != InvestmentProposalStatus.Closed,
            cancellationToken);

        if (existingProposal != null)
        {
            throw new InvalidOperationException(
                $"An active proposal already exists between these parties (Proposal ID: {existingProposal.Id})");
        }

        // 4. Create the investment proposal using domain factory method
        var proposal = InvestmentProposal.Create(
            investorProfileId: request.InvestorProfileId,
            startupProfileId: request.StartupProfileId,
            proposedAmount: request.ProposedAmount,
            message: request.Message,
            equityPercentage: request.EquityPercentage,
            valuation: request.Valuation,
            investmentType: request.InvestmentType,
            responseDeadline: request.ResponseDeadline);

        // 5. Persist to database
        await _proposalRepository.AddAsync(proposal, cancellationToken);
        await _proposalRepository.SaveChangesAsync(cancellationToken);

        // 6. Map to DTO and return
        var result = MapToDto(proposal);

        return result;
    }

    private static InvestmentProposalDto MapToDto(InvestmentProposal proposal)
    {
        return new InvestmentProposalDto
        {
            Id = proposal.Id,
            InvestorProfileId = proposal.InvestorProfileId,
            StartupProfileId = proposal.StartupProfileId,
            Status = proposal.Status.ToString(),
            ProposedAmount = proposal.ProposedAmount,
            EquityPercentage = proposal.EquityPercentage,
            Valuation = proposal.Valuation,
            InvestmentType = proposal.InvestmentType,
            Message = proposal.Message,
            Terms = proposal.Terms,
            TermSheetUrl = proposal.TermSheetUrl,
            DueDiligenceDeadline = proposal.DueDiligenceDeadline,
            ResponseDeadline = proposal.ResponseDeadline,
            SubmittedAt = proposal.SubmittedAt,
            AcceptedAt = proposal.AcceptedAt,
            DeclinedAt = proposal.DeclinedAt,
            DeclineReason = proposal.DeclineReason,
            Notes = proposal.Notes,
            CreatedAt = proposal.CreatedAt,
            UpdatedAt = proposal.UpdatedAt
        };
    }
}
