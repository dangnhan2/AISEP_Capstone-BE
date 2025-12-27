using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Audit log entity for tracking all critical actions in the system
/// Essential for compliance, security, and troubleshooting
/// </summary>
public class AuditLog : BaseEntity
{
    public int? UserId { get; private set; }
    public string UserEmail { get; private set; } = string.Empty;
    public AuditAction Action { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public int? EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public string? AdditionalData { get; private set; }

    // Navigation properties
    public User? User { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        int? userId,
        string userEmail,
        AuditAction action,
        string entityType,
        int? entityId,
        string ipAddress,
        string userAgent,
        string? oldValues = null,
        string? newValues = null,
        string? additionalData = null)
    {
        return new AuditLog
        {
            UserId = userId,
            UserEmail = userEmail,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            AdditionalData = additionalData
        };
    }

    /// <summary>
    /// Factory method for login audit
    /// </summary>
    public static AuditLog CreateLoginAudit(int userId, string userEmail, string ipAddress, string userAgent)
    {
        return Create(userId, userEmail, AuditAction.Login, "User", userId, ipAddress, userAgent);
    }

    /// <summary>
    /// Factory method for entity change audit
    /// </summary>
    public static AuditLog CreateEntityAudit(
        int userId,
        string userEmail,
        AuditAction action,
        string entityType,
        int entityId,
        string ipAddress,
        string userAgent,
        string? oldValues = null,
        string? newValues = null)
    {
        return Create(userId, userEmail, action, entityType, entityId, ipAddress, userAgent, oldValues, newValues);
    }
}
