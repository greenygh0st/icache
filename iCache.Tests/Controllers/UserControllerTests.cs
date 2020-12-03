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
            _mockService.Setup(x => x.UserExists(_user)).ReturnsAsync(true);

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
            // JsonWithResponse response = await _userController.RemoveUser(_user._Id);
            // Assert.Equal("User removed!", response.Message);
        }

        [Fact]
        public async Task RegenUserPassword_BadModel()
        {

        }

        [Fact]
        public async Task RegenUserPasswordUser()
        {

        }

        [Fact]
        public async Task LockUserAccount_BadModel()
        {

        }

        [Fact]
        public async Task LockUserAccount()
        {

        }

        [Fact]
        public async Task UnlockUserAccount_BadModel()
        {

        }

        [Fact]
        public async Task UnlockUserAccount()
        {

        }
    }
}
