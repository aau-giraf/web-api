using GirafRest.IntegrationTest.Order;
using GirafRest.IntegrationTest.Setup;
using GirafRest.IntegrationTest.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json.Linq;

namespace GirafRest.IntegrationTest.Tests
{
    [TestCaseOrderer("GirafRest.IntegrationTest.Order.PriorityOrderer", "GirafRest.IntegrationTest")]
    [Collection("Integration test")]
    public class AuthorizationControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<AuthorizationFixture>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly AuthorizationFixture _authorizationFixture;
        private const string BASE_URL = "https://localhost:5000/";
        private readonly HttpClient _client;

        public AuthorizationControllerTest(CustomWebApplicationFactory factory, AuthorizationFixture authorizationFixture)
        {
            _factory = factory;
            _authorizationFixture = authorizationFixture;
            _client = _factory.CreateClient();
        }

        #region Account endpoints
        /// <summary>
        /// Testing setting password
        /// Endpoint: POST:/v2/Account/password/{userId}
        /// </summary>
        [Fact, Priority(0)]
        public async void TestAuthPOSTAccountSetPasswordShouldFail()
        {
            var data = "{ 'unrelated': 'unrelated'}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("MissingProperties", content["errorKey"]);
        }

        /// <summary>
        /// Testing changing password
        /// Endpoint: PUT:/ v2 / Account / password /{ userId}
        /// </summary>
        [Fact, Priority(1)]
        public async void TestAuthPUTAccountChangePasswordShouldFail()
        {
            var data = "{ 'unrelated': 'unrelated'}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
                Method = HttpMethod.Put,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting password reset token
        /// Endpoint: GET:/v2/Account/password-reset-token/{userId}
        /// </summary>
        [Fact, Priority(2)]
        public async void TestAuthGETAccountPasswordResetTokenShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password-reset-token/{await TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);

        }

        /// <summary>
        /// Testing account login
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(3)]
        public async void TestAuthPOSTAccountLoginShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("MissingProperties", content["errorKey"]);
        }

        /// <summary>
        /// Testing account registration without authentication token
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(4)]
        public async void TestAuthPOSTRegisterAccountLoginShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing account DELETION with authentication token
        /// Endpoint: DELETE:/v2/Account/user/{id}
        /// </summary>
        [Fact, Priority(5)]
        public async void TestAuthDELETEAccountLoginShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/user/{await TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion

        #region Activity endpoints
        /// <summary>
        /// Testing creation of weekplanner activity without authentication token
        /// Endpoint: POST:/v2/Activity/{user_id}/{weekplan_name}/{week_year}/{week_number}/{week_day_number}
        /// </summary>
        [Fact, Priority(6)]
        public async void TestAuthPOSTWeekplannerActivityShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Activity/{await TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/{_authorizationFixture.WeekplanName}/{_authorizationFixture.WeekYear}/{_authorizationFixture.WeekNumber}/{_authorizationFixture.WeekDayNumber}"),
                Method = HttpMethod.Post,
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing DELETION of activity without authentication token
        /// Endpoint: DELETE:/v2/Activity/{user_id}/delete/{activity_id}
        /// </summary>
        [Fact, Priority(7)]
        public async void TestAuthDELETEActivityShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Activity/{await TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/delete/{_authorizationFixture.ActivityId}"),
                Method = HttpMethod.Delete,
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing PATCHING of activity or updating it without authentication token
        /// Endpoint: PATCH:/v2/Activity/{user_id}/update
        /// </summary>
        [Fact, Priority(8)]
        public async void TestAuthPATCHUpdateActivityShouldFail()
        {
            string data = "{'pictogram': {'id': 6}, 'id': 1}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Activity/{await TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/update"),
                Method = HttpMethod.Put,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion

        #region Department endpoints
        /// <summary>
        /// Testing creating new department
        /// Endpoint: POST:/v1/Department
        /// </summary>
        [Fact, Priority(9)]
        public async void TestAuthPOSTDepartmentShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department"),
                Method = HttpMethod.Post
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting department citizens
        /// Endpoint: GET:/v1/Department/{id}/citizens
        /// </summary>
        [Fact, Priority(10)]
        public async void TestAuthGETDepartmentCitizensShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{_authorizationFixture.DepartmentId}/citizens"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing GET on department
        /// Endpoint: GET:/v1/Department
        /// </summary>
        [Fact, Priority(11)]
        public async void TestAuthGETDepartmentSuccess()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing GET on department id
        /// Endpoint: GET:/v1/Department/{id}
        /// </summary>
        [Fact, Priority(12)]
        public async void TestAuthGETDepartmentIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{_authorizationFixture.DepartmentId}"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Testing PUT on department id name
        /// Endpoint: PUT:/v1/Department/{id}/name
        /// </summary>
        [Fact, Priority(13)]
        public async void TestAuthPUTDepartmentIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{_authorizationFixture.DepartmentId}/name"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing DELETE on department id
        /// Endpoint: DELETE:/v1/Department/{id}
        /// </summary>
        [Fact, Priority(14)]
        public async void TestAuthDELETEDepartmentIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{_authorizationFixture.DepartmentId}"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion

        #region Error endpoints
        /// <summary>
        /// Testing GET error
        /// Endpoint: GET:/v1/Error
        /// </summary>
        [Fact, Priority(15)]
        public async void TestAuthGETErrorShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }

        /// <summary>
        /// Testing POST error
        /// Endpoint: POST:/v1/Error
        /// </summary>
        [Fact, Priority(16)]
        public async void TestAuthPOSTErrorShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Post
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }

        /// <summary>
        /// Testing PUT error
        /// Endpoint: PUT:/v1/Error
        /// </summary>
        [Fact, Priority(17)]
        public async void TestAuthPUTErrorShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }

        /// <summary>
        /// Testing DELETE error
        /// Endpoint: DELETE:/v1/Error
        /// </summary>
        [Fact, Priority(18)]
        public async void TestAuthDELETEErrorShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }
        #endregion

        #region Pictogram endpoints
        /// <summary>
        /// Testing getting all pictograms
        /// Endpoint: GET:/v1/Pictogram
        /// </summary>
        [Fact, Priority(19)]
        public async void TestAuthGETAllPictogramsShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing creating new pictogram
        /// Endpoint: POST:/v1/Pictogram
        /// </summary>
        [Fact, Priority(20)]
        public async void TestAuthPOSTPictogramsShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Post
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting pictogram by id with
        /// Endpoint: GET:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(21)]
        public async void TestAuthGETPictogramsByIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing updating pictogram info
        /// Endpoint: PUT:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(22)]
        public async void TestAuthPUTUpdatePictogramsInfoShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing deleting pictogram by id
        /// Endpoint: DELETE:/v1/Pictogram/{id}
        /// </summary>
        [Fact, Priority(23)]
        public async void TestAuthDELETEUpdatePictogramsInfoShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting pictogram image by id
        /// Endpoint: GET:/v1/Pictogram/{id}/image
        /// </summary>
        [Fact, Priority(24)]
        public async void TestAuthGETPictogramsInfoShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0/image"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing updating pictogram image by id
        /// Endpoint: PUT:/v1/Pictogram/{id}/image
        /// </summary>
        [Fact, Priority(25)]
        public async void TestAuthPUTPictogramsImageByIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0/image"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting raw pictogram image by id
        /// Endpoint: GET:/v1/Pictogram/{id}/image/raw
        /// </summary>
        [Fact, Priority(26)]
        public async void TestAuthGETPictogramsImageRawByIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0/image/raw"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion

        #region Status endpoints
        /// <summary>
        /// Testing getting API status
        /// Endpoint: GET:/v1/Status
        /// </summary>
        [Fact, Priority(27)]
        public async void TestAuthGETStatus()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Status"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing getting API database status
        /// Endpoint: GET:/v1/Status/database
        /// </summary>
        [Fact, Priority(28)]
        public async void TestAuthGETDatabaseStatus()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Status/database"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        #endregion

        #region User endpoint
        /// <summary>
        /// Testing getting pictogram image by id
        /// Endpoint: GET:/v1/Pictogram/{id}/image
        /// </summary>
        [Fact, Priority(29)]
        public async void TestAuthGETUserShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0/image"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing updating user
        /// Endpoint: PUT:/v1/User/{id}
        /// </summary>
        [Fact, Priority(30)]
        public async void TestAuthPUTUserShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram/0/image"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting user settings by user id
        /// Endpoint: GET:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(31)]
        public async void TestAuthGETUserSettingsByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/settings"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing updating user settings by user id
        /// Endpoint: GET:/v1/User/{id}/settings
        /// </summary>
        [Fact, Priority(32)]
        public async void TestAuthPUTUpdateUserSettingsByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/settings"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting user icon by user id
        /// Endpoint: GET:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(33)]
        public async void TestAuthGETUserIconByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/icon"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing updating user icon by user id
        /// Endpoint: PUT:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(34)]
        public async void TestAuthPUTUpdateUserIconByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/icon"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing deleting user icon by user id
        /// Endpoint: DELETE:/v1/User/{id}/icon
        /// </summary>
        [Fact, Priority(35)]
        public async void TestAuthDELETEUserIconByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/icon"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting raw user icon by user id
        /// Endpoint: GET:/v1/User/{id}/icon/raw
        /// </summary>
        [Fact, Priority(36)]
        public async void TestAuthGETUserIconRawByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/icon"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting user citizens by user id
        /// Endpoint: GET:/v1/User/{userId}/citizens
        /// </summary>
        [Fact, Priority(37)]
        public async void TestAuthGETCitizenByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/citizen"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NotFound", content["errorKey"]);
        }

        /// <summary>
        /// Testing getting user guardians by user id
        /// Endpoint: GET:/v1/User/{userId}/guardians
        /// </summary>
        [Fact, Priority(38)]
        public async void TestAuthGETUserGuardiansByUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/0/guardians"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing GET user Id
        /// Endpoint: GET:/v1/User/{id}
        /// </summary>
        [Fact, Priority(39)]
        public async void TestAuthGETUserIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing POST as guardian for citizen
        /// Endpoint: POST:/v1/User/{userId}/citizens/{citizenId}
        /// </summary>
        [Fact, Priority(40)]
        public async void TestAuthPOSTUserIdCitizensCitizenIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User/{TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/citizens/{_authorizationFixture.CitizenId}"),
                Method = HttpMethod.Post
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion

        #region Week endpoints
        /// <summary>
        /// Testing GET on user specific week v1
        /// Endpoint: GET:/v1/Week/{userId}/week
        /// </summary>
        [Fact, Priority(41)]
        public async void TestAuthGETUserIdWeekV1ShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/week"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing GET on user specific weekyear and number
        /// Endpoint: GET:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(42)]
        public async void TestAuthGETUserIdWeekyearWeeknumberShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/{_authorizationFixture.WeekYear}/{_authorizationFixture.WeekNumber}"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing PUT on user specific weekyear and number
        /// Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(43)]
        public async void TestAuthPUTUserIdWeekyearWeeknumberShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/{_authorizationFixture.WeekYear}/{_authorizationFixture.WeekNumber}"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing DELETE on user specific weekyear and number
        /// Endpoint: DELETE:/v1/Week/{userId}/{weekYear}/{weekNumber}
        /// </summary>
        [Fact, Priority(44)]
        public async void TestAuthDELETEUserIdWeekyearWeeknumberShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Week/{TestExtension.GetUserIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}/{_authorizationFixture.WeekYear}/{_authorizationFixture.WeekNumber}"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion

        #region WeekTemplate endpoints
        /// <summary>
        /// Testing GET on weektemplate
        /// Endpoint: GET:/v1/WeekTemplate
        /// </summary>
        [Fact, Priority(45)]
        public async void TestAuthGETWeektemplateShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing POST on weektemplate
        /// Endpoint: POST:/v1/WeekTemplate
        /// </summary>
        [Fact, Priority(46)]
        public async void TestAuthPOSTWeektemplateShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate"),
                Method = HttpMethod.Post
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing GET on weektemplate id
        /// Endpoint: GET:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(47)]
        public async void TestAuthGETWeektemplateIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/123"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing PUT on weektemplate id
        /// Endpoint: PUT:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(48)]
        public async void TestAuthPutWeektemplateIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/123"),
                Method = HttpMethod.Put
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing DELETE on weektemplate id
        /// Endpoint: DELETE:/v1/WeekTemplate/{id}
        /// </summary>
        [Fact, Priority(49)]
        public async void TestAuthDELETEWeektemplateIdShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/WeekTemplate/123"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion

        #region Other tests
        /// <summary>
        /// Testing arbitrary request with expired token
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(50)]
        public async void TestAuthExpiredAuthorizationShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {_authorizationFixture.ExpiredToken}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        #endregion
    }
}
