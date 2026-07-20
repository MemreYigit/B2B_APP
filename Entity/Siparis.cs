namespace WebApplication1.Entity
{
    public class Siparis
    {
        public int Id { get; set; }

        public required string SiparisNo { get; set; }

        public required int CariId { get; set; }

        // Siparişi veren kullanıcı
        public required int UserId { get; set; }

        public DateTime SiparisTarihi { get; set; } = DateTime.UtcNow;

        public bool TeslimEdildiMi { get; set; } = false;

        public DateTime? TeslimTarihi { get; set; }

        public SiparisDurumu Durum { get; set; } = SiparisDurumu.Alindi;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Cari Cari { get; set; } = null!;

        public User User { get; set; } = null!;

        public ICollection<SiparisKalemi> Kalemler { get; set; } = new List<SiparisKalemi>();

        public Fatura? Fatura { get; set; }
    }

    public enum SiparisDurumu
    {
        Alindi = 0,
        Hazirlaniyor = 1,
        KargoyaVerildi = 2,
        TeslimEdildi = 3,
        IptalEdildi = 4
    }

    // Sipariş satırı: hangi üründen ne kadar, hangi fiyat ve iskontoyla sipariş edildiği.
    public class SiparisKalemi
    {
        public int Id { get; set; }

        public required int SiparisId { get; set; }

        public required int UrunId { get; set; }

        public decimal Miktar { get; set; }

        public decimal BirimFiyat { get; set; }

        public decimal IskontoOrani { get; set; } = 0;

        // Navigation properties
        public Siparis Siparis { get; set; } = null!;

        public Urun Urun { get; set; } = null!;
    }
}
