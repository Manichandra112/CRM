namespace CRM_Backend.DTOs.Roles
{
    public class RoleLookupDto
    {
        public long RoleId { get; set; }
        public string RoleCode { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
