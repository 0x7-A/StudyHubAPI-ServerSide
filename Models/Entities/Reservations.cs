using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.Entities
{
    public class Reservations
    {
        public int ReservationID { get; set; }
        public int AdminID { get; set; }

        public int CustomerID { get; set; }

        public int WorkspaceID { get; set; }
        public DateTime ReservationDate { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        // 1=Confirmed, 2=Pending, 3=Completed, 4=Cancelled, 5=NoShow
        public ReservationStatus ReservationStatus { get; set; }
        public virtual Administrators Admin { get; set; } = null!;
        public virtual Customers Customer { get; set; } = null!;
        public virtual Workspaces Workspace { get; set; } = null!;

        public virtual ICollection<Reviews> Reviews { get; set; } = new HashSet<Reviews>();
        public virtual ICollection<Payments> Payments { get; set; } = new HashSet<Payments>();

    }
}
