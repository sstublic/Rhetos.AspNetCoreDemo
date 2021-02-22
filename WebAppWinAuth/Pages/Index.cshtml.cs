using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;
using Rhetos.Utilities;

namespace WebAppWinAuth.Pages
{
    public class IndexModel : PageModel
    {
        public IUserInfo UserInfo { get; set; }

        public IndexModel(IUserInfo userInfo)
        {
            UserInfo = userInfo;
        }

        public void OnGet()
        {
        }
    }
}
