using JWT.WebApi.Data.Context;
using JWT.WebApi.Model;
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
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.Status = System.Net.HttpStatusCode.BadRequest;
            }
            return apiResponse;
        }

        public async Task<User?> CheckUserExistAsync(User user)
        {
            var query = (from x in this._context.Users
                         where x.EmailAddress == user.EmailAddress
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
    }
}