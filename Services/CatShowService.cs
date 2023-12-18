using Kissarekisteri.Database;
using Kissarekisteri.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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
        private readonly KissarekisteriDbContext _dbContext = dbContext;
        private readonly UserService _userService = userService;
        private readonly UploadService _uploadService = uploadService;

        public async Task<bool> JoinCatShowAsync(int catShowId, string userId, CatShowCatAttendeeIds catIds)
        {
            var catShow = _dbContext.CatShows.FirstOrDefault(e => e.Id == catShowId);
            if (catShow == null)
            {
                return false;
            }
            var attendee = new Attendee
            {
                UserId = userId,
                EventId = catShowId

            };
            _dbContext.Attendees.Add(attendee);
            _dbContext.SaveChanges();

            if (catIds.catIds != null)
            {

                foreach (var catId in catIds.catIds)
                {
                    var cat = _dbContext.Cats.FirstOrDefault(c => c.Id == catId);
                    if (cat != null)
                    {
                        var catAttendee = new CatAttendee
                        {
                            CatId = catId,
                            EventId = catShowId,

                        };
                        _dbContext.CatAttendees.Add(catAttendee);
                        _dbContext.SaveChanges();

                    }
                }
            }
            return true;
        }

        public async Task<CatShow> UploadCatShowPhoto(int catShowId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("no file");
                return null;
            }
            var cat = await _dbContext.CatShows.FirstOrDefaultAsync(catShow => catShow.Id == catShowId);

            var uploadedPhoto = await _uploadService.UploadFile(file);

            await _dbContext.CatShowPhotos.AddAsync(new CatShowPhoto
            {
                CatShowId = cat.Id,
                Url = uploadedPhoto.Uri.AbsoluteUri
            });

            await _dbContext.SaveChangesAsync();

            var catShowWithPhotos = await _dbContext.CatShows.Include(c => c.Photos).FirstOrDefaultAsync(catShow => catShow.Id == catShowId);

            return catShowWithPhotos;
        }

        public async Task<CatShow> GetCatShowByIdAsync(int catShowId)
        {
            var catShow = await _dbContext.CatShows
                .Include(e => e.Attendees)
                .Include(e => e.Photos)
                .FirstOrDefaultAsync(e => e.Id == catShowId);

            if (catShow == null) return null;

            catShow.AttendeeDetails = [];

            foreach (var attendee in catShow.Attendees)
            {
                var user = await _userService.GetUserById(attendee.UserId);
                if (user != null)
                {
                    catShow.AttendeeDetails.Add(user);
                }
            }

            return catShow;

        }
    }
}

