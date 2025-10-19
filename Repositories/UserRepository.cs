using Microsoft.EntityFrameworkCore;
using YouTubeCompanion.Data;
using YouTubeCompanion.Models;


namespace YouTubeCompanion.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) { _db = db; }
        public async Task<User?> GetByGoogleIdAsync(string googleId) => await _db.Users.Include(u => u.Token).FirstOrDefaultAsync(u => u.GoogleUserId == googleId);
        public async Task<User?> GetByIdAsync(Guid id) => await _db.Users.Include(u => u.Token).FirstOrDefaultAsync(u => u.Id == id);
        public async Task<User> CreateOrUpdateAsync(User user)
        {
            var existing = await GetByGoogleIdAsync(user.GoogleUserId);
            if (existing is null)
            {
                _db.Users.Add(user);
            }
            else
            {
                existing.DisplayName = user.DisplayName;
                existing.Email = user.Email;
                // merge token if provided
                if (user.Token != null)
                {
                    if (existing.Token == null) existing.Token = user.Token;
                    else { existing.Token.RefreshToken = user.Token.RefreshToken; existing.Token.AccessToken = user.Token.AccessToken; }
                }
            }
            await _db.SaveChangesAsync();
            return await GetByGoogleIdAsync(user.GoogleUserId) ?? user;
        }
    }
}