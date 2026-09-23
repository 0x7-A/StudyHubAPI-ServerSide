using static System.Net.Mime.MediaTypeNames;
using FileSignatures;
using FileSignatures.Formats;

namespace StudyHubAPI.Utils
{
    public class ImageValidator
    {
        private static readonly IFileFormatInspector Inspector = new FileFormatInspector();

        public static async Task<bool> IsValidImageAsync(IFormFile file)
        {
            // 1. Guard against empty files and oversized uploads (5MB)
            if (file is null || file.Length == 0 || file.Length > 5 * 1024 * 1024)
                return false;

            await using var stream = file.OpenReadStream();

            // 2. Fast check: verify magic bytes via FileSignatures
            var format = Inspector.DetermineFileFormat(stream);

            // Explicitly check if the format inherits from FileSignatures.Formats.Image
            if (format is not FileSignatures.Formats.Image)
                return false;

            // 3. Deep check: read headers/dimensions with ImageSharp
            stream.Position = 0; // Reset stream after reading signature
            try
            {
                var info = await SixLabors.ImageSharp.Image.IdentifyAsync(stream);
                return info is not null
                    && info.Width > 0 && info.Width <= 6000
                    && info.Height > 0 && info.Height <= 6000;
            }
            catch
            {
                return false; // Corrupt image or malformed payload
            }
        }
    }
}
