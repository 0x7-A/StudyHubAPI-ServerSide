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
            if (file is null || file.Length == 0 || file.Length > 5 * 1024 * 1024)
                return false;

            await using var stream = file.OpenReadStream();

            return Inspector.DetermineFileFormat(stream) is  FileSignatures.Formats.Image;
        }
    }
}
