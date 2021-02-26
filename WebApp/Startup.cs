using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Rhetos;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Extensions.NetCore.Logging;
using Rhetos.Extensions.RestApi.Utilities;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null); // unnecessary if using newtonsoft.json and configuring the same option

            // showcases using NewtonsoftJson and using legacy Microsoft Date serialization
            // also shows how to force byte[] serialization to serialize as JSon arrays instead of Base64 string
            services.AddControllers()
                .AddNewtonsoftJson(o =>
                {
                    o.UseMemberCasing();
                    o.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                    o.SerializerSettings.Converters.Add(new ByteArrayConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp", Version = "v1" });
                c.SwaggerDoc("rhetos", new OpenApiInfo { Title = "Rhetos Rest Api", Version = "v1" });
            });

            // Adding Rhetos to AspNetCore application
            services.AddRhetos(rhetosHostBuilder => ConfigureRhetosHostBuilder(rhetosHostBuilder, Configuration))
                .UseAspNetCoreIdentityUser()
                .AddImpersonation()
                .AddRestApi(o =>
                {
                    o.BaseRoute = "RhetosRestApiTest";
                    o.ConceptInfoRestMetadataProviders.Add(new RhetosExtendedControllerMetadataProvider());
                    o.GroupNameMapper = (conceptInfo, name) => "rhetos";
                });
            // Done adding Rhetos

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                });

            services.AddAuthorization(a =>
            {
                a.FallbackPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/rhetos/swagger.json", "Rhetos Rest Api");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This is extracted to separate public static method so it can be used BOTH from Startup class
        // and any other code that wishes to recreate RhetosHost specific for this web application
        // Common use is to call this from Program.CreateRhetosHostBuilder method which is by convention consumed by
        // Rhetos tools.
        public static void ConfigureRhetosHostBuilder(IRhetosHostBuilder rhetosHostBuilder, IConfiguration configuration)
        {
            rhetosHostBuilder
                .ConfigureRhetosHostDefaults()
                .UseBuilderLogProvider(new RhetosBuilderDefaultLogProvider()) // delegate RhetosHost logging to several NetCore targets
                .ConfigureConfiguration(cfg => cfg.MapNetCoreConfiguration(configuration));
        }
    }
}
