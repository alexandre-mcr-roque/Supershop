using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Supershop.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly CloudBlobClient _blobClient;

        public BlobHelper(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"]!;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
            => await UploadBlobAsync(file, containerName, Guid.Empty);
        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName, Guid oldImageId)
        {
            Stream stream = file.OpenReadStream();
            return await UploadStreamAsync(stream, containerName, oldImageId);
        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
            => await UploadBlobAsync(file, containerName, Guid.Empty);
        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName, Guid oldImageId)
        {
            Stream stream = new MemoryStream(file);
            return await UploadStreamAsync(stream, containerName, oldImageId);
        }

        public async Task<Guid> UploadBlobAsync(string imageUrl, string containerName)
            => await UploadBlobAsync(imageUrl, containerName, Guid.Empty);
        public async Task<Guid> UploadBlobAsync(string imageUrl, string containerName, Guid oldImageId)
        {
            Stream stream = File.OpenRead(imageUrl);
            return await UploadStreamAsync(stream, containerName, oldImageId);
        }

        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName, Guid oldImageUd)
        {
            Guid name = oldImageUd != Guid.Empty ? oldImageUd : Guid.NewGuid();
            var container = _blobClient.GetContainerReference(containerName);
            var blockBlob = container.GetBlockBlobReference(name.ToString());
            await blockBlob.UploadFromStreamAsync(stream);
            // Upload complete, dispose stream
            await stream.DisposeAsync();
            return name;
        }
    }
}
