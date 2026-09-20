using StudyHubAPI.Models.DTOs.Person;


namespace StudyHubAPI.Models.DTOs.Admin
{
    public class AdminDetailsDto : PersonDetailsDto
    {
        public DateOnly HireDate { get; set; }
    }
}
