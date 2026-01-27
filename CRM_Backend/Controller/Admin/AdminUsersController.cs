using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = "USER_VIEW")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserListService _listService;
    private readonly IAdminUserDetailsService _detailsService;
    private readonly IAdminUserSecurityService _securityService;
    private readonly IAdminUserAuditLogService _auditLogService;

    public AdminUsersController(
        IAdminUserListService listService,
        IAdminUserDetailsService detailsService,
        IAdminUserSecurityService securityService,
        IAdminUserAuditLogService auditLogService)
    {
        _listService = listService;
        _detailsService = detailsService;
        _securityService = securityService;
        _auditLogService = auditLogService;
    }

    // LIST
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        return Ok(await _listService.GetUsersAsync(page, pageSize));
    }

    // DETAILS
    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetUserDetails(long userId)
    {
        return Ok(await _detailsService.GetUserDetailsAsync(userId));
    }

    // SECURITY
    [HttpGet("{userId:long}/security")]
    public async Task<IActionResult> GetUserSecurity(long userId)
    {
        return Ok(await _securityService.GetUserSecurityAsync(userId));
    }

    // AUDIT LOGS
    [HttpGet("{userId:long}/audit-logs")]
    public async Task<IActionResult> GetUserAuditLogs(long userId)
    {
        return Ok(await _auditLogService.GetUserAuditLogsAsync(userId));
    }
}
