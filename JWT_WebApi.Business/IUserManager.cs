using JWT.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Business
{
    public interface IUserManager
    {
        Task<User?> RegisterUserAsync(User user, string password);
        Task<User> LoginUserAsync(User user);
    }
}
