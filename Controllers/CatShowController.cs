using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;

public class CatShowController(CatShowService catShowService)
    : Controller
{
    private readonly CatShowService _catShowService = catShowService;

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("catshows/{catShowId}/join")]
    public async Task<IActionResult> JoinCatShow(
        int catShowId,
        [FromBody] CatShowCatAttendeeIds catIds
    )
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
        await _catShowService.LeaveCatShowAsync(catShowId, userId);

        return Ok("Left cat show successfully");
    }

    [HttpGet("catshows")]
    public async Task<ActionResult<List<CatShow>>> GetEvents()
    {
        var catShows = await _catShowService.GetCatShows();
        return Json(catShows);
    }

    [HttpPost("catshows/{catShowId}/photos")]
    public async Task<ActionResult<CatShow>> UploadCatShowPhoto(int catShowId, IFormFile file)
    {
        var catShow = await _catShowService.UploadCatShowPhoto(catShowId, file);
        return Json(catShow);
    }

    [HttpGet("catshows/{catShowId}")]
    public async Task<ActionResult<CatShow>> GetEvent(int catShowId)
    {
        var catShow = await _catShowService.GetCatShowByIdAsync(catShowId);
        return Json(catShow);
    }

    [HttpPost("catshows/{catShowId}/place")]
    public async Task<ActionResult<CatShowResult>> AssignCatPlacing(int catShowId, [FromBody] CatShowResultDTO resultPayload)
    {
        var result = await _catShowService.AssignCatPlacing(catShowId, resultPayload);
        return result;
    }

    [HttpPost("catshows")]
    public async Task<ActionResult<CatShow>> CreateEvent([FromBody] CatShow newCatShow)
    {
        if (newCatShow == null)
        {
            return BadRequest("Invalid event data");
        }

        var catShow = await _catShowService.CreateCatShow(newCatShow);

        return Json(catShow);
    }
}
