namespace StudyHubAPI.Models.DTOs.Review
{
    public class ReviewDetailsDto
    {
        public int ReviewID { get; set; }

        public byte Rate { get; set; }

        public string? Comment { get; set; }

        public int ReservationID { get; set; }
    }
}
