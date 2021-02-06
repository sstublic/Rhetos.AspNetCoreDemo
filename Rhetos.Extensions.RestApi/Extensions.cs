using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rhetos.Dsl;
using Rhetos.Extensions.AspNetCore;

namespace Rhetos.Extensions.RestApi
{
    public static class Extensions
    {
        public static RhetosAspNetServiceCollectionBuilder AddRestApi(this RhetosAspNetServiceCollectionBuilder builder)
        {
            builder.ExposeRhetosComponent<IDslModel>(); // TODO remove
            builder.Services.AddSingleton<DslModelRestAspect>();
            return builder;
        }
    }
}
