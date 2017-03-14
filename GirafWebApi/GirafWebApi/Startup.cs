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
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GirafWebApi
{
    public class Startup
    {
        GirafDbContext context;

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

        public virtual void ConfigureServices(IServiceCollection services)
        {             
            services.AddIdentity<GirafUser, IdentityRole>(options => {
                options.Cookies.ApplicationCookie.LoginPath = new PathString("/api/values/");
            })
                .AddEntityFrameworkStores<GirafDbContext>()
                .AddDefaultTokenProviders();

            // Add framework services.
            services.AddMvc();
            //var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb;Trusted_Connection=True;";
            
            // configure identity server with in-memory stores, keys, clients and resources
           
            services.AddIdentityServer()
                .AddInMemoryApiResources(Configurations.ApiResources.GetApiResources())
                .AddInMemoryClients(Configurations.Clients.GetClients())
                .AddTemporarySigningCredential(); // Skal måske ændres?

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin",
                    policy => policy.RequireRole(context.Roles.Where(x => x.Name == "Admin").First().Name));
            });

            services.AddTransient<IResourceOwnerPasswordValidator, Configurations.ResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, Configurations.ProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, GirafDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            this.context = context;
            DBInitializer.Initialize(context);
            
            app.UseIdentity();
            app.UseIdentityServer();
            
            app.UseJwtBearerAuthentication();

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
