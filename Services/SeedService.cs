using Bogus;
using CountryData.Bogus;
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
            var permissions = PermissionSeed.GetSeedData();

            foreach (var permission in permissions)
            {
                if (!dbContext.Permissions.Any(p => p.Name == permission.Name))
                {
                    dbContext.Permissions.Add(permission);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task SeedRoles()
        {
            var roles = RoleSeed.GetSeedData();

            foreach (var role in roles)
            {
                if (!dbContext.Roles.Any(r => r.Name == role.Name))
                {
                    dbContext.Roles.Add(new Role { Name = role.Name, });
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task SeedRolePermissions()
        {
            var roles = RolePermissionSeed.GetSeedData();

            foreach (var role in roles)
            {
                var roleEntity = dbContext.Roles.FirstOrDefault(r => r.Name == role.Name);

                foreach (var permission in role.Permissions)
                {
                    var permissionEntity = dbContext.Permissions.FirstOrDefault(
                        p => p.Name == permission.ToString()
                    );

                    if (
                        !dbContext.RolePermissions.Any(
                            rp =>
                                rp.RoleId == roleEntity.Id && rp.PermissionId == permissionEntity.Id
                        )
                    )
                    {
                        dbContext.RolePermissions.Add(
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

            await dbContext.SaveChangesAsync();
        }


        public async Task SeedCatBreeds()
        {
            var breedsToAdd = new List<CatBreed>
            {
                new() { Name = "Siamese" },
                new() { Name = "Persian" },
                new() { Name = "Maine Coon" }
            };

            foreach (var breed in breedsToAdd)
            {
                if (!dbContext.CatBreeds.Any(b => b.Name == breed.Name))
                {
                    dbContext.CatBreeds.Add(breed);
                    await dbContext.SaveChangesAsync();
                }
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

            foreach (CatShow catShow in catShowData)
            {
                await catShowService.CreateCatShow(catShow);
            }
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
            var catBreeds = await dbContext.CatBreeds.ToListAsync();
            var catFaker = new Faker<Cat>("fi").CustomInstantiator(f =>
            {
                var gender = f.PickRandom(
                    Bogus.DataSets.Name.Gender.Male,
                    Bogus.DataSets.Name.Gender.Female
                );
                var catRequest = new Cat
                {
                    OwnerId = users.Any() ? f.PickRandom(users).Id : null,
                    BreederId = users.Any() ? f.PickRandom(users).Id : null,
                    Name = f.Name.FirstName(gender),
                    Breed = catBreeds.Any() ? f.PickRandom(catBreeds).Name : "Siamese",
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

        public async Task SeedUserRolesForUsers()
        {
            var users = await userService.GetUsers();
            var roles = await userService.GetRoles();

            var userRoles = new List<UserRole>
            {
                new() { UserId = users[0].Id, RoleId = roles[0].Id },
                new() { UserId = users[1].Id, RoleId = roles[1].Id },
            };

            foreach (var userRole in userRoles)
            {
                await userService.CreateUserRole(userRole);
            }
        }
    }
}
