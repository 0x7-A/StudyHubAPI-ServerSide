using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.Auth
{
    public class RefreshRequest
    {

        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
