namespace StudyHubAPI.Models.DTOs.Auth
{
    public class LogoutRequest
    {
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
