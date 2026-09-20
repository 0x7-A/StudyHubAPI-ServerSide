using StudyHubAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Reservation
{
    public class UpdateReservationDto
    {
        [Range(1, 6)]
        public ReservationStatus? ReservationStatus { get; set; }
    }
}
