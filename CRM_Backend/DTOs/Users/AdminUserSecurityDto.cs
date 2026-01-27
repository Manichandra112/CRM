namespace CRM_Backend.DTOs.Users
{
    public class AdminUserSecurityDto
    {
        public bool ForcePasswordReset { get; set; }
        public bool MfaEnabled { get; set; }
        public string? MfaType { get; set; }

        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }
        public string? LastLoginDevice { get; set; }
        public string? LastLoginLocation { get; set; }

        public int FailedLoginCount { get; set; }
        public DateTime? LockedUntil { get; set; }

        public DateTime? PasswordLastChangedAt { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        
    }
}
