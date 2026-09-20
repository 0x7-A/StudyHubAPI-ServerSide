using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Reservation
{
    public class ReservationSummaryDto
    {
        public int ReservationID { get; set; }
        public string CustomerName { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
    }
}
