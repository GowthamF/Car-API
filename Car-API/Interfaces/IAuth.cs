﻿using Car_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_API.Interfaces
{
    public interface IAuth
    {
        Task<User> Register(User user, string password);

        Task<User> Login(string userName, string password, string telNumber);

        Task<bool> UserExists(string userName, string telNumber);

        Task<User> UpdateUser(User user, string password);

        Task<bool> DeleteUser(User user);
    }
}
