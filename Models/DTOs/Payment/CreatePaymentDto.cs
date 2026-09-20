using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
        public int AdminID { get; set; }
        public int ReservationID { get; set; }
        // Mapped Enums
        public PaymentReason PaymentReason { get; set; }
    }
}
