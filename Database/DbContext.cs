using Kissarekisteribackend.Models;
using Microsoft.EntityFrameworkCore;

namespace Kissarekisteribackend.Database;

public class KissarekisteriDbContext(DbContextOptions<KissarekisteriDbContext> options)
    : DbContext(options)
{
    public DbSet<Cat> Cats { get; set; }
    public DbSet<CatPhoto> CatPhotos { get; set; }
    public DbSet<CatShow> CatShows { get; set; }
    public DbSet<CatShowPhoto> CatShowPhotos { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<CatAttendee> CatAttendees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
            .Entity<CatShow>()
            .HasMany(c => c.Attendees)
            .WithOne(a => a.CatShow)
            .HasForeignKey(a => a.EventId);

        modelBuilder
            .Entity<Cat>()
            .HasMany(c => c.Photos)
            .WithOne(p => p.Cat)
            .HasForeignKey(p => p.CatId);

        modelBuilder
            .Entity<CatShow>()
            .HasMany(c => c.Photos)
            .WithOne(p => p.CatShow)
            .HasForeignKey(p => p.CatShowId);
    }
};
