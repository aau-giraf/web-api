using GirafRest.IntegrationTest.Setup;
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
    public static class AccountExtension
    {
        private static readonly string BASE_URL = "https://localhost:5000/";

        public static async Task<string> GetTokenAsync(CustomWebApplicationFactory factory, string username)
        {
            var client = factory.CreateClient();
            var data = $"{{'username': '{username}', 'password': 'password'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["data"].ToString();
        }

        public static async Task<string> GetIdAsync(CustomWebApplicationFactory factory, string username)
        {
            string token = await GetTokenAsync(factory, username);
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

        public static async Task<string> GetResetTokenAsync(CustomWebApplicationFactory factory, string username, string token)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password-reset-token/{await GetIdAsync(factory, username)}"),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, token)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["data"].ToString();
        }

        public static async Task<string> RegisterAsync(CustomWebApplicationFactory factory, string username, string token)
        {
            var client = factory.CreateClient();
            var data = $"{{'username': '{username}', 'displayname': '{username}', 'password': 'password', 'role': 'Citizen', 'departmentId': 2}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, token)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            return content["data"].ToString();
        }

        public static async Task<string> LoginAsync(CustomWebApplicationFactory factory, string username)
        {
            var client = factory.CreateClient();
            var data = $"{{'username': '{username}', 'password': 'password'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            return content["data"].ToString();
        }

        public static async Task DeleteAsync(CustomWebApplicationFactory factory, string username, string token)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/user/{await AccountExtension.GetIdAsync(factory, username)}"),
                Method = HttpMethod.Delete
            };

            request.Headers.Add("Authorization", $"Bearer {await AccountExtension.GetTokenAsync(factory, token)}");

            await client.SendAsync(request);
        }
    }
}
