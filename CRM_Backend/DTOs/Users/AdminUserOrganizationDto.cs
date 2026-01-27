namespace CRM_Backend.DTOs.Users
{
    public class AdminUserOrganizationDto
    {
        public string? DomainName { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? ManagerName { get; set; }
        public string? AssignedRegion { get; set; }
        public string? AssignedBranch { get; set; }
        public string? EmploymentType { get; set; }
        public string? WorkShift { get; set; }
    }
}
