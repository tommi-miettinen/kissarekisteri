using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;

public class CatShowController(CatShowService catShowService) : ODataController
{
    [Authorize]
    [HttpPost("api/{catShowId}/join")]
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
    [HttpDelete("api/{catShowId}/leave")]
    public async Task<IActionResult> LeaveCatShow(int catShowId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await catShowService.LeaveCatShowAsync(catShowId, userId);

        return Ok("Left cat show successfully");
    }


    [HttpGet("odata/catshows")]
    [EnableQuery]
    public ActionResult<IQueryable<CatShow>> GetEvents()
    {
        return Ok(catShowService.GetCatShows());
    }

    [Authorize]
    [HttpPost("api/{catShowId}/photos")]
    public async Task<ActionResult<CatShow>> UploadCatShowPhoto(int catShowId, IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var catShow = await catShowService.UploadCatShowPhoto(userId, catShowId, file);
        return Ok(catShow);
    }

    [HttpGet("{catShowId}")]
    public async Task<ActionResult<CatShow>> GetEvent(int catShowId)
    {
        var catShow = await catShowService.GetCatShowByIdAsync(catShowId);
        if (catShow == null)
        {
            return NotFound();
        }
        return Ok(catShow);
    }

    [Authorize]
    [HttpPost("api/{catShowId}/place")]
    public async Task<ActionResult<CatShowResult>> AssignCatPlacing(int catShowId, [FromBody] CatShowResultDTO resultPayload)
    {
        var result = await catShowService.AssignCatPlacing(catShowId, resultPayload);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CatShow>> CreateEvent([FromBody] CatShow newCatShow)
    {
        var catShow = await catShowService.CreateCatShow(newCatShow);
        return Ok(catShow);
    }
}
