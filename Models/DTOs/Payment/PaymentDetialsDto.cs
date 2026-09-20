using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Payment
{
    public class PaymentDetialsDto
    {
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
        public int AdminID { get; set; }
        public int ReservationID { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentReason PaymentReason { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
    }
}
