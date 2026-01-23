namespace CRM_Backend.DTOs.Employees
{
    public class EmployeeFilterDto
    {
        public string? DomainCode { get; set; }
        public long? ManagerId { get; set; }
        public string? Search { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}
