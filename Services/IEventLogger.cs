namespace YouTubeCompanion.Services
{
    public interface IEventLogger
    {
        Task LogAsync(Guid? userId, string eventType, object details);
    }
}