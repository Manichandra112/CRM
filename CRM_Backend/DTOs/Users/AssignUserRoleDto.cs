namespace CRM_Backend.DTOs.Users
{
    public class AssignUserRoleDto
    {
        public long UserId { get; set; }
        public string RoleCode { get; set; } = null!;
    }
}
