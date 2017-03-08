using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GirafWebApi.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        public class Credentials {
            public string Username {get; set;}
            public string Password {get; set;}
        }

        [HttpPost]
        public async Task <IActionResult> Post([FromBody] Credentials credentials)
        {
            string u = credentials.Username;
            string p = credentials.Password;

            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5001");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "mvc", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(u, p, "api1");
            
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return Unauthorized();
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(JArray.Parse(content));
            }

            return Ok("Logged in");
        }
    }
}