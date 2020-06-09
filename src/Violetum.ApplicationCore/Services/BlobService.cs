using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using BlobInfo = Violetum.Domain.Models.BlobInfo;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<BlobInfo> GetBlob(string name)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pictures");
            BlobClient blobClient = containerClient.GetBlobClient(name);
            Response<BlobDownloadInfo> blobDownloadInfo = await blobClient.DownloadAsync();
            //"categories/41587092.jpg"

            return new BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
        }

        public async Task<IEnumerable<string>> ListBlobs()
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pictures");

            var items = new List<string>();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                items.Add(blobItem.Name);
            }

            return items;
        }

        public async Task UploadFileBlob(string filePath, string fileName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pictures");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(filePath, new BlobHttpHeaders {ContentType = filePath.GetContentType()});
        }

        public async Task UploadContentBlob(string content, string fileName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pictures");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            await using var memoryStream = new MemoryStream(bytes);

            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders {ContentType = fileName.GetContentType()});
        }

        public async Task UploadImageBlob(string content, string fileName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pictures");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            byte[] bytes = Convert.FromBase64String(content);
            await using var memoryStream = new MemoryStream(bytes);

            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders {ContentType = fileName.GetContentType()});
        }

        public async Task DeleteBlob(string blobName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pictures");
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}