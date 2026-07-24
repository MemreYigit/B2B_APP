using EDG_B2B.Entity.Enums;

namespace EDG_B2B.Entity
{
    public class Fatura
    {
        public Guid Id { get; set; }

        public required Guid SiparisId { get; set; }

        public required string FaturaNo { get; set; }

        public decimal Tutar { get; set; }

        public decimal KdvTutar { get; set; }

        public FaturaDurumu Durum { get; set; } = FaturaDurumu.Taslak;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Siparis Siparis { get; set; } = null!;
    }
}
