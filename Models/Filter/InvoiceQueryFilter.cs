using StudyHubAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.Filter
{
    public class InvoiceQueryFilter
    {
        [Range(1, int.MaxValue)]
        public int pageNumber { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int pageSize { get; set; } = 10;
        [Range(1, int.MaxValue)]
        public int? PaymentID { get; set; }

        [Range(1, int.MaxValue)]
        public int? GeneralOfferID { get; set; }

    }
}
