namespace WebApplication1.Models
{
    // Login sonrası dashboard'da gösterilecek cari risk bilgisi.
    public class CariDashboardDto
    {
        public required string CariKodu { get; set; }
        public required string Unvan { get; set; }
        public required decimal ToplamRiskLimiti { get; set; }
        public decimal? GuncelBakiye { get; set; }

        // ToplamRiskLimiti - GuncelBakiye (GuncelBakiye null ise hesaplanmaz)
        public decimal? KullanilabilirLimit { get; set; }
    }
}
