using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Person
{
    public class PersonAuthDto
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; } = null!;
  
        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public PersonRole Role { get; set; }

        public string PasswordHash { get; set; } = null!;

    }
}
