using AISEP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISEP.Infrastructure.Data.Configurations;

public class ConsultationReportConfiguration : IEntityTypeConfiguration<ConsultationReport>
{
    public void Configure(EntityTypeBuilder<ConsultationReport> builder)
    {
        builder.ToTable("consultation_reports");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.MentorshipSessionId)
            .HasColumnName("mentorship_session_id")
            .IsRequired();

        builder.Property(cr => cr.ConsultationRequestId)
            .HasColumnName("consultation_request_id")
            .IsRequired();

        builder.Property(cr => cr.Summary)
            .HasColumnName("summary")
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(cr => cr.KeyDiscussionPoints)
            .HasColumnName("key_discussion_points")
            .HasMaxLength(5000);

        builder.Property(cr => cr.ActionItems)
            .HasColumnName("action_items")
            .HasMaxLength(5000);

        builder.Property(cr => cr.RecommendedResources)
            .HasColumnName("recommended_resources")
            .HasMaxLength(2000);

        builder.Property(cr => cr.FollowUpNotes)
            .HasColumnName("follow_up_notes")
            .HasMaxLength(2000);

        builder.Property(cr => cr.NextSessionSuggested)
            .HasColumnName("next_session_suggested");

        builder.Property(cr => cr.IsSharedWithStartup)
            .HasColumnName("is_shared_with_startup")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(cr => cr.SharedAt)
            .HasColumnName("shared_at");

        builder.Property(cr => cr.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(cr => cr.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(cr => cr.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(cr => cr.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(cr => cr.DeletedAt)
            .HasColumnName("deleted_at");

        // Relationships - Use Restrict to avoid cascade path conflicts
        builder.HasOne(cr => cr.MentorshipSession)
            .WithMany()
            .HasForeignKey(cr => cr.MentorshipSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.ConsultationRequest)
            .WithMany()
            .HasForeignKey(cr => cr.ConsultationRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.CreatedByUser)
            .WithMany()
            .HasForeignKey(cr => cr.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(cr => cr.MentorshipSessionId)
            .HasDatabaseName("ix_consultation_reports_mentorship_session_id");

        builder.HasIndex(cr => cr.ConsultationRequestId)
            .HasDatabaseName("ix_consultation_reports_consultation_request_id");

        builder.HasIndex(cr => cr.CreatedByUserId)
            .HasDatabaseName("ix_consultation_reports_created_by_user_id");

        builder.HasQueryFilter(cr => !cr.IsDeleted);
        builder.Ignore(cr => cr.DomainEvents);
    }
}
