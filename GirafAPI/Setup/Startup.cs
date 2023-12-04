﻿using AspNetCoreRateLimit;
using GirafEntities.Authentication;
using GirafEntities.User;
using GirafRepositories;
using GirafRepositories.Persistence;
using GirafRepositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GirafServices;
using GirafServices.User;
using GirafServices.WeekPlanner;
using Microsoft.EntityFrameworkCore;

namespace GirafAPI.Setup
{
    /// <summary>
    /// The Startup class, that defines how the server should behave. In this class you may add services to
    /// the server, that serves different purposes on the server. All parameters to the methods in this class
    /// is delivered by the ASP.NET runtime via dependency injection.
    /// </summary>
    public class Startup
    {
        
        /// <summary>
        /// Startup Application, and set appsettings
        /// </summary>
        /// <param name="env">Hosting environment to start up into</param>
        public Startup(IWebHostEnvironment env)
        {
            HostingEnvironment = env;
            var coreEnvironement = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (coreEnvironement != null) env.EnvironmentName = coreEnvironement;
            else env.EnvironmentName = "Development";

            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath);
            if (env.IsDevelopment())
                builder.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            else if (env.IsStaging())
                builder.AddJsonFile("appsettings.Staging.json", optional: false, reloadOnChange: false);
            else if (env.IsProduction())
                builder.AddJsonFile("appsettings.Production.json", optional: false, reloadOnChange: true);
            else if (env.IsLocalDocker())
                builder.AddJsonFile("appsettings.LocalDocker.json", optional: false, reloadOnChange: true);
            else if (env.IsCI())
                builder.AddJsonFile("appsettings.CI.json", optional: false, reloadOnChange: true);
            else
                throw new NotSupportedException("No database option is supported by this Environment mode");
            builder.AddEnvironmentVariables();
            try
            {
                Configuration = builder.Build();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: Missing appsettings file");
                Console.WriteLine("Exiting...");
                System.Environment.Exit(1);
            }
        }


        /// <summary>
        /// Hosting Environment to be initialized with
        /// </summary>
        public IWebHostEnvironment HostingEnvironment { get; }

        /// <summary>
        /// The configuration, contains information regarding connecting to the database
        /// </summary>
        private IConfigurationRoot Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the application.
        /// A service is some class instance that may be used by all classes of the application.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            #region RateLimit
            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            services.Configure<IdentityOptions>(options => {
               // User settings.
               options.User.AllowedUserNameCharacters =
               "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+æøåÆØÅ";
            }); 

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion
            
            services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));

            //Add the database context to the server using extension-methods
            configureIdentity<GirafDbContext>(services);
            
            // Registering services and repositories from the individual projects
            // If new services or repositories needs to be created and registered
            // register them in the individual extension methods in ServiceExtension/RepositoryExtension.
            services.AddRepositories(Configuration); 
            services.AddServices();
            
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddControllers().AddNewtonsoftJson();
            
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
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "The Giraf REST API", 
                    Version = "v1" 
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
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
            services.AddIdentity<GirafUser, GirafRole>(options =>
            {
                // Removes the default password requirements from ASP.NET and set them to a bare minimum.
                //Set password requirements to an absolute bare minimum.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
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
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            UserManager<GirafUser> userManager,
            RoleManager<GirafRole> roleManager,
            IHostApplicationLifetime appLifetime)
        {
            app.UseIpRateLimiting();

            app.UseStatusCodePagesWithReExecute("/v1/Error", "?statusCode={0}");
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
                c.SwaggerEndpoint("v1/swagger.json", "Giraf REST API V1");
                c.DocExpansion(DocExpansion.None);
            });

            //Configures Identity, i.e. user management
            app.UseAuthentication();
            app.UseAuthorization();

            //Overrides the default behaviour on unauthorized to simply return Unauthorized when accessing an
            //[Authorize] endpoint without logging in.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=AccessDenied}");
            });
            GirafDbContext context = app.ApplicationServices.GetService<GirafDbContext>();
            //Only migrate in development or controlled environments.
            //Can be problematic in production
            if (env.IsDevelopment() || env.IsLocalDocker())
            {
                context.Database.Migrate();
            }

            // Create roles if they do not exist
            roleManager.EnsureRoleSetup().Wait();
            
            // Fill some sample data into the database
            if (ProgramOptions.GenerateSampleData)
            {
                var userService = app.ApplicationServices.GetService<IUserService>();
                var dbRepo = app.ApplicationServices.GetService<IDatabaseRepository>();
                var userRepo = app.ApplicationServices.GetService<IGirafUserRepository>();
                var weebbaseService = app.ApplicationServices.GetService<IWeekBaseService>();
                var dbinitializer = new DBInitializer(userService, dbRepo, userRepo, weebbaseService);
                
                dbinitializer.Initialize(userManager, ProgramOptions.Pictograms, env.EnvironmentName).Wait();
            }
            
            app.Run((context2) =>
            {
                context2.Response.StatusCode = 404;
                return Task.FromResult(0);
            });
        }
    }
}
