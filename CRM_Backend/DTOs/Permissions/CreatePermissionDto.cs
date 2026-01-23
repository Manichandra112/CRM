namespace CRM_Backend.DTOs.Permissions;

public class CreatePermissionDto
{
    public string PermissionCode { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Module { get; set; } = null!;
}
