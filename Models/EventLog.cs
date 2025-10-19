using System.Text.Json;


namespace YouTubeCompanion.Models
{
    public class EventLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string DetailsJson { get; set; } = JsonSerializer.Serialize(new { });
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}