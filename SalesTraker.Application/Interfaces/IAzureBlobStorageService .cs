using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.Interfaces
{
    public interface IAzureBlobStorageService 
    {
        Task<string> UploadAsync(Stream fileStream, string contentType, string blobPath);
        Task DeleteAsync(string blobPath);
        string ExtractBlobPath(string url);
    }

}
