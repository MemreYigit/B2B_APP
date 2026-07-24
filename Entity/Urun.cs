namespace EDG_B2B.Entity
{
    public class Urun
    {
        public Guid Id { get; set; }

        public required string Ad { get; set; }

        public required string StokKodu { get; set; }

        public decimal StandartFiyat { get; set; }

        public int StokMiktari { get; set; }

        public decimal KdvOrani { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<BayiUrunFiyat> BayiUrunFiyatlari { get; set; } = new List<BayiUrunFiyat>();

        public ICollection<SepetUrun> SepetUrunleri { get; set; } = new List<SepetUrun>();

        public ICollection<SiparisUrun> SiparisUrunleri { get; set; } = new List<SiparisUrun>();
    }
}
