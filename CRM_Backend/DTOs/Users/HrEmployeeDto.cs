namespace CRM_Backend.DTOs.Users
{
    public class HrEmployeeDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Designation { get; set; } = null!;
        public long? ManagerId { get; set; }
    }

}
