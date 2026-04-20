using LicenseKeyServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenseKeyServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AdminUser> AdminUsers { get; set; }
    public DbSet<LicenseKey> LicenseKeys => Set<LicenseKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔹 LicenseKey
        modelBuilder.Entity<LicenseKey>(entity =>
        {
            entity.HasIndex(x => x.KeyValue).IsUnique();

            entity.Property(x => x.KeyValue)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DeviceId)
                .HasMaxLength(255);

            entity.Property(x => x.Note)
                .HasMaxLength(500);

            entity.Property(x => x.ProductCode)
                .HasMaxLength(100);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30);
        });

        // 🔹 AdminUser
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.Property(x => x.Username)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Password)
                .HasMaxLength(255)
                .IsRequired();
        });
    }
}