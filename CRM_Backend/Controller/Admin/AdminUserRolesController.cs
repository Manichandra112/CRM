using CRM_Backend.DTOs.Users;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/user-roles")]
[Authorize(Policy = "CRM_FULL_ACCESS")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
public class AdminUserRolesController : ControllerBase
{
    private readonly IUserManagementService _users;

    public AdminUserRolesController(IUserManagementService users)
    {
        _users = users;
    }

    // Assign role to user
    [HttpPost]
    public async Task<IActionResult> AssignRole(AssignUserRoleDto dto)
    {
        var adminId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _users.AssignRoleToUserAsync(dto.UserId, dto.RoleCode, adminId);
        return Ok();
    }

    // Remove role from user
    [HttpDelete]
    public async Task<IActionResult> RemoveRole(AssignUserRoleDto dto)
    {
        await _users.RemoveRoleFromUserAsync(dto.UserId, dto.RoleCode);
        return Ok();
    }

    // Get roles of a user
    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetUserRoles(long userId)
    {
        var roles = await _users.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}
