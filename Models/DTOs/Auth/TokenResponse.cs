namespace StudyHubAPI.Models.DTOs.Auth
{
    public class TokenResponse
    {
       public string AccessToken { get; set; } = string.Empty;
       public string RefreshToken { get; set; } = string.Empty;
    }
}
