using CRM_Backend.DTOs.Domains;

namespace CRM_Backend.Services.Interfaces;

public interface IDomainService
{
    Task<DomainResponseDto> CreateAsync(CreateDomainDto dto);
    Task<List<DomainResponseDto>> GetAllAsync();
}
