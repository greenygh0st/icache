using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using iCache.API.Exceptions;
using iCache.API.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TCache;

namespace iCache.API.Services
{
    public class UserService : IDisposable
    {
        private TCacheService _cacheService;
        private string _userPrefix = "user:";

        public UserService()
        {
            _cacheService = new TCacheService(Configuration.RedisConnection);
        }

        public async Task<User> GetUser(string id)
        {
            return await _cacheService.GetValueFromKey<User>(_userPrefix + id);
        }

        public async Task<User> GetUser(User user)
        {
            return await _cacheService.GetValueFromKey<User>(_userPrefix + user._Id.ToString());
        }

        public async Task<bool> AuthenticateUser(string id, string password)
        {
            User user = await GetUser(id);

            if (user != null)
                return user.Password == $"{Hash(password, user.Password.Split(".")[1])}.{user.Password.Split(".")[1]}";

            return false;
        }

        public async Task<User> CreateUser(CreateUser user)
        {
            var (Hashed, Plaintext) = await CreateAndHashPassword();

            User newUser = new User
            {
                _Id = Guid.NewGuid(),
                DisplayName = user.DisplayName,
                Password = Hashed
            };

            await _cacheService.SetObjectAsKeyValue(_userPrefix + newUser._Id.ToString(), newUser);

            // temp assignment of password
            newUser.Password = Plaintext;

            return newUser;
        }

        public async Task<bool> RemoveUser(User removeUser)
        {
            User foundUser = await GetUser(removeUser);

            if (foundUser == null)
                throw new UserNotFoundException();

            await _cacheService.RemoveKey(_userPrefix + foundUser._Id.ToString());

            return true;
        }

        public async Task<User> RegeneratePassword(User user)
        {
            User foundUser = await GetUser(user._Id.ToString());

            if (foundUser != null)
            {
                var (Hashed, Plaintext) = await CreateAndHashPassword();

                foundUser.Password = Hashed;

                await _cacheService.SetObjectAsKeyValue(_userPrefix + foundUser._Id.ToString(), foundUser);

                // temp assignment of password
                foundUser.Password = Plaintext;

                return foundUser;
            }

            throw new UserNotFoundException();
        }

        private async Task<(string Hashed, string Plaintext)> CreateAndHashPassword()
        {
            string randomPassword = await Task.FromResult(GenerateRandomPassword());

            string salt = await Task.FromResult(GenerateSalt());

            string hashedPassword = await Task.FromResult(Hash(randomPassword, salt));

            return ($"{hashedPassword}.{salt}", randomPassword);
        }

        private string GenerateRandomPassword()
        {
            try
            {
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789&!#$_-";

                return new string(Enumerable.Repeat(chars, 24)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string Hash(string password, string storedSalt = null)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: (!string.IsNullOrEmpty(storedSalt.Trim())) ? Convert.FromBase64String(storedSalt) : Convert.FromBase64String(GenerateSalt()),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        private string GenerateSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public void Dispose()
        {
            ((IDisposable)_cacheService).Dispose();
        }
    }
}
