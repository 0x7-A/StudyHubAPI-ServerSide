namespace StudyHubAPI.Models.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

        public int PersonID { get; set; }
        public LoginInfo loginfo { get; set; } = null!;
    }
}
