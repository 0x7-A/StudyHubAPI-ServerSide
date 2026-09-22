namespace StudyHubAPI.Models.DTOs.WorkspaceImages
{
    public class WorkspaceImageResponseDto
    {
        public int ImageId { get; set; }
        public int WorkspaceId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
