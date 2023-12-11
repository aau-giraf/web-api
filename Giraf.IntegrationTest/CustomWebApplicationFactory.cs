using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;
using GirafAPI.Setup;

namespace Giraf.IntegrationTest
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
