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
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

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
            var coreEnvironement = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (coreEnvironement != null) env.EnvironmentName = coreEnvironement;
            else env.EnvironmentName = "Development";

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
            try {
                Configuration = builder.Build();
            } catch(FileNotFoundException ex) {
                Console.WriteLine("ERROR: Missing appsettings file");
                Console.WriteLine("Exiting...");
                System.Environment.Exit(1);
            }
        }

        public IHostingEnvironment HostingEnvironment { get; }
        /// <summary>
        /// The configuration, contains information regarding connecting to the database
        /// </summary>
        private IConfigurationRoot Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the application.
        /// A service is some class instance that may be used by all classes of the application.
        /// </summary>
        /// <param name="services">A collection of all services in the application</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));

            //Add the database context to the server using extension-methods
            services.AddMySql(Configuration);
            configureIdentity<GirafDbContext>(services);

            // Add email sender for account recorvery.
            services.Configure<EmailConfig>(Configuration.GetSection("Email"));
            services.AddTransient<IEmailService, MessageServices>();

            // Add the implementation of IGirafService to the context, i.e. all common functionality for
            // the controllers.
            services.AddTransient<IGirafService, GirafService>();
            services.AddMvc();

            // Set up Cross-Origin Requests
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            }));

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            { 
                c.SwaggerDoc("v1", new Info { Title = "The Giraf REST API", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "GirafRest.xml");
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });
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

            // Add Jwt Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["Jwt:JwtIssuer"],
                        ValidAudience = Configuration["Jwt:JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
        }

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
            //app.UsePathBase("/v1");
            //Configure logging for the application
            app.ConfigureLogging(loggerFactory);
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.UseStatusCodePagesWithReExecute("/v1/Error", "?status={0}");

            // Enable Cors, see configuration in ConfigureServices
            app.UseCors("AllowAll");
            
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Giraf REST API V1");
                c.DocExpansion(DocExpansion.None);
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
            context = app.ApplicationServices.GetService<GirafDbContext>();

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

            app.Run(context2 =>
            {
              context2.Response.StatusCode = 404;
              return Task.FromResult(0);
            });
        }
    }
}

