using Xunit;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System;
using Newtonsoft.Json.Linq;
using GirafRest.IntegrationTest.Extensions;
using GirafRest.IntegrationTest.Setup;
using GirafRest.IntegrationTest.Order;

namespace GirafRest.IntegrationTest.Tests
{
    [TestCaseOrderer("GirafRest.IntegrationTest.Order.PriorityOrderer", "GirafRest.IntegrationTest")]
    [Collection("Account Controller")]
    public class AccountControllerTest
    : IClassFixture<CustomWebApplicationFactory>, IClassFixture<AccountMock>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly AccountMock _accountMock;
        private const string BASE_URL = "https://localhost:5000/";
        private readonly HttpClient client;
        public AccountControllerTest(CustomWebApplicationFactory factory, AccountMock accountMock)
        {
            _factory = factory;
            client = _factory.CreateClient();
            _accountMock = accountMock;
        }

        /// <summary>
        ///Testing logging in as Guardian
        ///Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(0)]
        public async void TestAccountCanLoginAsGuardian()
        {
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(new LoginDTO("Graatand", "password")),
                                    Encoding.UTF8,
                                    "application/json");
            var response = await client.PostAsync($"{BASE_URL}v2/Account/login", httpContent);
            string data = await response.Content.ReadAsStringAsync();
            JObject content = JObject.Parse(data);
            string token = content["data"].ToString();

            response.EnsureSuccessStatusCode();
            Assert.NotEmpty(data);
        }

        /// <summary>
        /// Testing getting Guardian's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(1)]
        public async void TestAccountCanGetGuardianId()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {await LoginExtension.GetTokenAsync(_factory)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            Assert.NotEmpty(content["data"].ToString());
            Assert.NotEmpty(content["data"]["id"].ToString());
        }

        /// <summary>
        /// Testing registering a citizen fails without displayName
        /// Endpoint: POST:/v2/Account/register
        /// </summary>
        [Fact, Priority(2)]
        public async void TestAccountCannotRegisterCitizenWithoutDisplayName()
        {
            var data = "{'username': 'myUsername', 'password': 'password', 'role': 'Citizen', 'departmentId': 1}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Authorization", $"Bearer {await LoginExtension.GetTokenAsync(_factory)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("MissingProperties", content["errorKey"]);
        }

        /// <summary>
        /// Testing registering a citizen fails with empty displayName
        /// Endpoint: POST:/v2/Account/register
        /// </summary>
        [Fact, Priority(3)]
        public async void TestAccountCannotRegisterCitizenWithEmptyDisplayName()
        {
            var data = "{'username': 'myUsername', 'displayName': '', 'password': 'password'," +
                "'role': 'Citizen', 'departmentId': 1}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Authorization", $"Bearer {await LoginExtension.GetTokenAsync(_factory)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("MissingProperties", content["errorKey"]);
        }

        /// <summary>
        /// Testing registering Citizen2
        /// Endpoint: POST:/v2/Account/register
        /// </summary>
        [Fact, Priority(4)]
        public async void TestAccountCanRegisterCitizen2()
        {
            var data = $"{{'username': '{_accountMock.Citizen2Username}', 'displayName': '{_accountMock.Citizen2Username}', 'password': 'password', 'role': 'Citizen', 'departmentId': 2}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };

            request.Headers.Add("Authorization", $"Bearer {await LoginExtension.GetTokenAsync(_factory)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.NotEmpty(content["data"]);
        }

        /// <summary>
        /// Testing logging in as Citizen2
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(5)]
        public async void TestAccountCanLoginAsCitizen2()
        {
            var data = $"{{'username': '{_accountMock.Citizen2Username}', 'password': 'password'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };

            request.Headers.Add("Authorization", $"Bearer {await LoginExtension.GetTokenAsync(_factory)}");

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            Assert.NotEmpty(content["data"].ToString());
        }
    }
}
