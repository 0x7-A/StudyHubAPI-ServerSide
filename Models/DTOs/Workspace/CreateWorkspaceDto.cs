using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Workspace
{
    public class CreateWorkspaceDto
    {
        [Required]
        [StringLength(100)]
        public string Description { get; set; } = null!;

        [Range(1, 2)]
        public byte WorkspaceStatus { get; set; }

        [Range(typeof(decimal), "0.01", "999999")]
        public decimal HourlyRate { get; set; }

        [Range(1, int.MaxValue)]
        public int MaximumCapacity { get; set; }
    }
}
