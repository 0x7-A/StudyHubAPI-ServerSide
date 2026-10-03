using StudyHubAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Payment
{
    public class UpdatePaymentDto
    {
        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
