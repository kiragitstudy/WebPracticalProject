using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebPracticalProject.Domain.Catalog;
using WebPracticalProject.Domain.Common;
using WebPracticalProject.Domain.Contacts;
using WebPracticalProject.Domain.Rentals;
using WebPracticalProject.Domain.Users;

namespace WebPracticalProject.DAL;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Instrument> Instruments => Set<Instrument>();
    public DbSet<Rental> Rentals => Set<Rental>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.HasDefaultSchema("app");

        // Конвертеры: сохраняем в БД как нижний регистр (draft, active, ...)
        var roleConverter = new ValueConverter<UserRole, string>(
            v => v.ToString().ToLowerInvariant(),
            v => Enum.Parse<UserRole>(v, true));

        var statusConverter = new ValueConverter<RentalStatus, string>(
            v => v.ToString().ToLowerInvariant(),
            v => Enum.Parse<RentalStatus>(v, true));

        model.Entity<User>(e =>
        {
            e.ToTable("users");
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.Role)
                .HasConversion(roleConverter)
                .HasColumnType("text");
            
            e.ToTable(tb => tb.HasCheckConstraint(
                "ck_users_role",
                "role IN ('customer','manager','admin')"));
        });

        model.Entity<ContactMessage>(e =>
        {
            e.ToTable("contact_messages");
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.Message).IsRequired();
        });

        model.Entity<Instrument>(e =>
        {
            e.ToTable("instruments");
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            e.Property(x => x.Title).IsRequired();
            e.HasIndex(x => x.Category);
            e.HasIndex(x => x.Brand);
            e.HasIndex(x => x.PricePerDay);
            e.HasIndex(x => x.IsFeatured);
        });

        model.Entity<Rental>(e =>
        {
            e.ToTable("rentals", tb =>
            {
                tb.HasCheckConstraint("ck_rentals_dates", "end_at > start_at");
                tb.HasCheckConstraint("ck_rentals_status",
                    "status IN ('draft','active','closed','cancelled')");
            });
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();

            e.Property(x => x.Status)
                .HasConversion(statusConverter)
                .HasColumnType("text");

            e.HasOne(x => x.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(x => x.Instrument)
                .WithMany(i => i.Rentals)
                .HasForeignKey(x => x.InstrumentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.InstrumentId);
            e.HasIndex(x => x.Status);
        });
    }
}