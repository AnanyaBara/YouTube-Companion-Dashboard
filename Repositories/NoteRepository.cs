using Microsoft.EntityFrameworkCore;
using YouTubeCompanion.Data;
using YouTubeCompanion.Models;


namespace YouTubeCompanion.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _db;
        public NoteRepository(AppDbContext db) { _db = db; }
        public async Task<Note> CreateAsync(Note note)
        {
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();
            return note;
        }


        public async Task DeleteAsync(Guid id)
        {
            var n = await _db.Notes.FindAsync(id);
            if (n != null) { _db.Notes.Remove(n); await _db.SaveChangesAsync(); }
        }


        public async Task<Note?> GetByIdAsync(Guid id) => await _db.Notes.FindAsync(id);


        public async Task<IEnumerable<Note>> SearchAsync(Guid userId, string? videoId = null, string? q = null, string[]? tags = null)
        {
            var query = _db.Notes.AsQueryable().Where(n => n.UserId == userId);
            if (!string.IsNullOrEmpty(videoId)) query = query.Where(n => n.VideoId == videoId);
            if (!string.IsNullOrEmpty(q)) query = query.Where(n => EF.Functions.ILike(n.Title ?? string.Empty, $"%{q}%") || EF.Functions.ILike(n.Body ?? string.Empty, $"%{q}%"));
            if (tags != null && tags.Length > 0) query = query.Where(n => n.Tags != null && tags.All(t => n.Tags.Contains(t)));
            return await query.OrderByDescending(n => n.UpdatedAt).ToListAsync();
        }


        public async Task UpdateAsync(Note note)
        {
            note.UpdatedAt = DateTime.UtcNow;
            _db.Notes.Update(note);
            await _db.SaveChangesAsync();
        }
    }
}