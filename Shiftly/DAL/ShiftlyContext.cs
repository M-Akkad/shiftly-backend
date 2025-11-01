using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ShiftlyContext : DbContext
{
    public ShiftlyContext(DbContextOptions<ShiftlyContext> options) : base(options)
    {
    }

    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Speler> Spelers => Set<Speler>();
    public DbSet<Wedstrijd> Wedstrijden => Set<Wedstrijd>();
    public DbSet<WedstrijdSpeler> WedstrijdSpelers => Set<WedstrijdSpeler>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure WedstrijdSpeler as many-to-many relationship table
        modelBuilder.Entity<WedstrijdSpeler>()
            .HasKey(ws => new { ws.WedstrijdID, ws.SpelerID });

        modelBuilder.Entity<WedstrijdSpeler>()
            .HasOne(ws => ws.Wedstrijd)
            .WithMany(w => w.WedstrijdSpelers)
            .HasForeignKey(ws => ws.WedstrijdID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WedstrijdSpeler>()
            .HasOne(ws => ws.Speler)
            .WithMany(s => s.WedstrijdSpelers)
            .HasForeignKey(ws => ws.SpelerID)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Wedstrijd-Admin relationship
        modelBuilder.Entity<Wedstrijd>()
            .HasOne(w => w.Admin)
            .WithMany(a => a.Wedstrijden)
            .HasForeignKey(w => w.AdminID)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraints
        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<Speler>()
            .HasIndex(s => s.Email)
            .IsUnique();

        modelBuilder.Entity<Speler>()
            .HasIndex(s => s.Rugnummer)
            .IsUnique();
    }
}
