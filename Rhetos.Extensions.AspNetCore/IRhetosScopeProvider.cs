using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Rhetos.Processing;

namespace Rhetos.Extensions.AspNetCore
{
    public interface IRhetosScopeProvider : IDisposable
    {
        ILifetimeScope RequestLifetimeScope { get; }
        IProcessingEngine RequestProcessingEngine { get; }
    }
}
