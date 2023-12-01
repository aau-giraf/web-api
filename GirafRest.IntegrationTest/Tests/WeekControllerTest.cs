using GirafRest.IntegrationTest.Extensions;
using GirafRest.IntegrationTest.Order;
using GirafRest.IntegrationTest.Setup;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GirafRest.IntegrationTest.Tests
{
    [TestCaseOrderer("GirafRest.IntegrationTest.Order.PriorityOrderer", "GirafRest.IntegrationTest")]
    [Collection("Intgration test")]
    public class WeekControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<WeekFixture>
    {
        private const string BASE_URL = "https://localhost:5000/";
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private WeekFixture _weekFixture;
        
        public WeekControllerTest(CustomWebApplicationFactory factory, WeekFixture weekFixture)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _weekFixture = weekFixture;
        }

        /// <summary>
        /// Testing getting empty list of weeks
        /// Endpoint: GET:/v1/Week/{userId}/weekName
        /// </summary>
        [Fact, Priority(0)]
        public async void TestWeekCanGetNoWeeks()
        {
            await TestExtension.RegisterAccountAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password, _weekFixture.CitizenUsername, _weekFixture.GuardianUsername, departmentId: 1);
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/weekName"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsAssignableFrom<Array>(content["data"].ToObject<string[]>());
            Assert.Empty(content["data"]);
        }

        /// <summary>
        /// Testing adding week
        /// Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(1)]
        public async void TestWeekCanAddWeek()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/{_weekFixture.WeekYear}/{_weekFixture.Week1Number}"),
                Method = HttpMethod.Put,
                Content = new StringContent(_weekFixture.CorrectWeek, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.GuardianUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(2018, content["data"]["weekYear"]);
            Assert.Equal(11, content["data"]["weekNumber"]);
        }

        /// <summary>
        /// Testing getting list containing new week
        /// Endpoint: GET:/v1/Week/{userId}/weekName
        /// </summary>
        [Fact, Priority(2)]
        public async void TestWeekCanGetNewWeeks()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/weekname"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(2018, content["data"][0]["weekYear"]);
            Assert.Equal(11, content["data"][0]["weekNumber"]);
        }

        /// <summary>
        /// Testing getting list containing new week v2 endpoint
        /// Endpoint: GET:/v1/Week/{userId}/week
        /// </summary>
        [Fact, Priority(3)]
        public async void TestWeekCanGetNewWeeksNewV2Endpoint()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/week"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(2018, content["data"][0]["weekYear"]);
            Assert.Equal(11, content["data"][0]["weekNumber"]);
        }

        /// <summary>
        /// Testing adding week containing too many days
        /// Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(4)]
        public async void TestWeekCanAddWeekWithTooManyDaysShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/{_weekFixture.WeekYear}/{_weekFixture.Week2Number}"),
                Method = HttpMethod.Put,
                Content = new StringContent(_weekFixture.TooManyDaysWeek, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.GuardianUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("InvalidAmountOfWeekdays", content["errorKey"]);
        }

        /// <summary>
        /// Testing ensuring week containing too many days was not added
        /// Endpoint: GET:/v1/Week/{userId}/weekName
        /// </summary>
        [Fact, Priority(5)]
        public async void TestWeekEnsureWeekWithTooManyDaysNotAdded()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/weekName"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Single(content["data"]);
        }

        /// <summary>
        /// Testing adding new week with invalid enums
        /// Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(6)]
        public async void TestWeekCanAddWeekWithInvalidEnumsShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/{_weekFixture.WeekYear}/{_weekFixture.Week3Number}"),
                Method = HttpMethod.Put,
                Content = new StringContent(_weekFixture.BadEnumValueWeek, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.GuardianUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("InvalidDay", content["errorKey"]);
        }

        /// <summary>
        /// Testing ensuring week with invalid enums was not added
        /// Endpoint: GET:/v1/Week/{userId}/weekName
        /// </summary>
        [Fact, Priority(7)]
        public async void TestWeekEnsureWeekWithInvalidEnumsNotAdded()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/weekName"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Single(content["data"]);
        }

        /// <summary>
        /// Testing updating week
        /// Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(8)]
        public async void TestWeekCanUpdateWeek()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/{_weekFixture.WeekYear}/{_weekFixture.Week1Number}"),
                Method = HttpMethod.Put,
                Content = new StringContent(_weekFixture.DifferentCorrectWeek, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.GuardianUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing ensuring week has been updated
        /// Endpoint: GET:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(9)]
        public async void TestWeekEnsureWeekIsUpdated()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/{_weekFixture.WeekYear}/{_weekFixture.Week1Number}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            foreach (var day in content["data"]["days"])
            {
                Assert.True(day["activities"][0]["pictograms"][0]["id"].ToObject<int>() == 2);
            }
        }

        /// <summary>
        /// Testing deleting week
        /// Endpoint: DELETE:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(10)]
        public async void TestWeekCanDeleteWeek()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/{_weekFixture.WeekYear}/{_weekFixture.Week1Number}"),
                Method = HttpMethod.Delete,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.GuardianUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing ensuring week has been deleted
        /// Endpoint: GET:/v1/Week/{userId}/weekName
        /// </summary>
        [Fact, Priority(11)]
        public async void TestWeekEnsureWeekDeleted()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{await TestExtension.GetUserIdAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}/weekName"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekFixture.CitizenUsername, _weekFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsAssignableFrom<Array>(content["data"].ToObject<string[]>());
            Assert.Empty(content["data"]);
        }
    }
}
