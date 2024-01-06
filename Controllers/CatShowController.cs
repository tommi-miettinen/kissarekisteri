using Kissarekisteri.Authorization;
using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Kissarekisteri.RBAC;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;

[ApiController]
[Route("api/catshows")]
public class CatShowController(CatShowService catShowService, SeedService seedService) : Controller
{
    [Authorize]
    [HttpPost("{catShowId}/join")]
    public async Task<IActionResult> JoinCatShow(
        int catShowId,
        [FromBody] CatShowCatAttendeeIds catIds
    )
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await catShowService.JoinCatShowAsync(catShowId, userId, catIds);

        return Ok("onnistu");
    }

    [Authorize]
    [HttpDelete("{catShowId}/leave")]
    public async Task<IActionResult> LeaveCatShow(int catShowId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await catShowService.LeaveCatShowAsync(catShowId, userId);

        return Ok("Left cat show successfully");
    }

    [HttpPost("seed")]
    public async Task<ActionResult> Seed()
    {
        try
        {
            await seedService.SeedCatShows(true, 20);
            return Ok();
        }
        catch (System.Exception e)
        {
            return Json(e.ToString());
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<CatShow>>> GetEvents()
    {
        var catShows = await catShowService.GetCatShows();
        return Json(catShows);
    }

    [Authorize]
    [HttpPost("{catShowId}/photos")]
    public async Task<ActionResult<CatShow>> UploadCatShowPhoto(int catShowId, IFormFile file)
    {
        var catShow = await catShowService.UploadCatShowPhoto(catShowId, file);
        return Json(catShow);
    }

    [HttpGet("{catShowId}")]
    public async Task<ActionResult<CatShow>> GetEvent(int catShowId)
    {
        var catShow = await catShowService.GetCatShowByIdAsync(catShowId);
        if (catShow == null)
        {
            return NotFound();
        }
        return Json(catShow);
    }

    [Authorize]
    [PermissionAuthorize(PermissionType.CreateCatShowResult)]
    [HttpPost("{catShowId}/place")]
    public async Task<ActionResult<CatShowResult>> AssignCatPlacing(int catShowId, [FromBody] CatShowResultDTO resultPayload)
    {
        var result = await catShowService.AssignCatPlacing(catShowId, resultPayload);
        return Json(result);
    }

    [Authorize]
    [PermissionAuthorize(PermissionType.CreateEvent)]
    [HttpPost]
    public async Task<ActionResult<CatShow>> CreateEvent([FromBody] CatShow newCatShow)
    {
        var catShow = await catShowService.CreateCatShow(newCatShow);
        return Json(catShow);
    }
}
