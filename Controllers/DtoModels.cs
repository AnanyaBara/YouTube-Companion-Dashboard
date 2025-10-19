namespace YouTubeCompanion.Controllers
{
    public record UpdateVideoDto(string Title, string Description);
    public record CreateCommentDto(string Text);
    public record CreateNoteDto(string VideoId, string? Title, string? Body, string[]? Tags);
    public record UpdateNoteDto(string? Title, string? Body, string[]? Tags);
}