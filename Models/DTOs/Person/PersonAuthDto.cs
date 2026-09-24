using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Person
{
    public class PersonAuthDto
    {
        public int PersonID { get; set; }
        public string Email { get; set; } = null!;

        public PersonRole Role { get; set; }

        public string PasswordHash { get; set; } = null!;

    }
}
