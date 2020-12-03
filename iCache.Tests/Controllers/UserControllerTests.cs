using iCache.API.Interfaces;
using iCache.Common.Models;
using iCache.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using iCache.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using iCache.Common.Models;

namespace iCache.Tests.Controllers
{
    public class UserControllerTests
    {
        private Mock<IUserService> _mockService;
        private UserController _userController;
        private Guid _fakeGuid;
        private User _user;

        public UserControllerTests()
        {
            _fakeGuid = Guid.Parse("e5b0a652-28eb-4f18-bdca-960db253a0a5");
            _user = new User { _Id = _fakeGuid };



            // stub mock
            _mockService = new Mock<IUserService>();

            // setup service
            //var validUser = It.Is<User>(x => x._Id.ToString() == "e5b0a652-28eb-4f18-bdca-960db253a0a5");
            _mockService.Setup(x => x.UserExists(It.Is<User>(x => x._Id.ToString() == "e5b0a652-28eb-4f18-bdca-960db253a0a5"))).ReturnsAsync(true);

            // setup controller
            _userController = new UserController(_mockService.Object);
            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { }
            };

        }

        [Fact]
        public async Task CreateUser_BadModel()
        {
            // bad model test setup
            var controller = new UserController(_mockService.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { }
            };
            controller.ModelState.AddModelError("fakeError", "fakeError");

            JsonWithResponse response = await controller.CreateUser(new CreateUser { });
            Assert.Equal("Invalid user request!", response.Message);
        }

        [Fact]
        public async Task CreateUser()
        {
            JsonWithResponse response = await _userController.CreateUser(new CreateUser { DisplayName = "test-user" });
            Assert.Equal("created", response.Message);
        }

        [Fact]
        public async Task RemoveUser_MissingUser()
        {
            Guid noGuid = Guid.NewGuid();
            JsonWithResponse response = await _userController.RemoveUser(noGuid);
            Assert.Equal("Not found", response.Message);

            JsonError errorResponse = response as JsonError;

            Assert.Equal($"{noGuid} was not found!", errorResponse.Errors.Single());
        }

        [Fact]
        public async Task RemoveUser()
        {
            JsonWithResponse response = await _userController.RemoveUser(_user._Id);
            Assert.Equal("User removed!", response.Message);
        }

        [Fact]
        public async Task RegenUserPassword_MissingUser()
        {
            Guid noGuid = Guid.NewGuid();
            JsonWithResponse response = await _userController.RegenUserPassword(noGuid);
            Assert.Equal("Not found", response.Message);

            JsonError errorResponse = response as JsonError;

            Assert.Equal($"{noGuid} was not found!", errorResponse.Errors.Single());
        }

        [Fact]
        public async Task RegenUserPasswordUser()
        {
            JsonWithResponse response = await _userController.RegenUserPassword(_user._Id);
            Assert.Equal("success", response.Message);
        }

        [Fact]
        public async Task LockUserAccount_MissingUser()
        {
            Guid noGuid = Guid.NewGuid();
            JsonStatus response = await _userController.LockUserAccount(noGuid);
            Assert.Equal("Not found", response.Message);

            JsonError errorResponse = response as JsonError;

            Assert.Equal($"{noGuid} was not found!", errorResponse.Errors.Single());
        }

        [Fact]
        public async Task LockUserAccount()
        {
            JsonStatus response = await _userController.LockUserAccount(_user._Id);
            Assert.Equal($"User account {_user._Id} locked!", response.Message);
        }

        [Fact]
        public async Task UnlockUserAccount_MissingUser()
        {
            Guid noGuid = Guid.NewGuid();
            JsonStatus response = await _userController.UnlockUserAccount(noGuid);
            Assert.Equal("Not found", response.Message);

            JsonError errorResponse = response as JsonError;

            Assert.Equal($"{noGuid} was not found!", errorResponse.Errors.Single());
        }

        [Fact]
        public async Task UnlockUserAccount()
        {
            JsonStatus response = await _userController.UnlockUserAccount(_user._Id);
            Assert.Equal($"User account {_user._Id} unlocked!", response.Message);
        }
    }
}
