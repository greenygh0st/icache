using System;
using System.Threading.Tasks;
using iCache.Common.Models;
using iCache.API.Services;
using Xunit;
using System.Linq;

namespace iCache.Tests
{
    public class UserServiceTests
    {
        public UserServiceTests()
        {

        }

        [Fact]
        public async Task CreateUser()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Bart" });

                // user was indeed made
                Assert.NotNull(createdUser);

                // _Id is set
                //Assert.True(createdUser._Id !=);

                // password is set
                Assert.True(!string.IsNullOrEmpty(createdUser.DisplayName));

                // password is set
                Assert.True(!string.IsNullOrEmpty(createdUser.Password));

                // re-fetch user and make sure the password hash is built correctly
                User fetched = await userService.GetUser(createdUser._Id.ToString());

                // make sure the user exists
                Assert.NotNull(fetched);

                // make sure the password hash has two parts
                Assert.Equal(2, fetched.Password.Split(".").Length);

                // remove the user
                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task GetUser()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Phil" });

                // user was indeed made
                Assert.NotNull(createdUser);

                // re-fetch user and make sure the password hash is built correctly
                User fetched = await userService.GetUser(createdUser._Id.ToString());

                // make sure we got the right data
                Assert.Equal("Phil", fetched.DisplayName);

                // delete the user
                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task RemoveUser()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "John" });

                // user was indeed made
                Assert.NotNull(createdUser);

                // delete the user
                await userService.RemoveUser(createdUser);

                // make sure the user is deleted
                Assert.True(await userService.GetUser(createdUser._Id.ToString()) == null);
            }
        }

        [Fact]
        public async Task AuthenticateUser()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Joe" });

                // user was indeed made
                Assert.NotNull(createdUser);

                string plainTextPassword = createdUser.Password;

                // plain text password wont have the dot
                Assert.Single(plainTextPassword.Split("."));

                Assert.True(await userService.Authenticate(createdUser._Id.ToString(), plainTextPassword));

                // remove the user
                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task UpdateUserPassword()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Joe" });

                // user was indeed made
                Assert.NotNull(createdUser);

                string plainTextPassword = createdUser.Password;

                // plain text password wont have the dot
                Assert.Single(plainTextPassword.Split("."));

                // re-fetch user and make sure the password hash is built correctly
                User fetched = await userService.GetUser(createdUser._Id.ToString());

                // make sure the password hash has two parts
                Assert.Equal(2, fetched.Password.Split(".").Length);

                User regenPassword = await userService.RegeneratePassword(fetched);

                // make sure the plain text passwords are not equal
                Assert.NotEqual(createdUser.Password, regenPassword.Password);

                // fetch the user again and compare the hashes
                User fetchedTwo = await userService.GetUser(createdUser._Id.ToString());

                Assert.NotEqual(fetched.Password, fetchedTwo.Password);

                // remove the user
                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task IsUserLocked()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Phillis" });

                Assert.False(await userService.UserIsLocked(createdUser));

                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task NewUserPasswordIsRightLength()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Phillis" });

                Assert.True(createdUser.Password.Length == 40);

                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task UserLockedAfterTenFailedAttempts()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Donnie" });

                foreach (var item in Enumerable.Range(1, 10))
                    await userService.Authenticate(createdUser._Id.ToString(), "not-the-right-password");

                Assert.True(await userService.UserIsLocked(createdUser));

                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task LockUnlockUserAccount()
        {
            using (UserService userService = new UserService())
            {
                User createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Joan" });

                await userService.LockUser(createdUser);

                Assert.True(await userService.UserIsLocked(createdUser));

                await userService.UnlockUser(createdUser);

                Assert.False(await userService.UserIsLocked(createdUser));

                await userService.RemoveUser(createdUser);
            }
        }
    }
}
