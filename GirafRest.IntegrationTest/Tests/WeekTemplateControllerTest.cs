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
    [Collection("Week Controller")]
    public class WeekTemplateControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<WeekTemplateFixture>
    {
        private const string BASE_URL = "https://localhost:5000/";
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private WeekTemplateFixture _weekTemplateFixture;
        public WeekTemplateControllerTest(CustomWebApplicationFactory factory, WeekTemplateFixture weekTemplateFixture)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _weekTemplateFixture = weekTemplateFixture;
        }

        /// <summary>
        /// Testing getting all templates
        /// Endpoint: GET:/v1/WeekTemplate
        /// </summary>
        [Fact, Priority(0)]
        public async void TestWeekTemplateCanGetAllTemplates()
        {
            await TestExtension.RegisterAsync(_factory, _weekTemplateFixture.CitizenUsername, _weekTemplateFixture.Password, _weekTemplateFixture.CitizenUsername, _weekTemplateFixture.GuardianUsername, departmentId: 1);
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal("SkabelonUge", content["data"][0]["name"]);
            Assert.Equal(1, content["data"][0]["templateId"].ToObject<int>());
        }

        /// <summary>
        /// Testing getting specific template
        /// Endpoint: GET:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(1)]
        public async void TestWeekTemplateCanGetSpecificTemplate()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/1"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal("SkabelonUge", content["data"]["name"]);
            Assert.Equal(77, content["data"]["thumbnail"]["id"].ToObject<int>());
            Assert.Equal(1, content["data"]["days"][0]["day"].ToObject<int>());
            Assert.Equal(6, content["data"]["days"][5]["day"].ToObject<int>());
        }

        /// <summary>
        /// Testing getting template from outside department
        /// Endpoint: GET:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(2)]
        public async void TestWeekTemplateCanGetTemplateOutsideDepartmentShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/1"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.CitizenUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NoWeekTemplateFound", content["errorKey"]);
        }

        /// <summary>
        /// Testing adding new template
        /// Endpoint: GET:/v1/WeekTemplate
        /// </summary>
        [Fact, Priority(3)]
        public async void TestWeekTemplateCanAddNewTemplate()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate"),
                Method = HttpMethod.Post,
                Content = new StringContent(_weekTemplateFixture.Templates[0], Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.NotNull(content["data"]["id"]);
        }

        /// <summary>
        /// Testing ensuring template has been added
        /// Endpoint: GET:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(4)]
        public async void TestWeekTemplateEnsureTemplateIsAdded()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/{await TestExtension.GetWeekTemplateIdAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password, _weekTemplateFixture.templateName[0])}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(28, content["data"]["thumbnail"]["id"].ToObject<int>());
            Assert.Equal(6, content["data"]["days"][0]["activities"][1]["pictograms"][0]["id"].ToObject<int>());
            Assert.Equal(7, content["data"]["days"][1]["activities"][1]["pictograms"][0]["id"].ToObject<int>());
        }

        /// <summary>
        /// Testing updating template
        /// Endpoint: PUT:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(5)]
        public async void TestWeekTemplateCanUpdateTemplate()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/{await TestExtension.GetWeekTemplateIdAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password, _weekTemplateFixture.templateName[0])}"),
                Method = HttpMethod.Put,
                Content = new StringContent(_weekTemplateFixture.Templates[1].ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        ///  Testing ensuring template has been updated
        /// Endpoint: GET:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(6)]
        public async void TestWeekTemplateEnsureTemplateIsUpdated()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/{await TestExtension.GetWeekTemplateIdAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password, _weekTemplateFixture.templateName[1])}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(29, content["data"]["thumbnail"]["id"].ToObject<int>());
            Assert.Equal(7, content["data"]["days"][0]["activities"][1]["pictograms"][0]["id"].ToObject<int>());
            Assert.Equal(8, content["data"]["days"][1]["activities"][1]["pictograms"][0]["id"].ToObject<int>());
        }

        /// <summary>
        /// Testing deleting template
        /// Endpoint: DELETE:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(7)]
        public async void TestWeekTemplateCanDeleteTemplate()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/{await TestExtension.GetWeekTemplateIdAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password, _weekTemplateFixture.templateName[1])}"),
                Method = HttpMethod.Delete,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing ensuring template has been deleted
        /// Endpoint: GET:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(8)]
        public async void TestWeekTemplateEnsureTemplateIsDeleted()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/{await TestExtension.GetWeekTemplateIdAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password, _weekTemplateFixture.templateName[1])}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _weekTemplateFixture.GuardianUsername, _weekTemplateFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NoWeekTemplateFound", content["errorKey"]);
        }
    }
}
