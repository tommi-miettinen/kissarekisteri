using Kissarekisteri.Models;
using Kissarekisteri.RBAC;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
    public DbSet<CatParent> CatParents { get; set; }

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
            .Entity<Cat>()
            .HasMany(c => c.CatParents)
            .WithOne(cp => cp.ChildCat)
            .HasForeignKey(cp => cp.ParentCatId);


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

    public void SeedCatBreeds()
    {
        var catBreeds = CatBreedSeed.GetSeedData();

        foreach (var catBreed in catBreeds)
        {
            if (!CatBreeds.Any(cb => cb.Name == catBreed.Name))
            {
                CatBreeds.Add(catBreed);
            }
        }

        SaveChanges();
    }

    public void SeedPermissions()
    {
        var permissions = PermissionSeed.GetSeedData();

        foreach (var permission in permissions)
        {
            if (!Permissions.Any(p => p.Name == permission.Name))
            {
                Permissions.Add(permission);
            }
        }

        SaveChanges();
    }

    public void SeedRoles()
    {
        var roles = RoleSeed.GetSeedData();

        foreach (var role in roles)
        {
            if (!Roles.Any(r => r.Name == role.Name))
            {
                Roles.Add(new Role { Name = role.Name, });
            }
        }

        SaveChanges();
    }

    public void SeedRolePermissions()
    {
        var roles = RolePermissionSeed.GetSeedData();

        foreach (var role in roles)
        {
            var roleEntity = Roles.FirstOrDefault(r => r.Name == role.Name);

            foreach (var permission in role.Permissions)
            {
                var permissionEntity = Permissions.FirstOrDefault(
                    p => p.Name == permission.ToString()
                );

                if (
                    !RolePermissions.Any(
                        rp => rp.RoleId == roleEntity.Id && rp.PermissionId == permissionEntity.Id
                    )
                )
                {
                    RolePermissions.Add(
                        new RolePermission
                        {
                            RoleId = roleEntity.Id,
                            RoleName = roleEntity.Name,
                            PermissionName = permissionEntity.Name,
                            PermissionId = permissionEntity.Id
                        }
                    );
                }
            }
        }

        SaveChanges();
    }
};
