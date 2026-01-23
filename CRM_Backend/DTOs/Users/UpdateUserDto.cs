namespace CRM_Backend.DTOs.Users;

public class UpdateUserDto
{
    public string? Department { get; set; }
    public string? Designation { get; set; }

    public string? EmploymentType { get; set; }
    public string? WorkShift { get; set; }

    public string? AssignedRegion { get; set; }
    public string? AssignedBranch { get; set; }

    public string? Remarks { get; set; }

    // Profile updates
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }
}
