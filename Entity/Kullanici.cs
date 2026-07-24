using System.ComponentModel.DataAnnotations;
using EDG_B2B.Entity.Enums;

namespace EDG_B2B.Entity
{
    public class Kullanici
    {
        public Guid Id { get; set; }

        [Required]
        public required string Ad { get; set; }

        [Required]
        public required string Soyad { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public string? Telefon { get; set; }

        public KullaniciRolu Rol { get; set; } = KullaniciRolu.Bayi;

        public bool Aktif { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Onay iş akışı (mevcut admin onay ekranlarının çalışmaya devam etmesi için korundu)
        public KullaniciDurumu Durum { get; set; } = KullaniciDurumu.Beklemede;

        public DateTime? OnaylanmaTarihi { get; set; }

        public Guid? OnaylayanAdminId { get; set; }

        public string? OnayNotu { get; set; }

        public bool EmailDogrulandiMi { get; set; } = false;

        public string? EmailDogrulamaTokeni { get; set; }

        public DateTime? EmailDogrulanmaTarihi { get; set; }

        // Navigation properties
        public Kullanici? OnaylayanAdmin { get; set; }

        public Bayi? Bayi { get; set; }

        public SatisBayii? SatisBayii { get; set; }
    }
}
