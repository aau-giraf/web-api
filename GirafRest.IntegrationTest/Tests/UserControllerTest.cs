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
    public class UserControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<UserFixture>
    {
        private const string BASE_URL = "https://localhost:5000/";
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private UserFixture _userFixture;
        public UserControllerTest(CustomWebApplicationFactory factory, UserFixture userFixture)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _userFixture = userFixture;
        }

        /// <summary>
        /// Testing getting Citizen1's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(0)]
        public async void TestUserCanGetCitizen1Id()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen1Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]["id"]);
        }

        /// <summary>
        /// Testing getting Guardian's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(1)]
        public async void TestUserCanGetGuardianId()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]["id"]);
        }

        /// <summary>
        /// Testing getting Citizen2's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(2)]
        public async void TestUserCanGetCitizen2Id()
        {
            await TestExtension.RegisterAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password, _userFixture.Citizen2Username, _userFixture.GuardianUsername, departmentId: 1);
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]["id"]);
        }

        /// <summary>
        /// Testing getting Citizen3's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(3)]
        public async void TestUserCanGetCitizen3Id()
        {
            await TestExtension.RegisterAsync(_factory, _userFixture.Citizen3Username, _userFixture.Password, _userFixture.Citizen3Username, _userFixture.GuardianUsername, departmentId: 1);
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen3Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]["id"]);
        }

        /// <summary>
        /// Testing getting Citizen3's id with Citizen2
        /// Endpoint: GET:/v1/User/{id}
        /// </summary>
        [Fact, Priority(4)]
        public async void TestUserCanGetCitizen3IdWithCitizen2ShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen3Username, _userFixture.Password)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("Forbidden", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting citizen info as guardian
        /// Endpoint: GET:/v1/User/{id}
        /// </summary>
        [Fact, Priority(5)]
        public async void TestUserCanGetCitizenInfoAsGuardian()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(_userFixture.Citizen2Username, content["data"]["username"]);
        }

        /// <summary>
        /// Testing setting display name
        /// Endpoint: PUT:/v1/User/{id}
        /// </summary>
        [Fact, Priority(6)]
        public async void TestUserCanSetDisplayName()
        {
            var body = $"{{'username': '{_userFixture.Citizen2Username}', 'displayName': 'FBI Surveillance Van'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}"),
                Method = HttpMethod.Put,
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing ensuring display name is updated
        /// Endpoint: Get:/v1/User/
        /// </summary>
        [Fact, Priority(7)]
        public async void TestUserCanGetDisplayName()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal("FBI Surveillance Van", content["data"]["displayName"]);
        }

        /// <summary>
        /// Testing adding pictogram to Citizen3 as Citizen2
        /// Endpoint: POST:/v1/User/{id}/resource
        /// </summary>
        [Fact, Priority(8)]
        public async void TestUserCanAddPictogramToCitizen3AsCitizen2()
        {
            var pictogram = $"{{ 'accessLevel': 3, 'title': 'wednesday', 'id': 5, 'lastEdit': '2018-03-19T10:40:26.587Z'}}";
            TestExtension.CreatePictogramAsync(_factory, pictogram, 3, _userFixture.Citizen2Username, _userFixture.Password);

            var body = $"{{ 'id': '{JObject.Parse(pictogram)["id"]}'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen3Username, _userFixture.Password)}/resource"),
                Method = HttpMethod.Post,
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting user settings
        /// Endpoint: GET:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(9)]
        public async void TestUserCanGetSettings()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing setting grayscale theme
        /// Endpoint: PUT:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(10)]
        public async void TestUserCanEnableGrayscaleTheme()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Put,
                Content = new StringContent(_userFixture.GrayscaleTheme, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing ensuring theme has been updated
        /// Endpoint: GET:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(11)]
        public async void TestUserCanCheckTheme()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(JObject.Parse(_userFixture.GrayscaleTheme)["theme"], content["data"]["theme"]);
        }

        /// <summary>
        /// Testing setting default countdown time
        /// Endpoint: PUT:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(12)]
        public async void TestUserCanSetDefaultCountdownTime()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Put,
                Content = new StringContent(_userFixture.TimerOneHour, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing setting default countdown time
        /// Endpoint: PUT:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(13)]
        public async void TestUserCanCheckCountdownTimer()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(JObject.Parse(_userFixture.TimerOneHour)["timerSeconds"], content["data"]["timerSeconds"]);
        }

        /// <summary>
        /// Testing setting multiple settings at once
        /// Endpoint: PUT:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(14)]
        public async void TestUserCanSetMultipleSettings()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Put,
                Content = new StringContent(_userFixture.MultipleSettings, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing ensuring settings have been updated
        /// Endpoint: GET:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(15)]
        public async void TestUserCanCheckMultipleSettings()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(2, content["data"]["orientation"]);
            Assert.Equal(2, content["data"]["completeMark"]);
            Assert.Equal(1, content["data"]["cancelMark"]);
            Assert.Equal(2, content["data"]["defaultTimer"]);
            Assert.Equal(60, content["data"]["timerSeconds"]);
            Assert.Equal(3, content["data"]["activitiesCount"]);
            Assert.Equal(3, content["data"]["theme"]);
            Assert.Equal(1, content["data"]["nrOfDaysToDisplayPortrait"]);
            Assert.True(content["data"]["displayDaysRelativePortrait"].ToObject<bool>());
            Assert.Equal(7, content["data"]["nrOfDaysToDisplayLandscape"]);
            Assert.False(content["data"]["displayDaysRelativeLandscape"].ToObject<bool>());
            Assert.Equal(0, content["data"]["nrOfActivitiesToDisplay"]);
            Assert.True(content["data"]["greyScale"].ToObject<bool>());
            Assert.True(content["data"]["lockTimerControl"].ToObject<bool>());
            Assert.True(content["data"]["pictogramText"].ToObject<bool>());
            Assert.False(content["data"]["showPopup"].ToObject<bool>());
            Assert.False(content["data"]["showOnlyActivities"].ToObject<bool>());
            Assert.False(content["data"]["showSettingsForCitizen"].ToObject<bool>());
            Assert.Equal("#FF00FF", content["data"]["weekDayColors"][0]["hexColor"]);
            Assert.Equal(1, content["data"]["weekDayColors"][0]["day"]);
        }

        /// <summary>
        /// Testing getting Citizen1's citizens
        /// Endpoint: GET:/v1/User/{userId}/citizens
        /// </summary>
        [Fact, Priority(16)]
        public async void TestUserCanGetCitizen1CitizensShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen1Username, _userFixture.Password)}/citizens"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen1Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("Forbidden", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting Guardian's citizens
        /// Endpoint: GET:/v1/User/{userId}/citizens
        /// </summary>
        [Fact, Priority(17)]
        public async void TestUserCanGetGuardianCitizens()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}/citizens"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Contains(content["data"], x => x["displayName"].ToString() == _userFixture.Citizen3Username);
        }

        /// <summary>
        /// Testing getting Citizen2's guardians
        /// Endpoint: GET:/v1/User/{userId}/guardians
        /// </summary>
        [Fact, Priority(18)]
        public async void TestUserCanGetCitizen2Guardians()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/guardians"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Contains(content["data"], x => x["displayName"].ToString() == _userFixture.GuardianDisplayName);
        }

        /// <summary>
        /// Testing getting Guardian's guardians
        /// Endpoint: GET:/v1/User/{userId}/guardians
        /// </summary>
        [Fact, Priority(19)]
        public async void TestUserCanGetGuardianGuardiansShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}/guardians"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("Forbidden", content["errorKey"]);
        }

        /// <summary>
        /// Testing changing Guardian settings
        /// Endpoint: GET:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(20)]
        public async void TestUserCanChangeGuardianSettingsShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}/settings"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("RoleMustBeCitizien", content["errorKey"]);
        }

        /// <summary>
        /// Testing changing settings as citizen
        /// Endpoint: PUT:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(21)]
        public async void TestUserCanChangeSettingsAsCitizenShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen1Username, _userFixture.Password)}/settings"),
                Method = HttpMethod.Put,
                Content = new StringContent(_userFixture.MultipleSettings, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen1Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("Forbidden", content["errorKey"]);
        }

        /// <summary>
        /// Testing if a user can change another users icon
        /// Endpoint: PUT:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(26)]
        public async void TestUserUserCanSetIconOfAnotherUserShouldFail()
        {
            var body = $"{{'userIcon': {_userFixture.RawImage}}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/icon"),
                Method = HttpMethod.Put,
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen1Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing if a guardian can change another users icon
        /// Endpoint: PUT:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(27)]
        public async void TestUserGuardianCanSetIconOfAnotherUser()
        {
            var body = $"{{'userIcon': {_userFixture.RawImage}}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/icon"),
                Method = HttpMethod.Put,
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing if a user can get own userIcon
        /// Endpoint: GET:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(28)]
        public async void TestUserUserCanGetOwnIcon()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/icon"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing if a user can get the userIcon of another user
        /// Endpoint: GET:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(29)]
        public async void TestUserUserCanGetSpecificUserIcon()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/icon"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.Citizen1Username, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing if a guardian can get the userIcon of another user
        /// Endpoint: GET:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(30)]
        public async void TestUserGuardianCanGetSpecificUserIcon()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{await TestExtension.GetUserIdAsync(_factory, _userFixture.Citizen2Username, _userFixture.Password)}/icon"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _userFixture.GuardianUsername, _userFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }
    }
}
