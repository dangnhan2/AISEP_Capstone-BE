namespace AISEP.Domain.Enums;

/// <summary>
/// Types of audit actions tracked in the system
/// </summary>
public enum AuditAction
{
    Create,
    Update,
    Delete,
    Login,
    Logout,
    Export,
    Import,
    Approve,
    Reject,
    StatusChange,
    DocumentUpload,
    DocumentDelete,
    ConnectionRequest,
    ConnectionAccept,
    ConnectionDecline,
    InvestmentProposal,
    PaymentProcessed,
    ScoreGenerated
}
