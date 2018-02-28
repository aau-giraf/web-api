using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using System;
using Swashbuckle.AspNetCore.Swagger;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using GirafRest.Models.Responses;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Setup
{
    /// <summary>
    /// The Startup class, that defines how the server should behave. In this class you may add services to
    /// the server, that serves different purposes on the server. All parameters to the methods in this class
    /// is delivered by the ASP.NET runtime via dependency injection.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Creates a new Startup-instance, which is used to configure the server.
        /// Startup is automatically instantiated by the ASP.NET runtime.
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;
            //var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath);
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath);
            // delete all default configuration providers
            if (env.IsDevelopment())
                builder.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            else if (env.IsProduction())
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            else
                throw new NotSupportedException("No database option is supported by this Environment mode");
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IHostingEnvironment HostingEnvironment { get; }
        /// <summary>
        /// The configuration, contains information regarding connecting to the database
        /// </summary>
        private IConfigurationRoot Configuration { get; }
        /// <summary>
        /// Contains information regarding the environment the application is hosted in
        /// </summary>
        private IHostingEnvironment Environment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the application.
        /// A service is some class instance that may be used by all classes of the application.
        /// </summary>
        /// <param name="services">A collection of all services in the application</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //Add the database context to the server using extension-methods
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddSqlite();
                configureIdentity<GirafSqliteDbContext>(services);
            }
            else if (HostingEnvironment.IsProduction())
            {
                services.AddMySql(Configuration);
                configureIdentity<GirafMySqlDbContext>(services);
            }

            // Add email sender for account recorvery.
            services.Configure<EmailConfig>(Configuration.GetSection("Email"));
            services.AddTransient<IEmailService, MessageServices>();

            // Add the implementation of IGirafService to the context, i.e. all common functionality for
            // the controllers.
            services.AddTransient<IGirafService, GirafService>();
            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "GirafRest.xml");
                c.IncludeXmlComments(xmlPath);

            });

            services.ConfigurePolicies();
        }
        /// <summary>
        /// Configures the GirafUser Identity, changing what is needed by it, and how it should act
        /// </summary>
        /// <param name="services">A collection of all services in the application</param>
        private void configureIdentity<T>(IServiceCollection services)
            where T : GirafDbContext
        {
            //Add Identity for user management.
            services.AddIdentity<GirafUser, GirafRole>(options => {
                options.RemovePasswordRequirements();
            })
                .AddEntityFrameworkStores<T>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ErrorResponse(ErrorCode.Forbidden))));
                    return Task.FromResult(0);
                };
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ErrorResponse(ErrorCode.NotAuthorized))));
                    return Task.FromResult(0);
                };
            });
        }

        //https://stackoverflow.com/questions/42030137/suppress-redirect-on-api-urls-in-asp-net-core
        static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
    context => {
            context.Response.StatusCode = (int)statusCode;
            return Task.CompletedTask;
    };

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">An application builder, used to configure the request pipeline.</param>
        /// <param name="env">A reference to an implementation of IHostingEnvironment, that stores information
        /// on how the server should be hosted.</param>
        /// <param name="loggerFactory">A logger factory, in this context used to configure how the loggers
        /// should behave.</param>
        /// <param name="roleManager">A reference to the roleManager, used here to ensure that roles are setup</param>
        /// <param name="userManager">A reference to the usermanager, in this case used to create sample users.</param>
        /// <param name="appLifetime">A reference to an implementation of IApplicationLifetime, that has a set of events,
        /// that signal when the application starts, end and so fourth.</param>
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            UserManager<GirafUser> userManager,
            RoleManager<GirafRole> roleManager,
            IApplicationLifetime appLifetime)
        {
            //Configure logging for the application
            app.ConfigureLogging(loggerFactory);
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
            
            //Tells ASP.NET to generate an HTML exception page, if an exception occurs
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "¨Giraf REST API V1");
            });

            //Configures Identity, i.e. user management
            app.UseAuthentication();

            //Overrides the default behaviour on unauthorized to simply return Unauthorized when accessing an
            //[Authorize] endpoint without logging in.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=AccessDenied}");
            });

            GirafDbContext context;
            if (env.IsDevelopment())
                context = app.ApplicationServices.GetService<GirafSqliteDbContext>();
            
            else
                context = app.ApplicationServices.GetService<GirafMySqlDbContext>();

            // Create database + schemas if they do not exist
            context.Database.Migrate();

            // Create roles if they do not exist
            try
            {
                roleManager.EnsureRoleSetup();
            }
            catch { }

            //Fill some sample data into the database
            if (ProgramOptions.GenerateSampleData)
            {
                DBInitializer.Initialize(context, userManager);
            }
        }
    }
}

