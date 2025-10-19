using Microsoft.AspNetCore.Mvc;
using YouTubeCompanion.Repositories;
using YouTubeCompanion.Models;
using YouTubeCompanion.Services;

namespace YouTubeCompanion.Controllers
{
    [ApiController]
    [Route("api/notes")]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _notes;
        private readonly IUserRepository _users;
        private readonly IEventLogger _events;

        public NotesController(INoteRepository notes, IUserRepository users, IEventLogger events)
        {
            _notes = notes; _users = users; _events = events;
        }

        private async Task<User?> GetUserFromHeaderAsync()
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var s)) return null;
            if (!Guid.TryParse(s.First(), out var id)) return null;
            return await _users.GetByIdAsync(id);
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? videoId, [FromQuery] string? q, [FromQuery] string? tags)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null) return Unauthorized();
            string[]? tagArr = null;
            if (!string.IsNullOrEmpty(tags)) tagArr = tags.Split(',').Select(t => t.Trim()).ToArray();
            var res = await _notes.SearchAsync(user.Id, videoId, q, tagArr);
            await _events.LogAsync(user.Id, "SearchNotes", new { videoId, q, tags });
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoteDto dto)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null) return Unauthorized();
            var note = new Note { UserId = user.Id, VideoId = dto.VideoId, Title = dto.Title, Body = dto.Body, Tags = dto.Tags };
            var created = await _notes.CreateAsync(note);
            await _events.LogAsync(user.Id, "CreateNote", new { created.Id });
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteDto dto)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null) return Unauthorized();
            var note = await _notes.GetByIdAsync(id);
            if (note == null || note.UserId != user.Id) return NotFound();
            note.Title = dto.Title; note.Body = dto.Body; note.Tags = dto.Tags;
            await _notes.UpdateAsync(note);
            await _events.LogAsync(user.Id, "UpdateNote", new { id });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null) return Unauthorized();
            var note = await _notes.GetByIdAsync(id);
            if (note == null || note.UserId != user.Id) return NotFound();
            await _notes.DeleteAsync(id);
            await _events.LogAsync(user.Id, "DeleteNote", new { id });
            return NoContent();
        }
    }
}