using Kissarekisteri.Database;
using Kissarekisteri.DTOs;
using Kissarekisteri.ErrorHandling;
using Kissarekisteri.Models;
using Kissarekisteri.RBAC;
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
    public async Task<Result<Cat>> UploadCatPhoto(int catId, IFormFile file)
    {
        var result = new Result<Cat>();

        var cat = await dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);

        var uploadedPhoto = await uploadService.UploadFile(file);

        if (uploadedPhoto == null)
        {
            return result.AddError(CatErrors.PhotoUploadError);
        }

        await dbContext.CatPhotos.AddAsync(
            new CatPhoto { CatId = cat.Id, Url = uploadedPhoto.Uri.AbsoluteUri }
        );

        await dbContext.SaveChangesAsync();

        var catWithPhotos = await dbContext.Cats
            .Include(c => c.Photos)
            .FirstOrDefaultAsync(cat => cat.Id == catId);

        return result.Success(catWithPhotos);
    }

    public async Task<Result<Cat>> UpdateCatByIdAsync(int catId, CatRequest catPayload)
    {
        var result = new Result<Cat>();
        var cat = await dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);

        if (cat == null)
        {
            return result.AddError(CatErrors.NotFound);
        }

        cat.Name = catPayload.Name;
        cat.Breed = catPayload.Breed;
        cat.BirthDate = catPayload.BirthDate;
        cat.ImageUrl = catPayload.ImageUrl;

        await dbContext.SaveChangesAsync();

        return result.Success(cat);
    }


    public async Task<Result<Cat>> GetCatByIdAsync(int catId)
    {
        var result = new Result<Cat>();

        var cat = await dbContext.Cats
            .Include(c => c.Photos)
            .Include(c => c.Results)
            .ThenInclude(Results => Results.CatShow)
            .Include(c => c.Parents)
            .ThenInclude(parent => parent.ParentCat)
            .Include(c => c.Kittens)
            .ThenInclude(kitten => kitten.ChildCat)
            .FirstOrDefaultAsync(cat => cat.Id == catId);

        if (cat == null)
            return result.AddError(CatErrors.NotFound);

        cat.Owner = await userService.GetUserById(cat.OwnerId);
        cat.Breeder = await userService.GetUserById(cat.BreederId);


        return result.Success(cat);
    }

    public async Task<Result<CatTransfer>> CreateTransferRequest(string userId, int catId)
    {
        var result = new Result<CatTransfer>();

        var cat = await dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);

        if (cat == null)
        {
            return result.AddError(CatErrors.NotFound);
        }

        var catTransfer = new CatTransfer
        {
            CatId = catId,
            RequesterId = userId,
            ConfirmerId = cat.OwnerId,
        };

        await dbContext.CatTransfers.AddAsync(catTransfer);
        await dbContext.SaveChangesAsync();

        return result.Success(catTransfer);
    }

    public async Task<Result<List<CatTransferResultDTO>>> GetTransferRequests(string userId)
    {
        var result = new Result<List<CatTransferResultDTO>>();

        var user = await userService.GetUserById(userId);
        var userIsAdmin = user?.UserRole?.Role?.Name == RoleType.Admin.ToString();

        var catTransfers = await dbContext.CatTransfers
            .Where(ct => !ct.Confirmed)
            .Where(ct => ct.ConfirmerId == userId || userIsAdmin)
            .ToListAsync();


        var catTransferResultDTOs = new List<CatTransferResultDTO>();

        foreach (var transfer in catTransfers)
        {
            var cat = await GetCatByIdAsync(transfer.CatId);
            var dto = new CatTransferResultDTO
            {
                Id = transfer.Id,
                CatId = transfer.CatId,
                Cat = cat.Data,
                RequesterId = transfer.RequesterId,
                ConfirmerId = transfer.ConfirmerId,
                Confirmed = transfer.Confirmed,
                Requester = await userService.GetUserById(transfer.RequesterId)
            };

            catTransferResultDTOs.Add(dto);
        }

        return result.Success(catTransferResultDTOs);
    }


    public async Task<Result<bool>> ConfirmTransferRequest(string userId, int transferId)
    {
        var result = new Result<bool>();

        var user = await userService.GetUserById(userId);

        var transfer = await dbContext.CatTransfers
           .FirstOrDefaultAsync(ct =>
               (ct.Id == transferId && ct.ConfirmerId == userId) ||
               (ct.Id == transferId && user.UserRole.Role.Name == "Admin")
           );

        if (transfer == null)
        {
            return result.AddError(CatErrors.NotFound);
        }

        transfer.Confirmed = true;
        await dbContext.SaveChangesAsync();

        var updatedCat = dbContext.Cats.FirstOrDefault(c => c.Id == transfer.CatId);
        updatedCat.OwnerId = transfer.RequesterId;
        await dbContext.SaveChangesAsync();

        return result.Success(true);
    }


    public async Task<Result<List<Cat>>> GetCatsAsync(CatQueryParamsDTO queryParams = null)
    {
        var result = new Result<List<Cat>>();
        queryParams ??= new CatQueryParamsDTO();

        var queryableCats = dbContext.Cats.AsQueryable();


        if (!string.IsNullOrEmpty(queryParams.Name))
        {
            queryableCats = queryableCats.Where(c => c.Name.Contains(queryParams.Name));
        }

        if (!string.IsNullOrEmpty(queryParams.Breed))
        {
            queryableCats = queryableCats.Where(c => c.Breed == queryParams.Breed);
        }

        if (!string.IsNullOrEmpty(queryParams.Sex))
        {
            queryableCats = queryableCats.Where(c => c.Sex == queryParams.Sex);
        }

        if (queryParams.Limit.HasValue)
        {
            queryableCats = queryableCats.Take(queryParams.Limit.Value);
        }


        var filteredCats = await queryableCats.ToListAsync();

        result.AddError(CatErrors.NotFound);


        return result.Success(filteredCats);
    }

    public async Task<Result<List<CatBreed>>> GetBreedsAsync()
    {
        var result = new Result<List<CatBreed>>();
        var breeds = await dbContext.CatBreeds.ToListAsync();
        return result.Success(breeds);
    }

    public async Task<Result<bool>> DeleteCatByIdAsync(int catId)
    {
        var result = new Result<bool>();
        var cat = await dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);
        dbContext.Cats.Remove(cat);
        await dbContext.SaveChangesAsync();


        return result.Success(true);
    }

    public async Task<Result<Cat>> CreateCat(CatRequest newCatRequest)
    {
        var result = new Result<Cat>();
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
            var motherResult = await GetCatByIdAsync(newCatRequest.MotherId.Value);
            if (motherResult.IsSuccess)
            {
                var motherRelation = new CatRelation
                {
                    ParentId = newCatRequest.MotherId.Value,
                    KittenId = cat.Id
                };
                dbContext.CatRelations.Add(motherRelation);
            }
            else
            {
                result.AddError(CatErrors.MotherNotFound);
            }
        }

        if (newCatRequest.FatherId.HasValue)
        {
            var fatherResult = await GetCatByIdAsync(newCatRequest.FatherId.Value);
            if (fatherResult.IsSuccess)
            {
                var fatherRelation = new CatRelation
                {
                    ParentId = newCatRequest.FatherId.Value,
                    KittenId = cat.Id
                };
                dbContext.CatRelations.Add(fatherRelation);
            }
            else
            {
                result.AddError(CatErrors.FatherNotFound);
            }
        }

        await dbContext.SaveChangesAsync();

        return result.Success(cat);
    }


    public async Task<Result<List<Cat>>> GetCatByUserIdAsync(string userId)
    {
        var result = new Result<List<Cat>>();
        var cats = await dbContext.Cats.Where(cat => cat.OwnerId == userId).ToListAsync();
        return result.Success(cats);
    }
}
