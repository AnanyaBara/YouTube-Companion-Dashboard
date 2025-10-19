using YouTubeCompanion.Models;


namespace YouTubeCompanion.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User> CreateOrUpdateAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
    }
}