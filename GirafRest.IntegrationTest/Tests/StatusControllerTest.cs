using GirafRest.IntegrationTest.Extensions;
using GirafRest.IntegrationTest.Order;
using GirafRest.IntegrationTest.Setup;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GirafRest.IntegrationTest.Tests
{
    [TestCaseOrderer("GirafRest.IntegrationTest.Order.PriorityOrderer", "GirafRest.IntegrationTest")]
    [Collection("Intgration test")]
    public class StatusControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<AccountFixture>
    {
        private const string BASE_URL = "https://localhost:5000/";
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private readonly AccountFixture _accountFixture;
        public StatusControllerTest(CustomWebApplicationFactory factory, AccountFixture accountFixture)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _accountFixture = accountFixture;
        }


        /// <summary>
        /// Testing GET for status on web-api
        /// Endpoint: GET:/v1/Status
        /// </summary>
        [Fact, Priority(0)]
        public async void TestStatusIsOnline()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/status/"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing GET for status on web-api connection to database
        /// Endpoint: GET:/v1/Status/database
        /// </summary>
        [Fact, Priority(1)]
        public async void TestStatusWebapiDatabaseConnectionIsOnline()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/status/database"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing GET for status on web-api
        /// Endpoint: GET:/v1/Status/version-info
        /// </summary>
        [Fact, Priority(2)]
        public async void TestStatusVersioninfoIsOnline()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/status/version-info"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");
            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }
    }
}
