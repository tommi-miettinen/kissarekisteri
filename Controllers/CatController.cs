using Kissarekisteri.DTOs;
using Kissarekisteri.ErrorHandling;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;

public class CatController(CatService catService) : ODataController
{
    [HttpGet("odata/cats")]
    [EnableQuery]
    public ActionResult<IQueryable> GetCats()
    {
        return Ok(catService.GetCats());
    }

    [HttpGet("odata/catbreeds")]
    [EnableQuery]
    public ActionResult<IQueryable<CatBreed>> GetBreeds()
    {
        return Ok(catService.GetBreeds());
    }


    [Authorize]
    [HttpPost("api/cats/{catId}/photo")]
    public async Task<ActionResult<Cat>> UploadCatPhoto(int catId, IFormFile file)
    {
        var result = await catService.UploadCatPhoto(catId, file);
        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }
        return Ok(result.Data);
    }


    [Authorize]
    [HttpPut("api/cats/{catId}")]
    public async Task<ActionResult<Cat>> EditCat(int catId, [FromBody] CatRequest catPayload)
    {
        var result = await catService.UpdateCatByIdAsync(catId, catPayload);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("api/cats/{catId}/transfer")]
    public async Task<ActionResult<Result<CatTransfer>>> TransferCat([FromRoute] int catId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await catService.CreateTransferRequest(userId, catId);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("api/cats/transfer-requests")]
    public async Task<ActionResult<Result<List<CatTransfer>>>> GetTransferRequests()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await catService.GetTransferRequests(userId);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("api/cats/transfer-requests/{transferId}/confirm")]
    public async Task<ActionResult<Result<CatTransfer>>> ConfirmTransferRequest([FromRoute] int transferId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await catService.ConfirmTransferRequest(userId, transferId);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("api/cats")]
    public async Task<ActionResult<Cat>> CreateCat([FromBody] CatRequest catPayload)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        catPayload.OwnerId = userId;
        catPayload.BreederId = userId;

        var newCat = await catService.CreateCat(catPayload);
        return Ok(newCat);
    }


    [Authorize]
    [HttpDelete("api/cats/{catId}")]
    public async Task<ActionResult> DeleteCat(int catId)
    {
        await catService.DeleteCatByIdAsync(catId);
        return NoContent();
    }
}
