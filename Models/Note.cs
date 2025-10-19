namespace YouTubeCompanion.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string VideoId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string[]? Tags { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}