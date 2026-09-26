namespace StudyHubAPI.Models.Entities
{
    public class WorkspaceImages
    {
        public int ImageID { get; set; }
        public int WorkspaceID { get; set; }
        public string ImagePath { get; set; } = null!;
        public Workspaces Workspace { get; set; } = null!;
    }
}
