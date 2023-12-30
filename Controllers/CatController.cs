using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers;

public class CatController(CatService catService) : Controller
{
    private readonly CatService _catService = catService;

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

    [HttpGet("/cats")]
    public async Task<ActionResult<List<Cat>>> GetCats([FromQuery] CatQueryParamsDTO queryParameters)
    {
        var cats = await _catService.GetCatsAsync(queryParameters);
        return Json(cats);
    }

    /// <summary>
    /// Retrieves all cat breeds
    /// </summary>
    /// <returns>A list of breeds</returns>
    /// <response media="application/json" code="200">Returns the list of breeds</response>
    [HttpGet("/cats/breeds")]
    public async Task<ActionResult<List<CatBreed>>> GetBreeds()
    {
        var breeds = await _catService.GetBreedsAsync();
        return Json(breeds);
    }

    /// <summary>
    /// Retrieves a cat by its unique identifier.
    /// </summary>
    /// <param name="catId">The ID of the cat to retrieve.</param>
    /// <returns>The cat object if found.</returns>
    /// <response media="application/json" code="200">Returns the cat object</response>
    /// <response code="404">If a cat with the specified ID is not found</response>

    [HttpGet("/cats/{catId}")]
    public async Task<ActionResult<Cat>> GetCatAsync(int catId)
    {
        var cat = await _catService.GetCatByIdAsync(catId);
        if (cat == null)
        {
            return NotFound();
        }
        return Json(cat);
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
    [HttpPost("cats/{catId}/photo")]
    public async Task<ActionResult<Cat>> UploadCatPhoto(int catId, IFormFile file)
    {
        var cat = await _catService.UploadCatPhoto(catId, file);
        return Json(cat);
    }

    /// <summary>
    /// Updates the cats attributes.
    /// </summary>
    /// <param name="catId">The ID of the cat to update.</param>
    /// <param name="catPayload"></param>
    /// <returns>The updated cat</returns>
    [Authorize]
    [HttpPut("cats/{catId}")]
    public async Task<ActionResult<Cat>> EditCat(int catId, [FromBody] CatRequest catPayload)
    {
        var cat = await _catService.UpdateCatByIdAsync(catId, catPayload);
        return Json(cat);
    }

    /// <summary>
    /// Creates a new cat.
    /// </summary>
    /// <param name="catPayload"></param>
    /// <returns>The new cat</returns>
    [Authorize]
    [HttpPost("/cats")]
    public async Task<ActionResult<Cat>> CreateCat([FromBody] CatRequest catPayload)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        catPayload.OwnerId = userId;
        catPayload.BreederId = userId;

        var newCat = await _catService.CreateCat(catPayload);
        return Json(newCat);
    }

    /// <summary>
    /// Deletes a cat by its unique identifier.
    /// </summary>
    /// <param name="catId"></param>
    /// <returns>No Content</returns>
    [Authorize]
    [HttpDelete("/cats/{catId}")]
    public async Task<IActionResult> DeleteCat(int catId)
    {
        await _catService.DeleteCatByIdAsync(catId);
        return NoContent();
    }
}
