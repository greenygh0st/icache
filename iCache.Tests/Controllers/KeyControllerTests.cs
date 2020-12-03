using System;
using Xunit;
using iCache.Common.Models;
using iCache.API.Services;
using iCache.API.Interfaces;
using System.Threading.Tasks;
using Moq;
using iCache.API.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace iCache.Tests.Controllers
{
    public class KeyControllerTests
    {
        public KeyControllerTests()
        {
            _claimsPrinciple = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, "fd7044f7-142b-43d4-b317-b6b90c6c39dc"),
                        }, "mock"));

            _mockService = new Mock<IKeyService>();

            // setup
            _mockService.Setup(x => x.FetchKey("fd7044f7-142b-43d4-b317-b6b90c6c39dc:find-key")).ReturnsAsync("test123");
            _mockService.Setup(x => x.KeyExists("fd7044f7-142b-43d4-b317-b6b90c6c39dc:find-key")).ReturnsAsync(true);

            _mockService.Setup(x => x.SetKey("good-key", "happy-day")).ReturnsAsync(true);

            _mockService.Setup(x => x.SearchKeys("fd7044f7-142b-43d4-b317-b6b90c6c39dc:t*y")).ReturnsAsync(new List<string> { "toy" });
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("fd7044f7-142b-43d4-b317-b6b90c6c39dc:toy", "a magic toy");
            _mockService.Setup(x => x.SearchKeysGetValues("fd7044f7-142b-43d4-b317-b6b90c6c39dc:t*y")).ReturnsAsync(dic);

            _mockService.Setup(x => x.SearchKeys("fd7044f7-142b-43d4-b317-b6b90c6c39dc:not-a-key")).ReturnsAsync(new List<string>());

            _keyController = new KeyController(_mockService.Object);

            _keyController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _claimsPrinciple }
            };
        }

        string _findKey;
        string _deleteKey;
        User _user;
        ClaimsPrincipal _claimsPrinciple;
        KeyController _keyController;
        Mock<IKeyService> _mockService;

        [Fact]
        public async Task GetKey_FetchMissingKey()
        {

            JsonWithResponse response = await _keyController.GetKey("key123");

            Assert.Equal("Key not found!", response.Message);
        }

        [Fact]
        public async Task GetKey_MissingKey()
        {

            JsonWithResponse response = await _keyController.GetKey("");

            Assert.Equal("Key value not supplied!", response.Message);
        }

        [Fact]
        public async Task GetKey_FetchKey()
        {

            JsonWithResponse response = await _keyController.GetKey("find-key");

            Assert.Equal("success", response.Message);
        }

        #region Delete

        [Fact]
        public async Task DeleteKey_FetchMissingKey()
        {

            JsonStatus response = await _keyController.DeleteKey("key123");

            Assert.Equal("Key not found!", response.Message);
        }

        [Fact]
        public async Task DeleteKey_MissingKey()
        {

            JsonStatus response = await _keyController.DeleteKey("");

            Assert.Equal("Key not found!", response.Message);
        }

        [Fact]
        public async Task DeleteKey_FetchKey()
        {

            JsonStatus response = await _keyController.DeleteKey("find-key");

            Assert.Equal("Key deleted!", response.Message);
        }

        #endregion

        #region Create Key

        [Fact]
        public async Task CreateKey_BadModel()
        {
            // test setup
            var controller = new KeyController(_mockService.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _claimsPrinciple }
            };
            controller.ModelState.AddModelError("fakeError", "fakeError");

            // mock request
            JsonWithResponse response = await controller.CreateKey(new CreateValueItem { Key = "yo" });            

            Assert.Equal("Invalid key request!", response.Message);
        }

        [Fact]
        public async Task CreateKey_GoodModel()
        {
            JsonWithResponse response = await _keyController.CreateKey(new CreateValueItem { Key = "good-key", Value = "happy-day" });

            Assert.Equal("created", response.Message);
        }

        #endregion

        #region Search Tests

        [Fact]
        public async Task SearchKey_BadModel()
        {
            // test setup
            var controller = new KeyController(_mockService.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _claimsPrinciple }
            };
            controller.ModelState.AddModelError("fakeError", "fakeError");

            // mock request
            JsonWithResponse response = await controller.SearchKey(null);

            Assert.Equal("Invalid key search request!", response.Message);
        }

        [Fact]
        public async Task SearchKey_KeyOnly()
        {
            JsonWithResponse response = await _keyController.SearchKey(new KeySearch { SearchTerm = "t*y", IncludeValues = false });

            Dictionary<string, string> parsedResponse = response.Response as Dictionary<string, string>;

            var value = parsedResponse.First();

            Assert.Equal("toy", value.Key);
            Assert.Empty(value.Value);
        }

        [Fact]
        public async Task SearchKey_KeyAndValue()
        {
            JsonWithResponse response = await _keyController.SearchKey(new KeySearch { SearchTerm = "t*y", IncludeValues = true });

            Dictionary<string, string> parsedResponse = response.Response as Dictionary<string, string>;

            string valRes = parsedResponse.SingleOrDefault(x => x.Key == "toy").Value;

            Assert.Equal("a magic toy", valRes);
        }

        [Fact]
        public async Task SearchKey_MissingKey()
        {
            JsonWithResponse response = await _keyController.SearchKey(new KeySearch { SearchTerm = "not-a-key", IncludeValues = false });

            Dictionary<string, string> parsedResponse = response.Response as Dictionary<string, string>;

            Assert.Empty(parsedResponse);
        }

        #endregion
    }
}
