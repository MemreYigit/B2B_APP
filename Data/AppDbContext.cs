using Microsoft.EntityFrameworkCore;
using EDG_B2B.Entity;

namespace EDG_B2B.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<KullaniciOturumu> KullaniciOturumlari { get; set; }
        public DbSet<Bayi> Bayiler { get; set; }
        public DbSet<SatisBayii> SatisBayiler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<BayiUrunFiyat> BayiUrunFiyatlari { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<SepetUrun> SepetUrunleri { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisUrun> SiparisUrunleri { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kullanici configuration
            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.HasKey(k => k.Id);

                entity.Property(k => k.Ad)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(k => k.Soyad)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(k => k.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(k => k.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(k => k.Telefon)
                    .HasMaxLength(20);

                entity.Property(k => k.OnayNotu)
                    .HasMaxLength(1000);

                entity.Property(k => k.EmailDogrulamaTokeni)
                    .HasMaxLength(200);

                entity.HasIndex(k => k.EmailDogrulamaTokeni);

                // Unique index on Email
                entity.HasIndex(k => k.Email)
                    .IsUnique();

                // Self-referencing foreign key for OnaylayanAdmin
                entity.HasOne(k => k.OnaylayanAdmin)
                    .WithMany()
                    .HasForeignKey(k => k.OnaylayanAdminId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Soft delete global query filter
                entity.HasQueryFilter(k => !k.IsDeleted);
            });

            // KullaniciOturumu configuration
            modelBuilder.Entity<KullaniciOturumu>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Jti)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasIndex(s => s.Jti).IsUnique();

                entity.HasIndex(s => new { s.KullaniciId, s.IsRevoked });

                entity.HasOne(s => s.Kullanici)
                    .WithMany()
                    .HasForeignKey(s => s.KullaniciId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Apply same soft delete filter as Kullanici entity
                entity.HasQueryFilter(s => !s.Kullanici.IsDeleted);
            });

            // Bayi configuration
            modelBuilder.Entity<Bayi>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Unvan)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(b => b.VergiNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(b => b.Adres)
                    .HasMaxLength(500);

                entity.HasIndex(b => b.VergiNo)
                    .IsUnique();

                // Bir Kullanici yalnızca bir Bayi ile eşleşir (1-1)
                entity.HasIndex(b => b.KullaniciId)
                    .IsUnique();

                entity.HasOne(b => b.Kullanici)
                    .WithOne(k => k.Bayi)
                    .HasForeignKey<Bayi>(b => b.KullaniciId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Bir bayinin tek satış temsilcisi olur; temsilci silinirse bağ kopar (SetNull)
                entity.HasOne(b => b.SatisBayii)
                    .WithMany(sb => sb.Bayiler)
                    .HasForeignKey(b => b.SatisBayiiId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Apply same soft delete filter as Kullanici entity
                entity.HasQueryFilter(b => !b.Kullanici.IsDeleted);
            });

            // SatisBayii configuration
            modelBuilder.Entity<SatisBayii>(entity =>
            {
                entity.HasKey(sb => sb.Id);

                // Bir Kullanici yalnızca bir SatisBayii ile eşleşir (1-1)
                entity.HasIndex(sb => sb.KullaniciId)
                    .IsUnique();

                entity.HasOne(sb => sb.Kullanici)
                    .WithOne(k => k.SatisBayii)
                    .HasForeignKey<SatisBayii>(sb => sb.KullaniciId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Apply same soft delete filter as Kullanici entity
                entity.HasQueryFilter(sb => !sb.Kullanici.IsDeleted);
            });

            // Urun configuration
            modelBuilder.Entity<Urun>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Ad)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(u => u.StokKodu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.StandartFiyat)
                    .HasPrecision(18, 2);

                entity.Property(u => u.KdvOrani)
                    .HasPrecision(5, 2);

                entity.HasIndex(u => u.StokKodu)
                    .IsUnique();
            });

            // BayiUrunFiyat configuration
            modelBuilder.Entity<BayiUrunFiyat>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.OzelFiyat)
                    .HasPrecision(18, 2);

                // Bir Bayi + Urun kombinasyonu için tek fiyat kaydı
                entity.HasIndex(f => new { f.BayiId, f.UrunId })
                    .IsUnique();

                entity.HasOne(f => f.Bayi)
                    .WithMany(b => b.BayiUrunFiyatlari)
                    .HasForeignKey(f => f.BayiId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Urun)
                    .WithMany(u => u.BayiUrunFiyatlari)
                    .HasForeignKey(f => f.UrunId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Match Bayi's soft delete filter (required relationship)
                entity.HasQueryFilter(f => !f.Bayi.Kullanici.IsDeleted);
            });

            // Sepet configuration
            modelBuilder.Entity<Sepet>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.Bayi)
                    .WithMany(b => b.Sepetler)
                    .HasForeignKey(s => s.BayiId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Match Bayi's soft delete filter (required relationship)
                entity.HasQueryFilter(s => !s.Bayi.Kullanici.IsDeleted);
            });

            // SepetUrun configuration
            modelBuilder.Entity<SepetUrun>(entity =>
            {
                entity.HasKey(su => su.Id);

                entity.Property(su => su.Miktar)
                    .HasPrecision(18, 3);

                entity.Property(su => su.BirimFiyat)
                    .HasPrecision(18, 2);

                entity.HasOne(su => su.Sepet)
                    .WithMany(s => s.SepetUrunleri)
                    .HasForeignKey(su => su.SepetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(su => su.Urun)
                    .WithMany(u => u.SepetUrunleri)
                    .HasForeignKey(su => su.UrunId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Match Sepet's soft delete filter (required relationship)
                entity.HasQueryFilter(su => !su.Sepet.Bayi.Kullanici.IsDeleted);
            });

            // Siparis configuration
            modelBuilder.Entity<Siparis>(entity =>
            {
                entity.HasKey(s => s.Id);

                // Bir Sepet yalnızca bir Siparis'e dönüşür (1-1)
                entity.HasIndex(s => s.SepetId)
                    .IsUnique();

                entity.HasOne(s => s.Sepet)
                    .WithOne(sp => sp.Siparis)
                    .HasForeignKey<Siparis>(s => s.SepetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.OnaylayanSatisBayii)
                    .WithMany()
                    .HasForeignKey(s => s.OnaylayanSatisBayiiId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Match SatisBayii's soft delete filter (required relationship)
                entity.HasQueryFilter(s => !s.OnaylayanSatisBayii.Kullanici.IsDeleted);
            });

            // SiparisUrun configuration
            modelBuilder.Entity<SiparisUrun>(entity =>
            {
                entity.HasKey(su => su.Id);

                entity.Property(su => su.Miktar)
                    .HasPrecision(18, 3);

                entity.Property(su => su.BirimFiyat)
                    .HasPrecision(18, 2);

                entity.Property(su => su.SatirToplam)
                    .HasPrecision(18, 2);

                entity.HasOne(su => su.Siparis)
                    .WithMany(s => s.SiparisUrunleri)
                    .HasForeignKey(su => su.SiparisId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(su => su.Urun)
                    .WithMany(u => u.SiparisUrunleri)
                    .HasForeignKey(su => su.UrunId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Match Siparis's soft delete filter (required relationship)
                entity.HasQueryFilter(su => !su.Siparis.OnaylayanSatisBayii.Kullanici.IsDeleted);
            });

            // Fatura configuration
            modelBuilder.Entity<Fatura>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.FaturaNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(f => f.Tutar)
                    .HasPrecision(18, 2);

                entity.Property(f => f.KdvTutar)
                    .HasPrecision(18, 2);

                entity.HasIndex(f => f.FaturaNo)
                    .IsUnique();

                // Bir sipariş için tek fatura (1-1)
                entity.HasIndex(f => f.SiparisId)
                    .IsUnique();

                entity.HasOne(f => f.Siparis)
                    .WithOne(s => s.Fatura)
                    .HasForeignKey<Fatura>(f => f.SiparisId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Match Siparis's soft delete filter (required relationship)
                entity.HasQueryFilter(f => !f.Siparis.OnaylayanSatisBayii.Kullanici.IsDeleted);
            });
        }
    }
}
