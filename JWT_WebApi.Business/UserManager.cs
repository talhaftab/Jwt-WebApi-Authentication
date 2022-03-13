using JWT.WebApi.Data;
using JWT.WebApi.Business;
using JWT.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<ApiResponse<User?>> RegisterUserAsync(User user, string password)
        {
            var userHash = await _authManager.CreatePasswordHashAsync(password);
            user.PasswordHash = userHash.PasswordHash;
            user.PasswordSalt = userHash.PasswordSalt;
            var result = await this._userRepository.RegisterUserAsync(user);
            return result;
        }

        public Task<User> LoginUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
