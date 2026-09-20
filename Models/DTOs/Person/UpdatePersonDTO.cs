using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Person
{
    public abstract class UpdatePersonDTO
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }


        [StringLength(10, MinimumLength = 10)]
        public string? PhoneNumber { get; set; }
    }
}
