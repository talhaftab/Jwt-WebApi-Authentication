using JWT.WebApi.Data;
using JWT.WebApi.Model;
using JWT.WebApi.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Business
{
    public class AuthManager : IAuthManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository userRepository;


        public AuthManager(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this.userRepository = userRepository;
        }
        public async Task<User?> CreatePasswordHashAsync(string password)
        {
            var user = new User();
            using (var hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            return user;
        }
        public async Task<AuthResponse> GenerateTokenAsync(User user, string ipAddress)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,user.EmailAddress),
                new Claim(ClaimTypes.Role,"Admin"),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.
                GetBytes(_configuration.GetSection("Authentication:SecretKey").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = await GenerateRefreshToken();
            var result = await SaveTokenDetailAsync(ipAddress, refreshToken, jwt, user.Id);
            if (result.Content == null)
            {
                return await Task.FromResult<AuthResponse>(new AuthResponse()
                {
                    Token = jwt,
                    RefreshToken = refreshToken,
                    Message = "Not added into database, it could be problem."
                });
            }
            return await Task.FromResult<AuthResponse>(new AuthResponse()
            {
                Token = jwt,
                RefreshToken = refreshToken,
            });
        }

        private async Task<ApiResponse<UserRefreshToken?>> SaveTokenDetailAsync(string ipAddress, string refreshToken, string token, Guid userId)
        {
            var userRefreshToken = new UserRefreshToken()
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(1),
                IpAddress = ipAddress,
                IsInvalidated = false,
                RefreshToken = refreshToken,
                Token = token,
                UserId = userId,
            };
            var result = await this.userRepository.SaveUserRefreshTokenAsync(userRefreshToken);
            return result;
        }

        public async Task<bool> VerifyPasswordAsync(User user, string password)
        {
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(user.PasswordHash);
            }
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
        public async Task<string?> GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[64];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            await System.Threading.Tasks.Task.Run(() => randomNumberGenerator.GetBytes(secureRandomBytes));

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }
    }
}

