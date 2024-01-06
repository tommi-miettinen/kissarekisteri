using Kissarekisteri.DTOs;
using Kissarekisteri.ErrorHandling;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;


[ApiController]
[Route("api/cats")]
public class CatController(CatService catService, SeedService seedService) : Controller
{
    /// <summary>
    /// Retrieves all cats with optional filters.
    /// </summary>
    /// <remarks>
    /// Use the queryParameters to filter the results. You can specify 
    /// name, breed, limit, and include options. The 'include' parameter 
    /// allows including additional data like parents and results.
    /// </remarks>
    /// <param name="queryParameters">Optional query parameters for filtering the results</param>
    /// <returns>A list of cats</returns>
    /// <response media="application/json" code="200">Returns the list of cats</response>

    [HttpGet]
    public async Task<ActionResult<Result<List<Cat>>>> GetCats([FromQuery] CatQueryParamsDTO queryParameters)
    {
        var cats = await catService.GetCatsAsync(queryParameters);
        return Json(cats);
    }

    [HttpPost("seed")]
    public async Task<ActionResult> Seed()
    {
        try
        {
            await seedService.SeedCats(true, 200);
            return Ok();
        }
        catch (System.Exception e)
        {
            return Json(e.ToString());
        }
    }

    /// <summary>
    /// Retrieves all cat breeds
    /// </summary>
    /// <returns>A list of breeds</returns>
    /// <response media="application/json" code="200">Returns the list of breeds</response>
    [HttpGet("breeds")]
    public async Task<ActionResult<List<CatBreed>>> GetBreeds()
    {
        var breeds = await catService.GetBreedsAsync();
        return Json(breeds);
    }

    /// <summary>
    /// Retrieves a cat by its unique identifier.
    /// </summary>
    /// <param name="catId">The ID of the cat to retrieve.</param>
    /// <returns>The cat object if found.</returns>
    /// <response media="application/json" code="200">Returns the cat object</response>
    /// <response code="404">If a cat with the specified ID is not found</response>

    [HttpGet("{catId}")]
    public async Task<ActionResult<Cat>> GetCatAsync(int catId)
    {
        var result = await catService.GetCatByIdAsync(catId);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Json(result);
    }


    /// <summary>
    /// Uploads a photo for a specific cat identified by its ID.
    /// </summary>

    /// <param name="catId">The ID of the cat to update.</param>
    /// <param name="file"></param>
    /// <returns>
    /// Returns an ActionResult containing the updated Cat object. The Cat object
    /// will include the details of the uploaded photo if the operation is successful.
    /// In case of errors, appropriate HTTP status codes and error messages are returned.
    /// </returns>
    /// <example>
    /// POST /cats/123/photo
    /// Content-Disposition: form-data; name="file"; filename="cat_photo.jpg"
    /// </example>
    [Authorize]
    [HttpPost("{catId}/photo")]
    public async Task<ActionResult<Cat>> UploadCatPhoto(int catId, IFormFile file)
    {
        var result = await catService.UploadCatPhoto(catId, file);
        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }
        return Json(result.Data);
    }

    /// <summary>
    /// Updates the cats attributes.
    /// </summary>
    /// <param name="catId">The ID of the cat to update.</param>
    /// <param name="catPayload"></param>
    /// <returns>The updated cat</returns>
    [Authorize]
    [HttpPut("{catId}")]
    public async Task<ActionResult<Cat>> EditCat(int catId, [FromBody] CatRequest catPayload)
    {
        var result = await catService.UpdateCatByIdAsync(catId, catPayload);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Json(result.Data);
    }

    [Authorize]
    [HttpPost("{catId}/transfer")]
    public async Task<ActionResult<Result<CatTransfer>>> TransferCat([FromRoute] int catId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await catService.CreateTransferRequest(userId, catId);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Json(result);
    }

    [Authorize]
    [HttpGet("transfer-requests")]
    public async Task<ActionResult<Result<List<CatTransfer>>>> GetTransferRequests()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await catService.GetTransferRequests(userId);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Json(result);
    }

    [Authorize]
    [HttpPost("transfer-requests/{transferId}/confirm")]
    public async Task<ActionResult<Result<CatTransfer>>> ConfirmTransferRequest([FromRoute] int transferId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await catService.ConfirmTransferRequest(userId, transferId);

        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }

        return Json(result);
    }

    /// <summary>
    /// Creates a new cat.
    /// </summary>
    /// <param name="catPayload"></param>
    /// <returns>The new cat</returns>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Cat>> CreateCat([FromBody] CatRequest catPayload)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        catPayload.OwnerId = userId;
        catPayload.BreederId = userId;

        var newCat = await catService.CreateCat(catPayload);
        return Json(newCat);
    }

    /// <summary>
    /// Deletes a cat by its unique identifier.
    /// </summary>
    /// <param name="catId"></param>
    /// <returns>No Content</returns>
    [Authorize]
    [HttpDelete("{catId}")]
    public async Task<ActionResult> DeleteCat(int catId)
    {
        await catService.DeleteCatByIdAsync(catId);
        return NoContent();
    }
}
