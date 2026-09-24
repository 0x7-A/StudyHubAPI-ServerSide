using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.Entities
{
    public class LoginInfo
    {
        public int LoginID { get; set; }
        public int PersonID { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // 1 = Customer  2 = Administrator  3 = SuperAdmin
        public PersonRole Role { get; set; }

        public string PasswordHash { get; set; } = null!;



        public Person Person { get; set; } = null!;
    }
}
