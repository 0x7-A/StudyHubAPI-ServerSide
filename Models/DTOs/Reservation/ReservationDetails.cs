using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Reservation
{
    public class ReservationDetails
    {
        public int ReservationID { get; set; }
  
        public int CustomerID { get; set; }

        public string? CustomerName { get; set; }

        public int WorkspaceID { get; set; }

        public DateTime ReservationDate { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? TotalAmount { get; set; }

        // 1=Confirmed, 2=Pending, 3=Completed, 4=Cancelled, 5=NoShow
        public ReservationStatus ReservationStatus { get; set; }
    }
}
