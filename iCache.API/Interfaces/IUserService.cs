using System;
using System.Threading.Tasks;
using iCache.Common.Models;

namespace iCache.API.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser(string id);
        Task<User> GetUser(User user);
        Task<bool> Authenticate(string id, string password);
        Task<User> CreateUser(CreateUser user);
        Task<bool> RemoveUser(User removeUser);
        Task<User> RegeneratePassword(User user);
        Task<bool> UserIsLocked(User user);
        Task<bool> LockUser(User user);
        Task<bool> UnlockUser(User user);
        Task<bool> UserExists(User user);
        Task<bool> UpdateUser(User user);
        Task<(string Hashed, string Plaintext)> CreateAndHashPassword();
        string GenerateRandomPassword();
        string Hash(string password, string storedSalt = null);
        string GenerateSalt();
    }
}
