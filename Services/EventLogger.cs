using YouTubeCompanion.Data;
using YouTubeCompanion.Models;
using System.Text.Json;


namespace YouTubeCompanion.Services
{
    public class EventLogger : IEventLogger
    {
        private readonly AppDbContext _db;
        public EventLogger(AppDbContext db) { _db = db; }
        public async Task LogAsync(Guid? userId, string eventType, object details)
        {
            var log = new EventLog { UserId = userId, EventType = eventType, DetailsJson = JsonSerializer.Serialize(details) };
            _db.EventLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}