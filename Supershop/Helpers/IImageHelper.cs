namespace Supershop.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder, string? oldImageUrl = null);
    }
}
