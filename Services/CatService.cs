using Kissarekisteribackend.Database;
using Kissarekisteribackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Services;

public class CatService(KissarekisteriDbContext dbContext, UploadService uploadService)
{
    private readonly KissarekisteriDbContext _dbContext = dbContext;
    private readonly UploadService _uploadService = uploadService;

    public async Task<Cat> UploadCatPhoto(int catId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("no file");
            return null;
        }
        var cat = await _dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);

        var uploadedPhoto = await _uploadService.UploadFile(file);

        await _dbContext.CatPhotos.AddAsync(new CatPhoto
        {
            CatId = cat.Id,
            Url = uploadedPhoto.Uri.AbsoluteUri
        });

        await _dbContext.SaveChangesAsync();

        var catWithPhotos = await _dbContext.Cats.Include(c => c.Photos).FirstOrDefaultAsync(cat => cat.Id == catId);

        return catWithPhotos;
    }

    public async Task<Cat> UpdateCatByIdAsync(int catId, Cat catPayload)
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
        return await _dbContext.Cats.Include(c => c.Photos).FirstOrDefaultAsync(cat => cat.Id == catId);
    }

    public async Task<List<Cat>> GetCatsAsync()
    {
        var cats = await _dbContext.Cats.Include(c => c.Photos).ToListAsync();
        return cats;
    }

    public async Task DeleteCatByIdAsync(int catId)
    {
        var cat = await _dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);
        _dbContext.Cats.Remove(cat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Cat> CreateCat(Cat newCat)
    {
        if (newCat == null)
        {
            return null;
        }

        await _dbContext.Cats.AddAsync(newCat);
        await _dbContext.SaveChangesAsync();

        return newCat;
    }

    public async Task<List<Cat>> GetCatByUserIdAsync(string userId)
    {
        var cats = await _dbContext.Cats.Where(cat => cat.OwnerId == userId).ToListAsync();
        return cats;
    }
}


