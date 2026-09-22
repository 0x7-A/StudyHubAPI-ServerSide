using Microsoft.EntityFrameworkCore;
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

        public async Task<WorkspaceImages?> GetWorkspaceImageById(int imageId)
        {
            return await _context.WorkspaceImages.FindAsync(imageId);
        }

        public async Task<int> DeleteWorkspaceImage(WorkspaceImages workspaceImage)
        {
            return await _context.WorkspaceImages.Where(wi => wi.ImageID == workspaceImage.ImageID).ExecuteDeleteAsync();
        }

    }
}
