using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    public class RhetosAspNetCoreUser : IUserInfo
    {
        public bool IsUserRecognized => true;
        public string UserName { get; }
        public string Workstation => "";

        public RhetosAspNetCoreUser(string userName)
        {
            UserName = userName;
        }

        public string Report()
        {
            return $"{nameof(RhetosAspNetCoreUser)}(UserName='{UserName}')";
        }
    }
}
