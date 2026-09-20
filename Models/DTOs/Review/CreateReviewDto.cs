using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Review
{
    public class CreateReviewDto
    {
        [Range(1, 5)]
        public byte Rate { get; set; }

        [StringLength(200)]
        public string? Comment { get; set; }

        [Range(1, int.MaxValue)]
        public int ReservationID { get; set; }
    }
}
