using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// User entity representing all 5 user types in the system
/// </summary>
public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool EmailVerified { get; private set; } = false;
    public string? PhoneNumber { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    // Navigation properties
    public StartupProfile? StartupProfile { get; private set; }
    public InvestorProfile? InvestorProfile { get; private set; }
    public AdvisorProfile? AdvisorProfile { get; private set; }
    public ICollection<Message> SentMessages { get; private set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; private set; } = new List<Message>();
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();

    // Private constructor for EF Core
    private User() { }

    // Factory method for creating new users
    public static User Create(string email, string fullName, string passwordHash, UserRole role)
    {
        var user = new User
        {
            Email = email.ToLower(),
            FullName = fullName,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true,
            EmailVerified = false
        };

        // TODO: Add domain event UserCreatedEvent
        return user;
    }

    public void UpdateProfile(string fullName, string? phoneNumber, string? profileImageUrl)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        ProfileImageUrl = profileImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
