using AISEP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEP.Infrastructure.Data.Configurations;

public class InvestorProfileConfiguration : IEntityTypeConfiguration<InvestorProfile>
{
    public void Configure(EntityTypeBuilder<InvestorProfile> builder)
    {
        builder.ToTable("investor_profiles");

        builder.HasKey(ip => ip.Id);

        builder.Property(ip => ip.Id)
            .HasColumnName("id");

        builder.Property(ip => ip.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(ip => ip.OrganizationName)
            .HasColumnName("organization_name")
            .HasMaxLength(200);

        builder.Property(ip => ip.InvestmentThesis)
            .HasColumnName("investment_thesis")
            .HasMaxLength(2000);

        builder.Property(ip => ip.PreferredIndustries)
            .HasColumnName("preferred_industries")
            .HasMaxLength(1000);

        builder.Property(ip => ip.PreferredStages)
            .HasColumnName("preferred_stages")
            .HasMaxLength(500);

        builder.Property(ip => ip.MinInvestmentSize)
            .HasColumnName("min_investment_size")
            .HasColumnType("decimal(18,2)");

        builder.Property(ip => ip.MaxInvestmentSize)
            .HasColumnName("max_investment_size")
            .HasColumnType("decimal(18,2)");

        builder.Property(ip => ip.GeographicFocus)
            .HasColumnName("geographic_focus")
            .HasMaxLength(1000);

        builder.Property(ip => ip.PortfolioCompanies)
            .HasColumnName("portfolio_companies")
            .HasMaxLength(2000);

        builder.Property(ip => ip.Website)
            .HasColumnName("website")
            .HasMaxLength(500);

        builder.Property(ip => ip.IsPublished)
            .HasColumnName("is_published")
            .HasDefaultValue(false)
            .IsRequired();

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
        builder.HasMany(ip => ip.Connections)
            .WithOne(c => c.InvestorProfile)
            .HasForeignKey(c => c.InvestorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ip => ip.Watchlist)
            .WithOne(w => w.InvestorProfile)
            .HasForeignKey(w => w.InvestorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ip => ip.UserId)
            .IsUnique()
            .HasDatabaseName("ix_investor_profiles_user_id");

        builder.HasIndex(ip => ip.IsPublished)
            .HasDatabaseName("ix_investor_profiles_is_published");

        builder.HasQueryFilter(ip => !ip.IsDeleted);
        builder.Ignore(ip => ip.DomainEvents);
    }
}
