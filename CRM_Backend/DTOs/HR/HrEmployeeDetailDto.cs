namespace CRM_Backend.DTOs.HR;

public class HrEmployeeDetailDto
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }

    public string? Department { get; set; }
    public string? Designation { get; set; }

    public long? ManagerId { get; set; }
    public string? ManagerName { get; set; }

    public string AccountStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
