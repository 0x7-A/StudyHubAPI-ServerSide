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

        public async Task<ServiceResult<int>> UploadWorkspaceImage(int workspaceId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Please select an image file.");


            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();


            if (await ImageValidator.IsValidImageAsync(file))
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Invalid image file.");
            }

            var fileName = $"{Guid.NewGuid()}{ext}";

            var folderPath = _configuration["FileStorage:WorkspaceImagesPath"]?? "C:\\WorkspaceImages";

            Directory.CreateDirectory(folderPath);
            var fullFilePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var result = await _workspaceImagesRepository.UploadWorkspaceImage(new WorkspaceImages { ImagePath = fullFilePath, WorkspaceID = workspaceId });
            return ServiceResult<int>.Success(result, ResultType.Created);
        }

        public async Task<List<WorkspaceImageResponseDto>?> GetWorkspaceImagesAsync(int workspaceId, string baseUrl)
        {
            var images = await _workspaceImagesRepository.GetImagesByWorkspaceIdAsync(workspaceId);

            return images.Select(img => new WorkspaceImageResponseDto
            {
                ImageId = img.ImageID,
                WorkspaceId = img.WorkspaceID,

                ImageUrl = $"{baseUrl}/api/WorkspaceImages/{img.ImageID}/file"
            }).ToList();
        }


        public async Task<ServiceResult<ImageFileStreamDto?>> GetImageFileAsync(int imageId)
        {
            var image = await _workspaceImagesRepository.GetImageByIdAsync(imageId);
            if (image == null)
                return ServiceResult<ImageFileStreamDto?>.Failure(ResultType.NotFound, "Image not found.");


            if (!File.Exists(image.ImagePath))
                return ServiceResult<ImageFileStreamDto?>.Failure(ResultType.NotFound, "Image file not found.")     ;

            var ext = Path.GetExtension(image.ImagePath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return ServiceResult<ImageFileStreamDto?>.Success(new ImageFileStreamDto
            { FilePath = image.ImagePath, ContentType = contentType}, ResultType.Ok);
        }
        public async Task<ServiceResult> DeleteWorkspaceImage(int imageId)
        {
            var image = await _workspaceImagesRepository.GetWorkspaceImageById(imageId);
            if (image == null)
            {
                return ServiceResult.Failure(ResultType.NotFound, "Image not found.");
         
            }

            var folderPath = _configuration["FileStorage:WorkspaceImagesPath"]
                 ?? @"C:\WorkspaceImages";

            var fullPath = Path.Combine(folderPath, image.ImagePath);

            if (File.Exists(image.ImagePath))
            {
                File.Delete(image.ImagePath);
            }

            if(await _workspaceImagesRepository.DeleteWorkspaceImage(image) > 0)
            {
                return ServiceResult.Success(ResultType.NoContent);
            }

            return ServiceResult.Failure(ResultType.Failure, "Failed to delete workspace image.");
        }



    }
}
