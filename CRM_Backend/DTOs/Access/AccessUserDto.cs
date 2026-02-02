namespace CRM_Backend.DTOs.Access
{
    public class AccessUserDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AccountStatus { get; set; }
        public bool PwdResetRequired { get; set; }
    }
}
