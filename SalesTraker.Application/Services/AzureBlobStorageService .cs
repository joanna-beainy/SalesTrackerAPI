using Microsoft.Extensions.Options;
using SalesTracker.Application.Interfaces;
using SalesTracker.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SalesTracker.Application.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly AzureBlobOptions _options;
        private readonly BlobContainerClient _container;

        public AzureBlobStorageService(IOptions<AzureBlobOptions> options)
        {
            _options = options.Value;
            _container = new BlobContainerClient(_options.ConnectionString, _options.Container);
        }

        public async Task<string> UploadAsync(Stream fileStream, string contentType, string blobPath)
        {
            var blobClient = _container.GetBlobClient(blobPath);

            var headers = new BlobHttpHeaders
            {
                ContentType = contentType,
                CacheControl = "public,max-age=31536000"
            };

            await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = headers });
            return blobClient.Uri.ToString();
        }

        public async Task DeleteAsync(string blobPath)
        {
            var blobClient = _container.GetBlobClient(blobPath);
            await blobClient.DeleteIfExistsAsync();
        }

        public string ExtractBlobPath(string url)
        {
            var uri = new Uri(url);
            return uri.AbsolutePath.TrimStart('/').Split('/', 2)[1]; 
        }

    }

}
