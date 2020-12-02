using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Rhetos.Extensions.AspNetCore
{
    public class RhetosContainerRoot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IContainer Container { get; }

        public RhetosContainerRoot(IContainer container)
        {
            Container = container;
        }
    }
}
