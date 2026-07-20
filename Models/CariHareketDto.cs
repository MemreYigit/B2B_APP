namespace WebApplication1.Models
{
    // Cari hareket listesi için, frontend'de "expandable row" (satır genişletme)
    // yapısına uygun DTO. Üst seviye alanlar tabloda görünür; Detay, satır
    // genişletildiğinde gösterilecek ek bilgileri taşır.
    public class CariHareketListItemDto
    {
        public required int Id { get; set; }
        public required DateTime Tarih { get; set; }
        public required string UrunAdi { get; set; }
        public required decimal Miktar { get; set; }
        public required decimal BirimFiyat { get; set; }
        public required decimal NetTutar { get; set; }
        public required string HareketTipi { get; set; }

        public required CariHareketDetayDto Detay { get; set; }
    }

    public class CariHareketDetayDto
    {
        public required string UrunKodu { get; set; }
        public decimal BrutTutar { get; set; }
        public decimal IskontoOrani { get; set; }
        public decimal IskontoTutari { get; set; }
        public string? Aciklama { get; set; }
    }
}
