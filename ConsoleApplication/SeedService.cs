using Bogus;
using CountryData.Bogus;
using Kissarekisteri.AccessControl;
using Kissarekisteri.Data;
using Kissarekisteri.Database;
using Kissarekisteri.Models;
using Kissarekisteri.SeedData;
using Kissarekisteri.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace KissarekisteriConsole.ConsoleApplication.Services;

public class SeedService(
    UserService userService,
    CatService catService,
    CatShowService catShowService,
    KissarekisteriDbContext dbContext
)
{
    public async Task SeedCatRelations()
    {
        var cats = await dbContext.Cats.ToListAsync();
        var existingRelations = await dbContext.CatRelations.ToListAsync();
        var catRelationships = new List<CatRelation>();
        var catsWithParents = new HashSet<int>();

        foreach (var relation in existingRelations)
        {
            catsWithParents.Add(relation.KittenId);
        }

        foreach (var cat in cats)
        {
            var catHasParentsAlready = catsWithParents.Contains(cat.Id);
            if (catHasParentsAlready)
            {
                continue;
            }

            var mother = cats.Where(c => c.BirthDate < cat.BirthDate)
                .Where(c => c.Sex == "Female")
                .Where(c => cat.Breed == c.Breed)
                .Where(c => c.Id != cat.Id)
                .FirstOrDefault();

            if (mother != null)
            {
                var motherRelation = new CatRelation
                {
                    ParentId = mother.Id,
                    KittenId = cat.Id,
                    ParentType = "Mother"
                };

                catRelationships.Add(motherRelation);
            }

            var father = cats.Where(c => c.BirthDate < cat.BirthDate)
                .Where(c => c.Sex == "Male")
                .Where(c => cat.Breed == c.Breed)
                .Where(c => c.Id != cat.Id)
                .FirstOrDefault();

            if (father != null)
            {
                var fatherRelation = new CatRelation
                {
                    ParentId = father.Id,
                    KittenId = cat.Id,
                    ParentType = "Father"
                };

                catRelationships.Add(fatherRelation);
            }

            catsWithParents.Add(cat.Id);
        }

        dbContext.CatRelations.AddRange(catRelationships);
        await dbContext.SaveChangesAsync();
    }

    public async Task SeedCatShowResults()
    {
        var existingResults = await dbContext.CatShowResults.ToListAsync();
        var cats = await dbContext.Cats.ToListAsync();
        var catShowResults = new List<CatShowResult>();
        var catsWithResults = new HashSet<int>();
        var catShows = await dbContext.CatShows.ToListAsync();

        foreach (var catShow in catShows)
        {
            var existingResultsForShow = existingResults
                .Where(r => r.CatShowId == catShow.Id)
                .ToList();

            foreach (var cat in cats)
            {
                var catHasResultsAlready = existingResultsForShow.Any(r => r.CatId == cat.Id);
                if (catHasResultsAlready)
                {
                    continue;
                }

                var catShowResult = new CatShowResult
                {
                    CatId = cat.Id,
                    CatShowId = catShow.Id,
                    Breed = cat.Breed,
                    Place = (Place)1
                };

                catShowResults.Add(catShowResult);
            }
        }

        dbContext.CatShowResults.AddRange(catShowResults);
        await dbContext.SaveChangesAsync();
    }

    public async Task SeedPermissions()
    {
        dbContext.Permissions.RemoveRange(dbContext.Permissions);
        await dbContext.SaveChangesAsync();

        var permissionsToAdd = Permissions.All.Select(p => new Permission { Name = p });

        dbContext.Permissions.AddRange(permissionsToAdd);
        await dbContext.SaveChangesAsync();
    }

    public async Task SeedRoles()
    {
        dbContext.Roles.RemoveRange(dbContext.Roles);
        await dbContext.SaveChangesAsync();

        var rolesToAdd = Roles.All.Select(r => new Role { Name = r });

        dbContext.Roles.AddRange(rolesToAdd);
        await dbContext.SaveChangesAsync();
    }

    public async Task SeedRolePermissions()
    {
        dbContext.RolePermissions.RemoveRange(dbContext.RolePermissions);
        await dbContext.SaveChangesAsync();

        foreach (var rolePermissionPair in RolePermissions.RolesWithPermissions)
        {
            var roleName = rolePermissionPair.Key;
            var roleEntity = dbContext.Roles.FirstOrDefault(r => r.Name == roleName);

            if (roleEntity != null)
            {
                var permissionsToAdd = new List<RolePermission>();
                foreach (var permissionName in rolePermissionPair.Value)
                {
                    var permissionEntity = dbContext.Permissions.FirstOrDefault(
                        perm => perm.Name == permissionName
                    );
                    if (permissionEntity != null)
                    {
                        permissionsToAdd.Add(
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

                dbContext.RolePermissions.AddRange(permissionsToAdd);
            }
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserRoles()
    {
        var existingUserRoles = await dbContext.UserRoles.ToListAsync();

        foreach (var existingRole in existingUserRoles)
        {
            var matchingNewRole = await dbContext.Roles.FirstOrDefaultAsync(
                r => r.Name == existingRole.RoleName
            );
            if (matchingNewRole != null)
            {
                existingRole.RoleId = matchingNewRole.Id;
            }
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task SeedCatBreeds()
    {
        var existingBreedNames = dbContext.CatBreeds.Select(b => b.Name).ToList();

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
            await dbContext.CatShows.ExecuteDeleteAsync();
            await dbContext.CatShowPhotos.ExecuteDeleteAsync();
        }

        var catShowFaker = new Faker<CatShow>("fi")
            .RuleFor(cs => cs.Name, f => f.Country().Finland().Place().Name)
            .RuleFor(cs => cs.Location, f => f.Address.StreetAddress())
            .RuleFor(cs => cs.StartDate, f => f.Date.Future(10))
            .RuleFor(cs => cs.EndDate, f => f.Date.Future(11))
            .RuleFor(cs => cs.Description, f => f.Lorem.Sentence())
            .RuleFor(cs => cs.ImageUrl, f => f.PickRandom(CatImageUrls.ImageUrls));

        var catShowData = catShowFaker.Generate(amount);

        dbContext.CatShows.AddRange(catShowData);
        await dbContext.SaveChangesAsync();

        var catShowPhotos = new List<CatShowPhoto>();
        var catShowCats = new List<CatShowCats>();
        var randomGenerator = new Random();
        var allCatIds = await dbContext.Cats.Select(c => c.Id).ToListAsync();

        foreach (var catShow in catShowData)

        {
            var count = randomGenerator.Next(1, 10);
            for (int i = 0; i < count; i++)
            {
                catShowPhotos.Add(new CatShowPhoto
                {
                    CatShowId = catShow.Id,
                    Url = CatImageUrls.ImageUrls[randomGenerator.Next(CatImageUrls.ImageUrls.Count)]
                });
            }

            var catCount = randomGenerator.Next(1, 30);
            var selectedCatIds = allCatIds.OrderBy(c => Guid.NewGuid()).Take(catCount).ToList();

            foreach (var catId in selectedCatIds)
            {
                if (!catShowCats.Any(csc => csc.CatId == catId && csc.CatShowId == catShow.Id))
                {
                    catShowCats.Add(new CatShowCats
                    {
                        CatId = catId,
                        CatShowId = catShow.Id
                    });
                }
            }

        }

        dbContext.CatShowCats.AddRange(catShowCats);
        dbContext.CatShowPhotos.AddRange(catShowPhotos);
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
                ImageUrl = f.PickRandom(CatImageUrls.ImageUrls),
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
                            existingCat.Name.Equals(newCat.Name, StringComparison.OrdinalIgnoreCase)
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
