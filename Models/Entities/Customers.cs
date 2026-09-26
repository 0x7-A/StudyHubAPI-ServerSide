namespace StudyHubAPI.Models.Entities
{
    public class Customers : Person
    {
       
        public DateOnly RegisteredAt { get; set; }
        public ICollection<Reservations> Reservation { get; set; }
            = new List<Reservations>();
        public ICollection<Payments> Payments { get; set; }
            = new HashSet<Payments>();

    }
}
