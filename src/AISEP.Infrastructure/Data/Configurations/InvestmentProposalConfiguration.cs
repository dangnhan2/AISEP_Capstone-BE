using AISEP.Domain.Entities;
using AISEP.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEP.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for InvestmentProposal entity
/// </summary>
public class InvestmentProposalConfiguration : IEntityTypeConfiguration<InvestmentProposal>
{
    public void Configure(EntityTypeBuilder<InvestmentProposal> builder)
    {
        // Table configuration
        builder.ToTable("investment_proposals");

        // Primary key
        builder.HasKey(ip => ip.Id);

        // Properties configuration
        builder.Property(ip => ip.Id)
            .HasColumnName("id");

        builder.Property(ip => ip.InvestorProfileId)
            .HasColumnName("investor_profile_id")
            .IsRequired();

        builder.Property(ip => ip.StartupProfileId)
            .HasColumnName("startup_profile_id")
            .IsRequired();

        builder.Property(ip => ip.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<InvestmentProposalStatus>(v))
            .IsRequired();

        builder.Property(ip => ip.ProposedAmount)
            .HasColumnName("proposed_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ip => ip.EquityPercentage)
            .HasColumnName("equity_percentage")
            .HasColumnType("decimal(5,2)");

        builder.Property(ip => ip.Valuation)
            .HasColumnName("valuation")
            .HasColumnType("decimal(18,2)");

        builder.Property(ip => ip.InvestmentType)
            .HasColumnName("investment_type")
            .HasMaxLength(50);

        builder.Property(ip => ip.Message)
            .HasColumnName("message")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(ip => ip.Terms)
            .HasColumnName("terms")
            .HasMaxLength(5000);

        builder.Property(ip => ip.TermSheetUrl)
            .HasColumnName("term_sheet_url")
            .HasMaxLength(500);

        builder.Property(ip => ip.DueDiligenceDeadline)
            .HasColumnName("due_diligence_deadline");

        builder.Property(ip => ip.ResponseDeadline)
            .HasColumnName("response_deadline");

        builder.Property(ip => ip.SubmittedAt)
            .HasColumnName("submitted_at");

        builder.Property(ip => ip.AcceptedAt)
            .HasColumnName("accepted_at");

        builder.Property(ip => ip.DeclinedAt)
            .HasColumnName("declined_at");

        builder.Property(ip => ip.DeclineReason)
            .HasColumnName("decline_reason")
            .HasMaxLength(1000);

        builder.Property(ip => ip.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        // BaseEntity properties
        builder.Property(ip => ip.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ip => ip.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(ip => ip.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(ip => ip.DeletedAt)
            .HasColumnName("deleted_at");

        // Relationships
        builder.HasOne(ip => ip.InvestorProfile)
            .WithMany(inv => inv.InvestmentProposals)
            .HasForeignKey(ip => ip.InvestorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ip => ip.StartupProfile)
            .WithMany(s => s.InvestmentProposals)
            .HasForeignKey(ip => ip.StartupProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(ip => ip.InvestorProfileId)
            .HasDatabaseName("ix_investment_proposals_investor_profile_id");

        builder.HasIndex(ip => ip.StartupProfileId)
            .HasDatabaseName("ix_investment_proposals_startup_profile_id");

        builder.HasIndex(ip => ip.Status)
            .HasDatabaseName("ix_investment_proposals_status");

        builder.HasIndex(ip => ip.CreatedAt)
            .HasDatabaseName("ix_investment_proposals_created_at");

        builder.HasIndex(ip => new { ip.InvestorProfileId, ip.Status })
            .HasDatabaseName("ix_investment_proposals_investor_status");

        builder.HasIndex(ip => new { ip.StartupProfileId, ip.Status })
            .HasDatabaseName("ix_investment_proposals_startup_status");

        // Query filter for soft delete
        builder.HasQueryFilter(ip => !ip.IsDeleted);

        // Ignore domain events (not persisted)
        builder.Ignore(ip => ip.DomainEvents);
    }
}
