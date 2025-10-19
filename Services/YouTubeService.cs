using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;

namespace YouTubeCompanion.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly IConfiguration _config;
        public YouTubeService(IConfiguration config) { _config = config; }

        private async Task<Google.Apis.YouTube.v3.YouTubeService> CreateClientAsync(string refreshToken)
        {
            // NOTE: This is a simplified token exchange flow. In production, securely encrypt tokens and use
            // Google APIs OAuth2 flows. Here we use GoogleCredential.FromAccessToken when we have access token.
            // We'll implement refreshing using Google's token endpoint manually to obtain access_token.

            var clientId = _config["Google:ClientId"] ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var clientSecret = _config["Google:ClientSecret"] ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(refreshToken))
                throw new InvalidOperationException("Missing Google OAuth configuration or refresh token.");

            // Exchange refresh token for access token
            using var http = new HttpClient();
            var form = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["refresh_token"] = refreshToken,
                ["grant_type"] = "refresh_token"
            };
            var resp = await http.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(form));
            resp.EnsureSuccessStatusCode();
            var j = System.Text.Json.JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var accessToken = j.RootElement.GetProperty("access_token").GetString()!;

            var cred = GoogleCredential.FromAccessToken(accessToken);
            return new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = "YouTubeCompanion"
            });
        }

        public async Task<Video?> GetVideoAsync(string videoId, string refreshToken)
        {
            var client = await CreateClientAsync(refreshToken);
            var req = client.Videos.List(new[] { "snippet", "statistics", "contentDetails" });
            req.Id = videoId;
            var res = await req.ExecuteAsync();
            return res.Items?.FirstOrDefault();
        }

        public async Task<VideoListResponse?> ListMyUploadsAsync(string refreshToken)
        {
            var client = await CreateClientAsync(refreshToken);
            // Get channel, then list uploads playlist
            var channels = await client.Channels.List(new[] { "contentDetails" }).ExecuteAsync();
            var channel = channels.Items?.FirstOrDefault();
            if (channel == null) return null;
            var uploadsId = channel.ContentDetails?.RelatedPlaylists?.Uploads;
            if (string.IsNullOrEmpty(uploadsId)) return null;
            var plReq = client.PlaylistItems.List(new[] { "snippet", "contentDetails" });
            plReq.PlaylistId = uploadsId;
            plReq.MaxResults = 50;
            var plRes = await plReq.ExecuteAsync();
            // Map to VideoListResponse-like object by fetching Video details per videoId (simplified)
            var ids = plRes.Items.Select(i => i.ContentDetails.VideoId).ToArray();
            var videos = await client.Videos.List(new[] { "snippet", "statistics" }).ExecuteAsync();
            return videos;
        }

        public async Task<CommentThread> PostCommentAsync(string videoId, string text, string refreshToken)
        {
            var client = await CreateClientAsync(refreshToken);
            var commentThread = new CommentThread
            {
                Snippet = new CommentThreadSnippet
                {
                    VideoId = videoId,
                    TopLevelComment = new Comment
                    {
                        Snippet = new CommentSnippet { TextOriginal = text }
                    }
                }
            };
            var req = client.CommentThreads.Insert(commentThread, "snippet");
            return await req.ExecuteAsync();
        }

        public async Task<Comment> ReplyToCommentAsync(string parentCommentId, string text, string refreshToken)
        {
            var client = await CreateClientAsync(refreshToken);
            var comment = new Comment { Snippet = new CommentSnippet { ParentId = parentCommentId, TextOriginal = text } };
            var req = client.Comments.Insert(comment, "snippet");
            return await req.ExecuteAsync();
        }

        public async Task UpdateVideoMetadataAsync(string videoId, string title, string description, string refreshToken)
        {
            var client = await CreateClientAsync(refreshToken);
            var video = new Video { Id = videoId, Snippet = new VideoSnippet { Title = title, Description = description } };
            var req = client.Videos.Update(video, "snippet");
            await req.ExecuteAsync();
        }

        public async Task DeleteCommentAsync(string commentId, string refreshToken)
        {
            var client = await CreateClientAsync(refreshToken);
            var req = client.Comments.Delete(commentId);
            await req.ExecuteAsync();
        }
    }
}
