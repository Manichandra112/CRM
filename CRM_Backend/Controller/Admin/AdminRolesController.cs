using CRM_Backend.DTOs.Roles;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/roles")]
[Authorize(Policy = "CRM_FULL_ACCESS")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

public class AdminRolesController : ControllerBase
{
    private readonly IRoleService _roles;

    public AdminRolesController(IRoleService roles)
    {
        _roles = roles;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        return Ok(await _roles.CreateAsync(dto));
    }

    // ✅ GET ALL OR FILTER BY DOMAIN
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? domainCode)
    {
        if (!string.IsNullOrWhiteSpace(domainCode))
        {
            return Ok(await _roles.GetByDomainAsync(domainCode));
        }

        return Ok(await _roles.GetAllAsync());
    }
}
