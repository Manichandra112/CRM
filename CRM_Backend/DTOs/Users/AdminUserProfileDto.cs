namespace CRM_Backend.DTOs.Users
{
    public class AdminUserProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MobileNumber { get; set; }
        public string? Gender { get; set; }
        public string? LanguagePreference { get; set; }
        public string? Timezone { get; set; }
    }
}
