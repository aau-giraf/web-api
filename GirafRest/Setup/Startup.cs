using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Services;
using GirafRest.Extensions;
using Microsoft.AspNetCore.Identity;

namespace GirafRest.Setup
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add the database context to the server using extension-methods
            if(Program.DbOption == DbOption.SQLite) services.AddSqlite();
            else services.AddSql();   

            services.AddIdentity<GirafUser, IdentityRole>(options => {
                options.RemovePasswordRequirements();
            })
                .AddEntityFrameworkStores<GirafDbContext>()
                .AddDefaultTokenProviders();

            //Really ugly way to avoid redirecting.
            services.Configure<IdentityOptions>(options =>
            {
                options.StopRedirectOnUnauthorized();
            });

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            GirafDbContext context, UserManager<GirafUser> userManager)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentity();
            app.UseStaticFiles();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=AccessDenied}");
            });

            //Fill some sample data into the database
            if(Program.DbOption == DbOption.SQLite) DBInitializer.Initialize(context, userManager);
        }
    }
}
