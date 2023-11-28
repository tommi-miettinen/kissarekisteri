using Kissarekisteribackend.Database;
using Kissarekisteribackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers
{
    public class CatController : Controller
    {
        private readonly KissarekisteriDbContext _dbContext;

        public CatController(KissarekisteriDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("/cats")]
        public async Task<IActionResult> GetCats()
        {
            var cats = await _dbContext.Cats.ToListAsync();

            return Json(cats);
        }

        [HttpGet("/cats/{catId}")]
        public async Task<IActionResult> GetCat(int catId)
        {
            var catById = await _dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);
            if (catById == null)
            {
                return NotFound();
            }

            return Json(catById);
        }

        [HttpGet("/users/{userId}/cats")]
        public async Task<IActionResult> GetCatsByUserId(string userId)
        {
            var catsByUserId = await _dbContext.Cats.Where(cat => cat.OwnerId == userId).ToListAsync();

            return Json(catsByUserId);
        }

        [HttpPut("cats/{catId}")]
        public async Task<IActionResult> EditCat(int catId, [FromBody] Cat catPayload)
        {
            var cat = await _dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);

            if (cat == null)
            {
                return NotFound();
            }

            cat.Name = catPayload.Name;
            cat.Breed = catPayload.Breed;
            cat.BirthDate = catPayload.BirthDate;

            await _dbContext.SaveChangesAsync();

            return Ok(cat);
        }

        [HttpPost("/cats")]
        public async Task<IActionResult> CreateCat([FromBody] Cat newCat)
        {
            if (newCat == null)
            {
                return BadRequest("Invalid cat data");
            }

            await _dbContext.Cats.AddAsync(newCat);
            await _dbContext.SaveChangesAsync();

            return Json(newCat);
        }

        [HttpDelete("/cats/{catId}")]
        public async Task<IActionResult> DeleteCat(int catId)
        {
            var cat = await _dbContext.Cats.FirstOrDefaultAsync(c => c.Id == catId);

            if (cat == null)
            {
                return NotFound();
            }

            _dbContext.Cats.Remove(cat);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
