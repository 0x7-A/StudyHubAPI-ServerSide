using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Offer
{
    public class CreateOfferDto
    {
        [Required]
        [StringLength(30)]
        public string OfferName { get; set; } = null!;

        [Range(typeof(decimal), "0.01", "1.00")]
        public decimal OfferPercentage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, short.MaxValue)]
        public short? MaximumDiscountAmount { get; set; }

        [Range(1, short.MaxValue)]
        public short? MinimumDiscountAmount { get; set; }

    }
}
