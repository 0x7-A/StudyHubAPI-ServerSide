namespace StudyHubAPI.Models.DTOs.Workspace
{
    public class WorkspaceSummaryDto
    {
        public int WorkspaceID { get; set; }

        public string Description { get; set; } = null!;

        public byte WorkspaceStatus { get; set; }

        public decimal HourlyRate { get; set; }

        public int MaximumCapacity { get; set; }
    }
}
