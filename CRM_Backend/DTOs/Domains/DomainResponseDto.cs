namespace CRM_Backend.DTOs.Domains;

public class DomainResponseDto
{
    public long DomainId { get; set; }
    public string DomainCode { get; set; } = default!;
    public string DomainName { get; set; } = default!;
    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
