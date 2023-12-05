using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Controllers
{
    public class ImageController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        public ImageController(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        [HttpPost("/images")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file received");
            }
            var containerName = "images";
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = container.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }

            var blobUrl = blobClient.Uri.AbsoluteUri;
            return Ok(new { Url = blobUrl });
        }
    }
}