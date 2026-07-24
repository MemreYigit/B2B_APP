using EDG_B2B.Entity.Enums;

namespace EDG_B2B.Entity
{
    // Sepet onaylandığında oluşan sipariş.
    public class Siparis
    {
        public Guid Id { get; set; }

        public required Guid SepetId { get; set; }

        public SiparisDurumu Durum { get; set; } = SiparisDurumu.Onaylandi;

        // Sepeti onaylayıp siparişe dönüştüren satış temsilcisi
        public required Guid OnaylayanSatisBayiiId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Sepet Sepet { get; set; } = null!;

        public SatisBayii OnaylayanSatisBayii { get; set; } = null!;

        public ICollection<SiparisUrun> SiparisUrunleri { get; set; } = new List<SiparisUrun>();

        public Fatura? Fatura { get; set; }
    }

    // Sipariş satırı: hangi üründen ne kadar, hangi fiyatla sipariş edildiği.
    public class SiparisUrun
    {
        public Guid Id { get; set; }

        public required Guid SiparisId { get; set; }

        public required Guid UrunId { get; set; }

        public decimal Miktar { get; set; }

        public decimal BirimFiyat { get; set; }

        public decimal SatirToplam { get; set; }

        // Navigation properties
        public Siparis Siparis { get; set; } = null!;

        public Urun Urun { get; set; } = null!;
    }
}
