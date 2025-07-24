using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENDE.Models.User;

namespace ENDE.Data.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();
        public Task<User?> GetUserByGuidAsync(string id);
        public Task<User?> GetUserByUsernameAsync(string username);
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User?> AddUserAsync(User user);
        public Task<User?> UpdateUserAsync(User user);
        public Task<User?> DeleteUserByUsernameAsync(string username);
    }
}
