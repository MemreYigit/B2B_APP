namespace WebApplication1.Entity
{
    // e-Arşiv / e-Fatura sürecinin simülasyonu için üretilen belge kaydı.
    public class Fatura
    {
        public int Id { get; set; }

        public required int SiparisId { get; set; }

        public required string FaturaNo { get; set; }

        public DateTime FaturaTarihi { get; set; } = DateTime.UtcNow;

        public FaturaTipi Tipi { get; set; } = FaturaTipi.EArsiv;

        public FaturaDurumu Durum { get; set; } = FaturaDurumu.Taslak;

        public decimal ToplamTutar { get; set; }

        public decimal KdvTutari { get; set; }

        // Üretilen PDF/UBL-TR belgesinin dosya yolu (simülasyon amaçlı)
        public string? DokumanYolu { get; set; }

        // GİB'e gönderim simülasyonu için referans numarası
        public string? GibReferansNo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public Siparis Siparis { get; set; } = null!;
    }

    public enum FaturaTipi
    {
        EFatura = 0,
        EArsiv = 1
    }

    public enum FaturaDurumu
    {
        Taslak = 0,
        Olusturuldu = 1,
        GibeGonderildi = 2,
        Hata = 3
    }
}
