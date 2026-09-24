using System.ComponentModel.DataAnnotations;

namespace StudyHubAPI.Models.DTOs.LoginInfo
{
    public class CreateLoginInfoDto
    {
        [Range(1, int.MaxValue)]
        public int PersonID { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 32 characters.")]
        public string Password{ get; set; } = null!;

    }
}
