using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Services
{
    public interface IUploadToAzureStorageService
    {
        Task<string> UploadFileAsync(byte[] fileData, string fileName, string storageConnectionString, string containerName);
    }
}
