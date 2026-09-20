using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int PersonID { get; set; }
        public string FullName { get; set; } = null!;
        public PersonRole Role { get; set; }

        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
