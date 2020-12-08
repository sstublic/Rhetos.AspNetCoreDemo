using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Rhetos.Extensions.AspNetCore
{
    internal class RhetosRootServiceProvider
    {
        public IContainer Container { get; }

        public RhetosRootServiceProvider(IContainer container)
        {
            Container = container;
        }
    }
}
