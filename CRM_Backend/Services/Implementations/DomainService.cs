using CRM_Backend.DTOs.Domains;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;
using DomainEntity = CRM_Backend.Domain.Entities.Domain;
using System.Linq;

namespace CRM_Backend.Services.Implementations;

public class DomainService : IDomainService
{
    private readonly IDomainRepository _domains;

    public DomainService(IDomainRepository domains)
    {
        _domains = domains;
    }

    private static DomainResponseDto Map(DomainEntity domain)
    {
        return new DomainResponseDto
        {
            DomainId = domain.DomainId,
            DomainCode = domain.DomainCode,
            DomainName = domain.DomainName,
            Active = domain.Active
        };
    }

    public async Task<DomainResponseDto> CreateAsync(CreateDomainDto dto)
    {
        var domain = await _domains.CreateAsync(dto.DomainCode, dto.DomainName);
        return Map(domain);
    }

    public async Task<List<DomainResponseDto>> GetAllAsync()
    {
        var domains = await _domains.GetAllAsync();
        return domains.Select(d => Map(d)).ToList();
    }
}
