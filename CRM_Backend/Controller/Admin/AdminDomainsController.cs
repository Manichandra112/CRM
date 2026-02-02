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
    public async Task<IActionResult> Create([FromBody] CreateDomainDto dto)
    {
        var created = await _domains.CreateAsync(dto);
        return Ok(created);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var domains = await _domains.GetAllAsync();
        return Ok(domains);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateDomainDto dto)
    {
        var updated = await _domains.UpdateAsync(id, dto);
        return Ok(updated);
    }
}
