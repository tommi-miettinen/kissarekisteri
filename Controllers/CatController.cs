using Kissarekisteri.DTOs;
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

[Route("odata/cats")]
public class CatController(CatService catService) : ODataController
{
    [HttpGet]
    [EnableQuery]
    public IQueryable<Cat> GetCats()
    {
        return catService.GetCats();
    }

    [HttpGet("catbreeds")]
    [EnableQuery]
    public IQueryable<CatBreed> GetBreeds()
    {
        return catService.GetBreeds();
    }

    [Authorize]
    [HttpPost("{catId}/photo")]
    public async Task<ActionResult<Cat>> UploadCatPhoto(int catId, IFormFile file)
    {
        var result = await catService.UploadCatPhoto(catId, file);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{catId}")]
    public async Task<ActionResult<Cat>> EditCat(
        int catId,
        [FromBody] CatCreateRequestDTO catPayload
    )
    {
        var result = await catService.UpdateCatByIdAsync(catId, catPayload);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{catId}/transfer")]
    public async Task<ActionResult<CatTransfer>> TransferCat([FromRoute] int catId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await catService.CreateTransferRequest(userId, catId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("transfer-requests")]
    public async Task<ActionResult<List<CatTransfer>>> GetTransferRequests()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await catService.GetTransferRequests(userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("transfer-requests/{transferId}/confirm")]
    public async Task<ActionResult<CatTransfer>> ConfirmTransferRequest([FromRoute] int transferId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await catService.ConfirmTransferRequest(userId, transferId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Cat>> CreateCat([FromBody] CatCreateRequestDTO catPayload)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        catPayload.OwnerId = userId;
        catPayload.BreederId = userId;

        var newCat = await catService.CreateCat(catPayload);
        return Ok(newCat);
    }

    [Authorize]
    [HttpDelete("{catId}")]
    public async Task<ActionResult> DeleteCat(int catId)
    {
        await catService.DeleteCatByIdAsync(catId);
        return NoContent();
    }
}
