namespace CRM_Backend.DTOs.Employees
{
    public class EmployeeDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public long? ManagerId { get; set; }
        public long DomainId { get; set; }
    }
}
