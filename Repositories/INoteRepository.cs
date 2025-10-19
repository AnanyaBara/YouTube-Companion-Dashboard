using YouTubeCompanion.Models;


namespace YouTubeCompanion.Repositories
{
    public interface INoteRepository
    {
        Task<Note> CreateAsync(Note note);
        Task<IEnumerable<Note>> SearchAsync(Guid userId, string? videoId = null, string? q = null, string[]? tags = null);
        Task<Note?> GetByIdAsync(Guid id);
        Task UpdateAsync(Note note);
        Task DeleteAsync(Guid id);
    }
}