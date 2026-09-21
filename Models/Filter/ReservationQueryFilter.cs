using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.Filter
{
    public class ReservationQueryFilter
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;

        public ReservationStatus? ReservationStatus { get; set; }
        public DateTime? StartTime { get; set; }

    }
}
