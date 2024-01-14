using Kissarekisteri.AccessControl;
using Kissarekisteri.Database;
using Kissarekisteri.DTOs;
using Kissarekisteri.ErrorHandling;
using Kissarekisteri.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services
{
    public class CatShowService(
        KissarekisteriDbContext dbContext,
        UploadService uploadService,
        PermissionService permissionService
    )
    {
        public async Task<Result<bool>> JoinCatShowAsync(
            int catShowId,
            string userId,
            CatShowCatAttendeeIds catIds
        )
        {
            var result = new Result<bool>();
            var catShow = dbContext.CatShows.FirstOrDefault(e => e.Id == catShowId);
            if (catShow == null)
            {
                return result.AddError(CatShowErrors.NotFound);
            }

            var catsThatHavePlacingIds = await dbContext.CatShowResults
               .Where(c => c.CatShowId == catShowId)
               .Select(c => c.CatId)
               .ToListAsync();

            var catsToRemove = await dbContext.CatShowCats
                .Where(c => c.CatShowId == catShowId)
                .Where(c => c.Cat.OwnerId == userId)
                .Where(c => !catsThatHavePlacingIds.Contains(c.CatId))
                .ToListAsync();

            dbContext.CatShowCats.RemoveRange(catsToRemove);

            await dbContext.SaveChangesAsync();

            var cats = await dbContext.Cats
                .Where(c => catIds.CatIds.Contains(c.Id))
                .Where(c => c.OwnerId == userId)
                .Where(c => !catsThatHavePlacingIds.Contains(c.Id))
                .ToListAsync();

            var catsToAdd = new List<CatShowCats>();

            foreach (var cat in cats)
            {
                catsToAdd.Add(new CatShowCats { CatId = cat.Id, CatShowId = catShowId });
            }

            dbContext.CatShowCats.AddRange(catsToAdd);
            await dbContext.SaveChangesAsync();

            return result.Success(true);
        }


        public async Task<Result<bool>> LeaveCatShowAsync(int catShowId, string userId)
        {
            var result = new Result<bool>();
            var catsThatHavePlacingIds = await dbContext.CatShowResults
       .Where(c => c.CatShowId == catShowId)
       .Select(c => c.CatId)
       .ToListAsync();

            var catsToRemove = await dbContext.CatShowCats
                .Where(c => c.CatShowId == catShowId)
                .Where(c => c.Cat.OwnerId == userId)
                .Where(c => !catsThatHavePlacingIds.Contains(c.CatId))
                .ToListAsync();

            dbContext.CatShowCats.RemoveRange(catsToRemove);

            await dbContext.SaveChangesAsync();

            return result.Success(true);
        }

        public async Task<List<CatShow>> GetCatShows()
        {
            var catShows = await dbContext.CatShows.ToListAsync();
            return catShows;
        }

        public async Task<CatShow> UploadCatShowPhoto(string userId, int catShowId, IFormFile file)
        {
            var hasPermission = await permissionService.HasPermission(userId, Permissions.CatShowWrite);

            if (!hasPermission)
            {
                return null;
            }

            var cat = await dbContext.CatShows.FirstOrDefaultAsync(
                catShow => catShow.Id == catShowId
            );

            var uploadedPhoto = await uploadService.UploadFile(file);

            await dbContext.CatShowPhotos.AddAsync(
                new CatShowPhoto { CatShowId = cat.Id, Url = uploadedPhoto.Uri.AbsoluteUri }
            );

            await dbContext.SaveChangesAsync();

            var catShowWithPhotos = await dbContext.CatShows
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(catShow => catShow.Id == catShowId);

            return catShowWithPhotos;
        }

        public async Task<CatShow> GetCatShowByIdAsync(int catShowId)
        {
            var catShow = await dbContext.CatShows
                .Include(catShow => catShow.Cats)
                .ThenInclude(catShowCats => catShowCats.Cat)
                .Include(e => e.Photos)
                .Include(e => e.Results)
                .FirstOrDefaultAsync(e => e.Id == catShowId);

            if (catShow == null)
                return null;

            return catShow;
        }

        public async Task<Result<CatShowResult>> AssignCatPlacing(
            int catShowId,
            CatShowResultDTO newPlacing
        )
        {
            var result = new Result<CatShowResult>();
            var catShowExists = await dbContext.CatShows.AnyAsync(e => e.Id == catShowId);

            if (!catShowExists)
            {
                return result.AddError(CatShowErrors.NotFound);
            }

            var conflictingResults = await dbContext.CatShowResults
            .Where(v => v.CatShowId == catShowId)
            .Where(v => v.Breed == newPlacing.Breed && v.Place == (Place)newPlacing.Place ||
                        v.CatId == newPlacing.CatId)
            .ToListAsync();

            if (conflictingResults.Count != 0)
            {
                dbContext.CatShowResults.RemoveRange(conflictingResults);
                await dbContext.SaveChangesAsync();
            }

            var newCatShowResult = new CatShowResult
            {
                CatId = newPlacing.CatId,
                Breed = newPlacing.Breed,
                CatShowId = catShowId,
                Place = (Place)newPlacing.Place
            };
            dbContext.CatShowResults.Add(newCatShowResult);

            await dbContext.SaveChangesAsync();
            return result.Success(newCatShowResult);
        }

        public async Task<CatShow> CreateCatShow(CatShow newCatShow)
        {
            var catShow = await dbContext.CatShows.AddAsync(newCatShow);
            await dbContext.SaveChangesAsync();

            return catShow.Entity;
        }
    }
}
