using Kissarekisteri.Models;
using Microsoft.EntityFrameworkCore;

namespace Kissarekisteri.Database;

public class KissarekisteriDbContext(DbContextOptions<KissarekisteriDbContext> options)
    : DbContext(options)
{
    public DbSet<Cat> Cats { get; set; }
    public DbSet<CatPhoto> CatPhotos { get; set; }
    public DbSet<CatShow> CatShows { get; set; }
    public DbSet<CatShowResult> CatShowResults { get; set; }
    public DbSet<CatShowPhoto> CatShowPhotos { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<CatAttendee> CatAttendees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<CatBreed> CatBreeds { get; set; }
    public DbSet<CatTransfer> CatTransfers { get; set; }
    public DbSet<CatRelation> CatRelations { get; set; }

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
            .Entity<Cat>()
            .HasMany(c => c.Results)
            .WithOne(r => r.Cat)
            .HasForeignKey(r => r.CatId);

        modelBuilder
            .Entity<CatTransfer>()
            .HasOne(ct => ct.Cat)
            .WithOne()
            .HasForeignKey<CatTransfer>(ct => ct.CatId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Cat>()
            .HasMany(c => c.Parents)
            .WithOne(cr => cr.ChildCat)
            .HasForeignKey(cr => cr.KittenId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Cat>()
            .HasMany(c => c.Kittens)
            .WithOne(cr => cr.ParentCat)
            .HasForeignKey(cr => cr.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<CatShow>()
            .HasMany(c => c.Photos)
            .WithOne(p => p.CatShow)
            .HasForeignKey(p => p.CatShowId);

        modelBuilder
            .Entity<CatShow>()
            .HasMany(c => c.Results)
            .WithOne(r => r.CatShow)
            .HasForeignKey(r => r.CatShowId);

        modelBuilder
            .Entity<Attendee>()
            .HasMany(Attendee => Attendee.CatAttendees)
            .WithOne(CatAttendee => CatAttendee.Attendee)
            .HasForeignKey(CatAttendee => CatAttendee.AttendeeId);
    }
};
