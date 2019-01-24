using FutureNow.Constants;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Services
{
    public class UploadToAzureStorageService : IUploadToAzureStorageService
    {
        public async Task<string> UploadFileAsync(byte[] fileData, string fileName, string storageConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            var blobContainer = blobClient.GetContainerReference(containerName);

            var fileBlob = blobContainer.GetBlockBlobReference(fileName);

            bool isExist = await fileBlob.ExistsAsync();

            if (isExist)
            {
                string newFileName = "";

                if (fileName.Contains("."))
                {
                    newFileName = fileName.Substring(0, fileName.LastIndexOf(".")) + "." +
                        DateTime.Now.ToString(DateTimeFormats.dateWithTimeIncludingSecondForFileNameFormat) + "." +
                        fileName.Substring(fileName.LastIndexOf(".") + 1);
                }
                else
                {
                    newFileName = $"{ fileName }-{ DateTime.Now.ToString(DateTimeFormats.dateWithTimeIncludingSecondForFileNameFormat) }";
                }

                fileBlob = blobContainer.GetBlockBlobReference(newFileName);
            }

            fileBlob.Properties.ContentType = GetFileContentType(fileName);

            await fileBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);

            return fileBlob.Uri.ToString();
        }

        private string GetFileContentType(string fileName)
        {
            string fileType = fileName.Substring(fileName.LastIndexOf(".") + 1).ToUpper();

            foreach (FileType type in Enum.GetValues(typeof(FileType)))
            {
                if (fileType == type.ToString())
                {
                    return type.ToContentType();
                }
            }

            return "application/octet-stream";
        }
    }
}
