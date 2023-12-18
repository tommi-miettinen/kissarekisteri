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
public class CatController(CatService catService) : Controller
{
    private readonly CatService _catService = catService;

    [HttpGet("/cats")]
    public async Task<ActionResult<List<Cat>>> GetCats()
    {
        var cats = await _catService.GetCatsAsync();
        return Json(cats);
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
    /// Gets all cats owned by a user.
    /// <summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>The cats owned by the user</returns>
    [HttpGet("/users/{userId}/cats")]
    public async Task<ActionResult<Cat>> GetCatsByUserId(string userId)
    {
        var catsByUserId = await _catService.GetCatByUserIdAsync(userId);
        return Json(catsByUserId);
    }

    /// <summary>
    /// Uploads a photo for a specific cat identified by its ID.
    /// </summary>
    /// <remarks>
    /// This method accepts a photo file as form data. The photo is then associated 
    /// with the cat having the specified ID. It's important that the file is sent 
    /// as part of a multipart/form-data request with the input name set to "file".
    /// </remarks>
    /// <param name="catId">
    /// The unique identifier of the cat for which the photo is being uploaded. This 
    /// ID is used to locate the cat in the database.
    /// </param>
    /// <param name="file">
    /// The photo file to upload. This should be a form file provided as part of the 
    /// request body. The file format can be restricted based on requirements (e.g., 
    /// JPEG, PNG).
    /// </param>
    /// <returns>
    /// Returns an ActionResult containing the updated Cat object. The Cat object 
    /// will include the details of the uploaded photo if the operation is successful. 
    /// In case of errors, appropriate HTTP status codes and error messages are returned.
    /// </returns>
    /// <example>
    /// POST /cats/123/photo
    /// Content-Disposition: form-data; name="file"; filename="cat_photo.jpg"
    /// </example>
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
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("/cats")]
    public async Task<ActionResult<Cat>> CreateCat([FromBody] CatRequest catPayload)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var catToBeInserted = new Cat
        {
            Name = catPayload.Name,
            BirthDate = catPayload.BirthDate,
            OwnerId = userId,
            BreederId = userId,
            Breed = catPayload.Breed,

        };
        var newCat = await _catService.CreateCat(catToBeInserted);
        return Json(newCat);
    }

    /// <summary>
    /// Deletes a cat by its unique identifier.
    /// </summary>
    /// <param name="catId"></param>
    /// <returns>No Content</returns>
    [HttpDelete("/cats/{catId}")]
    public async Task<IActionResult> DeleteCat(int catId)
    {
        await _catService.DeleteCatByIdAsync(catId);
        return NoContent();
    }
}

