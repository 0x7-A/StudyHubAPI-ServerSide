using StudyHubAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Reservation
{
    public class CreateReservationDto
    {
        [Range(1, int.MaxValue)]
        public int AdminID { get; set; }

        [Range(1, int.MaxValue)]
        public int CustomerID { get; set; }

        [Range(1, int.MaxValue)]
        public int WorkspaceID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [EnumDataType(typeof(ReservationStatus))]
        public ReservationStatus ReservationStatus { get; set; }

    }
}
