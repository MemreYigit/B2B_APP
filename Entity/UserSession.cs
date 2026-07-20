namespace WebApplication1.Entity
{
    public class UserSession
    {
        public int Id { get; set; }

        public required string Jti { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime? RevokedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
