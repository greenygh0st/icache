using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using iCache.API.Handlers;
using iCache.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using Xunit;
using iCache.Common.Models;
using iCache.API;
using System.ComponentModel;

namespace iCache.Tests
{
    public class BasicAuthenticationHandlerTests
    {
        private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _options;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<UrlEncoder> _encoder;
        private readonly Mock<ISystemClock> _clock;
        private readonly Mock<UserService> _userService;
        private readonly BasicAuthenticationHandler _handler;

        public BasicAuthenticationHandlerTests()
        {
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _options.Setup(x => x.Get("Basic")).Returns(new AuthenticationSchemeOptions());

            var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            _userService = new Mock<UserService>();

            _handler = new BasicAuthenticationHandler(_options.Object, _loggerFactory.Object, _encoder.Object, _clock.Object, _userService.Object);
        }

        private string EncodeAuthData(string username, string password)
        {
            string preencoded = $"{username}:{password}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(preencoded));
        }

        [Fact]
        public async Task NotAuthorizedNoHeader()
        {
            var context = new DefaultHttpContext();

            await _handler.InitializeAsync(
                new AuthenticationScheme("Basic",
                "Basic",
                typeof(BasicAuthenticationHandler)),
                context);

            var result = await _handler.AuthenticateAsync();

            Assert.False(result.Succeeded);
            Assert.Equal("Missing Authorization Header", result.Failure.Message);

        }

        [Fact]
        public async Task BadAuthorizationHeader()
        {
            var context = new DefaultHttpContext();
            var authorizationHeader = new StringValues("Basc VGVzdc3RQYXNzd29yZA==");
            context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

            await _handler.InitializeAsync(
                new AuthenticationScheme("Basic",
                "Basic",
                typeof(BasicAuthenticationHandler)),
                context);

            var result = await _handler.AuthenticateAsync();

            Assert.False(result.Succeeded);
            Assert.Equal("Invalid Authorization Header", result.Failure.Message);
        }

        [Fact]
        public async Task ValidHeaderInvalidUser()
        {
            var context = new DefaultHttpContext();
            var authorizationHeader = new StringValues($"Basic {EncodeAuthData("not-a-user", "123456789")}");
            context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

            await _handler.InitializeAsync(
                new AuthenticationScheme("Basic",
                "Basic",
                typeof(BasicAuthenticationHandler)),
                context);

            var result = await _handler.AuthenticateAsync();

            Assert.False(result.Succeeded);
            Assert.Equal("Invalid Username or Password", result.Failure.Message);
        }

        [Fact]
        public async Task ValidUser()
        {
            using (UserService userService = new UserService())
            {
                User user = await userService.CreateUser(new CreateUser { DisplayName = "handler-test-user" });

                // plain text password wont have the dot
                Assert.Single(user.Password.Split("."));

                Assert.True(await userService.UserExists(user));

                var context = new DefaultHttpContext();
                var authorizationHeader = new StringValues($"Basic {EncodeAuthData(user._Id.ToString(), user.Password)}");
                context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

                await _handler.InitializeAsync(
                    new AuthenticationScheme("Basic",
                    "Basic",
                    typeof(BasicAuthenticationHandler)),
                    context);

                var result = await _handler.AuthenticateAsync();

                Assert.True(result.Succeeded);

                await userService.RemoveUser(user);
            }   
        }
    }
}
