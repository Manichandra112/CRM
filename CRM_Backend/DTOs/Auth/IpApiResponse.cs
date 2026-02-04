namespace CRM_Backend.DTOs.Auth
{
    public class IpApiResponse
    {
        public string Status { get; set; } = "";
        public string Country { get; set; } = "";
        public string RegionName { get; set; } = "";
        public string City { get; set; } = "";
    }
}
