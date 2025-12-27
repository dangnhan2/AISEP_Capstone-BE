namespace AISEP.Domain.Enums;

/// <summary>
/// Defines the 5 main user roles in the AISEP platform
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Startup founder - creates profile, uploads documents, gets scored
    /// </summary>
    StartupFounder = 1,

    /// <summary>
    /// Investor - browses startups, views analytics, makes connections
    /// </summary>
    Investor = 2,

    /// <summary>
    /// Advisor/Mentor - provides guidance and mentorship to startups
    /// </summary>
    Advisor = 3,

    /// <summary>
    /// Operations staff - manages applications, events, and moderation
    /// </summary>
    OperationsStaff = 4,

    /// <summary>
    /// Administrator - full system access and configuration
    /// </summary>
    Administrator = 5
}
