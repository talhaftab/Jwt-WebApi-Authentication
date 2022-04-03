using JWT.WebApi.Data;
using JWT.WebApi.Business;
using JWT.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using System.Security.Claims;
using JWT.WebApi.Model.Request;

namespace JWT.WebApi.Business
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthManager _authManager;

        public UserManager(IUserRepository userRepository, IAuthManager authManager)
        {
            _userRepository = userRepository;
            _authManager = authManager;
        }

        public async Task<ApiResponse<User?>> RegisterUserAsync(AuthRequest request)
        {
            var userHash = await _authManager.CreatePasswordHashAsync(request.Password);
            var user = new User()
            {
                EmailAddress = request.EmailAddress,
                PasswordHash = userHash?.PasswordHash,
                PasswordSalt = userHash?.PasswordSalt,
            };
            var result = await this._userRepository.RegisterUserAsync(user);
            return result;
        }

        public async Task<User?> CheckUserAsync(string emailAddress)
        {
            var result = await _userRepository.CheckUserExistAsync(emailAddress);
            return result;
        }

        public async Task<List<User?>> GetAllUserAsync()
        {
            var result = await this._userRepository.GetAllUserAsync();
            return result;
        }
        public async Task<UserRefreshToken> GetFirstOrDefaultRefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
        {
            var userRefreshToken = await this._userRepository.GetFirstOrDefaultRefreshTokenAsync(request, ipAddress);
            return userRefreshToken;
        }
    }
}
