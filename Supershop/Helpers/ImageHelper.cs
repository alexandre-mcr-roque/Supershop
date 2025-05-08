
namespace Supershop.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder, string? oldImageUrl = null)
        {
            // replace old image if it exists
            var file = string.IsNullOrEmpty(oldImageUrl)
                ? $"{Guid.NewGuid()}.jpg"
                : oldImageUrl.Split('/').Last();

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                @$"wwwroot\images\{folder}",
                file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/images/{folder}/{file}";
        }
    }
}
