using StudyHubAPI.Models.DTOs.Person;
using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Admin
{
    public class CreateAdminDto : CreatePersonDto
    {

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Range(1, int.MaxValue)]
        public int CountryID { get; set; }

    }
}
