namespace CRM_Backend.DTOs.Roles;

public class RoleResponseDto
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = default!;
    public string RoleCode { get; set; } = default!;
    public string? Description { get; set; }
    public long DomainId { get; set; }
    public bool IsSystemRole { get; set; }
    public bool Active { get; set; }
}
