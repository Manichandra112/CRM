using CRM_Backend.DTOs.Domains;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/domains")]
[Authorize(Policy = "CRM_FULL_ACCESS")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
public class AdminDomainsController : ControllerBase
{
    private readonly IDomainService _domains;

    public AdminDomainsController(IDomainService domains)
    {
        _domains = domains;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDomainDto dto)
    {
        return Ok(await _domains.CreateAsync(dto));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _domains.GetAllAsync());
    }
}
