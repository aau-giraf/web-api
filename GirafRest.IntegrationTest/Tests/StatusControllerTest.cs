using GirafRest.IntegrationTest.Order;
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
    [Collection("Account Controller")]
    public class StatusControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private const string BASE_URL = "https://localhost:5000/";
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        public StatusControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
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
                RequestUri = new Uri($"{BASE_URL}v1/status"),
                Method = HttpMethod.Get,
            };

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

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }
    }
}
