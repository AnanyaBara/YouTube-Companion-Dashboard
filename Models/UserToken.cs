namespace YouTubeCompanion.Models
{
    public class UserToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty; // encrypt in prod
        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}