namespace EDG_B2B.Models
{
    public class SiparisItemDto
    {
        public required Guid Id { get; set; }
        public required Guid UrunId { get; set; }
        public required string UrunAdi { get; set; }
        public required string StokKodu { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal SatirToplam { get; set; }
    }

    public class SiparisDto
    {
        public required Guid Id { get; set; }
        public required string Durum { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SiparisItemDto> Urunler { get; set; } = new();
        public decimal ToplamTutar { get; set; }
    }
}
