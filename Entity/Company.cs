namespace WebApplication1.Entity
{
    public class Company
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? TaxNumber { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
