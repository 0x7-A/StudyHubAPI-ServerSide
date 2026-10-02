using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Damage
{
    public class CreateDamageDto
    {
        [Range(1, int.MaxValue)]
        public int PaymentId { get; set; }
        public string? Notes { get; set; }
        public IFormFile? File { get; set; } 

    }
}
