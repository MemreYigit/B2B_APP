namespace EDG_B2B.Entity
{
    // Belirli bir Bayi'ye, belirli bir Urun için tanımlanmış özel fiyat.
    // Kayıt yoksa (veya geçerlilik aralığı dışındaysa), Urun.StandartFiyat kullanılır.
    public class BayiUrunFiyat
    {
        public Guid Id { get; set; }

        public required Guid BayiId { get; set; }

        public required Guid UrunId { get; set; }

        public decimal OzelFiyat { get; set; }

        // Kampanya/dönem bazlı fiyat için opsiyonel geçerlilik aralığı.
        // Her ikisi de null ise fiyat süresiz geçerlidir.
        public DateTime? GecerlilikBaslangic { get; set; }

        public DateTime? GecerlilikBitis { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Bayi Bayi { get; set; } = null!;

        public Urun Urun { get; set; } = null!;
    }
}
