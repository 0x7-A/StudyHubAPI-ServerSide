namespace StudyHubAPI.Models.Entities
{
    public class Damage
    {
        public int DamageId { get; set; }
        public int PaymentId { get; set; }
        public string? Notes { get; set; }
        public string? ImagePath { get; set; }
        public Payments Payment { get; set; } = null!;
    }
}
