using System.Diagnostics.Metrics;

namespace StudyHubAPI.Models.Entities
{
    public class Administrators : Person
    {
        public DateOnly HireDate { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public int CountryID { get; set; }

        public Countries Country { get; set; } = null!;

        public ICollection<Reservations> Reservation { get; set; }
            = new List<Reservations>();
        public ICollection<Payments> Payments { get; set; }
           = new HashSet<Payments>();
    }
}
