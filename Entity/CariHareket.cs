namespace WebApplication1.Entity
{
    public class CariHareket
    {
        public int Id { get; set; }

        public required int CariId { get; set; }

        public DateTime Tarih { get; set; }

        public required int UrunId { get; set; }

        public decimal Miktar { get; set; }

        public decimal BirimFiyat { get; set; }

        // Yüzde iskonto oranı (örn. 10 => %10)
        public decimal IskontoOrani { get; set; } = 0;

        public string? Aciklama { get; set; }

        public CariHareketTipi Tipi { get; set; } = CariHareketTipi.Satis;

        // Navigation properties
        public Cari Cari { get; set; } = null!;

        public Urun Urun { get; set; } = null!;
    }

    public enum CariHareketTipi
    {
        Satis = 0,
        Iade = 1,
        Tahsilat = 2,
        Odeme = 3
    }
}
