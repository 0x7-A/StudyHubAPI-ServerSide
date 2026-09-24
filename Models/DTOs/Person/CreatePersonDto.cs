using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Person
{
    public abstract class CreatePersonDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string PhoneNumber { get; set; } = null!;

    }
}
