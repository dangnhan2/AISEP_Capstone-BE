using AISEP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AISEP.Infrastructure.Data;

/// <summary>
/// Application DbContext for AISEP
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<StartupProfile> StartupProfiles => Set<StartupProfile>();
    public DbSet<InvestorProfile> InvestorProfiles => Set<InvestorProfile>();
    public DbSet<AdvisorProfile> AdvisorProfiles => Set<AdvisorProfile>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentAnalysis> DocumentAnalyses => Set<DocumentAnalysis>();
    public DbSet<BlockchainTransaction> BlockchainTransactions => Set<BlockchainTransaction>();
    public DbSet<StartupScore> StartupScores => Set<StartupScore>();
    public DbSet<Connection> Connections => Set<Connection>();
    public DbSet<MentorshipSession> MentorshipSessions => Set<MentorshipSession>();
    public DbSet<InvestmentProposal> InvestmentProposals => Set<InvestmentProposal>();
    public DbSet<InvestorWatchlist> InvestorWatchlists => Set<InvestorWatchlist>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<ConsultationRequest> ConsultationRequests => Set<ConsultationRequest>();
    public DbSet<ConsultationReport> ConsultationReports => Set<ConsultationReport>();
    public DbSet<ConsultationFeedback> ConsultationFeedbacks => Set<ConsultationFeedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filters can be added here if needed
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-set UpdatedAt for modified entities
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        // Auto-set DeletedAt for soft deleted entities
        var deletedEntries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in deletedEntries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
