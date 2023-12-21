using AutoMapper;
using Kissarekisteri.Database;
using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services;

public class CatService(
    KissarekisteriDbContext dbContext,
    UploadService uploadService,
    IMapper mapper,
    UserService userService
)
{
    private readonly KissarekisteriDbContext _dbContext = dbContext;
    private readonly UploadService _uploadService = uploadService;
    private readonly UserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    public async Task<Cat> UploadCatPhoto(int catId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("no file");
            return null;
        }
        var cat = await _dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);

        var uploadedPhoto = await _uploadService.UploadFile(file);

        await _dbContext.CatPhotos.AddAsync(
            new CatPhoto { CatId = cat.Id, Url = uploadedPhoto.Uri.AbsoluteUri }
        );

        await _dbContext.SaveChangesAsync();

        var catWithPhotos = await _dbContext.Cats
            .Include(c => c.Photos)
            .FirstOrDefaultAsync(cat => cat.Id == catId);

        return catWithPhotos;
    }

    public async Task<Cat> UpdateCatByIdAsync(int catId, CatRequest catPayload)
    {
        var cat = await _dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);

        if (cat == null)
        {
            return null;
        }

        cat.Name = catPayload.Name;
        cat.Breed = catPayload.Breed;
        cat.BirthDate = catPayload.BirthDate;
        cat.ImageUrl = catPayload.ImageUrl;

        await _dbContext.SaveChangesAsync();
        return cat;
    }

    public async Task<Cat> GetCatByIdAsync(int catId)
    {
        var cat = await _dbContext.Cats
            .Include(c => c.Photos)
            .Include(c => c.Results)
            .ThenInclude(Results => Results.CatShow)
            .FirstOrDefaultAsync(cat => cat.Id == catId);

        cat.Owner = await _userService.GetUserById(cat.OwnerId);
        cat.Breeder = await _userService.GetUserById(cat.BreederId);
        cat.CatParents = await _dbContext.CatParents
            .Include(cp => cp.Cat)
            .Where(cp => cp.ChildCatId == cat.Id)
            .ToListAsync();

        return cat;
    }

    public async Task<List<Cat>> GetCatsAsync(string? name, int? limit)
    {
        var queryableCats = _dbContext.Cats
            .Include(c => c.Photos)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            queryableCats = queryableCats.Where(c => c.Name.Contains(name));
        }
        if (limit.HasValue)
        {
            queryableCats = queryableCats.Take(limit.Value);
        }

        var filteredCats = await queryableCats.ToListAsync();

        foreach (Cat cat in filteredCats)
        {
            cat.CatParents = await _dbContext.CatParents
                .Include(cp => cp.ChildCat)
                .Where(cp => cp.ChildCatId == cat.Id)
                .ToListAsync();
        }

        return filteredCats;
    }

    public async Task<List<CatBreed>> GetBreedsAsync()
    {
        var breeds = await _dbContext.CatBreeds.ToListAsync();
        return breeds;
    }

    public async Task DeleteCatByIdAsync(int catId)
    {
        var cat = await _dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);
        _dbContext.Cats.Remove(cat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Cat> CreateCat(CatRequest newCatRequest)
    {
        if (newCatRequest == null)
        {
            return null;
        }

        var cat = new Cat
        {
            Name = newCatRequest.Name,
            BirthDate = newCatRequest.BirthDate,
            Breed = newCatRequest.Breed,
            Sex = newCatRequest.Sex,
            OwnerId = newCatRequest.OwnerId,
            BreederId = newCatRequest.BreederId
        };

        await _dbContext.Cats.AddAsync(cat);
        await _dbContext.SaveChangesAsync();

        if (newCatRequest.MotherId.HasValue)
        {
            var motherRelation = new CatParent
            {
                ParentCatId = newCatRequest.MotherId.Value,
                ChildCatId = cat.Id
            };
            _dbContext.CatParents.Add(motherRelation);
        }

        if (newCatRequest.FatherId.HasValue)
        {
            var fatherRelation = new CatParent
            {
                ParentCatId = newCatRequest.FatherId.Value,
                ChildCatId = cat.Id
            };
            _dbContext.CatParents.Add(fatherRelation);
        }

        await _dbContext.SaveChangesAsync();

        return cat;
    }

    public async Task<List<Cat>> GetCatByUserIdAsync(string userId)
    {
        var cats = await _dbContext.Cats.Where(cat => cat.OwnerId == userId).ToListAsync();
        return cats;
    }
}
