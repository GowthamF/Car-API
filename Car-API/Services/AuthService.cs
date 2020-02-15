using Car_API.Interfaces;
using Car_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_API.Services
{
    public class AuthService:IAuth
    {
        private readonly CarDBContext _context;

        public AuthService(CarDBContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteUser(User user)
        {
            var _user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);

            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(_user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User> Login(string userName, string password, string telNumber)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName || x.TelephoneNumber==telNumber);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            //Out keyword will store the data coming from CreatePasswordHash without declaring variables globally
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(User user,string password)
        {

            byte[] passwordHash, passwordSalt;
            var _user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            _user.UserName = user.UserName;
            _user.TelephoneNumber = user.TelephoneNumber;
            _user.PasswordHash = passwordHash;
            _user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();

            return _user;
        }

        public async Task<bool> UserExists(string userName, string telNumber)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == userName || x.TelephoneNumber==telNumber))
            {
                return true;
            }

            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }

                }

            }

            return true;
        }
    }
}
