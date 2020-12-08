using System;
using Autofac;
using Microsoft.AspNetCore.Http;
using Rhetos.Processing;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    internal class RhetosScopeServiceProvider : IRhetosScopeServiceProvider
    {
        public ILifetimeScope RequestLifetimeScope => lifetimeScope.Value;

        private readonly RhetosRootServiceProvider containerRoot;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Lazy<ILifetimeScope> lifetimeScope;

        public RhetosScopeServiceProvider(RhetosRootServiceProvider containerRoot, IHttpContextAccessor httpContextAccessor)
        {
            this.containerRoot = containerRoot;
            this.httpContextAccessor = httpContextAccessor;
            lifetimeScope = new Lazy<ILifetimeScope>(CreateLifetimeScope);
        }

        private ILifetimeScope CreateLifetimeScope()
        {
            var scope = containerRoot.Container.BeginLifetimeScope(builder =>
            {
                var userName = httpContextAccessor.HttpContext.User.Identity?.Name;
                if (!string.IsNullOrEmpty(userName))
                    builder.RegisterInstance(new RhetosAspNetCoreUser(userName)).As<IUserInfo>();
            });

            return scope;
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (lifetimeScope.IsValueCreated)
                    lifetimeScope.Value.Dispose();
            }

            disposed = true;
        }
    }
}
