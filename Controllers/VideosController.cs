using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTubeCompanion.Data;
using YouTubeCompanion.Models;
using YouTubeCompanion.Repositories;
using YouTubeCompanion.Services;

namespace YouTubeCompanion.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly IYouTubeService _yt;
        private readonly IEventLogger _events;
        private readonly AppDbContext _db;

        public VideosController(IUserRepository users, IYouTubeService yt, IEventLogger events, AppDbContext db)
        {
            _users = users; _yt = yt; _events = events; _db = db;
        }

        // The client must send userId as header X-User-Id for this scaffold. Replace with auth in prod.
        private async Task<User?> GetUserFromHeaderAsync()
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var s)) return null;
            if (!Guid.TryParse(s.First(), out var id)) return null;
            return await _users.GetByIdAsync(id);
        }

        [HttpGet]
        public async Task<IActionResult> ListUploads()
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null || user.Token == null) return Unauthorized();
            var res = await _yt.ListMyUploadsAsync(user.Token.RefreshToken);
            await _events.LogAsync(user.Id, "ListUploads", new { });
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null || user.Token == null) return Unauthorized();
            var vid = await _yt.GetVideoAsync(id, user.Token.RefreshToken);
            await _events.LogAsync(user.Id, "GetVideo", new { videoId = id });
            return Ok(vid);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateVideoDto dto)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null || user.Token == null) return Unauthorized();
            await _yt.UpdateVideoMetadataAsync(id, dto.Title, dto.Description, user.Token.RefreshToken);
            await _events.LogAsync(user.Id, "UpdateVideo", new { videoId = id, dto.Title });
            return NoContent();
        }

        [HttpPost("{id}/comment")]
        public async Task<IActionResult> Comment(string id, [FromBody] CreateCommentDto dto)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null || user.Token == null) return Unauthorized();
            var thread = await _yt.PostCommentAsync(id, dto.Text, user.Token.RefreshToken);
            await _events.LogAsync(user.Id, "PostComment", new { videoId = id });
            return Ok(thread);
        }

        [HttpPost("{id}/comment/{parent}/reply")]
        public async Task<IActionResult> Reply(string id, string parent, [FromBody] CreateCommentDto dto)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null || user.Token == null) return Unauthorized();
            var comment = await _yt.ReplyToCommentAsync(parent, dto.Text, user.Token.RefreshToken);
            await _events.LogAsync(user.Id, "ReplyComment", new { videoId = id, parent });
            return Ok(comment);
        }

        [HttpDelete("/api/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var user = await GetUserFromHeaderAsync();
            if (user == null || user.Token == null) return Unauthorized();
            await _yt.DeleteCommentAsync(commentId, user.Token.RefreshToken);
            await _events.LogAsync(user.Id, "DeleteComment", new { commentId });
            return NoContent();
        }
    }
}
