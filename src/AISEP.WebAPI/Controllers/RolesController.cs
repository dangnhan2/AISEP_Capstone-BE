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
[Route("api/roles")]
[Authorize(Policy = "AdminOnly")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public RolesController(ApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.RoleName)
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
    /// Get role by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(int id)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RoleID == id);

        if (role == null)
            return ErrorEnvelope("Role not found", StatusCodes.Status404NotFound);

        var response = new RoleResponse(
            role.RoleID,
            role.RoleName,
            role.Description,
            role.CreatedAt,
            role.UpdatedAt,
            role.RolePermissions.Select(rp => new PermissionResponse(
                rp.Permission.PermissionID,
                rp.Permission.PermissionName,
                rp.Permission.Description,
                rp.Permission.Category
            ))
        );

        return OkEnvelope<RoleResponse>(response);
    }

    /// <summary>
    /// Create new role
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var exists = await _context.Roles.AnyAsync(r => r.RoleName == request.RoleName);
        if (exists)
            return ErrorEnvelope("Role name already exists", StatusCodes.Status409Conflict);

        var role = new Role
        {
            RoleName = request.RoleName,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("CREATE_ROLE", "Role", role.RoleID, $"RoleName: {role.RoleName}");

        var response = new RoleResponse(
            role.RoleID,
            role.RoleName,
            role.Description,
            role.CreatedAt,
            role.UpdatedAt,
            Enumerable.Empty<PermissionResponse>()
        );

        return CreatedEnvelope<RoleResponse>(response, "Role created");
    }

    /// <summary>
    /// Update role
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RoleID == id);

        if (role == null)
            return ErrorEnvelope("Role not found", StatusCodes.Status404NotFound);

        if (!string.IsNullOrWhiteSpace(request.RoleName) && request.RoleName != role.RoleName)
        {
            var nameExists = await _context.Roles.AnyAsync(r => r.RoleName == request.RoleName && r.RoleID != id);
            if (nameExists)
                return ErrorEnvelope("Role name already exists", StatusCodes.Status409Conflict);
            role.RoleName = request.RoleName;
        }

        if (request.Description != null)
            role.Description = request.Description;

        role.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("UPDATE_ROLE", "Role", id, $"RoleName: {role.RoleName}");

        var response = new RoleResponse(
            role.RoleID,
            role.RoleName,
            role.Description,
            role.CreatedAt,
            role.UpdatedAt,
            role.RolePermissions.Select(rp => new PermissionResponse(
                rp.Permission.PermissionID,
                rp.Permission.PermissionName,
                rp.Permission.Description,
                rp.Permission.Category
            ))
        );

        return OkEnvelope<RoleResponse>(response, "Role updated");
    }

    /// <summary>
    /// Delete role
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.RoleID == id);

        if (role == null)
            return ErrorEnvelope("Role not found", StatusCodes.Status404NotFound);

        if (role.UserRoles.Any())
            return ErrorEnvelope("Cannot delete role that is assigned to users", StatusCodes.Status400BadRequest);

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("DELETE_ROLE", "Role", id, $"RoleName: {role.RoleName}");

        return DeletedEnvelope("Role deleted");
    }

    /// <summary>
    /// Assign permission to role
    /// </summary>
    [HttpPost("{id}/permissions")]
    public async Task<IActionResult> AssignPermission(int id, [FromBody] AssignPermissionRequest request)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            return ErrorEnvelope("Role not found", StatusCodes.Status404NotFound);

        var permission = await _context.Permissions.FindAsync(request.PermissionId);
        if (permission == null)
            return ErrorEnvelope("Permission not found", StatusCodes.Status404NotFound);

        var exists = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleID == id && rp.PermissionID == request.PermissionId);
        if (exists)
            return ErrorEnvelope("Permission already assigned to this role", StatusCodes.Status409Conflict);

        var rolePermission = new RolePermission
        {
            RoleID = id,
            PermissionID = request.PermissionId,
            AssignedAt = DateTime.UtcNow
        };

        _context.RolePermissions.Add(rolePermission);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("ASSIGN_PERMISSION_TO_ROLE", "Role", id,
            $"PermissionId: {request.PermissionId}");

        return OkEnvelope<object>(null, $"Permission '{permission.PermissionName}' assigned to role");
    }

    /// <summary>
    /// Remove permission from role
    /// </summary>
    [HttpDelete("{id}/permissions/{permissionId}")]
    public async Task<IActionResult> RemovePermission(int id, int permissionId)
    {
        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleID == id && rp.PermissionID == permissionId);

        if (rolePermission == null)
            return ErrorEnvelope("Permission not assigned to this role", StatusCodes.Status404NotFound);

        _context.RolePermissions.Remove(rolePermission);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("REMOVE_PERMISSION_FROM_ROLE", "Role", id,
            $"PermissionId: {permissionId}");

        return OkEnvelope<object>(null, "Permission removed from role");
    }

    /// <summary>
    /// Get users with this role
    /// </summary>
    [HttpGet("{id}/users")]
    public async Task<IActionResult> GetRoleUsers(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            return ErrorEnvelope("Role not found", StatusCodes.Status404NotFound);

        var users = await _context.UserRoles
            .Where(ur => ur.RoleID == id)
            .Include(ur => ur.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Select(ur => ur.User)
            .ToListAsync();

        var response = users.Select(u => new UserListResponse(
            u.UserID, u.Email, u.UserType, u.IsActive, u.EmailVerified,
            u.CreatedAt, u.LastLoginAt, u.UserRoles.Select(ur => ur.Role.RoleName)
        )).ToList();

        return OkEnvelope<List<UserListResponse>>(response);
    }
}
