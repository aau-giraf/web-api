using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using GirafRest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace GirafRest.IntegrationTest
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        /// <summary>
        /// The configuration, contains information regarding connecting to the database
        /// </summary>
        private IConfigurationRoot Configuration { get; }

        UserManager<GirafUser> UserManager;
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(x =>
                              {
                                  x.UseStartup<Setup.Startup>().UseTestServer();
                              });
            return builder;
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
    }
}
