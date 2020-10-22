using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using iCache.API.Exceptions;
using iCache.Common.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TCache;

namespace iCache.API.Services
{
    public interface IUserService
    {
        Task<bool> Authenticate(string id, string password);
    }

    public class UserService : IDisposable, IUserService
    {
        private TCacheService _cacheService;
        private string _userPrefix = "user:";

        public UserService()
        {
            _cacheService = new TCacheService(Configuration.RedisConnection);
        }

        /// <summary>
        /// Get a user
        /// </summary>
        /// <param name="id"><see cref="string"/> GUID for user.</param>
        /// <returns><see cref="User"/> object</returns>
        public async Task<User> GetUser(string id)
        {
            return await _cacheService.GetValueFromKey<User>(_userPrefix + id);
        }

        /// <summary>
        /// Get a user based on thier user object
        /// </summary>
        /// <param name="user"><see cref="User"/> you want to get</param>
        /// <returns><see cref="User"/> object</returns>
        public async Task<User> GetUser(User user)
        {
            return await _cacheService.GetValueFromKey<User>(_userPrefix + user._Id.ToString());
        }

        /// <summary>
        /// Authenticate a user
        /// </summary>
        /// <param name="id"><see cref="string"/> of user <see cref="Guid"/>.</param>
        /// <param name="password">Plaintext password you want to authenciate the user with</param>
        /// <returns><see cref="bool"/>, indicating whether or not the authentication was successful</returns>
        public async Task<bool> Authenticate(string id, string password)
        {
            User user = await GetUser(id);

            if (user != null)
                if (!user.Locked && user.Password == $"{Hash(password, user.Password.Split(".")[1])}.{user.Password.Split(".")[1]}")
                {
                    // zero out login attempts
                    user.LoginAttempts = 0;

                    // save back the user data
                    await UpdateUser(user);

                    return true;
                } else
                {
                    // increment the login attempts
                    user.LoginAttempts++;
                    user.Locked = user.LoginAttempts >= 10;

                    // save back the user data
                    await UpdateUser(user);
                }

            return false;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"><see cref="CreateUser"/> required information to create a user</param>
        /// <returns></returns>
        public async Task<User> CreateUser(CreateUser user)
        {
            var (Hashed, Plaintext) = await CreateAndHashPassword();

            User newUser = new User
            {
                _Id = Guid.NewGuid(),
                DisplayName = user.DisplayName,
                Password = Hashed,
                LoginAttempts = 0,
                Locked = false
            };

            await _cacheService.SetObjectAsKeyValue(_userPrefix + newUser._Id.ToString(), newUser);

            // temp assignment of password
            newUser.Password = Plaintext;

            return newUser;
        }

        /// <summary>
        /// Remove a user
        /// </summary>
        /// <param name="removeUser"></param>
        /// <returns></returns>
        public async Task<bool> RemoveUser(User removeUser)
        {
            User foundUser = await GetUser(removeUser);

            if (foundUser == null)
                throw new UserNotFoundException();

            await _cacheService.RemoveKey(_userPrefix + foundUser._Id.ToString());

            return true;
        }

        /// <summary>
        /// Re-generate the password for the provided user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Determine whether or not a user account is locked.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object you want to check.</param>
        /// <returns></returns>
        public async Task<bool> UserIsLocked(User user)
        {
            User fetched = await GetUser(user);
            return fetched.Locked;
        }

        /// <summary>
        /// Lock the provided user
        /// </summary>
        /// <param name="user">The <see cref="User"/> object for the user that you want to lock</param>
        /// <returns></returns>
        public async Task<bool> LockUser(User user)
        {
            User fetched = await GetUser(user);
            if (!fetched.Locked)
            {
                fetched.Locked = true;
                await UpdateUser(fetched);
            }

            return true;
        }

        /// <summary>
        /// Lock the provided user
        /// </summary>
        /// <param name="user">The <see cref="User"/> object for the user that you want to lock</param>
        /// <returns></returns>
        public async Task<bool> UnlockUser(User user)
        {
            User fetched = await GetUser(user);
            if (fetched.Locked)
            {
                fetched.LoginAttempts = 0;
                fetched.Locked = false;
                await UpdateUser(fetched);
            }

            return true;
        }

        /// <summary>
        /// Check to see if a user exists
        /// </summary>
        /// <param name="user"><see cref="User"/> object which contains a valid user id</param>
        /// <returns></returns>
        public async Task<bool> UserExists(User user)
        {
            return await _cacheService.KeyExists(_userPrefix + user._Id.ToString());
        }

        #region Private Methods

        private async Task<bool> UpdateUser(User user)
        {
            await _cacheService.SetObjectAsKeyValue(_userPrefix + user._Id.ToString(), user);
            return true;
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
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789&!#$_-";

            return new string(Enumerable.Repeat(chars, 40)
              .Select(s => s[random.Next(s.Length)]).ToArray());
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

        #endregion
    }
}
