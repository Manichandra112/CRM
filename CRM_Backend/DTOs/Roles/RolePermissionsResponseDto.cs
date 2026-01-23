namespace CRM_Backend.DTOs.Roles;

public class RolePermissionsResponseDto
{
    public string RoleCode { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
}
