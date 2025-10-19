using Google.Apis.YouTube.v3.Data;


namespace YouTubeCompanion.Services
{
    public interface IYouTubeService
    {
        Task<Video?> GetVideoAsync(string videoId, string refreshToken);
        Task<VideoListResponse?> ListMyUploadsAsync(string refreshToken);
        Task<CommentThread> PostCommentAsync(string videoId, string text, string refreshToken);
        Task<Comment> ReplyToCommentAsync(string parentCommentId, string text, string refreshToken);
        Task UpdateVideoMetadataAsync(string videoId, string title, string description, string refreshToken);
        Task DeleteCommentAsync(string commentId, string refreshToken);
    }
}