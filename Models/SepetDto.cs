namespace EDG_B2B.Models
{
    public class SepetItemDto
    {
        public required Guid Id { get; set; }
        public required Guid UrunId { get; set; }
        public required string UrunAdi { get; set; }
        public required string StokKodu { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal SatirToplam { get; set; }
    }

    public class SepetDto
    {
        // Henüz hiç ürün eklenmemişse Id null olur (sepet kaydı oluşturulmamıştır)
        public Guid? Id { get; set; }
        public required string Durum { get; set; }
        public List<SepetItemDto> Urunler { get; set; } = new();
        public decimal ToplamTutar { get; set; }
    }

    public class AddSepetItemRequest
    {
        public required Guid UrunId { get; set; }
        public decimal Miktar { get; set; } = 1;
    }

    public class UpdateSepetItemRequest
    {
        public required decimal Miktar { get; set; }
    }
}
