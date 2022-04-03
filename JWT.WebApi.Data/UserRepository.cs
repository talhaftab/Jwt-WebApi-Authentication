using JWT.WebApi.Data.Context;
using JWT.WebApi.Model;
using JWT.WebApi.Model.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Data
{
    public class UserRepository : IUserRepository
    {
        readonly JwtContext _context;
        public UserRepository(JwtContext context)
        {
            this._context = context;
        }
        public async Task<ApiResponse<User?>> RegisterUserAsync(User user)
        {
            var apiResponse = new ApiResponse<User?>();
            try
            {
                _ = await _context.AddAsync(user);
                var result = await this._context.SaveChangesAsync();
                if (result > 0)
                {
                    apiResponse.Content = user;
                    apiResponse.Message = "New user has been registerd.";
                    apiResponse.Status = System.Net.HttpStatusCode.OK;
                }
                else
                {
                    apiResponse.Content = null;
                    apiResponse.Message = "Not registerd.";
                    apiResponse.Status = System.Net.HttpStatusCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                apiResponse.Content = null;
                apiResponse.Message = ex.Message;
                apiResponse.Status = System.Net.HttpStatusCode.BadRequest;
            }
            return apiResponse;
        }

        public async Task<User?> CheckUserExistAsync(string emailAddress)
        {
            var query = (from x in this._context.Users
                         where x.EmailAddress.Equals(emailAddress)
                         select x);
            var result = await query.FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            var query = (from x in this._context.Users
                         select x);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<ApiResponse<UserRefreshToken?>> SaveUserRefreshTokenAsync(UserRefreshToken? userRefreshToken)
        {
            var apiResponse = new ApiResponse<UserRefreshToken?>();
            try
            {
                _ = await _context.UserRefreshTokens.AddAsync(userRefreshToken);
                var result = await this._context.SaveChangesAsync();
                if (result > 0)
                {
                    apiResponse.Content = userRefreshToken;
                    apiResponse.Message = "Refresh token has been added into database.";
                    apiResponse.Status = System.Net.HttpStatusCode.OK;
                }
                else
                {
                    apiResponse.Content = null;
                    apiResponse.Message = "Not added into database, it could be problem.";
                    apiResponse.Status = System.Net.HttpStatusCode.ExpectationFailed;
                }
            }
            catch (Exception ex)
            {
                apiResponse.Content = null;
                apiResponse.Message = ex.Message;
                apiResponse.Status = System.Net.HttpStatusCode.BadRequest;
            }
            return apiResponse;
        }
        public async Task<UserRefreshToken> GetFirstOrDefaultRefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
        {
            var response = await this._context.UserRefreshTokens.FirstOrDefaultAsync(
                x => x.IsInvalidated == false &&
                x.Token.Equals(request.ExpiredToken) &&
                x.RefreshToken.Equals(request.RefreshToken) &&
                x.IpAddress.Equals(ipAddress));
            return response;
        }
    }
}