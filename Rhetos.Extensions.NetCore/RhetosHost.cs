using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Rhetos.Extensions.NetCore
{
    public class RhetosHost
    {
        public IContainer Container { get; }

        public RhetosHost(IContainer container)
        {
            this.Container = container;
        }

        public TransactionScopeContainer CreateScope()
        {
            return new TransactionScopeContainer(Container);
        }
    }
}
