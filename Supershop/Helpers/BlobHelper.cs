using Azure.Storage.Blobs;

namespace Supershop.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly BlobServiceClient _blobService;

        public BlobHelper(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"]!;
            _blobService = new BlobServiceClient(keys);
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

        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName, Guid oldImageId)
        {
            Guid name = oldImageId != Guid.Empty ? oldImageId : Guid.NewGuid();
            var containerClient = _blobService.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name.ToString());
            await blobClient.UploadAsync(stream, overwrite: true);
            // Upload complete, dispose stream
            await stream.DisposeAsync();
            return name;
        }
    }
}
