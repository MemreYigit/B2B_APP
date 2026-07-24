namespace EDG_B2B.Entity
{
    public class SatisBayii
    {
        public Guid Id { get; set; }

        public required Guid KullaniciId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Kullanici Kullanici { get; set; } = null!;

        // Bir satış temsilcisinin birden fazla bayisi olabilir
        public ICollection<Bayi> Bayiler { get; set; } = new List<Bayi>();
    }
}
