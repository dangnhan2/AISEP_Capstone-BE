namespace AISEP.Domain.Entities;

/// <summary>
/// Advisor profile for mentor/advisor users
/// </summary>
public class AdvisorProfile : BaseEntity
{
    public int UserId { get; private set; }
    public string? ProfessionalTitle { get; private set; }
    public string? Company { get; private set; }
    public string? ExpertiseAreas { get; private set; } // JSON array
    public string? Bio { get; private set; }
    public int YearsOfExperience { get; private set; }
    public string? Certifications { get; private set; } // JSON array
    public string? LinkedInUrl { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public int MaxMenteesPerMonth { get; private set; } = 5;
    public bool IsPublished { get; private set; } = false;

    // Navigation properties
    public User User { get; private set; } = null!;
    public ICollection<Connection> Connections { get; private set; } = new List<Connection>();
    public ICollection<MentorshipSession> MentorshipSessions { get; private set; } = new List<MentorshipSession>();
    public ICollection<ConsultationRequest> ConsultationRequests { get; private set; } = new List<ConsultationRequest>();

    private AdvisorProfile() { }

    public static AdvisorProfile Create(int userId, int yearsOfExperience)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));

        return new AdvisorProfile
        {
            UserId = userId,
            YearsOfExperience = yearsOfExperience,
            IsAvailable = true,
            IsPublished = false
        };
    }

    public void Update(
        string? professionalTitle,
        string? company,
        string? expertiseAreas,
        string? bio,
        int yearsOfExperience,
        string? certifications,
        string? linkedInUrl,
        int maxMenteesPerMonth)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));

        if (maxMenteesPerMonth < 0)
            throw new ArgumentException("Max mentees cannot be negative", nameof(maxMenteesPerMonth));

        ProfessionalTitle = professionalTitle;
        Company = company;
        ExpertiseAreas = expertiseAreas;
        Bio = bio;
        YearsOfExperience = yearsOfExperience;
        Certifications = certifications;
        LinkedInUrl = linkedInUrl;
        MaxMenteesPerMonth = maxMenteesPerMonth;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (string.IsNullOrEmpty(Bio))
            throw new InvalidOperationException("Cannot publish profile without bio");

        IsPublished = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        IsPublished = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
