namespace StudyHubAPI.Models.DTOs.Damage
{
    public class DamageSummaryDto
    {
        public int DamageId { get; set; }
        public int PaymentId { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }

    }
}
