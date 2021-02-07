using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet]
        public string RequestInfo()
        {
            return $"UserName: '{User?.Identity?.Name}'";
        }

        [HttpPost]
        public async Task Login(string userName)
        {
            var claimsIdentity = new ClaimsIdentity(new []{ new Claim(ClaimTypes.Name, userName) }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties() {IsPersistent = true});
        }

        [HttpGet]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
