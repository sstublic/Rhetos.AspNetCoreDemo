using System;
using Autofac;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    internal class RhetosScopeServiceProvider : IDisposable
    {
        private readonly TransactionScopeContainer transactionScopeContainer;

        public RhetosScopeServiceProvider(RhetosHost rhetosHost, IUserInfo rhetosUser)
        {
            transactionScopeContainer = rhetosHost.CreateScope(builder => builder.RegisterInstance(rhetosUser));
        }

        public T Resolve<T>()
        {
            return transactionScopeContainer.Resolve<T>();
        }

        public void Dispose()
        {
            transactionScopeContainer.CommitChanges();
            transactionScopeContainer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
