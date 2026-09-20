using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Workspace
{
    public class UpdateWorkspaceDto
    {
        [StringLength(100)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0.01", "999999")]
        public decimal? HourlyRate { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaximumCapacity { get; set; }
    }
}
