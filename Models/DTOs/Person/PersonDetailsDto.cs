using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Person
{
    public abstract class PersonDetailsDto
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public virtual PersonRole Role { get; set; }
    }
}
