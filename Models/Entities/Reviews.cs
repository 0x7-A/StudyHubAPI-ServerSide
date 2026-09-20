namespace StudyHubAPI.Models.Entities
{
    public class Reviews
    {
        public int ReviewID { get; set; }

        public byte Rate { get; set; }

        public string? Comment {  get; set; }

        public int ReservationID { get; set; }

        public virtual Reservations Reservation { get; set; } = null!;

    }
}
