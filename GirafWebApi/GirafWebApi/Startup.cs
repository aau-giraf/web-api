using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GirafWebApi.Contexts;
using GirafWebApi.Models;
using IdentityServer4.Validation;
using IdentityServer4.Services;

namespace GirafWebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {             
            services.AddIdentity<GirafUser, IdentityRole>()
            .AddEntityFrameworkStores<GirafDbContext>()
            .AddDefaultTokenProviders();

            // Add framework services.
            services.AddMvc();

            //var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb;Trusted_Connection=True;";
            
            services.AddEntityFrameworkSqlite()
                .AddDbContext<GirafDbContext>();
            // configure identity server with in-memory stores, keys, clients and resources
           
            services.AddIdentityServer()
                .AddInMemoryApiResources(Configurations.ApiResources.GetApiResources())
                .AddInMemoryClients(Configurations.Clients.GetClients())
                .AddTemporarySigningCredential(); // Skal måske ændres?

            services.AddTransient<IResourceOwnerPasswordValidator, Configurations.ResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, Configurations.ProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, GirafDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.UseIdentity();
            app.UseIdentityServer();
            
            DBInitializer.Initialize(context);
            app.UseMvc();
            
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5001",
                RequireHttpsMetadata = false,
                ApiName = "MyApi",
                AllowedScopes = { "MyApi"}
            });

            app.UseMvc();
        }
    }
}
