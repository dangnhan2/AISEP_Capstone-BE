using AISEP.Application.DTOs.Common;
using AISEP.Application.DTOs.QueryParams;
using AISEP.Application.DTOs.Startup;
using AISEP.Application.Extension;
using AISEP.Application.Interfaces;
using AISEP.Domain.Entities;
using AISEP.Domain.Enums;
using AISEP.Infrastructure.Constant;
using AISEP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AISEP.Infrastructure.Services;

public class StartupService : IStartupService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<StartupService> _logger;
    private readonly ICloudinaryService _cloudinaryService;

    public StartupService(ApplicationDbContext context, IAuditService auditService, ILogger<StartupService> logger, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _auditService = auditService;
        _logger = logger;
        _cloudinaryService = cloudinaryService;
    }

    // ========== STARTUP PROFILE ==========

    public async Task<ApiResponse<StartupMeDto>> CreateStartupAsync(int userId, CreateStartupRequest request)
    {
        // Check if user already has a startup profile
        var exists = await _context.Startups.AnyAsync(s => s.UserID == userId);
        if (exists)
        {
            return ApiResponse<StartupMeDto>.ErrorResponse("STARTUP_PROFILE_EXISTS",
                "You already have a startup profile. Each user can only create one startup.");
        }

        var logoUrl = await _cloudinaryService.UploadImage(request.LogoURL, CloudinarySavingFolder.LogoFolder);

        // Validate industry exists in master data
        if (request.IndustryID.HasValue)
        {
            var industryExists = await _context.Industries
                .AsNoTracking()
                .AnyAsync(i => i.IndustryID == request.IndustryID.Value);

            if (!industryExists)
            {
                return ApiResponse<StartupMeDto>.ErrorResponse("INVALID_INDUSTRY",
                    $"Industry with ID {request.IndustryID} does not exist in master data.");
            }
        }

        var startup = new Startup
        {
            UserID = userId,
            CompanyName = request.CompanyName,
            OneLiner = request.OneLiner,
            Description = request.Description,
            IndustryID = request.IndustryID,
            SubIndustry = request.SubIndustry,
            Stage = request.Stage,
            FoundedDate = request.FoundedDate.HasValue
                ? DateTime.SpecifyKind(request.FoundedDate.Value, DateTimeKind.Utc)
                : null,
            TeamSize = request.TeamSize,
            Location = request.Location,
            Country = request.Country,
            Website = request.Website,
            LogoURL = logoUrl,
            FundingAmountSought = request.FundingAmountSought,
            CurrentFundingRaised = request.CurrentFundingRaised,
            Valuation = request.Valuation,
            ProfileStatus = ProfileStatus.Draft,
            ProfileCompleteness = CalculateProfileCompleteness(request),
            CreatedAt = DateTime.UtcNow
        };

            _context.Startups.Add(startup);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("CREATE_STARTUP", "Startup", startup.StartupID,
            $"CompanyName: {startup.CompanyName}");

        return ApiResponse<StartupMeDto>.SuccessResponse(MapToMeDto(startup), "Startup profile created successfully");
    }

    public async Task<ApiResponse<StartupMeDto>> GetMyStartupAsync(int userId)
    {
        var startup = await _context.Startups
            .AsNoTracking()
            .Include(s => s.TeamMembers)
            .Include(s => s.Industry)
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (startup == null)
        {
            return ApiResponse<StartupMeDto>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                "You haven't created a startup profile yet.");
        }

        return ApiResponse<StartupMeDto>.SuccessResponse(MapToMeDto(startup));
    }

    public async Task<ApiResponse<StartupMeDto>> UpdateStartupAsync(int userId, UpdateStartupRequest request)
    {
        var startup = await _context.Startups
            .Include(s => s.TeamMembers)
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (startup == null)
        {
            return ApiResponse<StartupMeDto>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                "You haven't created a startup profile yet.");
        }

        if (request.LogoURL != null)
        {
            var logoUrl = await _cloudinaryService.UploadImage(request.LogoURL, CloudinarySavingFolder.LogoFolder);
            await _cloudinaryService.DeleteImage(startup.LogoURL);
            startup.LogoURL = logoUrl;
        }

        // Validate industry if provided
        if (request.IndustryID.HasValue)
        {
            var industryExists = await _context.Industries
                .AsNoTracking()
                .AnyAsync(i => i.IndustryID == request.IndustryID.Value);

            if (!industryExists)
            {
                return ApiResponse<StartupMeDto>.ErrorResponse("INVALID_INDUSTRY",
                    $"Industry with ID {request.IndustryID} does not exist in master data.");
            }
        }

        // Apply partial updates (only non-null fields)
        if (request.CompanyName != null) startup.CompanyName = request.CompanyName;
        if (request.OneLiner != null) startup.OneLiner = request.OneLiner;
        if (request.Description != null) startup.Description = request.Description;
        if (request.IndustryID.HasValue) startup.IndustryID = request.IndustryID;
        if (request.SubIndustry != null) startup.SubIndustry = request.SubIndustry;          
        if (request.FoundedDate.HasValue) startup.FoundedDate = DateTime.SpecifyKind(request.FoundedDate.Value, DateTimeKind.Utc);
        if (request.TeamSize.HasValue) startup.TeamSize = request.TeamSize;
        if (request.Location != null) startup.Location = request.Location;
        if (request.Country != null) startup.Country = request.Country;
        if (request.Website != null) startup.Website = request.Website;
        if (request.CoverImageURL != null) startup.CoverImageURL = request.CoverImageURL;
        if (request.FundingAmountSought.HasValue) startup.FundingAmountSought = request.FundingAmountSought;
        if (request.CurrentFundingRaised.HasValue) startup.CurrentFundingRaised = request.CurrentFundingRaised;
        if (request.Valuation.HasValue) startup.Valuation = request.Valuation;

        startup.Stage = request.Stage;
        startup.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("UPDATE_STARTUP", "Startup", startup.StartupID,
            $"Updated fields for {startup.CompanyName}");

        return ApiResponse<StartupMeDto>.SuccessResponse(MapToMeDto(startup), "Startup profile updated successfully");
    }

    public async Task<ApiResponse<StartupMeDto>> SubmitForApprovalAsync(int userId)
    {
        var startup = await _context.Startups
            .Include(s => s.TeamMembers)
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (startup == null)
        {
            return ApiResponse<StartupMeDto>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                "You haven't created a startup profile yet.");
        }

        if (startup.ProfileStatus == ProfileStatus.Pending)
        {
            return ApiResponse<StartupMeDto>.ErrorResponse("ALREADY_PENDING",
                "Your startup profile is already pending approval.");
        }

        if (startup.ProfileStatus == ProfileStatus.Approved)
        {
            return ApiResponse<StartupMeDto>.ErrorResponse("ALREADY_APPROVED",
                "Your startup profile is already approved.");
        }

        startup.ProfileStatus = ProfileStatus.Pending;
        startup.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("SUBMIT_STARTUP_APPROVAL", "Startup", startup.StartupID,
            $"{startup.CompanyName} submitted for approval");

        return ApiResponse<StartupMeDto>.SuccessResponse(MapToMeDto(startup), "Startup submitted for approval");
    }

    // ========== PUBLIC ENDPOINTS ==========

    public async Task<ApiResponse<StartupPublicDto>> GetStartupByIdAsync(int startupId)
    {
        var startup = await _context.Startups
            .AsNoTracking()
            .Include(s => s.TeamMembers)
            .Include(s => s.Industry)
            .FirstOrDefaultAsync(s => s.StartupID == startupId);

        if (startup == null)
        {
            return ApiResponse<StartupPublicDto>.ErrorResponse("STARTUP_NOT_FOUND",
                "Startup not found.");
        }

        return ApiResponse<StartupPublicDto>.SuccessResponse(MapToPublicDto(startup));
    }

    public async Task<ApiResponse<PagedResponse<StartupListItemDto>>> SearchStartupsAsync(StartupQueryParams queryParams)
    {
        var query = _context.Startups.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.Keyword))      
            query = query.Where(s => s.CompanyName.ToLower().Contains(queryParams.Keyword)
            || s.Industry.IndustryName == queryParams.Keyword);      
        
        // Filter by stage
        if (queryParams.Stage.HasValue)      
            query = query.Where(s => s.Stage == queryParams.Stage.Value);
        

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize);

        var items = await query
            .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
            .Paging(queryParams.Page, queryParams.PageSize)
            .Select(s => new StartupListItemDto
            {
                StartupID = s.StartupID,
                CompanyName = s.CompanyName,
                OneLiner = s.OneLiner,
                IndustryName = s.Industry != null ? s.Industry.IndustryName : null,
                SubIndustry = s.SubIndustry,
                Stage = s.Stage != null ? s.Stage.ToString() : null,
                Location = s.Location,
                Country = s.Country,
                LogoURL = s.LogoURL,
                ProfileStatus = s.ProfileStatus.ToString(),
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();

        var result = new PagedResponse<StartupListItemDto>
        {
            Items = items,
            Paging = new PagingInfo
            {
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        };

        return ApiResponse<PagedResponse<StartupListItemDto>>.SuccessResponse(result);
    }

     //========== TEAM MEMBERS ==========

    public async Task<ApiResponse<List<TeamMemberDto>>> GetTeamMembersAsync(int userId)
    {
        var startup = await _context.Startups
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (startup == null)
        {
            return ApiResponse<List<TeamMemberDto>>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                "You haven't created a startup profile yet.");
        }

        var members = await _context.TeamMembers
            .AsNoTracking()
            .Where(tm => tm.StartupID == startup.StartupID)
            .OrderBy(tm => tm.CreatedAt)
            .Select(tm => MapToTeamMemberDto(tm))
            .ToListAsync();

        return ApiResponse<List<TeamMemberDto>>.SuccessResponse(members);
    }

    public async Task<ApiResponse<string>> AddTeamMemberAsync(int userId, CreateTeamMemberRequest request)
    {
        var startup = await _context.Startups
            .Include(s => s.TeamMembers)
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (startup == null)
        {
            return ApiResponse<string>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                "You haven't created a startup profile yet.");
        }

        var photoUrl = await _cloudinaryService.UploadImage(request.PhotoURL, CloudinarySavingFolder.ProfilePicFolder);

        var newMember = new TeamMember
        {
            StartupID = startup.StartupID,
            FullName = request.FullName,
            Role = request.Role,
            Title = request.Title,
            LinkedInURL = request.LinkedInURL,
            Bio = request.Bio,
            PhotoURL = photoUrl,
            IsFounder = request.IsFounder,
            YearsOfExperience = request.YearsOfExperience,
            CreatedAt = DateTime.UtcNow
        };


        startup.TeamMembers.Add(newMember);      
        
        await _context.SaveChangesAsync();

        return ApiResponse<string>.SuccessResponse("", "Team member added");
    }

    public async Task<ApiResponse<string>> UpdateTeamMemberAsync(int userId, UpdateTeamMemberRequest request)
    {
        var startup = await _context.Startups
            .AsNoTracking()
            .Include(s => s.TeamMembers)
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (startup == null)
        {
            return ApiResponse<string>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                "You haven't created a startup profile yet.");
        }

        var member = startup.TeamMembers.FirstOrDefault(x => x.TeamMemberID == request.MemberId);

        if (member == null)
        {
            return ApiResponse<string>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                 "Member not found");
        }

        if (member.FullName != null) member.FullName = request.FullName;
        if (member.Role != null) member.Role = request.Role;
        if (member.Title != null) member.Title = request.Title;
        if (member.LinkedInURL != null) member.LinkedInURL = request.LinkedInURL;
        if (member.Bio != null) member.Bio = request.Bio;
        if (member.IsFounder) member.IsFounder = request.IsFounder.Value;
        if (member.YearsOfExperience.HasValue) member.YearsOfExperience = request.YearsOfExperience;

        if (member.PhotoURL != null)
        {
            var photoUrl = await _cloudinaryService.UploadImage(request.PhotoURL, CloudinarySavingFolder.ProfilePicFolder);
            await _cloudinaryService.DeleteImage(member.PhotoURL);
            member.PhotoURL = photoUrl;
        }
        // Apply partial updates

        await _context.SaveChangesAsync();

        return ApiResponse<string>.SuccessResponse("", "Team member updated");
    }

    public async Task<ApiResponse<string>> DeleteTeamMemberAsync(int userId, int memberId)
    {
        var startup = await _context.Startups
            .Include(x => x.TeamMembers)
            .FirstOrDefaultAsync(x => x.UserID == userId);
           
        if (startup == null)
        {
            return ApiResponse<string>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                "You haven't created a startup profile yet.");
        }

        var member = startup.TeamMembers.FirstOrDefault(x => x.TeamMemberID == memberId);

        if (member == null)
        {
            return ApiResponse<string>.ErrorResponse("STARTUP_PROFILE_NOT_FOUND",
                 "Member not found");
        }

        startup.TeamMembers.Remove(member);

        await _cloudinaryService.DeleteImage(member.PhotoURL);
        await _context.SaveChangesAsync();

        return ApiResponse<string>.SuccessResponse("Team member deleted", "Team member removed successfully");
    }

    // ========== MAPPING HELPERS ==========
    #region helper method
    private static StartupMeDto MapToMeDto(Startup s)
    {
        return new StartupMeDto
        {
            StartupID = s.StartupID,
            UserID = s.UserID,
            CompanyName = s.CompanyName,
            OneLiner = s.OneLiner,
            Description = s.Description,
            IndustryID = s.IndustryID,
            IndustryName = s.Industry?.IndustryName,
            SubIndustry = s.SubIndustry,
            Stage = s.Stage?.ToString(),
            FoundedDate = s.FoundedDate,
            TeamSize = s.TeamSize,
            Location = s.Location,
            Country = s.Country,
            Website = s.Website,
            LogoURL = s.LogoURL,
            CoverImageURL = s.CoverImageURL,
            FundingAmountSought = s.FundingAmountSought,
            CurrentFundingRaised = s.CurrentFundingRaised,
            Valuation = s.Valuation,
            ProfileStatus = s.ProfileStatus.ToString(),
            ProfileCompleteness = s.ProfileCompleteness,
            ApprovedAt = s.ApprovedAt,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            TeamMembers = s.TeamMembers?.Select(MapToTeamMemberDto).ToList() ?? new()
        };
    }

    private static StartupPublicDto MapToPublicDto(Startup s)
    {
        return new StartupPublicDto
        {
            StartupID = s.StartupID,
            CompanyName = s.CompanyName,
            OneLiner = s.OneLiner,
            Description = s.Description,
            IndustryID = s.IndustryID,
            IndustryName = s.Industry?.IndustryName,
            SubIndustry = s.SubIndustry,
            Stage = s.Stage?.ToString(),
            FoundedDate = s.FoundedDate,
            TeamSize = s.TeamSize,
            Location = s.Location,
            Country = s.Country,
            Website = s.Website,
            LogoURL = s.LogoURL,
            CoverImageURL = s.CoverImageURL,
            FundingAmountSought = s.FundingAmountSought,
            CurrentFundingRaised = s.CurrentFundingRaised,
            ProfileStatus = s.ProfileStatus.ToString(),
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            TeamMembers = s.TeamMembers?.Select(tm => new TeamMemberPublicDto
            {
                FullName = tm.FullName,
                Role = tm.Role,
                Title = tm.Title,
                LinkedInURL = tm.LinkedInURL,
                Bio = tm.Bio,
                PhotoURL = tm.PhotoURL,
                IsFounder = tm.IsFounder
            }).ToList() ?? new()
        };
    }

    private static TeamMemberDto MapToTeamMemberDto(TeamMember tm)
    {   

        return new TeamMemberDto
        {
            TeamMemberID = tm.TeamMemberID,
            FullName = tm.FullName,
            Role = tm.Role,
            Title = tm.Title,
            LinkedInURL = tm.LinkedInURL,
            Bio = tm.Bio,
            PhotoURL = tm.PhotoURL,
            IsFounder = tm.IsFounder,
            YearsOfExperience = tm.YearsOfExperience,
            CreatedAt = tm.CreatedAt
        };
    }

    private static int CalculateProfileCompleteness(CreateStartupRequest r)
    {
        var fields = new object?[]
        {
            r.CompanyName, r.OneLiner, r.Description, r.IndustryID,
            r.Stage, r.Location, r.Country, r.Website,
            r.FoundedDate, r.TeamSize
        };
        var filled = fields.Count(f => f != null && f.ToString() != string.Empty);
        return (int)Math.Round(filled * 100.0 / fields.Length);
    }
    #endregion
}
