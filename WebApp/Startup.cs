using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using Rhetos;
using Rhetos.Processing;
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp", Version = "v1" });
            });

            // Adding Rhetos to AspNetCore application
            services.AddRhetos(rhetos =>
                {
                    rhetos.ConfigureConfiguration(cfg => cfg.MapNetCoreConfiguration(Configuration));
                })
                .UseAspNetCoreIdentityUser()
                .ExposeRhetosComponent<IProcessingEngine>();
            // Done adding Rhetos

            services.AddAuthentication(o => o.AddScheme(DummyAuthenticationHandler.Scheme, b =>
            {
                b.HandlerType = typeof(DummyAuthenticationHandler);
            }));

            services.AddAuthorization(a =>
            {
                a.FallbackPolicy = new AuthorizationPolicyBuilder(DummyAuthenticationHandler.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
