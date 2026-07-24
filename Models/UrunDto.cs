namespace EDG_B2B.Models
{
    // Bayiye özel fiyatı da içeren ürün görünümü
    public class UrunListItemDto
    {
        public required Guid Id { get; set; }
        public required string Ad { get; set; }
        public required string StokKodu { get; set; }
        public int StokMiktari { get; set; }
        public decimal KdvOrani { get; set; }
        public decimal StandartFiyat { get; set; }

        // Bayiye özel fiyat tanımlıysa dolu, yoksa null
        public decimal? OzelFiyat { get; set; }

        // Bayinin göreceği geçerli fiyat (OzelFiyat varsa o, yoksa StandartFiyat)
        public decimal Fiyat { get; set; }
    }
}
