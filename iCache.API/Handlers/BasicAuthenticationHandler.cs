using System;
using System.Threading.Tasks;
using iCache.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;

namespace iCache.API.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                bool isAuthenticated = false;
                bool isAdmin = false;
                string username;

                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                username = credentials[0];
                string password = credentials[1];

                if (await _userService.Authenticate(username, password))
                {
                    isAuthenticated = true;
                }

                if (ValidAdminCredentials(username, password))
                {
                    isAuthenticated = true;
                    isAdmin = true;
                }

                if (!isAuthenticated)
                    return AuthenticateResult.Fail("Invalid Username or Password");

                List<Claim> claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username)
                };

                if (isAdmin)
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }

        private bool ValidAdminCredentials(string username, string password)
        {
            if (!string.IsNullOrEmpty(Configuration.AdminUserClient) && !string.IsNullOrEmpty(Configuration.AdminPassword))
                if (Configuration.AdminUserClient == username && Configuration.AdminPassword == password)
                    return true;

            return false;
        }
    }
}
