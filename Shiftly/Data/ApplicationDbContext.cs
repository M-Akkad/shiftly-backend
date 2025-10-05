using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiftly.Models;

namespace Shiftly.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Wedstrijd> Wedstrijden => Set<Wedstrijd>();
    public DbSet<WedstrijdSpeler> WedstrijdSpelers => Set<WedstrijdSpeler>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // voorkom dubbele toewijzing: uniek op WedstrijdId + SpelerId
        builder.Entity<WedstrijdSpeler>()
            .HasIndex(ws => new { ws.WedstrijdId, ws.SpelerId })
            .IsUnique();
    }
}
