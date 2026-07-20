using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class UrunDto
    {
        public required int Id { get; set; }
        public required string UrunKodu { get; set; }
        public required string UrunAdi { get; set; }
        public string? Birim { get; set; }

        // Stok 50'den büyükse "50+" (string), değilse gerçek adet (number) döner.
        // System.Text.Json, object tipini runtime türüne göre serileştirir.
        public required object StokDurumu { get; set; }

        public required FiyatDto Fiyat { get; set; }
    }

    // PriceResolver çıktısı (CariUrunFiyat tablosundan çözümlenir).
    // Cari'ye özel fiyat yoksa veya ListeFiyati'na eşitse: sadece "fiyat" alanı döner (tek fiyat).
    // Farklıysa: "listeFiyati" ve "cariyeOzelFiyat" alanlarının ikisi de döner.
    public class FiyatDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Fiyat { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? ListeFiyati { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? CariyeOzelFiyat { get; set; }
    }
}
