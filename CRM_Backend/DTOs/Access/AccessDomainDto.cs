namespace CRM_Backend.DTOs.Access
{
    public class AccessDomainDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<AccessPermissionDto> Permissions { get; set; }
    }
}
