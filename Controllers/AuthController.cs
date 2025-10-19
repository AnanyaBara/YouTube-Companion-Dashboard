using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using YouTubeCompanion.Data;
using YouTubeCompanion.Models;
using YouTubeCompanion.Repositories;
using YouTubeCompanion.Services;
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _users;
    private readonly AppDbContext _db;
    private readonly IEventLogger _events;


    public AuthController(IConfiguration config, IUserRepository users, AppDbContext db, IEventLogger events)
    {
        _config = config; _users = users; _db = db; _events = events;
    }


    [HttpGet("google/login")]
    public IActionResult LoginWithGoogle()
    {
        var clientId = _config["Google:ClientId"] ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        var redirect = _config["Google:RedirectUri"] ?? Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI");
        var scopes = HttpUtility.UrlEncode("https://www.googleapis.com/auth/youtube https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile");
        var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&response_type=code&scope={scopes}&redirect_uri={redirect}&access_type=offline&prompt=consent";
        return Redirect(url);
    }


    [HttpGet("google/callback")]
    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        var clientId = _config["Google:ClientId"] ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        var clientSecret = _config["Google:ClientSecret"] ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
        var redirect = _config["Google:RedirectUri"] ?? Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI");
        using var http = new HttpClient();
        var form = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = clientId!,
            ["client_secret"] = clientSecret!,
            ["redirect_uri"] = redirect!,
            ["grant_type"] = "authorization_code"
        };
        var resp = await http.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(form));
        resp.EnsureSuccessStatusCode();
        var content = await resp.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        var accessToken = doc.RootElement.GetProperty("access_token").GetString();
        var refreshToken = doc.RootElement.TryGetProperty("refresh_token", out var r) ? r.GetString() : null;


        // Get userinfo
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken!);
        var userInfo = await http.GetStringAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        var uobj = JsonDocument.Parse(userInfo).RootElement;
        var googleId = uobj.GetProperty("id").GetString();
        var email = uobj.GetProperty("email").GetString();
        var name = uobj.TryGetProperty("name", out var n) ? n.GetString() : null;


        var user = new User { GoogleUserId = googleId!, Email = email, DisplayName = name };
        if (!string.IsNullOrEmpty(refreshToken)) user.Token = new UserToken { RefreshToken = refreshToken };


        var created = await _users.CreateOrUpdateAsync(user);
        await _events.LogAsync(created.Id, "Auth:GoogleLogin", new { googleId = googleId });


        // For simplicity redirect to a frontend page with a userId param. In production, issue JWT.
        var frontend = Environment.GetEnvironmentVariable("FRONTEND_ORIGIN") ?? _config["FrontendOrigin"] ?? "http://localhost:4200";
        var redirectUrl = $"{frontend}/auth/success?userId={created.Id}";
        return Redirect(redirectUrl);
    }
}
