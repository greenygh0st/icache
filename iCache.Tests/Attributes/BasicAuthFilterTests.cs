using System;
using System.Text;
using System.Threading.Tasks;
using iCache.API.Services;
using iCache.API.Models;
using iCache.API.Filters;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace iCache.Tests
{
    public class BasicAuthFilterTests
    {

        [Fact]
        public async Task Authorized()
        {
            using (UserService userService = new UserService())
            {
                var createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Andrea" });
                User fetchUser = await userService.GetUser(createdUser._Id.ToString());

                string authData = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{fetchUser._Id}:{createdUser.Password}"));

                var httpContextMock = new Mock<HttpContext>();
                httpContextMock
                  .Setup(a => a.Request.Headers["Authorization"])
                  .Returns($"Basic {authData}");



                ActionContext fakeActionContext =
                new ActionContext(httpContextMock.Object,
                  new Microsoft.AspNetCore.Routing.RouteData(),
                  new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

                AuthorizationFilterContext fakeAuthFilterContext =
                new AuthorizationFilterContext(fakeActionContext,
                  new List<IFilterMetadata> { });

                BasicAuthFilter basicAuthAuthAttribute =
                  new BasicAuthFilter();

                basicAuthAuthAttribute.OnAuthorization(fakeAuthFilterContext);

                Assert.Null(fakeAuthFilterContext.Result); // shouldn't be anything since we are just testing the attribute

                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task NotAuthorized()
        {
            using (UserService userService = new UserService())
            {
                var createdUser = await userService.CreateUser(new CreateUser { DisplayName = "Joe" });
                User fetchUser = await userService.GetUser(createdUser._Id.ToString());

                string authData = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{fetchUser._Id}:{createdUser.Password}12345"));

                var httpContextMock = new Mock<HttpContext>();
                httpContextMock
                  .Setup(a => a.Request.Headers["Authorization"])
                  .Returns($"Basic {authData}");



                ActionContext fakeActionContext =
                new ActionContext(httpContextMock.Object,
                  new Microsoft.AspNetCore.Routing.RouteData(),
                  new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

                AuthorizationFilterContext fakeAuthFilterContext =
                new AuthorizationFilterContext(fakeActionContext,
                  new List<IFilterMetadata> { });

                BasicAuthFilter basicAuthAuthAttribute =
                  new BasicAuthFilter();

                basicAuthAuthAttribute.OnAuthorization(fakeAuthFilterContext);

                Assert.Equal(typeof(UnauthorizedResult), fakeAuthFilterContext.Result.GetType());

                await userService.RemoveUser(createdUser);
            }
        }

        [Fact]
        public async Task NotAuthorizedNoHeader()
        {
            using (UserService userService = new UserService())
            {
                var createdUser = await userService.CreateUser(new CreateUser { DisplayName = "El" });
                User fetchUser = await userService.GetUser(createdUser._Id.ToString());

                string authData = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{fetchUser._Id}:{createdUser.Password}"));

                var httpContextMock = new Mock<HttpContext>();
                //httpContextMock
                //  .Setup(a => a.Request.Headers["Authorization"])
                //  .Returns($"Basic {authData}");



                ActionContext fakeActionContext =
                new ActionContext(httpContextMock.Object,
                  new Microsoft.AspNetCore.Routing.RouteData(),
                  new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

                AuthorizationFilterContext fakeAuthFilterContext =
                new AuthorizationFilterContext(fakeActionContext,
                  new List<IFilterMetadata> { });

                BasicAuthFilter basicAuthAuthAttribute =
                  new BasicAuthFilter();

                basicAuthAuthAttribute.OnAuthorization(fakeAuthFilterContext);


                Assert.Equal(typeof(UnauthorizedResult), fakeAuthFilterContext.Result.GetType());

                await userService.RemoveUser(createdUser);
            }
        }
    }
}
