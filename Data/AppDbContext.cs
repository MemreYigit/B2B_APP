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
                    .HasMaxLength(20);

                entity.Property(c => c.Address)
                    .HasMaxLength(500);

                entity.Property(c => c.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(c => c.Email)
                    .HasMaxLength(256);

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
        }
    }
}
