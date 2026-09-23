using StudyHubAPI.Models.DTOs.WorkspaceImages;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class WorkspaceImagesService
    {
        private readonly WorkspaceImagesRepository _workspaceImagesRepository;
        private readonly IConfiguration _configuration; 

        public WorkspaceImagesService(WorkspaceImagesRepository repository,IConfiguration configuration)
        {
            _workspaceImagesRepository = repository;
            _configuration = configuration;
        }


        public async Task<int> UploadWorkspaceImage(int workspaceId, IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();


            if (await ImageValidator.IsValidImageAsync(file))
            {
                throw new InvalidOperationException("Invalid image file.");
            }

    

            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var fileName = $"{Guid.NewGuid()}{ext}";

            var folderPath = _configuration["FileStorage:WorkspaceImagesPath"]?? "C:\\WorkspaceImages";

            Directory.CreateDirectory(folderPath);
            var fullFilePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return await _workspaceImagesRepository.UploadWorkspaceImage(new WorkspaceImages { ImagePath = fullFilePath, WorkspaceID = workspaceId});
        }

        public async Task<List<WorkspaceImageResponseDto>?> GetWorkspaceImagesAsync(int workspaceId, string baseUrl)
        {
            
            var exists = await _workspaceImagesRepository.WorkspaceExistsAsync(workspaceId);
            if (!exists)
                return null; 

            var images = await _workspaceImagesRepository.GetImagesByWorkspaceIdAsync(workspaceId);

            return images.Select(img => new WorkspaceImageResponseDto
            {
                ImageId = img.ImageID,
                WorkspaceId = img.WorkspaceID,

                ImageUrl = $"{baseUrl}/api/WorkspaceImages/{img.ImageID}/file"
            }).ToList();
        }


        public async Task<ImageFileStreamDto?> GetImageFileAsync(int imageId)
        {
            var image = await _workspaceImagesRepository.GetImageByIdAsync(imageId);
            if (image == null)
                return null;

            if (!File.Exists(image.ImagePath))
                return null;

            var ext = Path.GetExtension(image.ImagePath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return new ImageFileStreamDto
            {
                FilePath = image.ImagePath,
                ContentType = contentType
            };
        }
        public async Task<bool> DeleteWorkspaceImage(int imageId)
        {
            var image = await _workspaceImagesRepository.GetWorkspaceImageById(imageId);
            if (image == null)
            {
                return false;
            }

            var folderPath = _configuration["FileStorage:WorkspaceImagesPath"]
                 ?? @"C:\WorkspaceImages";

            var fullPath = Path.Combine(folderPath, image.ImagePath);

            if (File.Exists(image.ImagePath))
            {
                File.Delete(image.ImagePath);
            }
            
            return await _workspaceImagesRepository.DeleteWorkspaceImage(image) > 0;
        }



    }
}
