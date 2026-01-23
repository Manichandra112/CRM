using CRM_Backend.DTOs.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/employees")]
[Authorize(Policy = "EMP_VIEW")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] EmployeeFilterDto filter)
    {
        var userId = long.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var roles = User.FindAll(ClaimTypes.Role)
                        .Select(r => r.Value)
                        .ToList();

        var permissions = User.FindAll("perm")
                              .Select(p => p.Value)
                              .ToList();

        var result = await _service.GetVisibleEmployeesAsync(
            userId, roles, permissions, filter
        );

        return Ok(result);
    }
}
