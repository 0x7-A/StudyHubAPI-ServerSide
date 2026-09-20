using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Payment
{
    public class UpdatePaymentDto
    {
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } = null!;
    }
}
