using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
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
using GirafRest.Setup;

namespace GirafRest.IntegrationTest
{
    public class CustomWebApplicationFactory
    : WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = false;
                })
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseStartup<Startup>().UseTestServer();
                });
            return builder;
        }
    }
}
