using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.WorkspaceImages
{
    public class UploadWorkspaceImageDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
