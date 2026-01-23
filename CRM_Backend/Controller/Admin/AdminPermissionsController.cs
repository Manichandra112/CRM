using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.DTOs.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/permissions")]
[Authorize(Policy = "CRM_FULL_ACCESS")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
public class AdminPermissionsController : ControllerBase
{
    private readonly IPermissionRepository _permissions;

    public AdminPermissionsController(IPermissionRepository permissions)
    {
        _permissions = permissions;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePermissionDto dto)
    {
        var id = await _permissions.CreateAsync(
            dto.PermissionCode,
            dto.Description,
            dto.Module
        );

        return Ok(new { permissionId = id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _permissions.GetAllAsync());
    }
}
