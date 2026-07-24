namespace EDG_B2B.Entity
{
    public class Bayi
    {
        public Guid Id { get; set; }

        public required Guid KullaniciId { get; set; }

        // Bir bayinin tek satış temsilcisi olur (nullable: henüz atanmamış olabilir)
        public Guid? SatisBayiiId { get; set; }

        public required string Unvan { get; set; }

        public required string VergiNo { get; set; }

        public string? Adres { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Kullanici Kullanici { get; set; } = null!;

        public SatisBayii? SatisBayii { get; set; }

        public ICollection<BayiUrunFiyat> BayiUrunFiyatlari { get; set; } = new List<BayiUrunFiyat>();

        public ICollection<Sepet> Sepetler { get; set; } = new List<Sepet>();
    }
}
