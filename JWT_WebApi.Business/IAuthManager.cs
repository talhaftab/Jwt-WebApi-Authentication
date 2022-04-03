using JWT.WebApi.Model;
using JWT.WebApi.Model.Response;

namespace JWT.WebApi.Business
{
    public interface IAuthManager
    {
        Task<User?> CreatePasswordHashAsync(string password);
        Task<bool> VerifyPasswordAsync(User user, string password);
        Task<AuthResponse> GenerateTokenAsync(User user, string ipAddress);
        string GetClaimName();
        Task<string?> GenerateRefreshToken();
    }
}