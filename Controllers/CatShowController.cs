﻿using Kissarekisteri.DTOs;
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

[Route("odata/catshows")]
public class CatShowController(CatShowService catShowService) : ODataController
{
    [Authorize]
    [HttpPost("{catShowId}/join")]
    public async Task<IActionResult> JoinCatShow(int catShowId, [FromBody] CatIdsDTO catIds)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await catShowService.JoinCatShowAsync(catShowId, userId, catIds.CatIds);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{catShowId}/leave")]
    public async Task<IActionResult> LeaveCatShow(int catShowId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await catShowService.LeaveCatShowAsync(catShowId, userId);
        return Ok(result);
    }

    [HttpGet]
    [EnableQuery]
    public ActionResult<IQueryable<CatShow>> GetEvents()
    {
        return Ok(catShowService.GetCatShows());
    }

    [Authorize]
    [HttpPost("{catShowId}/photos")]
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
    [HttpPost("{catShowId}/place")]
    public async Task<ActionResult<CatShowResult>> AssignCatPlacing(
        int catShowId,
        [FromBody] CatShowResultDTO resultPayload
    )
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await catShowService.AssignCatPlacing(userId, catShowId, resultPayload);
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
