using CRM_Backend.DTOs.Users;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Backend.Controller.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = "CRM_FULL_ACCESS")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserManagementService _users;

    public AdminUsersController(IUserManagementService users)
    {
        _users = users;
    }

    // --------------------------------------------------
    // CREATE USER
    // --------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var adminId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var userId = await _users.CreateUserAsync(dto, adminId);
        return Ok(new { userId });
    }

    // --------------------------------------------------
    // UPDATE USER (Edit profile & job details)
    // --------------------------------------------------
    [HttpPut("{userId:long}")]
    public async Task<IActionResult> UpdateUser(
        long userId,
        UpdateUserDto dto)
    {
        var adminId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _users.UpdateUserAsync(userId, dto, adminId);
        return Ok();
    }

    // --------------------------------------------------
    // ASSIGN MANAGER TO USER
    // --------------------------------------------------
    [HttpPut("{userId:long}/manager")]
    public async Task<IActionResult> AssignManager(
        long userId,
        AssignManagerDto dto)
    {
        await _users.AssignManagerAsync(userId, dto.ManagerId);
        return Ok();
    }

    // --------------------------------------------------
    // GET EMPLOYEES BY DOMAIN 
    // --------------------------------------------------
    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] string domainCode)
    {
        var employees = await _users.GetEmployeesByDomainAsync(domainCode);
        return Ok(employees);
    }

    // --------------------------------------------------
    // GET MANAGERS BY DOMAIN 
    // --------------------------------------------------
    [HttpGet("managers")]
    public async Task<IActionResult> GetManagers(
      [FromQuery] string domainCode,
      [FromQuery] string roleCode)
    {
        var managers = await _users.GetManagersByDomainAsync(domainCode, roleCode);
        return Ok(managers);
    }


    // --------------------------------------------------
    // GET SINGLE USER DETAILS
    // --------------------------------------------------
    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetUser(long userId)
    {
        var user = await _users.GetUserDetailsAsync(userId);
        return Ok(user);
    }

    // --------------------------------------------------
    // GET TEAM BY MANAGER 
    // --------------------------------------------------
    [HttpGet("{managerId:long}/team")]
    public async Task<IActionResult> GetTeam(long managerId)
    {
        var team = await _users.GetTeamByManagerAsync(managerId);
        return Ok(team);
    }

    // --------------------------------------------------
    // LOCK USER
    // --------------------------------------------------
    [HttpPut("{userId:long}/lock")]
    public async Task<IActionResult> LockUser(
        long userId,
        LockUserDto dto)
    {
        var adminId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _users.LockUserAsync(userId, dto.Reason, adminId);
        return Ok();
    }

    // --------------------------------------------------
    // UNLOCK USER
    // --------------------------------------------------
    [HttpPut("{userId:long}/unlock")]
    public async Task<IActionResult> UnlockUser(long userId)
    {
        var adminId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _users.UnlockUserAsync(userId, adminId);
        return Ok();
    }

    // --------------------------------------------------
    // ADMIN USER LIST (Search + Filters)
    // --------------------------------------------------
    [HttpGet("admin-list")]
    public async Task<IActionResult> GetAdminUsers(
        [FromQuery] string domainCode,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? roleCode)
    {
        var users = await _users.GetAdminUsersByDomainAsync(
            domainCode,
            search,
            status,
            roleCode
        );

        return Ok(users);
    }



}
