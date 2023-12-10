using Kissarekisteribackend.Database;
using Kissarekisteribackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Services
{
    public class CatShowService(KissarekisteriDbContext dbContext, UserService userService)
    {
        private readonly KissarekisteriDbContext _dbContext = dbContext;
        private readonly UserService _userService = userService;

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

        public async Task<CatShow> GetCatShowByIdAsync(int catShowId)
        {
            var catShow = await _dbContext.CatShows
                .Include(e => e.Attendees)
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

