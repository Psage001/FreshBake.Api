// Controllers/RolesController.cs
using FreshBake.API.Data;
using FreshBake.API.Dtos.Maintenance;
using FreshBake.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBake.API.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Policy = "AdminOnly")]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController ( AppDbContext context )
    {
        _context = context;
    }

    // GET /api/roles
    [HttpGet]
    public async Task<ActionResult<List<RoleListItemDto>>> GetRoles ()
    {
        var roles = await _context.Roles
            .Select(r => new RoleListItemDto
            {
                RoleId = r.RoleId,
                Name = r.Name,
                Description = r.Description,
            })
            .ToListAsync();

        return Ok(roles);
    }

    // POST /api/roles
    [HttpPost]
    public async Task<ActionResult<RoleListItemDto>> CreateRole ( CreateRoleRequestDto request )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Role name is required." });
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return Ok(new RoleListItemDto
        {
            RoleId = role.RoleId,
            Name = role.Name,
            Description = role.Description,
        });
    }

    // GET /api/roles/{id}/permissions
    [HttpGet("{id}/permissions")]
    public async Task<ActionResult<RolePermissionsDto>> GetRolePermissions ( int id )
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound(new { error = "Role not found." });
        }

        var allPages = await _context.Pages.OrderBy(p => p.Name).ToListAsync();

        var existingPermissions = await _context.RolePagePermissions
            .Where(rp => rp.RoleId == id)
            .ToListAsync();

        var pageDtos = allPages.Select(p => new PagePermissionDto
        {
            PageId = p.PageId,
            PageName = p.Name,
            RouteKey = p.RouteKey,
            CanView = existingPermissions.Any(rp => rp.PageId == p.PageId && rp.CanView),
        }).ToList();

        return Ok(new RolePermissionsDto
        {
            RoleId = role.RoleId,
            RoleName = role.Name,
            Pages = pageDtos,
        });
    }

    // PUT /api/roles/{id}/permissions
    [HttpPut("{id}/permissions")]
    public async Task<IActionResult> UpdateRolePermissions ( int id, UpdateRolePermissionsRequestDto request )
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound(new { error = "Role not found." });
        }

        var existing = await _context.RolePagePermissions
            .Where(rp => rp.RoleId == id)
            .ToListAsync();

        _context.RolePagePermissions.RemoveRange(existing);

        var updated = request.Pages.Select(p => new RolePagePermission
        {
            RoleId = id,
            PageId = p.PageId,
            CanView = p.CanView,
        });

        _context.RolePagePermissions.AddRange(updated);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Permissions updated." });
    }
}