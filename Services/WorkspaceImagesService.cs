using StudyHubAPI.Repositories;
using StudyHubAPI.Models.Entities;

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


        public async Task<bool> DeleteWorkspaceImage(int imageId)
        {
            var image = await _workspaceImagesRepository.GetWorkspaceImageById(imageId);
            if (image == null)
            {
                return false;
            }
            if (File.Exists(image.ImagePath))
            {
                File.Delete(image.ImagePath);
            }
            
            return await _workspaceImagesRepository.DeleteWorkspaceImage(image) > 0;
        }



    }
}
