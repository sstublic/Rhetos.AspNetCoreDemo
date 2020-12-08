using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Rhetos.Processing;

namespace Rhetos.Extensions.AspNetCore
{
    internal interface IRhetosScopeServiceProvider : IDisposable
    {
        ILifetimeScope RequestLifetimeScope { get; }
    }
}
