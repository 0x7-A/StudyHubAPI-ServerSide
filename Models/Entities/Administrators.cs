using System.Diagnostics.Metrics;

namespace StudyHubAPI.Models.Entities
{
    public class Administrators : Person
    {
        public DateOnly HireDate { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public int CountryID { get; set; }

        public virtual Countries Country { get; set; } = null!;

        public virtual ICollection<Reservations> Reservation { get; set; }
            = new List<Reservations>();
        public virtual ICollection<Payments> Payments { get; set; }
           = new HashSet<Payments>();
    }
}
