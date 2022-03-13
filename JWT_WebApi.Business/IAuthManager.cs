using JWT.WebApi.Model;

namespace JWT.WebApi.Business
{
    public interface IAuthManager
    {
        Task<User?> CreatePasswordHashAsync(string password);
        Task<bool> VerifyPasswordAsync(User user, string password);
    }
}