using AISEP.Application.Features.InvestmentProposals.Commands;
using FluentValidation;

namespace AISEP.Application.Features.InvestmentProposals.Validators;

/// <summary>
/// Validator for CreateInvestmentProposalCommand
/// </summary>
public class CreateInvestmentProposalCommandValidator : AbstractValidator<CreateInvestmentProposalCommand>
{
    public CreateInvestmentProposalCommandValidator()
    {
        RuleFor(x => x.InvestorProfileId)
            .GreaterThan(0)
            .WithMessage("Investor profile ID must be greater than 0");

        RuleFor(x => x.StartupProfileId)
            .GreaterThan(0)
            .WithMessage("Startup profile ID must be greater than 0");

        RuleFor(x => x.ProposedAmount)
            .GreaterThan(0)
            .WithMessage("Proposed amount must be greater than 0")
            .LessThanOrEqualTo(1_000_000_000) // 1 billion max
            .WithMessage("Proposed amount cannot exceed 1 billion");

        RuleFor(x => x.EquityPercentage)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EquityPercentage.HasValue)
            .WithMessage("Equity percentage must be non-negative")
            .LessThanOrEqualTo(100)
            .When(x => x.EquityPercentage.HasValue)
            .WithMessage("Equity percentage cannot exceed 100%");

        RuleFor(x => x.Valuation)
            .GreaterThan(0)
            .When(x => x.Valuation.HasValue)
            .WithMessage("Valuation must be greater than 0");

        RuleFor(x => x.InvestmentType)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.InvestmentType))
            .WithMessage("Investment type cannot exceed 50 characters");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .MinimumLength(10)
            .WithMessage("Message must be at least 10 characters")
            .MaximumLength(2000)
            .WithMessage("Message cannot exceed 2000 characters");

        RuleFor(x => x.ResponseDeadline)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ResponseDeadline.HasValue)
            .WithMessage("Response deadline must be in the future");

        // Business rule: If equity percentage is provided, valuation should also be provided
        RuleFor(x => x.Valuation)
            .NotNull()
            .When(x => x.EquityPercentage.HasValue)
            .WithMessage("Valuation must be provided when equity percentage is specified");

        // Business rule: Proposed amount should align with equity and valuation
        RuleFor(x => x)
            .Must(HaveConsistentFinancials)
            .When(x => x.EquityPercentage.HasValue && x.Valuation.HasValue)
            .WithMessage("Proposed amount, equity percentage, and valuation must be consistent");
    }

    private bool HaveConsistentFinancials(CreateInvestmentProposalCommand command)
    {
        if (!command.EquityPercentage.HasValue || !command.Valuation.HasValue)
        {
            return true; // Skip validation if values are not provided
        }

        // Calculate expected investment based on valuation and equity
        var expectedInvestment = command.Valuation.Value * (command.EquityPercentage.Value / 100);
        var tolerance = expectedInvestment * 0.05m; // Allow 5% tolerance

        // Check if proposed amount is within tolerance
        return Math.Abs(command.ProposedAmount - expectedInvestment) <= tolerance;
    }
}
