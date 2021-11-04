using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Extensions
{
    public static class LoginExtension
    {
        private static readonly string BASE_URL = "https://localhost:5000/";

        public static async Task<string> GetTokenAsync(CustomWebApplicationFactory factory)
        {
            var client = factory.CreateClient();
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(new LoginDTO("Graatand", "password")),
                                    Encoding.UTF8,
                                    "application/json");
            var response = await client.PostAsync($"{BASE_URL}v2/Account/login", httpContent);
            string data = await response.Content.ReadAsStringAsync();
            JObject content = JObject.Parse(data);
            return content["data"].ToString();
        }
        public static async Task<string> GetGuardianId(CustomWebApplicationFactory factory)
        {
            string token = await GetTokenAsync(factory);
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["data"]["id"].ToString();
        }
    }
}
