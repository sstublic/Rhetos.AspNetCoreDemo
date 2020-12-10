using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet]
        public string RequestInfo()
        {
            return $"UserName: '{User?.Identity?.Name}'";
        }

        [HttpPost]
        public void SetUserCookie(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                Response.Cookies.Delete(DummyAuthenticationHandler.CookieName);
            else
                Response.Cookies.Append(DummyAuthenticationHandler.CookieName, userName);
        }

        [HttpGet]
        public string GetUserCookie()
        {
            var userName = Request.Cookies[DummyAuthenticationHandler.CookieName];
            return $"UserName cookie value: '{userName}'";
        }
    }
}
