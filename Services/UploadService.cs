using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kissarekisteri.Services;

public class UploadService(BlobServiceClient blobServiceClient, IWebHostEnvironment env)
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;

    public async Task<BlobClient> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }
        var containerName = env.IsDevelopment() ? "dev-images" : "images";
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var publicAccessType = PublicAccessType.Blob;
        await container.CreateIfNotExistsAsync(publicAccessType: publicAccessType);

        var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blobClient = container.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/jpeg"
                }
            });
        }

        return blobClient;
    }
}
