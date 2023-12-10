using Kissarekisteribackend.Models;
using Kissarekisteribackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers
{
    public class CatController(CatService catService) : Controller
    {
        private readonly CatService _catService = catService;

        [HttpGet("/cats")]
        public async Task<IActionResult> GetCats()
        {
            var cats = await _catService.GetCatsAsync();
            return Json(cats);
        }

        [HttpGet("/cats/{catId}")]
        public async Task<IActionResult> GetCatAsync(int catId)
        {
            var cat = await _catService.GetCatByIdAsync(catId);
            return Json(cat);
        }

        [HttpGet("/users/{userId}/cats")]
        public async Task<IActionResult> GetCatsByUserId(string userId)
        {
            var catsByUserId = await _catService.GetCatByUserIdAsync(userId);
            return Json(catsByUserId);
        }

        [HttpPost("cats/{catId}/photo")]
        public async Task<IActionResult> UploadCatPhoto(int catId, IFormFile file)
        {
            var cat = await _catService.UploadCatPhoto(catId, file);
            return Json(cat);
        }

        [HttpPut("cats/{catId}")]
        public async Task<IActionResult> EditCat(int catId, [FromBody] Cat catPayload)
        {
            var cat = await _catService.UpdateCatByIdAsync(catId, catPayload);
            return Json(cat);
        }

        [HttpPost("/cats")]
        public async Task<IActionResult> CreateCat([FromBody] Cat catPayload)
        {
            var newCat = await _catService.CreateCat(catPayload);
            return Json(newCat);
        }

        [HttpDelete("/cats/{catId}")]
        public async Task<IActionResult> DeleteCat(int catId)
        {
            await _catService.DeleteCatByIdAsync(catId);
            return NoContent();
        }
    }
}
