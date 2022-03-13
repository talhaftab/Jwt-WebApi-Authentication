﻿using JWT.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Data
{
    public interface IUserRepository
    {
        Task<User?> RegisterUserAsync(User user);
    }
}
