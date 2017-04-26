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
using GirafRest.Controllers;
using Serilog;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace GirafRest.Setup
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            if (env.IsDevelopment())
            {
                builder.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            } else {
                builder.AddJsonFile(Program.ConnectionStringName, optional: false, reloadOnChange: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add the database context to the server using extension-methods
            switch (Program.DbOption) {
                case DbOption.SQLite:
                    services.AddSqlite();
                    break;
                case DbOption.MySQL:
                    services.AddMySql(Configuration);
                    break;
                default:
                    services.AddSqlite();
                    break;
            }

            //Add Identity for user management.
            services.AddIdentity<GirafUser, IdentityRole>(options => {
                options.RemovePasswordRequirements();
                options.StopRedirectOnUnauthorized();
            })
                .AddEntityFrameworkStores<GirafDbContext>()
                .AddDefaultTokenProviders();
                
            // Add email sender for account recorvery.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            
            services.AddTransient<IGirafService, GirafController>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            GirafDbContext context,
            UserManager<GirafUser> userManager,
            IApplicationLifetime appLifetime)
        {
            loggerFactory
                .AddConsole()
                .AddDebug();
            if (Program.LogToFile)
            {
                loggerFactory.AddFile(Path.Combine(Program.LogDirectory, Program.LogFilepath), LogLevel.Warning);
                app.UseStaticFiles(); // For the wwwroot folder

                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), Program.LogDirectory);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(
                        directoryPath),
                    RequestPath = new PathString("/logs")
                });
            }
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

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
            if(Program.GenerateSampleData) DBInitializer.Initialize(context, userManager);
        }
    }
}
