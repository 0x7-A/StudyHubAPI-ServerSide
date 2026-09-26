using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.Entities
{
    public class Payments
    {
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
        public int AdminID { get; set; }
        public int ReservationID { get; set; }
        public decimal TotalPrice { get; set; }

        // Mapped Enums
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentReason PaymentReason { get; set; }

        public string PaymentMethod { get; set; } = null!;
        public DateTime PaymentDate { get; set; }

        // Navigation Properties
        public virtual Customers Customer { get; set; } = null!;
        public virtual Administrators Administrator { get; set; } = null!;
        public virtual Reservations Reservation { get; set; } = null!;

        public Damage? Damages { get; set; } 
    }

}

