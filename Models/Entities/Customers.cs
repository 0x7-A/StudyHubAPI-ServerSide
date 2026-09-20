namespace StudyHubAPI.Models.Entities
{
    public class Customers : Person
    {
       
        public DateOnly RegisteredAt { get; set; }
        public virtual ICollection<Reservations> Reservation { get; set; }
            = new List<Reservations>();
        public virtual ICollection<Payments> Payments { get; set; }
            = new HashSet<Payments>();

    }
}
