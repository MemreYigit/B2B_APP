namespace WebApplication1.Entity
{
    public class Cari
    {
        public int Id { get; set; }

        // Netsis ERP tarafındaki cari kodu (senkronizasyon anahtarı)
        public required string CariKodu { get; set; }

        public required string Unvan { get; set; }

        public string? VergiNumarasi { get; set; }

        // Login sonrası dashboard'da gösterilecek toplam risk limiti
        public decimal ToplamRiskLimiti { get; set; }

        public decimal? GuncelBakiye { get; set; }

        public required int CompanyId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Company Company { get; set; } = null!;

        public ICollection<CariHareket> CariHareketler { get; set; } = new List<CariHareket>();

        public ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();

        public ICollection<CariUrunFiyat> CariUrunFiyatlari { get; set; } = new List<CariUrunFiyat>();
    }
}
