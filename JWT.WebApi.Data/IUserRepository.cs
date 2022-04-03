using JWT.WebApi.Model;
using JWT.WebApi.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Data
{
    public interface IUserRepository
    {
        Task<ApiResponse<User?>> RegisterUserAsync(User user);
        Task<User?> CheckUserExistAsync(string emailAddress);
        Task<List<User?>> GetAllUserAsync();
        Task<ApiResponse<UserRefreshToken?>> SaveUserRefreshTokenAsync(UserRefreshToken? userRefreshToken);
        Task<UserRefreshToken> GetFirstOrDefaultRefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
    }
}
