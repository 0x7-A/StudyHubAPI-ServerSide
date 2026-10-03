using StudyHubAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerID { get; set; }

        [Range(1, int.MaxValue)]
        public int AdminID { get; set; }

        [Range(1, int.MaxValue)]
        public int ReservationID { get; set; }

        [EnumDataType(typeof(PaymentReason))]
        public PaymentReason PaymentReason { get; set; }
    }
}
