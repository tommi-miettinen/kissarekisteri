using Kissarekisteri.Database;
using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Kissarekisteri.RBAC;
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
                            rp => rp.RoleId == roleEntity.Id && rp.PermissionId == permissionEntity.Id
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
        public async Task SeedUsers()
        {
            var userData = new List<UserCreatePayloadDTO>
            {
                new()
                {
                    MailNickname = "johndoe",
                    GivenName = "John",
                    Surname = "Doe",
                    DisplayName = "John Doe",
                    Password = "John1234!",
                    Email = "john.doe@example.com"
                },
                new()
                {
                    MailNickname = "janedoe",
                    GivenName = "Jane",
                    Surname = "Doe",
                    DisplayName = "Jane Doe",
                    Password = "Jane1234!",
                    Email = "jane.doe@example.com"
                },
                new()
                {
                    MailNickname = "alexsmith",
                    GivenName = "Alex",
                    Surname = "Smith",
                    DisplayName = "Alex Smith",
                    Password = "Alex1234!",
                    Email = "alex.smith@example.com"
                }
            };

            var existingUsers = await userService.GetUsers();

            var usersToCreate = userData
                .Where(
                    newUser =>
                        !existingUsers.Any(existingUser => existingUser.Email == newUser.Email)
                )
                .ToList();


            foreach (var userPayload in usersToCreate)
            {
                var createdUser = await userService.CreateUser(userPayload);

            }
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

        public async Task SeedCatShows()
        {
            var catShowData = new List<CatShow>
            {
                new()
                {
                    Name = "Kissakilpailu 2021",
                    Location = "Helsinki",
                    StartDate = new DateTime(2021, 1, 1),
                    EndDate = new DateTime(2021, 1, 2),
                    Description = "Kissakilpailu 2021",
                },
                new()
                {
                    Name = "Kissakilpailu 2022",
                    Location = "Helsinki",
                    StartDate = new DateTime(2022, 1, 1),
                    EndDate = new DateTime(2022, 1, 2),
                    Description = "Kissakilpailu 2022",
                },
                new()
                {
                    Name = "Kissakilpailu 2023",
                    Location = "Helsinki",
                    StartDate = new DateTime(2023, 1, 1),
                    EndDate = new DateTime(2023, 1, 2),
                    Description = "Kissakilpailu 2023",
                }
            };

            foreach (CatShow catShow in catShowData)
            {
                await catShowService.CreateCatShow(catShow);
            }
        }

        public async Task<List<Cat>> SeedCats()
        {
            var users = await userService.GetUsers();
            var random = new Random();
            var catData = new List<CatRequest>
            {
                new()
                {
                    OwnerId = users[random.Next(users.Count)].Id,
                    BreederId = users[random.Next(users.Count)].Id,
                    Name = "Misse",
                    Breed = "Persian",
                    BirthDate = new DateTime(2019, 1, 1),
                    Sex = "male",
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg"
                },
                new()
                {
                    OwnerId = users[random.Next(users.Count)].Id,
                    BreederId = users[random.Next(users.Count)].Id,
                    Name = "Mouru",
                    Breed = "Maine Coon",
                    BirthDate = new DateTime(2019, 1, 1),
                    Sex = "female",
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg"
                },
                new()
                {
                    OwnerId = users[random.Next(users.Count)].Id,
                    BreederId = users[random.Next(users.Count)].Id,
                    Name = "Mauku",
                    Breed = "Maine Coon",
                    BirthDate = new DateTime(2019, 1, 1),
                    Sex = "female",
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg"
                }
            };

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
            foreach (var catRequest in catsToCreate)
            {
                var createdCat = await catService.CreateCat(catRequest);
                createdCats.Add(createdCat.Data);
            }

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
