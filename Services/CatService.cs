using Kissarekisteri.AccessControl;
using Kissarekisteri.Database;
using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services;

public class CatService(
    KissarekisteriDbContext dbContext,
    UploadService uploadService,
    UserService userService
)
{
    public async Task<bool> UploadCatPhoto(int catId, IFormFile file)
    {
        var cat = await dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);
        var uploadedPhoto = await uploadService.UploadFile(file);

        if (uploadedPhoto == null)
        {
            return false;
        }

        await dbContext.CatPhotos.AddAsync(new CatPhoto { CatId = cat.Id, Url = uploadedPhoto.Uri.AbsoluteUri });
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Cat> UpdateCatByIdAsync(int catId, CatCreateRequestDTO catPayload)
    {
        var cat = await dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);

        if (cat == null)
        {
            return null;
        }

        cat.Name = catPayload.Name;
        cat.Breed = catPayload.Breed;
        cat.BirthDate = catPayload.BirthDate;
        cat.ImageUrl = catPayload.ImageUrl;

        await dbContext.SaveChangesAsync();
        return cat;
    }

    public async Task<CatTransfer> CreateTransferRequest(string userId, int catId)
    {
        var cat = await dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);

        if (cat == null)
        {
            return null;
        }

        var catTransfer = new CatTransfer
        {
            CatId = catId,
            RequesterId = userId,
            ConfirmerId = cat.OwnerId,
        };

        await dbContext.CatTransfers.AddAsync(catTransfer);
        await dbContext.SaveChangesAsync();
        return catTransfer;
    }

    public async Task<List<CatTransferResultDTO>> GetTransferRequests(string userId)
    {
        var user = await userService.GetUserById(userId);
        var userIsAdmin = user?.UserRole?.Role?.Name == Roles.Admin;

        var catTransfers = await dbContext.CatTransfers
            .Where(ct => !ct.Confirmed)
            .Where(ct => ct.ConfirmerId == userId || userIsAdmin)
            .ToListAsync();

        var catTransferResultDTOs = new List<CatTransferResultDTO>();

        foreach (var transfer in catTransfers)
        {
            var cat = await dbContext.Cats.FirstOrDefaultAsync(c => c.Id == transfer.CatId);
            var dto = new CatTransferResultDTO
            {
                Id = transfer.Id,
                CatId = transfer.CatId,
                Cat = cat,
                RequesterId = transfer.RequesterId,
                ConfirmerId = transfer.ConfirmerId,
                Confirmed = transfer.Confirmed,
                Requester = await userService.GetUserById(transfer.RequesterId)
            };

            catTransferResultDTOs.Add(dto);
        }

        return catTransferResultDTOs;
    }

    public async Task<bool> ConfirmTransferRequest(string userId, int transferId)
    {
        var user = await userService.GetUserById(userId);

        var transfer = await dbContext.CatTransfers
           .FirstOrDefaultAsync(ct =>
               (ct.Id == transferId && ct.ConfirmerId == userId) ||
               (ct.Id == transferId && user.UserRole.Role.Name == "Admin")
           );

        transfer.Confirmed = true;
        await dbContext.SaveChangesAsync();

        var updatedCat = dbContext.Cats.FirstOrDefault(c => c.Id == transfer.CatId);
        updatedCat.OwnerId = transfer.RequesterId;
        await dbContext.SaveChangesAsync();

        return true;
    }

    public IQueryable<Cat> GetCats()
    {
        return dbContext.Cats;
    }

    public IQueryable<CatBreed> GetBreeds()
    {
        return dbContext.CatBreeds;
    }

    public async Task<bool> DeleteCatByIdAsync(int catId)
    {
        var cat = await dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);
        dbContext.Cats.Remove(cat);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Cat> CreateCat(CatCreateRequestDTO newCatRequest)
    {
        var cat = new Cat
        {
            Name = newCatRequest.Name,
            BirthDate = newCatRequest.BirthDate,
            Breed = newCatRequest.Breed,
            Sex = newCatRequest.Sex,
            OwnerId = newCatRequest.OwnerId,
            BreederId = newCatRequest.BreederId,
            ImageUrl = newCatRequest.ImageUrl
        };

        await dbContext.Cats.AddAsync(cat);
        await dbContext.SaveChangesAsync();

        if (newCatRequest.MotherId.HasValue)
        {
            var mother = await dbContext.Cats.AnyAsync(c => c.Id == newCatRequest.MotherId.Value);

            if (mother)
            {
                var motherRelation = new CatRelation
                {
                    ParentId = newCatRequest.MotherId.Value,
                    KittenId = cat.Id
                };
                await dbContext.CatRelations.AddAsync(motherRelation);
            }
        }

        if (newCatRequest.FatherId.HasValue)
        {
            var father = await dbContext.Cats.AnyAsync(c => c.Id == newCatRequest.FatherId.Value);
            if (father)
            {
                var fatherRelation = new CatRelation
                {
                    ParentId = newCatRequest.FatherId.Value,
                    KittenId = cat.Id
                };
                await dbContext.CatRelations.AddAsync(fatherRelation);
            }
        }

        await dbContext.SaveChangesAsync();
        return cat;
    }
}
