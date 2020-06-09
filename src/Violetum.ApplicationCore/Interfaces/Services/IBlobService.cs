using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IBlobService
    {
        public Task<BlobInfo> GetBlob(string name);
        public Task<IEnumerable<string>> ListBlobs();
        public Task UploadFileBlob(string filePath, string fileName);
        public Task UploadContentBlob(string content, string fileName);
        public Task UploadImageBlob(string content, string fileName);
        public Task DeleteBlob(string blobName);
    }
}