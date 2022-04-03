using JWT.WebApi.Model;
using JWT.WebApi.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Business
{
    public interface IUserManager
    {
        Task<ApiResponse<User?>> RegisterUserAsync(AuthRequest request);
        Task<User> CheckUserAsync(string emailAddress);
        Task<List<User>> GetAllUserAsync();

    }
}
