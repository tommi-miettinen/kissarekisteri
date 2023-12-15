using Kissarekisteribackend.Database;
using Kissarekisteribackend.Models;
using Kissarekisteribackend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;
public class CatShowController(KissarekisteriDbContext dbContext, CatShowService catShowService) : Controller
{
    private readonly KissarekisteriDbContext _dbContext = dbContext;
    private readonly CatShowService _catShowService = catShowService;

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("catshows/{catShowId}/join")]
    public async Task<IActionResult> JoinCatShow(int catShowId, [FromBody] CatShowCatAttendeeIds catIds)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _catShowService.JoinCatShowAsync(catShowId, userId, catIds);

        return Ok("onnistu");
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpDelete("catshows/{catShowId}/leave")]
    public async Task<IActionResult> LeaveCatShow(int catShowId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var catShow = await _dbContext.CatShows.FirstOrDefaultAsync(e => e.Id == catShowId);
        if (catShow == null)
        {
            return NotFound("Cat show not found");
        }

        var attendee = await _dbContext.Attendees.FirstOrDefaultAsync(a => a.UserId == userId && a.EventId == catShowId);
        if (attendee != null)
        {
            _dbContext.Attendees.Remove(attendee);
            await _dbContext.SaveChangesAsync();
        }

        var catAttendees = await _dbContext.CatAttendees.Where(ca => ca.EventId == catShowId && ca.Cat.OwnerId == userId).ToListAsync();

        foreach (var catAttendee in catAttendees)
        {
            _dbContext.CatAttendees.Remove(catAttendee);
        }

        if (catAttendees.Count > 0)
        {
            await _dbContext.SaveChangesAsync();
        }

        return Ok("Left cat show successfully");
    }

    [HttpGet("catshows")]
    public IActionResult GetEvents()
    {
        var catShows = _dbContext.CatShows.ToList();
        return Json(catShows);
    }

    [HttpGet("catshows/{catShowId}")]
    public async Task<IActionResult> GetEvent(int catShowId)
    {

        var catShow = await _catShowService.GetCatShowByIdAsync(catShowId);
        return Json(catShow);
    }

    [HttpPost("catshows")]
    public IActionResult CreateEvent([FromBody] CatShow newCatShow)
    {
        if (newCatShow == null)
        {
            return BadRequest("Invalid event data");
        }

        _dbContext.CatShows.Add(newCatShow);
        _dbContext.SaveChanges();

        return Json(newCatShow);
    }
}

