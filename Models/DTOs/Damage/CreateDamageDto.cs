namespace StudyHubAPI.Models.DTOs.Damage
{
    public class CreateDamageDto
    {
        public int PaymentId { get; set; }
        public string? Notes { get; set; }
        public IFormFile? File { get; set; } 

    }
}
