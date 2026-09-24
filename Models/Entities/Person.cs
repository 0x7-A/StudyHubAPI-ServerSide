using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.Entities
{
    public class Person
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public bool IsActive { get; set; }
        public string PhoneNumber { get; set; } = null!;

        public LoginInfo? LoginInfo { get; set; }
    }
}
