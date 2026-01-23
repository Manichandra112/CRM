namespace CRM_Backend.DTOs.Roles;

public class CreateRoleDto
{
    public string RoleName { get; set; } = null!;
    public string RoleCode { get; set; } = null!;
    public string? Description { get; set; }
}
