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

namespace JWT.WebApi.Business
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthManager _authManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserManager(IUserRepository userRepository, IAuthManager authManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _authManager = authManager;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<User?>> RegisterUserAsync(User user, string password)
        {
            var userHash = await _authManager.CreatePasswordHashAsync(password);
            user.PasswordHash = userHash.PasswordHash;
            user.PasswordSalt = userHash.PasswordSalt;
            var result = await this._userRepository.RegisterUserAsync(user);
            return result;
        }

        public async Task<User?> CheckUserAsync(User user)
        {
            var result = await _userRepository.CheckUserExistAsync(user);
            return result;
        }

        public async Task<List<User?>> GetAllUserAsync()
        {
            var result = await this._userRepository.GetAllUserAsync();
            return result;
        }

        public string? GetClaimName()
        {
            var result = string.Empty;
            if (this.httpContextAccessor.HttpContext != null)
            {
                result = httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value.ToString();
            }
            return result;
        }
    }
}
