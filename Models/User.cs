using System.ComponentModel.DataAnnotations;


namespace YouTubeCompanion.Models
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string GoogleUserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public UserToken? Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}