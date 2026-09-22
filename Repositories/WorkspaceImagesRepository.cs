using StudyHubAPI.Data;
using StudyHubAPI.Models.Entities;

namespace StudyHubAPI.Repositories
{
    public class WorkspaceImagesRepository
    {
        private readonly StudyHubDbContext _context;
        public WorkspaceImagesRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<int> UploadWorkspaceImage(WorkspaceImages workspaceImage)
        {
            _context.WorkspaceImages.Add(workspaceImage);
            await _context.SaveChangesAsync();
            return workspaceImage.ImageID;
        }
     

    }
}
