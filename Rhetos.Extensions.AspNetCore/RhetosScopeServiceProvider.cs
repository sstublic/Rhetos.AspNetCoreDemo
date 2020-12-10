using System;
using Autofac;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    internal class RhetosScopeServiceProvider : TransactionScopeContainer, IDisposable
    {
        public RhetosScopeServiceProvider(RhetosRootServiceProvider containerRoot, IUserInfo rhetosUser)
        : base(containerRoot.Container, builder => builder.RegisterInstance(rhetosUser))
        { }

        public new void Dispose()
        {
            CommitChanges();

            Dispose(true);
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
