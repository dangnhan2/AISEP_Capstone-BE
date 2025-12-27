using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISEP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    email_verified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    phone_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    profile_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "advisor_profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    professional_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    expertise_areas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    years_of_experience = table.Column<int>(type: "int", nullable: false),
                    certifications = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    linkedin_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_available = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    max_mentees_per_month = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    is_published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advisor_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_advisor_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "investor_profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    organization_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    investment_thesis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    preferred_industries = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    preferred_stages = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    min_investment_size = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    max_investment_size = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    geographic_focus = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    portfolio_companies = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investor_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_investor_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "startup_profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    logo_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    cover_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    founding_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    team_size = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    funding_amount_sought = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    is_published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_startup_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_startup_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemConfigs_users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartupProfileId = table.Column<int>(type: "int", nullable: false),
                    InvestorProfileId = table.Column<int>(type: "int", nullable: true),
                    AdvisorProfileId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclinedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_advisor_profiles_AdvisorProfileId",
                        column: x => x.AdvisorProfileId,
                        principalTable: "advisor_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Connections_investor_profiles_InvestorProfileId",
                        column: x => x.InvestorProfileId,
                        principalTable: "investor_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Connections_startup_profiles_StartupProfileId",
                        column: x => x.StartupProfileId,
                        principalTable: "startup_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartupProfileId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_startup_profiles_StartupProfileId",
                        column: x => x.StartupProfileId,
                        principalTable: "startup_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "investment_proposals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    investor_profile_id = table.Column<int>(type: "int", nullable: false),
                    startup_profile_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    proposed_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    equity_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    valuation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    investment_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    terms = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    term_sheet_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    due_diligence_deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    response_deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    accepted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    declined_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    decline_reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investment_proposals", x => x.id);
                    table.ForeignKey(
                        name: "FK_investment_proposals_investor_profiles_investor_profile_id",
                        column: x => x.investor_profile_id,
                        principalTable: "investor_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_investment_proposals_startup_profiles_startup_profile_id",
                        column: x => x.startup_profile_id,
                        principalTable: "startup_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestorWatchlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvestorProfileId = table.Column<int>(type: "int", nullable: false),
                    StartupProfileId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsNotificationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastViewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorWatchlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorWatchlists_investor_profiles_InvestorProfileId",
                        column: x => x.InvestorProfileId,
                        principalTable: "investor_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestorWatchlists_startup_profiles_StartupProfileId",
                        column: x => x.StartupProfileId,
                        principalTable: "startup_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StartupScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartupProfileId = table.Column<int>(type: "int", nullable: false),
                    OverallScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TeamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarketScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TractionScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinancialsScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reasoning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StartupScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StartupScores_startup_profiles_StartupProfileId",
                        column: x => x.StartupProfileId,
                        principalTable: "startup_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentorshipSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionItems = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductivityRating = table.Column<int>(type: "int", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorshipSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorshipSessions_Connections_ConnectionId",
                        column: x => x.ConnectionId,
                        principalTable: "Connections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    ConnectionId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplyToMessageId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Connections_ConnectionId",
                        column: x => x.ConnectionId,
                        principalTable: "Connections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Messages_ReplyToMessageId",
                        column: x => x.ReplyToMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockchainTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    TransactionHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlockNumber = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GasUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockchainTransactions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExtractedText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnalysisResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAnalyses_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartupProfileId = table.Column<int>(type: "int", nullable: false),
                    AdvisorProfileId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedOutcome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclinedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclineReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MentorshipSessionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_MentorshipSessions_MentorshipSessionId",
                        column: x => x.MentorshipSessionId,
                        principalTable: "MentorshipSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_advisor_profiles_AdvisorProfileId",
                        column: x => x.AdvisorProfileId,
                        principalTable: "advisor_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationRequests_startup_profiles_StartupProfileId",
                        column: x => x.StartupProfileId,
                        principalTable: "startup_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "consultation_reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mentorship_session_id = table.Column<int>(type: "int", nullable: false),
                    consultation_request_id = table.Column<int>(type: "int", nullable: false),
                    summary = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    key_discussion_points = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    action_items = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    recommended_resources = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    follow_up_notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    next_session_suggested = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_shared_with_startup = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    shared_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by_user_id = table.Column<int>(type: "int", nullable: false),
                    MentorshipSessionId1 = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultation_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_consultation_reports_ConsultationRequests_consultation_request_id",
                        column: x => x.consultation_request_id,
                        principalTable: "ConsultationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_consultation_reports_MentorshipSessions_MentorshipSessionId1",
                        column: x => x.MentorshipSessionId1,
                        principalTable: "MentorshipSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_consultation_reports_MentorshipSessions_mentorship_session_id",
                        column: x => x.mentorship_session_id,
                        principalTable: "MentorshipSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_consultation_reports_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MentorshipSessionId = table.Column<int>(type: "int", nullable: false),
                    ConsultationReportId = table.Column<int>(type: "int", nullable: false),
                    ProvidedByUserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHelpful = table.Column<bool>(type: "bit", nullable: false),
                    WouldRecommend = table.Column<bool>(type: "bit", nullable: false),
                    SpecificFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImprovementSuggestions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationFeedbacks_MentorshipSessions_MentorshipSessionId",
                        column: x => x.MentorshipSessionId,
                        principalTable: "MentorshipSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationFeedbacks_consultation_reports_ConsultationReportId",
                        column: x => x.ConsultationReportId,
                        principalTable: "consultation_reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationFeedbacks_users_ProvidedByUserId",
                        column: x => x.ProvidedByUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_advisor_profiles_is_available",
                table: "advisor_profiles",
                column: "is_available");

            migrationBuilder.CreateIndex(
                name: "ix_advisor_profiles_is_published",
                table: "advisor_profiles",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ix_advisor_profiles_user_id",
                table: "advisor_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainTransactions_DocumentId",
                table: "BlockchainTransactions",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Connections_AdvisorProfileId",
                table: "Connections",
                column: "AdvisorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_InvestorProfileId",
                table: "Connections",
                column: "InvestorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_StartupProfileId",
                table: "Connections",
                column: "StartupProfileId");

            migrationBuilder.CreateIndex(
                name: "ix_consultation_reports_consultation_request_id",
                table: "consultation_reports",
                column: "consultation_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_consultation_reports_created_by_user_id",
                table: "consultation_reports",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_consultation_reports_mentorship_session_id",
                table: "consultation_reports",
                column: "mentorship_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_consultation_reports_MentorshipSessionId1",
                table: "consultation_reports",
                column: "MentorshipSessionId1",
                unique: true,
                filter: "[MentorshipSessionId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationFeedbacks_ConsultationReportId",
                table: "ConsultationFeedbacks",
                column: "ConsultationReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationFeedbacks_MentorshipSessionId",
                table: "ConsultationFeedbacks",
                column: "MentorshipSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationFeedbacks_ProvidedByUserId",
                table: "ConsultationFeedbacks",
                column: "ProvidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_AdvisorProfileId",
                table: "ConsultationRequests",
                column: "AdvisorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_MentorshipSessionId",
                table: "ConsultationRequests",
                column: "MentorshipSessionId",
                unique: true,
                filter: "[MentorshipSessionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_StartupProfileId",
                table: "ConsultationRequests",
                column: "StartupProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAnalyses_DocumentId",
                table: "DocumentAnalyses",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_StartupProfileId",
                table: "Documents",
                column: "StartupProfileId");

            migrationBuilder.CreateIndex(
                name: "ix_investment_proposals_created_at",
                table: "investment_proposals",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_investment_proposals_investor_profile_id",
                table: "investment_proposals",
                column: "investor_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_investment_proposals_investor_status",
                table: "investment_proposals",
                columns: new[] { "investor_profile_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_investment_proposals_startup_profile_id",
                table: "investment_proposals",
                column: "startup_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_investment_proposals_startup_status",
                table: "investment_proposals",
                columns: new[] { "startup_profile_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_investment_proposals_status",
                table: "investment_proposals",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_investor_profiles_is_published",
                table: "investor_profiles",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ix_investor_profiles_user_id",
                table: "investor_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestorWatchlists_InvestorProfileId",
                table: "InvestorWatchlists",
                column: "InvestorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorWatchlists_StartupProfileId",
                table: "InvestorWatchlists",
                column: "StartupProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorshipSessions_ConnectionId",
                table: "MentorshipSessions",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConnectionId",
                table: "Messages",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyToMessageId",
                table: "Messages",
                column: "ReplyToMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_startup_profiles_company_name",
                table: "startup_profiles",
                column: "company_name");

            migrationBuilder.CreateIndex(
                name: "ix_startup_profiles_industry",
                table: "startup_profiles",
                column: "industry");

            migrationBuilder.CreateIndex(
                name: "ix_startup_profiles_is_published",
                table: "startup_profiles",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ix_startup_profiles_stage",
                table: "startup_profiles",
                column: "stage");

            migrationBuilder.CreateIndex(
                name: "ix_startup_profiles_user_id",
                table: "startup_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StartupScores_StartupProfileId",
                table: "StartupScores",
                column: "StartupProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigs_ModifiedByUserId",
                table: "SystemConfigs",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_is_active",
                table: "users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_users_role",
                table: "users",
                column: "role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BlockchainTransactions");

            migrationBuilder.DropTable(
                name: "ConsultationFeedbacks");

            migrationBuilder.DropTable(
                name: "DocumentAnalyses");

            migrationBuilder.DropTable(
                name: "investment_proposals");

            migrationBuilder.DropTable(
                name: "InvestorWatchlists");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "StartupScores");

            migrationBuilder.DropTable(
                name: "SystemConfigs");

            migrationBuilder.DropTable(
                name: "consultation_reports");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "ConsultationRequests");

            migrationBuilder.DropTable(
                name: "MentorshipSessions");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "advisor_profiles");

            migrationBuilder.DropTable(
                name: "investor_profiles");

            migrationBuilder.DropTable(
                name: "startup_profiles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
