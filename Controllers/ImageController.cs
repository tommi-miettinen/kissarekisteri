using Kissarekisteribackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers
{
    public class ImageController : Controller
    {
        private readonly UploadService _uploadService;
        public ImageController(UploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("/images")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var uploadedFile = await _uploadService.UploadFile(file);
            return Json(uploadedFile);
        }
    }
}