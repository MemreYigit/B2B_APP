namespace WebApplication1.Entity
{
    public class Urun
    {
        public int Id { get; set; }

        public required string UrunKodu { get; set; }

        public required string UrunAdi { get; set; }

        public string? Aciklama { get; set; }

        public string? Birim { get; set; }

        public int StokMiktari { get; set; }

        public decimal ListeFiyati { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ICollection<CariHareket> CariHareketler { get; set; } = new List<CariHareket>();

        public ICollection<SiparisKalemi> SiparisKalemleri { get; set; } = new List<SiparisKalemi>();

        public ICollection<CariUrunFiyat> CariUrunFiyatlari { get; set; } = new List<CariUrunFiyat>();
    }
}
