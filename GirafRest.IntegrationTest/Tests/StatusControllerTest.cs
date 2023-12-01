using GirafRest.IntegrationTest.Order;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GirafRest.IntegrationTest.Extensions;
using GirafRest.IntegrationTest.Setup;
using Xunit;

namespace GirafRest.IntegrationTest.Tests
{
    [TestCaseOrderer("GirafRest.IntegrationTest.Order.PriorityOrderer", "GirafRest.IntegrationTest")]
    [Collection("Intgration test")]
    public class StatusControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<StatusFixture>
    {
        private const string BASE_URL = "https://localhost:5000/";
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private StatusFixture _statusFixture;
        public StatusControllerTest(CustomWebApplicationFactory factory, StatusFixture statusFixture)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _statusFixture = statusFixture;
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
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _statusFixture.Username, _statusFixture.Password)}");
                
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
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _statusFixture.Username, _statusFixture.Password)}");

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
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, "guardian-dev", "password")}");

            var response = await _client.SendAsync(request);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    //Checking it actually returns something if the status code is OK
                    var content = JObject.Parse(await response.Content?.ReadAsStringAsync());
                    Assert.NotNull(content["data"]);
                    break;
                case HttpStatusCode.InternalServerError:
                    Assert.True(false);
                    break;
                    // If not OK and not an InternalServerError it should not return anything
            }
        }
    }
}
