using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entity
{
    public class User
    {   
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string CompanyName { get; set; }

        public string? PhoneNumber { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Pending; // Pending, Approved, Rejected

        public UserRole Role { get; set; } = UserRole.User; // Admin, User

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedByAdminId { get; set; }

        public string? ApprovalNotes { get; set; }
    }

    public enum UserStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum UserRole
    {
        Admin = 0,
        User = 1
    }
}
