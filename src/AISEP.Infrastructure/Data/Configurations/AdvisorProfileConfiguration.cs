using AISEP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEP.Infrastructure.Data.Configurations;

public class AdvisorProfileConfiguration : IEntityTypeConfiguration<AdvisorProfile>
{
    public void Configure(EntityTypeBuilder<AdvisorProfile> builder)
    {
        builder.ToTable("advisor_profiles");

        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(ap => ap.ProfessionalTitle)
            .HasColumnName("professional_title")
            .HasMaxLength(200);

        builder.Property(ap => ap.Company)
            .HasColumnName("company")
            .HasMaxLength(200);

        builder.Property(ap => ap.ExpertiseAreas)
            .HasColumnName("expertise_areas")
            .HasMaxLength(1000);

        builder.Property(ap => ap.Bio)
            .HasColumnName("bio")
            .HasMaxLength(2000);

        builder.Property(ap => ap.YearsOfExperience)
            .HasColumnName("years_of_experience")
            .IsRequired();

        builder.Property(ap => ap.Certifications)
            .HasColumnName("certifications")
            .HasMaxLength(1000);

        builder.Property(ap => ap.LinkedInUrl)
            .HasColumnName("linkedin_url")
            .HasMaxLength(500);

        builder.Property(ap => ap.IsAvailable)
            .HasColumnName("is_available")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(ap => ap.MaxMenteesPerMonth)
            .HasColumnName("max_mentees_per_month")
            .HasDefaultValue(5)
            .IsRequired();

        builder.Property(ap => ap.IsPublished)
            .HasColumnName("is_published")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(ap => ap.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ap => ap.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(ap => ap.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(ap => ap.DeletedAt)
            .HasColumnName("deleted_at");

        // Relationships
        builder.HasMany(ap => ap.Connections)
            .WithOne(c => c.AdvisorProfile)
            .HasForeignKey(c => c.AdvisorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ap => ap.ConsultationRequests)
            .WithOne(cr => cr.AdvisorProfile)
            .HasForeignKey(cr => cr.AdvisorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ap => ap.UserId)
            .IsUnique()
            .HasDatabaseName("ix_advisor_profiles_user_id");

        builder.HasIndex(ap => ap.IsAvailable)
            .HasDatabaseName("ix_advisor_profiles_is_available");

        builder.HasIndex(ap => ap.IsPublished)
            .HasDatabaseName("ix_advisor_profiles_is_published");

        builder.HasQueryFilter(ap => !ap.IsDeleted);
        builder.Ignore(ap => ap.DomainEvents);
        builder.Ignore(ap => ap.MentorshipSessions); // MentorshipSessions are accessed via Connection
    }
}
