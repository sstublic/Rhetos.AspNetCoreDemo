using System;
using Autofac;
using Microsoft.AspNetCore.Http;
using Rhetos.Processing;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    internal class RhetosScopeServiceProvider : TransactionScopeContainer, IDisposable
    {
        public RhetosScopeServiceProvider(RhetosRootServiceProvider containerRoot, IHttpContextAccessor httpContextAccessor)
        : base(containerRoot.Container, builder => RegisterUserInstance(builder, httpContextAccessor))
        { }

        private static void RegisterUserInstance(ContainerBuilder builder, IHttpContextAccessor httpContextAccessor)
        {
            var userName = httpContextAccessor.HttpContext.User.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
                builder.RegisterInstance(new RhetosAspNetCoreUser(userName)).As<IUserInfo>();
        }

        public new void Dispose()
        {
            CommitChanges();

            Dispose(true);
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
