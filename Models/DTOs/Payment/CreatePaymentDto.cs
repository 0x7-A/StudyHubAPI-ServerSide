using StudyHubAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
        public int AdminID { get; set; }
        public int ReservationID { get; set; }

        [EnumDataType(typeof(PaymentReason))]
        public PaymentReason PaymentReason { get; set; }
    }
}
