using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.Entities
{
    public class Person
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

       // 1 = Customer  2 = Administrator  3 = SuperAdmin
        public PersonRole Role { get; set; }

        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; }


    }
}
