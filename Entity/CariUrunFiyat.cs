namespace WebApplication1.Entity
{
    // Belirli bir Cari'ye, belirli bir Urun için tanımlanmış özel fiyat.
    // Kayıt yoksa, ürünün ListeFiyati kullanılır.
    public class CariUrunFiyat
    {
        public int Id { get; set; }

        public required int CariId { get; set; }

        public required int UrunId { get; set; }

        public decimal OzelFiyat { get; set; }

        // Kampanya/dönem bazlı fiyat için opsiyonel geçerlilik aralığı.
        // Her ikisi de null ise fiyat süresiz geçerlidir.
        public DateTime? GecerlilikBaslangic { get; set; }

        public DateTime? GecerlilikBitis { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Cari Cari { get; set; } = null!;

        public Urun Urun { get; set; } = null!;
    }
}
