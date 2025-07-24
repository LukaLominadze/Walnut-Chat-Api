using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENDE.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ENDE.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserRepository(DataContext dataContext, PasswordHasher<User> passwordHasher)
        {
            _dataContext = dataContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> AddUserAsync(User user)
        {
            if (await _dataContext.Users.AnyAsync(x => x.Username == user.Username))
            {
                return null;
            }
            if (await _dataContext.Users.AnyAsync(u => u.Email == user.Email))
            {
                return null;
            }

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }

        public async Task<User?> DeleteUserByUsernameAsync(string username)
        {
            var user = _dataContext.Users.FirstOrDefault(x => x.Username == username);
            if (user == null)
            {
                return null;
            }

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dataContext.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetUserByGuidAsync(string id)
        {
            return await _dataContext.Users
                .FirstOrDefaultAsync(x => x.Id.ToString().ToLower().Equals(id.ToLower()));
        }

        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _dataContext.Users
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _dataContext.Users.ToListAsync();
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }
    }
}
