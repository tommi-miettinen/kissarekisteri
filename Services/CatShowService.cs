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

namespace Kissarekisteri.Services
{
    public class CatShowService(
        KissarekisteriDbContext dbContext,
        UserService userService,
        UploadService uploadService,
        IMapper mapper
        )
    {
        private readonly KissarekisteriDbContext _dbContext = dbContext;
        private readonly UserService _userService = userService;
        private readonly UploadService _uploadService = uploadService;
        private readonly IMapper _mapper = mapper;

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
            await _dbContext.SaveChangesAsync();

            if (catIds.CatIds != null)
            {
                foreach (var catId in catIds.CatIds)
                {
                    var cat = _dbContext.Cats.FirstOrDefault(c => c.Id == catId);
                    if (cat != null)
                    {
                        _dbContext.CatAttendees.Add(new CatAttendee
                        {
                            AttendeeId = attendee.Id,
                            CatId = catId,
                            EventId = catShowId,

                        });
                        await _dbContext.SaveChangesAsync();

                    }
                }
            }
            return true;
        }

        public async Task<bool> LeaveCatShowAsync(int catShowId, string userId)
        {
            var catShow = await _dbContext.CatShows.FirstOrDefaultAsync(e => e.Id == catShowId);
            if (catShow == null)
            {
                return false;
            }

            var attendee = await _dbContext.Attendees
                .FirstOrDefaultAsync(a => a.UserId == userId && a.EventId == catShowId);

            if (attendee == null)
            {
                return false;
            }

            _dbContext.Attendees.Remove(attendee);

            var catAttendees = await _dbContext.CatAttendees
                .Where(ca => ca.EventId == catShowId && ca.Cat.OwnerId == userId)
                .ToListAsync();

            _dbContext.CatAttendees.RemoveRange(catAttendees);

            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<List<CatShow>> GetCatShows()
        {
            var catShows = await _dbContext.CatShows.ToListAsync();
            return catShows;
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
                .Include(e => e.Attendees).ThenInclude(a => a.CatAttendees).ThenInclude(c => c.Cat)
                .Include(e => e.Photos)
                .Include(e => e.Results)
                .FirstOrDefaultAsync(e => e.Id == catShowId);

            if (catShow == null) return null;


            foreach (var attendee in catShow.Attendees)
            {
                var user = await _userService.GetUserById(attendee.UserId);
                if (user != null)
                {
                    attendee.User = user;
                }
            }

            return catShow;
        }

        public async Task<CatShowResult> AssignCatPlacing(int catShowId, CatShowResultDTO newPlacing)
        {
            var catShow = await _dbContext.CatShows.AnyAsync(e => e.Id == catShowId);

            if (!catShow)
            {
                return null;
            }

            var catShowResult = await _dbContext.CatShowResults.AddAsync(new CatShowResult
            {
                CatId = newPlacing.CatId,
                Breed = newPlacing.Breed,
                CatShowId = catShowId,
                Place = (Place)newPlacing.Place
            });
            await _dbContext.SaveChangesAsync();

            return catShowResult.Entity;
        }

        public async Task<CatShow> CreateCatShow(CatShow newCatShow)
        {

            var catShow = await _dbContext.CatShows.AddAsync(newCatShow);
            await _dbContext.SaveChangesAsync();

            return catShow.Entity;
        }
    }
}

