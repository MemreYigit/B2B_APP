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
        public int CompanyId { get; set; }

        public string? PhoneNumber { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Pending;

        public UserRole Role { get; set; } = UserRole.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedByAdminId { get; set; }

        public string? ApprovalNotes { get; set; }

        // E-posta doğrulaması: kayıt sonrası gönderilen linkteki token doğrulanınca
        // IsEmailVerified true olur. Admin onayı (Status) ile karıştırılmamalı;
        // örn. admin onay ekranı, doğrulanmamış e-postalı kullanıcıları filtreleyebilir.
        public bool IsEmailVerified { get; set; } = false;

        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerifiedAt { get; set; }

        // Navigation properties
        public Company Company { get; set; } = null!;

        public User? ApprovedByAdmin { get; set; }

        public ICollection<UserDocument> Documents { get; set; } = new List<UserDocument>();
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
