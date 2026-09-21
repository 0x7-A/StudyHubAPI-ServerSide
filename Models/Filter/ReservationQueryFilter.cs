namespace StudyHubAPI.Models.Filter
{
    public class ReservationQueryFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public byte? ReservationStatus { get; set; }
        public DateTime? StartTime { get; set; }

    }
}
