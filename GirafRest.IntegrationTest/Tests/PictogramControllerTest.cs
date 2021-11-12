using GirafRest.IntegrationTest.Extensions;
using GirafRest.IntegrationTest.Order;
using GirafRest.IntegrationTest.Setup;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GirafRest.IntegrationTest.Tests
{
    [TestCaseOrderer("GirafRest.IntegrationTest.Order.PriorityOrderer", "GirafRest.IntegrationTest")]
    [Collection("Account Controller")]
    public class PictogramControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<PictogramFixture>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly PictogramFixture _pictogramFixture;
        private const string BASE_URL = "https://localhost:5000/";
        private readonly HttpClient _client;
        public PictogramControllerTest(CustomWebApplicationFactory factory, PictogramFixture pictogramFixture)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _pictogramFixture = pictogramFixture;
        }

        /// <summary>
        /// Testing getting all pictograms
        /// Endpoint: GET:/v1/Pictogram
        /// </summary>
        [Fact, Priority(0)]
        public async void TestPictogramCanGetAllPictograms()
        {
            var parameters = "page=1,pageSize=10";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram?{parameters}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);

            foreach (var pictos in content["data"])
                pictos["lastEdit"] = _pictogramFixture.LAST_EDIT_TIMESTAMP;
            var thing = JObject.Parse(_pictogramFixture.Pictograms);
            foreach (var picto in thing["data"])
            {
                Assert.Contains(picto, content["data"]);
            }
        }

        /// <summary>
        /// Testing getting single public pictogram
        /// Endpoint: GET:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(1)]
        public async void TestPictogramCanGetSinglePublicPictogram()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/2"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);

            content["data"]["lastEdit"] = _pictogramFixture.LAST_EDIT_TIMESTAMP;
            Assert.Equal(JObject.Parse(_pictogramFixture.Pictograms)["data"][1].ToString(), content["data"].ToString());
        }

        /// <summary>
        /// Testing adding pictogram with invalid access level
        /// Endpoint: POST:/v1/Pictogram
        /// </summary>
        [Fact, Priority(2)]
        public async void TestPictogramCanAddPictogramInvalidAccessLevelShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Post,
                Content = new StringContent(_pictogramFixture.Fish, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("MissingProperties", content["errorKey"]);
        }

        /// <summary>
        /// Testing adding private pictogram
        /// Endpoint: POST:/v1/Pictogram
        /// </summary>
        [Fact, Priority(3)]
        public async void TestPictogramCanAddPrivatePictogram()
        {
            var data = JObject.Parse(_pictogramFixture.Fish);
            data["accessLevel"] = 3;
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Post,
                Content = new StringContent(data.ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.NotEqual(data["lastEdit"], content["data"]["lastEdit"]);
            Assert.NotEqual(data["id"], content["data"]["id"]);
        }

        /// <summary>
        /// Testing getting the newly added pictogram
        /// Endpoint: GET:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(4)]
        public async void TestPictogramCanGetNewPictogram()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{await TestExtension.GetPictogramIdAsync(_factory, _pictogramFixture.FishName, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(_pictogramFixture.FishName, content["data"]["title"]);
        }

        /// <summary>
        /// Testing updating the newly added pictogram
        /// Endpoint: PUT:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(5)]
        public async void TestPictogramCanUpdateNewPictogram()
        {
            var data = JObject.Parse(_pictogramFixture.Fish);
            data["title"] = _pictogramFixture.FishRename;
            data["accessLevel"] = 3;

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{await TestExtension.GetPictogramIdAsync(_factory, _pictogramFixture.FishName, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}"),
                Method = HttpMethod.Put,
                Content = new StringContent(data.ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing checking if update was successful
        /// Endpoint: GET:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(6)]
        public async void TestPictogramCheckUpdatedNewPictogram()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{await TestExtension.GetPictogramIdAsync(_factory, _pictogramFixture.FishRename, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(_pictogramFixture.FishRename, content["data"]["title"]);
        }

        /// <summary>
        /// Testing getting raw image of public pictogram
        /// Endpoint: GET:/v1/Pictogram/{id}/image/raw
        /// </summary>
        [Fact, Priority(7)]
        public async void TestPictogramCanGetPublicPictogramImage()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/6/image/raw"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = Image.FromStream(await response.Content.ReadAsStreamAsync());
            Assert.Equal(200, content.Width);
            Assert.Equal(200, content.Height);
            Assert.Equal("Png", content.RawFormat.ToString());
        }

        /// <summary>
        /// Testing updating image of newly added pictogram
        /// Endpoint: PUT:/v1/Pictogram/{id}/image
        /// </summary>
        [Fact, Priority(8)]
        public async void TestPictogramCanUpdateNewPictogramImage()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{await TestExtension.GetPictogramIdAsync(_factory, _pictogramFixture.FishRename, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}/image"),
                Method = HttpMethod.Put,
                Content = new StringContent(_pictogramFixture.RawImage, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing deleting the newly added pictogram
        /// Endpoint: DELETE:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(9)]
        public async void TestPictogramCanDeleteNewPictogram()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{await TestExtension.GetPictogramIdAsync(_factory, _pictogramFixture.FishRename, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}"),
                Method = HttpMethod.Delete,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing checking if the pictogram was deleted
        /// Endpoint: GET:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(10)]
        public async void TestPictogramEnsureNewPictogramDeleted()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{await TestExtension.GetPictogramIdAsync(_factory, _pictogramFixture.FishRename, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("PictogramNotFound", content["errorKey"]);
        }

        /// <summary>
        /// Testing querying for a pictogram
        /// Endpoint: GET:/v1/Pictogram?query=Epik
        /// </summary>
        [Fact, Priority(11)]
        public async void TestPictogramQuery()
        {
            var parameters = "query=Epik,page=1,pageSize=10";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/{parameters}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _pictogramFixture.Citizen1Username, _pictogramFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }
    }
}
