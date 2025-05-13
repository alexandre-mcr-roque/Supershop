namespace Supershop.Helpers
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName, Guid oldImageId);
        Task<Guid> UploadBlobAsync(byte[] file, string containerName);
        Task<Guid> UploadBlobAsync(byte[] file, string containerName, Guid oldImageId);
        Task<Guid> UploadBlobAsync(string imageUrl, string containerName);
        Task<Guid> UploadBlobAsync(string imageUrl, string containerName, Guid oldImageId);
    }
}
