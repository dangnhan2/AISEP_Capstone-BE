using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Connection between startup and investor/advisor
/// </summary>
public class Connection : BaseEntity
{
    public int StartupProfileId { get; private set; }
    public int? InvestorProfileId { get; private set; }
    public int? AdvisorProfileId { get; private set; }
    public ConnectionStatus Status { get; private set; }
    public string? Message { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? DeclinedAt { get; private set; }

    // Navigation properties
    public StartupProfile StartupProfile { get; private set; } = null!;
    public InvestorProfile? InvestorProfile { get; private set; }
    public AdvisorProfile? AdvisorProfile { get; private set; }

    private Connection() { }

    public static Connection CreateWithInvestor(
        int startupProfileId,
        int investorProfileId,
        string? message)
    {
        return new Connection
        {
            StartupProfileId = startupProfileId,
            InvestorProfileId = investorProfileId,
            Status = ConnectionStatus.Pending,
            Message = message
        };
    }

    public static Connection CreateWithAdvisor(
        int startupProfileId,
        int advisorProfileId,
        string? message)
    {
        return new Connection
        {
            StartupProfileId = startupProfileId,
            AdvisorProfileId = advisorProfileId,
            Status = ConnectionStatus.Pending,
            Message = message
        };
    }

    public void Accept()
    {
        if (Status != ConnectionStatus.Pending)
            throw new InvalidOperationException("Only pending connections can be accepted");

        Status = ConnectionStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConnectionAcceptedEvent
    }

    public void Decline()
    {
        if (Status != ConnectionStatus.Pending)
            throw new InvalidOperationException("Only pending connections can be declined");

        Status = ConnectionStatus.Declined;
        DeclinedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event ConnectionDeclinedEvent
    }

    public void Archive()
    {
        Status = ConnectionStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
