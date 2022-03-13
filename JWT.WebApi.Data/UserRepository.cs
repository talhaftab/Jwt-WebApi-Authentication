using JWT.WebApi.Data.Context;
using JWT.WebApi.Model;
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
            var isExist = (from x in this._context.Users
                           where x.EmailAddress == user.EmailAddress
                           select x).FirstOrDefault();
            if (isExist is not null)
            {
                apiResponse.Content = isExist;
                apiResponse.Message = "User is already exisit.";
                apiResponse.Status = System.Net.HttpStatusCode.BadRequest;
            }
            else
            {
                _ = await this._context.AddAsync(user);
                var result = await this._context.SaveChangesAsync();
                apiResponse.Content = user;
                apiResponse.Message = "New user has been registerd.";
                apiResponse.Status = System.Net.HttpStatusCode.OK;
            }
            return apiResponse;
        }
    }
}
