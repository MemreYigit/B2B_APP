using Microsoft.EntityFrameworkCore;
using WebApplication1.Entity;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Cari> Cariler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<CariUrunFiyat> CariUrunFiyatlari { get; set; }
        public DbSet<CariHareket> CariHareketler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisKalemi> SiparisKalemleri { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Company configuration
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.TaxNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Address)
                    .HasMaxLength(500);

                entity.Property(c => c.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(c => c.Email)
                    .HasMaxLength(256);

                // Vergi numarası eşsiz olmalıdır
                entity.HasIndex(c => c.TaxNumber)
                    .IsUnique();
                    
                // Soft delete global query filter
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(u => u.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(u => u.ApprovalNotes)
                    .HasMaxLength(1000);

                entity.Property(u => u.EmailVerificationToken)
                    .HasMaxLength(200);

                entity.HasIndex(u => u.EmailVerificationToken);

                // Unique index on Email
                entity.HasIndex(u => u.Email)
                    .IsUnique();

                // Foreign key to Company
                entity.HasOne(u => u.Company)
                    .WithMany(c => c.Users)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Self-referencing foreign key for ApprovedByAdmin
                entity.HasOne(u => u.ApprovedByAdmin)
                    .WithMany()
                    .HasForeignKey(u => u.ApprovedByAdminId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Soft delete global query filter
                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            // UserSession configuration
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Jti)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasIndex(s => s.Jti).IsUnique();

                entity.HasIndex(s => new { s.UserId, s.IsRevoked });

                entity.HasOne(s => s.User)
                    .WithMany()
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Apply same soft delete filter as User entity
                entity.HasQueryFilter(s => !s.User.IsDeleted);
            });

            // Cari configuration
            modelBuilder.Entity<Cari>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.CariKodu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Unvan)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.VergiNumarasi)
                    .HasMaxLength(50);

                entity.Property(c => c.ToplamRiskLimiti)
                    .HasPrecision(18, 2);

                entity.Property(c => c.GuncelBakiye)
                    .HasPrecision(18, 2);

                entity.HasIndex(c => c.CariKodu)
                    .IsUnique();

                // Bir Company yalnızca bir Cari ile eşleşir (1-1)
                entity.HasIndex(c => c.CompanyId)
                    .IsUnique();

                entity.HasOne(c => c.Company)
                    .WithOne(co => co.Cari)
                    .HasForeignKey<Cari>(c => c.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            // Urun configuration
            modelBuilder.Entity<Urun>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.UrunKodu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.UrunAdi)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(u => u.Birim)
                    .HasMaxLength(20);

                entity.Property(u => u.ListeFiyati)
                    .HasPrecision(18, 2);

                entity.HasIndex(u => u.UrunKodu)
                    .IsUnique();

                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            // CariUrunFiyat configuration
            modelBuilder.Entity<CariUrunFiyat>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.OzelFiyat)
                    .HasPrecision(18, 2);

                // Belirli bir Cari + Urun için fiyat sorgusu bu index'i kullanır
                entity.HasIndex(f => new { f.CariId, f.UrunId });

                entity.HasOne(f => f.Cari)
                    .WithMany(c => c.CariUrunFiyatlari)
                    .HasForeignKey(f => f.CariId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Urun)
                    .WithMany(u => u.CariUrunFiyatlari)
                    .HasForeignKey(f => f.UrunId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Cari'nin soft-delete filtresiyle eşleşir
                entity.HasQueryFilter(f => !f.Cari.IsDeleted);
            });

            // CariHareket configuration
            modelBuilder.Entity<CariHareket>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.Property(h => h.BirimFiyat)
                    .HasPrecision(18, 2);

                entity.Property(h => h.Miktar)
                    .HasPrecision(18, 3);

                entity.Property(h => h.IskontoOrani)
                    .HasPrecision(5, 2);

                entity.Property(h => h.Aciklama)
                    .HasMaxLength(500);

                entity.HasIndex(h => new { h.CariId, h.Tarih });

                entity.HasOne(h => h.Cari)
                    .WithMany(c => c.CariHareketler)
                    .HasForeignKey(h => h.CariId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.Urun)
                    .WithMany(u => u.CariHareketler)
                    .HasForeignKey(h => h.UrunId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Cari'nin soft-delete filtresiyle eşleşir
                entity.HasQueryFilter(h => !h.Cari.IsDeleted);
            });

            // Siparis configuration
            modelBuilder.Entity<Siparis>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.SiparisNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(s => s.SiparisNo)
                    .IsUnique();

                entity.HasOne(s => s.Cari)
                    .WithMany(c => c.Siparisler)
                    .HasForeignKey(s => s.CariId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.User)
                    .WithMany()
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(s => !s.IsDeleted);
            });

            // SiparisKalemi configuration
            modelBuilder.Entity<SiparisKalemi>(entity =>
            {
                entity.HasKey(k => k.Id);

                entity.Property(k => k.Miktar)
                    .HasPrecision(18, 3);

                entity.Property(k => k.BirimFiyat)
                    .HasPrecision(18, 2);

                entity.Property(k => k.IskontoOrani)
                    .HasPrecision(5, 2);

                entity.HasOne(k => k.Siparis)
                    .WithMany(s => s.Kalemler)
                    .HasForeignKey(k => k.SiparisId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(k => k.Urun)
                    .WithMany(u => u.SiparisKalemleri)
                    .HasForeignKey(k => k.UrunId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Siparis'in soft-delete filtresiyle eşleşir
                entity.HasQueryFilter(k => !k.Siparis.IsDeleted);
            });

            // Fatura configuration
            modelBuilder.Entity<Fatura>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.FaturaNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(f => f.ToplamTutar)
                    .HasPrecision(18, 2);

                entity.Property(f => f.KdvTutari)
                    .HasPrecision(18, 2);

                entity.Property(f => f.DokumanYolu)
                    .HasMaxLength(500);

                entity.Property(f => f.GibReferansNo)
                    .HasMaxLength(100);

                entity.HasIndex(f => f.FaturaNo)
                    .IsUnique();

                // Bir sipariş için tek fatura (1-1)
                entity.HasIndex(f => f.SiparisId)
                    .IsUnique();

                entity.HasOne(f => f.Siparis)
                    .WithOne(s => s.Fatura)
                    .HasForeignKey<Fatura>(f => f.SiparisId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Siparis'in soft-delete filtresiyle eşleşir
                entity.HasQueryFilter(f => !f.Siparis.IsDeleted);
            });

            // UserDocument configuration
            modelBuilder.Entity<UserDocument>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(d => d.OriginalFileName)
                    .HasMaxLength(255);

                entity.HasIndex(d => new { d.UserId, d.Tip });

                entity.HasOne(d => d.User)
                    .WithMany(u => u.Documents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // User'ın soft-delete filtresiyle eşleşir
                entity.HasQueryFilter(d => !d.User.IsDeleted);
            });
        }
    }
}
