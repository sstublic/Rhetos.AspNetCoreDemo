using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    public class RhetosAspNetCoreUser : IUserInfo
    {
        public bool IsUserRecognized => !string.IsNullOrEmpty(UserName);
        public string UserName => userNameValueGenerator.Value;
        public string Workstation => "";

        private readonly Lazy<string> userNameValueGenerator;

        public RhetosAspNetCoreUser(IHttpContextAccessor httpContextAccessor)
        {
            userNameValueGenerator = new Lazy<string>(() => GetUserName(httpContextAccessor.HttpContext?.User));
        }

        private string GetUserName(ClaimsPrincipal httpContextUser)
        {
            var userNameFromContext = httpContextUser?.Identity?.Name;
            if (string.IsNullOrEmpty(userNameFromContext))
                throw new InvalidOperationException($"No username found while trying to resolve user from HttpContext.");

            return userNameFromContext;
        }

        public string Report()
        {
            return $"{nameof(RhetosAspNetCoreUser)}(UserName='{UserName}')";
        }
    }
}
