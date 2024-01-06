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
        UserService userService,
        UploadService uploadService
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
            var attendee = new Attendee { UserId = userId, EventId = catShowId };
            dbContext.Attendees.Add(attendee);
            await dbContext.SaveChangesAsync();

            foreach (var catId in catIds.CatIds)
            {
                var cat = dbContext.Cats.FirstOrDefault(c => c.Id == catId);
                if (cat != null)
                {
                    dbContext.CatAttendees.Add(
                        new CatAttendee
                        {
                            AttendeeId = attendee.Id,
                            CatId = catId,
                            EventId = catShowId,
                        }
                    );
                    await dbContext.SaveChangesAsync();
                }
            }
            return result.Success(true);
        }

        public async Task<Result<bool>> LeaveCatShowAsync(int catShowId, string userId)
        {
            var result = new Result<bool>();

            var catShow = await dbContext.CatShows.FirstOrDefaultAsync(e => e.Id == catShowId);

            if (catShow == null)
            {
                return result.AddError(CatShowErrors.NotFound);
            }

            var attendee = await dbContext.Attendees.FirstOrDefaultAsync(
                a => a.UserId == userId && a.EventId == catShowId
            );

            if (attendee == null)
            {
                return result.AddError(CatShowErrors.NotFound);
            }

            dbContext.Attendees.Remove(attendee);

            var catAttendees = await dbContext.CatAttendees
                .Where(ca => ca.EventId == catShowId && ca.Cat.OwnerId == userId)
                .ToListAsync();

            dbContext.CatAttendees.RemoveRange(catAttendees);

            await dbContext.SaveChangesAsync();

            return result.Success(true);
        }

        public async Task<List<CatShow>> GetCatShows()
        {
            var catShows = await dbContext.CatShows.ToListAsync();
            return catShows;
        }

        public async Task<CatShow> UploadCatShowPhoto(int catShowId, IFormFile file)
        {
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
                .Include(e => e.Attendees)
                .ThenInclude(a => a.CatAttendees)
                .ThenInclude(c => c.Cat)
                .Include(e => e.Photos)
                .Include(e => e.Results)
                .FirstOrDefaultAsync(e => e.Id == catShowId);

            if (catShow == null)
                return null;

            catShow.Attendees.ForEach(async attendee =>
            {
                var user = await userService.GetUserById(attendee.UserId);
                if (user != null)
                {
                    attendee.User = user;
                }
            });

            return catShow;
        }

        public async Task<Result<CatShowResult>> AssignCatPlacing(
            int catShowId,
            CatShowResultDTO newPlacing
        )
        {
            var result = new Result<CatShowResult>();
            var catShow = await dbContext.CatShows.AnyAsync(e => e.Id == catShowId);

            if (!catShow)
            {
                return result.AddError(CatShowErrors.NotFound);
            }

            var existingCatShowResult = await dbContext.CatShowResults.FirstOrDefaultAsync(
                e => e.CatId == newPlacing.CatId && e.CatShowId == catShowId
            );

            if (existingCatShowResult != null)
            {
                existingCatShowResult.Place = (Place)newPlacing.Place;
                result.Success(existingCatShowResult);
            }
            else
            {
                var catShowResult = await dbContext.CatShowResults.AddAsync(
                    new CatShowResult
                    {
                        CatId = newPlacing.CatId,
                        Breed = newPlacing.Breed,
                        CatShowId = catShowId,
                        Place = (Place)newPlacing.Place
                    }
                );

                result = result.Success(catShowResult.Entity);
            }

            await dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<CatShow> CreateCatShow(CatShow newCatShow)
        {
            var catShow = await dbContext.CatShows.AddAsync(newCatShow);
            await dbContext.SaveChangesAsync();

            return catShow.Entity;
        }
    }
}
