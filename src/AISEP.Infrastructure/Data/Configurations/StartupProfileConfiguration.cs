using AISEP.Domain.Entities;
using AISEP.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEP.Infrastructure.Data.Configurations;

public class StartupProfileConfiguration : IEntityTypeConfiguration<StartupProfile>
{
    public void Configure(EntityTypeBuilder<StartupProfile> builder)
    {
        builder.ToTable("startup_profiles");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id)
            .HasColumnName("id");

        builder.Property(sp => sp.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(sp => sp.CompanyName)
            .HasColumnName("company_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(sp => sp.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        builder.Property(sp => sp.CoverImageUrl)
            .HasColumnName("cover_image_url")
            .HasMaxLength(500);

        builder.Property(sp => sp.Industry)
            .HasColumnName("industry")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sp => sp.Stage)
            .HasColumnName("stage")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<StartupStage>(v))
            .IsRequired();

        builder.Property(sp => sp.FoundingDate)
            .HasColumnName("founding_date")
            .IsRequired();

        builder.Property(sp => sp.TeamSize)
            .HasColumnName("team_size")
            .IsRequired();

        builder.Property(sp => sp.Location)
            .HasColumnName("location")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(sp => sp.Website)
            .HasColumnName("website")
            .HasMaxLength(500);

        builder.Property(sp => sp.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(sp => sp.FundingAmountSought)
            .HasColumnName("funding_amount_sought")
            .HasColumnType("decimal(18,2)");

        builder.Property(sp => sp.IsPublished)
            .HasColumnName("is_published")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(sp => sp.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(sp => sp.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(sp => sp.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(sp => sp.DeletedAt)
            .HasColumnName("deleted_at");

        // Relationships
        builder.HasMany(sp => sp.Documents)
            .WithOne(d => d.StartupProfile)
            .HasForeignKey(d => d.StartupProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Scores)
            .WithOne(s => s.StartupProfile)
            .HasForeignKey(s => s.StartupProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Connections)
            .WithOne(c => c.StartupProfile)
            .HasForeignKey(c => c.StartupProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sp => sp.WatchedBy)
            .WithOne(w => w.StartupProfile)
            .HasForeignKey(w => w.StartupProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sp => sp.ConsultationRequests)
            .WithOne(cr => cr.StartupProfile)
            .HasForeignKey(cr => cr.StartupProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(sp => sp.UserId)
            .IsUnique()
            .HasDatabaseName("ix_startup_profiles_user_id");

        builder.HasIndex(sp => sp.CompanyName)
            .HasDatabaseName("ix_startup_profiles_company_name");

        builder.HasIndex(sp => sp.Industry)
            .HasDatabaseName("ix_startup_profiles_industry");

        builder.HasIndex(sp => sp.Stage)
            .HasDatabaseName("ix_startup_profiles_stage");

        builder.HasIndex(sp => sp.IsPublished)
            .HasDatabaseName("ix_startup_profiles_is_published");

        builder.HasQueryFilter(sp => !sp.IsDeleted);
        builder.Ignore(sp => sp.DomainEvents);
    }
}
