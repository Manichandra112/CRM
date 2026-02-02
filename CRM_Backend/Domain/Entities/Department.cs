namespace CRM_Backend.Domain.Entities
{
    public class Department
    {
        public long Id { get; set; }

        public string DomainCode { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
