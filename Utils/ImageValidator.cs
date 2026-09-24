using FileSignatures;

namespace StudyHubAPI.Utils
{
    public class ImageValidator
    {
        private static readonly IFileFormatInspector _Inspector = new FileFormatInspector();

        public static async Task<bool> IsValidImageAsync(IFormFile file)
        {
            if (file is null || file.Length == 0 || file.Length > 5 * 1024 * 1024)
                return false;

            await using var stream = file.OpenReadStream();

            return _Inspector.DetermineFileFormat(stream) is  FileSignatures.Formats.Image;
        }
    }
}
