using EDG_B2B.Entity.Enums;

namespace EDG_B2B.Entity
{
    public class Sepet
    {
        public Guid Id { get; set; }

        public required Guid BayiId { get; set; }

        public SepetDurumu Durum { get; set; } = SepetDurumu.Aktif;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Bayi Bayi { get; set; } = null!;

        public ICollection<SepetUrun> SepetUrunleri { get; set; } = new List<SepetUrun>();

        // Sepet onaylandığında oluşan sipariş (1-1)
        public Siparis? Siparis { get; set; }
    }

    // Sepet-Urun ilişkisi: hangi üründen ne kadar, hangi fiyatla (snapshot) sepete eklendiği.
    public class SepetUrun
    {
        public Guid Id { get; set; }

        public required Guid SepetId { get; set; }

        public required Guid UrunId { get; set; }

        public decimal Miktar { get; set; }

        // Ekleme anındaki fiyatın anlık görüntüsü (BayiUrunFiyat varsa o, yoksa Urun.StandartFiyat)
        public decimal BirimFiyat { get; set; }

        // Navigation properties
        public Sepet Sepet { get; set; } = null!;

        public Urun Urun { get; set; } = null!;
    }
}
