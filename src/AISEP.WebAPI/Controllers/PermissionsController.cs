using AISEP.Application.DTOs.Common;
using AISEP.Application.DTOs;
using AISEP.Application.Interfaces;
using AISEP.Domain.Entities;
using AISEP.Infrastructure.Data;
using AISEP.WebAPI.Extensions;
using static AISEP.WebAPI.Extensions.ApiEnvelopeExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AISEP.WebAPI.Controllers;

[ApiController]
[Route("api/permissions")]
[Authorize(Policy = "AdminOnly")]
public class PermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public PermissionsController(ApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    /// <summary>
    /// Get all permissions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions([FromQuery] string? category = null)
    {
        var query = _context.Permissions.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        var permissions = await query
            .OrderBy(p => p.Category)
            .ThenBy(p => p.PermissionName)
            .ToListAsync();

        var response = permissions.Select(p => new PermissionResponse(
            p.PermissionID,
            p.PermissionName,
            p.Description,
            p.Category
        )).ToList();

        return OkEnvelope<List<PermissionResponse>>(response);
    }

    /// <summary>
    /// Get permission by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPermission(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null)
            return ErrorEnvelope("Permission not found", StatusCodes.Status404NotFound);

        var response = new PermissionResponse(
            permission.PermissionID,
            permission.PermissionName,
            permission.Description,
            permission.Category
        );

        return OkEnvelope<PermissionResponse>(response);
    }

    /// <summary>
    /// Create new permission
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
    {
        var exists = await _context.Permissions.AnyAsync(p => p.PermissionName == request.PermissionName);
        if (exists)
            return ErrorEnvelope("Permission name already exists", StatusCodes.Status409Conflict);

        var permission = new Permission
        {
            PermissionName = request.PermissionName,
            Description = request.Description,
            Category = request.Category
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("CREATE_PERMISSION", "Permission", permission.PermissionID,
            $"PermissionName: {permission.PermissionName}");

        var response = new PermissionResponse(
            permission.PermissionID,
            permission.PermissionName,
            permission.Description,
            permission.Category
        );

        return CreatedEnvelope<PermissionResponse>(response, "Permission created");
    }

    /// <summary>
    /// Update permission
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionRequest request)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null)
            return ErrorEnvelope("Permission not found", StatusCodes.Status404NotFound);

        if (!string.IsNullOrWhiteSpace(request.PermissionName) && request.PermissionName != permission.PermissionName)
        {
            var nameExists = await _context.Permissions.AnyAsync(p => p.PermissionName == request.PermissionName && p.PermissionID != id);
            if (nameExists)
                return ErrorEnvelope("Permission name already exists", StatusCodes.Status409Conflict);
            permission.PermissionName = request.PermissionName;
        }

        if (request.Description != null)
            permission.Description = request.Description;

        if (request.Category != null)
            permission.Category = request.Category;

        await _context.SaveChangesAsync();
        await _auditService.LogAsync("UPDATE_PERMISSION", "Permission", id,
            $"PermissionName: {permission.PermissionName}");

        var response = new PermissionResponse(
            permission.PermissionID,
            permission.PermissionName,
            permission.Description,
            permission.Category
        );

        return OkEnvelope<PermissionResponse>(response, "Permission updated");
    }

    /// <summary>
    /// Delete permission
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var permission = await _context.Permissions
            .Include(p => p.RolePermissions)
            .FirstOrDefaultAsync(p => p.PermissionID == id);

        if (permission == null)
            return ErrorEnvelope("Permission not found", StatusCodes.Status404NotFound);

        if (permission.RolePermissions.Any())
            return ErrorEnvelope("Cannot delete permission that is assigned to roles. Remove from roles first.", StatusCodes.Status400BadRequest);

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("DELETE_PERMISSION", "Permission", id,
            $"PermissionName: {permission.PermissionName}");

        return DeletedEnvelope("Permission deleted");
    }

    /// <summary>
    /// Get roles that have this permission
    /// </summary>
    [HttpGet("{id}/roles")]
    public async Task<IActionResult> GetPermissionRoles(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null)
            return ErrorEnvelope("Permission not found", StatusCodes.Status404NotFound);

        var roles = await _context.RolePermissions
            .Where(rp => rp.PermissionID == id)
            .Include(rp => rp.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp2 => rp2.Permission)
            .Select(rp => rp.Role)
            .ToListAsync();

        var response = roles.Select(r => new RoleResponse(
            r.RoleID,
            r.RoleName,
            r.Description,
            r.CreatedAt,
            r.UpdatedAt,
            r.RolePermissions.Select(rp => new PermissionResponse(
                rp.Permission.PermissionID,
                rp.Permission.PermissionName,
                rp.Permission.Description,
                rp.Permission.Category
            ))
        )).ToList();

        return OkEnvelope<List<RoleResponse>>(response);
    }

    /// <summary>
    /// Get permission categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Permissions
            .Where(p => p.Category != null)
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return OkEnvelope<List<string>>(categories);
    }
}
