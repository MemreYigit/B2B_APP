namespace EDG_B2B.Entity
{
    public class KullaniciOturumu
    {
        public Guid Id { get; set; }

        public required string Jti { get; set; }

        public Guid KullaniciId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime? RevokedAt { get; set; }

        // Navigation property
        public Kullanici Kullanici { get; set; } = null!;
    }
}
