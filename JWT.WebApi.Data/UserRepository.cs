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
        public async Task<User?> RegisterUserAsync(User user)
        {
            var isExist = (from x in this._context.Users
                           where x.EmailAddress == user.EmailAddress
                           select x).FirstOrDefault();
            if (isExist is null)
            {
                _ = await this._context.AddAsync(user);
            }
            var result = await this._context.SaveChangesAsync();
            return result > 0 ? user : null;
        }
    }
}
