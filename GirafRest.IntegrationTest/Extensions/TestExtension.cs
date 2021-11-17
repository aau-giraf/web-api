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
    public static class TestExtension
    {
        private static readonly string BASE_URL = "https://localhost:5000/";

        public static async Task<string> GetTokenAsync(CustomWebApplicationFactory factory, string username, string password)
        {
            var client = factory.CreateClient();
            var data = $"{{'username': '{username}', 'password': '{password}'}}";

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

        public static async Task<string> GetUserIdAsync(CustomWebApplicationFactory factory, string username, string password)
        {
            string token = await GetTokenAsync(factory, username, password);
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

        public static async Task<long> GetDepartmentIdAsync(CustomWebApplicationFactory factory, string name)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/"),
                Method = HttpMethod.Get
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["data"].FirstOrDefault(data => data["name"].ToString() == name)["id"].ToObject<long>();
        }

        public static async Task<string> GetResetTokenAsync(CustomWebApplicationFactory factory, string username, string password, string token, string tokenPassword)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password-reset-token/{await GetUserIdAsync(factory, username, password)}"),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, token, tokenPassword)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["data"].ToString();
        }

        public static async Task<string> RegisterAsync(CustomWebApplicationFactory factory, string username, string password, string displayName, string token, string role = "Citizen", long departmentId = 2)
        {
            var client = factory.CreateClient();
            var data = $"{{'username': '{username}', 'displayname': '{displayName}', 'password': '{password}', 'role': '{role}', 'departmentId': {departmentId}}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, token, password)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            return content["data"].ToString();
        }

        public static async Task DeleteAsync(CustomWebApplicationFactory factory, string username, string password, string token)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/user/{await GetUserIdAsync(factory, username, password)}"),
                Method = HttpMethod.Delete
            };

            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, token, password)}");

            await client.SendAsync(request);
        }

        public static async Task<long> GetPictogramIdAsync(CustomWebApplicationFactory factory, string pictogramTitle, string username, string password)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram?query={pictogramTitle}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, username, password)}");

            var response = await client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["data"].FirstOrDefault(data => data["title"].ToString() == pictogramTitle)["id"].ToObject<long>();
        }

        public static async Task<JToken> CreatePictogramAsync(CustomWebApplicationFactory factory, string pictogram, int accessLevel, string username, string password)
        {
            var client = factory.CreateClient();
            var data = JObject.Parse(pictogram);
            data["accessLevel"] = accessLevel;
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Post,
                Content = new StringContent(data.ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, username, password)}");

            var response = await client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["data"];
        }

        public static async Task DeletePictogramAsync(CustomWebApplicationFactory factory, int id, string username, string password)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{id}"),
                Method = HttpMethod.Delete,
            };
            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, username, password)}");

            var response = await client.SendAsync(request);
        }

        public static async Task<JToken> GetWeekAsync(CustomWebApplicationFactory factory, string username, string password)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await GetUserIdAsync(factory, username, password)}/week"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, username, password)}");

            var response = await client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            return content["data"];
        }

        public static async Task<string> GetWeekTemplateIdAsync(CustomWebApplicationFactory factory, string username, string password, string templateName)
        {
            var client = factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await GetTokenAsync(factory, username, password)}");

            var response = await client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            return content["data"].First(x => x["name"].ToString() == templateName)["templateId"].ToString();
        }
    }
}
