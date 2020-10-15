using System;
using iCache.API.Controllers;
using Xunit;

namespace iCache.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void ShowsWelcomeMessage()
        {
            var controller = new HomeController();

            var result = controller.Welcome();

            Assert.Equal("Welcome to the iCache service!", result.Message);
        }
    }
}
