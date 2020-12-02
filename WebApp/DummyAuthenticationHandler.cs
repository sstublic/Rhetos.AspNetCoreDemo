using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace WebApp
{
    public class DummyIdentity : IIdentity
    {
        public string AuthenticationType { get; }
        public bool IsAuthenticated { get; }
        public string Name { get; }

        public DummyIdentity(string name, string type)
        {
            Name = name;
            IsAuthenticated = true;
            AuthenticationType = type;
        }
    }

    public class DummyAuthenticationHandler : IAuthenticationHandler
    {
        private HttpContext context;
        public const string Scheme = "DummyAuthenticationScheme";
        public const string CookieName = "RhetosUserName";

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            this.context = context;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            var userName = context.Request.Cookies[CookieName];

            if (string.IsNullOrEmpty(userName))
                return Task.FromResult(AuthenticateResult.Fail("No username cookie."));

            var principal = new ClaimsPrincipal(new DummyIdentity(userName, Scheme));
            var result = AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme));
            return Task.FromResult(result);
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            throw new NotImplementedException("Forbid");
        }
    }
}
