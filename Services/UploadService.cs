using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Services;

public class UploadService(BlobServiceClient blobServiceClient)
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;

    public async Task<BlobClient> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
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
        return blobClient;
    }
}
