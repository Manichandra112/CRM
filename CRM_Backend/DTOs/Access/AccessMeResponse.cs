namespace CRM_Backend.DTOs.Access
{
    public class AccessMeResponse
    {
        public AccessUserDto User { get; set; }
        public List<AccessRoleDto> Roles { get; set; }
        public List<AccessDomainDto> Domains { get; set; }
        public List<string> PermissionsFlat { get; set; }
    }
}
