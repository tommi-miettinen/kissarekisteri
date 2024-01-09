using Bogus;
using CountryData.Bogus;
using Kissarekisteri.Data;
using Kissarekisteri.Database;
using Kissarekisteri.Models;
using Kissarekisteri.RBAC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services
{
    public class SeedService(
        UserService userService,
        CatService catService,
        CatShowService catShowService,
        KissarekisteriDbContext dbContext
    )
    {
        public async Task SeedPermissions()
        {
            var existingPermissions = dbContext.Permissions.Select(p => p.Name).ToList();
            var permissionsToAdd = Permissions.GetPermissions()
                .Where(p => !existingPermissions.Contains(p.Name))
                .ToList();

            dbContext.Permissions.AddRange(permissionsToAdd);
            await dbContext.SaveChangesAsync();
        }

        public async Task SeedRoles()
        {
            var existingRoles = dbContext.Roles.Select(r => r.Name).ToList();
            var rolesToAdd = Roles.GetRoles().Where(r => !existingRoles.Contains(r.Name)).ToList();

            dbContext.Roles.AddRange(rolesToAdd);
            await dbContext.SaveChangesAsync();
        }

        public async Task SeedRolePermissions()
        {
            var roles = RolePermissions.RolesWithPermissions;

            foreach (var role in roles)
            {
                var roleEntity = dbContext.Roles.FirstOrDefault(r => r.Name == role.Name);

                if (roleEntity != null)
                {
                    var existingRolePermissions = dbContext.RolePermissions
                        .Where(rp => rp.RoleId == roleEntity.Id)
                        .Select(rp => rp.PermissionId)
                        .ToList();

                    var permissionsToAdd = role.Permissions
                        .Select(
                            p =>
                                dbContext.Permissions.FirstOrDefault(
                                    perm => perm.Name == p.ToString()
                                )
                        )
                        .Where(p => p != null && !existingRolePermissions.Contains(p.Id))
                        .Select(
                            p =>
                                new RolePermission
                                {
                                    RoleId = roleEntity.Id,
                                    RoleName = roleEntity.Name,
                                    PermissionName = p.Name,
                                    PermissionId = p.Id
                                }
                        )
                        .ToList();

                    dbContext.RolePermissions.AddRange(permissionsToAdd);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task SeedCatBreeds()
        {
            var existingBreedNames = dbContext.CatBreeds
                .Select(b => b.Name)
                .ToList();

            var breedsToAdd = CatBreeds.Breeds
                .Where(b => !existingBreedNames.Contains(b.Name))
                .Select(b => new CatBreed { Name = b.Name })
                .ToList();

            if (breedsToAdd.Count != 0)
            {
                dbContext.CatBreeds.AddRange(breedsToAdd);
                await dbContext.SaveChangesAsync();
            }
        }
        public async Task SeedCatShows(bool deleteExisting = false, int amount = 10)
        {
            if (deleteExisting)
            {
                var allCatShows = await dbContext.CatShows.ToListAsync();
                dbContext.CatShows.RemoveRange(allCatShows);
                await dbContext.SaveChangesAsync();
            }

            var catShowFaker = new Faker<CatShow>("fi")
                .RuleFor(cs => cs.Name, f => f.Country().Finland().Place().Name)
                .RuleFor(cs => cs.Location, f => f.Address.StreetAddress())
                .RuleFor(cs => cs.StartDate, f => f.Date.Future(10))
                .RuleFor(cs => cs.EndDate, f => f.Date.Future(11))
                .RuleFor(cs => cs.Description, f => f.Lorem.Sentence());

            var catShowData = catShowFaker.Generate(amount);

            dbContext.CatShows.AddRange(catShowData);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Cat>> SeedCats(bool deleteExisting = false, int amount = 10)
        {
            if (deleteExisting)
            {
                var allCats = await dbContext.Cats.ToListAsync();
                dbContext.Cats.RemoveRange(allCats);
                await dbContext.SaveChangesAsync();
            }

            var users = await userService.GetUsers();
            var catBreeds = CatBreeds.Breeds;
            var catFaker = new Faker<Cat>("fi").CustomInstantiator(f =>
            {
                var gender = f.PickRandom(
                    Bogus.DataSets.Name.Gender.Male,
                    Bogus.DataSets.Name.Gender.Female
                );
                var catRequest = new Cat
                {
                    OwnerId = users.Count != 0 ? f.PickRandom(users).Id : null,
                    BreederId = users.Count != 0 ? f.PickRandom(users).Id : null,
                    Name = f.Name.FirstName(gender),
                    Breed = f.PickRandom(catBreeds).Name,
                    BirthDate = f.Date.Past(10),
                    Sex = gender == Bogus.DataSets.Name.Gender.Male ? "Male" : "Female",
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg"
                };
                return catRequest;
            });

            var catData = catFaker.Generate(amount);

            var existingCats = await catService.GetCatsAsync();

            var catsToCreate = catData
                .Where(
                    newCat =>
                        !existingCats.Data.Any(
                            existingCat =>
                                existingCat.Name.Equals(
                                    newCat.Name,
                                    StringComparison.OrdinalIgnoreCase
                                )
                                && existingCat.Breed.Equals(
                                    newCat.Breed,
                                    StringComparison.OrdinalIgnoreCase
                                )
                        )
                )
                .ToList();

            var createdCats = new List<Cat>();

            dbContext.Cats.AddRange(catsToCreate);
            await dbContext.SaveChangesAsync();

            return createdCats;
        }
    }
}
