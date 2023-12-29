using Kissarekisteri.Authorization;
using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Kissarekisteri.RBAC;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;

public class CatShowController(CatShowService catShowService) : Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("catshows/{catShowId}/join")]
    public async Task<IActionResult> JoinCatShow(
        int catShowId,
        [FromBody] CatShowCatAttendeeIds catIds
    )
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await catShowService.JoinCatShowAsync(catShowId, userId, catIds);

        return Ok("onnistu");
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpDelete("catshows/{catShowId}/leave")]
    public async Task<IActionResult> LeaveCatShow(int catShowId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await catShowService.LeaveCatShowAsync(catShowId, userId);

        return Ok("Left cat show successfully");
    }

    [HttpGet("catshows")]
    public async Task<ActionResult<List<CatShow>>> GetEvents()
    {
        var catShows = await catShowService.GetCatShows();
        return Json(catShows);
    }

    [HttpPost("catshows/{catShowId}/photos")]
    public async Task<ActionResult<CatShow>> UploadCatShowPhoto(int catShowId, IFormFile file)
    {
        var catShow = await catShowService.UploadCatShowPhoto(catShowId, file);
        return Json(catShow);
    }

    [HttpGet("catshows/{catShowId}")]
    public async Task<ActionResult<CatShow>> GetEvent(int catShowId)
    {
        var catShow = await catShowService.GetCatShowByIdAsync(catShowId);
        if (catShow == null)
        {
            return NotFound();
        }
        return Json(catShow);
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [PermissionAuthorize(PermissionType.CreateCatShowResult)]
    [HttpPost("catshows/{catShowId}/place")]
    public async Task<ActionResult<CatShowResult>> AssignCatPlacing(int catShowId, [FromBody] CatShowResultDTO resultPayload)
    {
        var result = await catShowService.AssignCatPlacing(catShowId, resultPayload);
        return result;
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [PermissionAuthorize(PermissionType.CreateEvent)]
    [HttpPost("catshows")]
    public async Task<ActionResult<CatShow>> CreateEvent([FromBody] CatShow newCatShow)
    {
        if (newCatShow == null)
        {
            return BadRequest("Invalid event data");
        }


        var catShow = await catShowService.CreateCatShow(newCatShow);

        return Json(catShow);
    }
}
