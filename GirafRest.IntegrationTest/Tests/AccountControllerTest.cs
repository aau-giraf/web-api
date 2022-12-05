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
    [Collection("Integration test")]
    public class AccountControllerTest
    : IClassFixture<CustomWebApplicationFactory>, IClassFixture<AccountFixture>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly AccountFixture _accountFixture;
        private const string BASE_URL = "https://localhost:5000/";
        private readonly HttpClient _client;
        public AccountControllerTest(CustomWebApplicationFactory factory, AccountFixture accountFixture)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _accountFixture = accountFixture;
        }

        /// <summary>
        ///Testing logging in as Guardian
        ///Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(0)]
        public async void TestAccountCanLoginAsGuardian()
        {
            HttpContent httpContent = new StringContent($"{{'username': '{_accountFixture.GuardianUsername}', 'password': '{_accountFixture.Password}'}}",
                                    Encoding.UTF8,
                                    "application/json");
            var response = await _client.PostAsync($"{BASE_URL}v2/Account/login", httpContent);
            string data = await response.Content.ReadAsStringAsync();
            JObject content = JObject.Parse(data);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
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
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
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
            var data = "{'username': 'myUsername', 'password': '{_accountFixture.Password}', 'role': 'Citizen', 'departmentId': 1}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

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
            var data = "{'username': 'myUsername', 'displayName': '', 'password': '{_accountFixture.Password}'," +
                "'role': 'Citizen', 'departmentId': 1}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

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
            var data = $"{{'username': '{_accountFixture.Citizen2Username}', 'displayName': '{_accountFixture.Citizen2Username}', 'password': '{_accountFixture.Password}', 'role': 'Citizen', 'departmentId': 2}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

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
            var data = $"{{'username': '{_accountFixture.Citizen2Username}', 'password': '{_accountFixture.Password}'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
        }

        /// <summary>
        /// Testing getting Citizen2's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(6)]
        public async void TestAccountCanGetCitizen2Id()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get,
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
            Assert.NotEmpty(content["data"]["id"].ToString());
        }

        /// <summary>
        /// Testing getting username using authorization
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(7)]
        public async void TestAccountCanGetUsernameWithAuth()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get,
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
        }

        /// <summary>
        /// Testing logging in with invalid password
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(8)]
        public async void TestAccountCanLoginInvalidPasswordShouldFail()
        {
            var data = $"{{ 'username': '{_accountFixture.GuardianUsername}', 'password': 'this-wont-work'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Invalid Credentials", content["message"].ToString());
        }

        /// <summary>
        /// Testing logging in with invalid username
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(9)]
        public async void TestAccountCanLoginInvalidUsernameShouldFail()
        {
            var data = "{'username': 'this-wont-work-either', 'password': '{_accountFixture.Password}'}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Invalid credentials", content["message"].ToString());
        }

        /// <summary>
        /// Testing registering Citizen1 with no authorization
        /// Endpoint: POST:/v2/Account/register
        /// </summary>
        [Fact, Priority(10)]
        public async void TestAccountCanRegisterCitizen1ShouldFail()
        {
            var data = $"{{'username': '{_accountFixture.Citizen1Username}', 'displayname': '{_accountFixture.Citizen1Username}', 'password': '{_accountFixture.Password}', 'role': 'Citizen', 'departmentId': 2}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"].ToString());
        }

        /// <summary>
        /// Testing registering Citizen1
        /// Endpoint: POST:/v2/Account/register
        /// </summary>
        [Fact, Priority(11)]
        public async void TestAccountCanRegisterCitizen1()
        {
            var data = $"{{'username': '{_accountFixture.Citizen1Username}', 'displayname': '{_accountFixture.Citizen1Username}', 'password': '{_accountFixture.Password}', 'role': 'Citizen', 'departmentId': 2}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
        }

        /// <summary>
        /// Testing logging in as Citizen1
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(12)]
        public async void TestAccountCanLoginAsCitizen1()
        {
            var data = $"{{'username': '{_accountFixture.Citizen1Username}', 'password': '{_accountFixture.Password}'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
        }

        /// <summary>
        /// Testing getting Citizen1's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(13)]
        public async void TestAccountCanGetCitizen1Id()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
            Assert.NotEmpty(content["data"]["id"].ToString());
        }

        /// <summary>
        /// Testing getting Citizen1's username
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(14)]
        public async void TestAccountCanGetCitizen1Username()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
            Assert.NotEmpty(content["data"]["username"].ToString());
        }

        /// <summary>
        /// Testing getting Citizen1's username
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(15)]
        public async void TestAccountCanGetCitizen1Role()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
            Assert.NotEmpty(content["data"]["role"].ToString());
        }

        /// <summary>
        /// Testing deleting Guardian with Citizen2
        /// Endpoint: DELETE:/v2/Account/user/{id}
        /// </summary>
        [Fact, Priority(17)]
        public async void TestAccountCanDeleteGuardianWithCitizen2ShouldFail()
        {
            var data = $"{{'username': '{_accountFixture.GuardianUsername}', 'password': '{_accountFixture.Password}'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/user/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}"),
                Method = HttpMethod.Delete
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("Forbidden", content["errorKey"].ToString());
        }

        /// <summary>
        ///  Testing deleting Citizen2
        /// Endpoint: DELETE:/v2/Account/user/{id}
        /// </summary>
        [Fact, Priority(18)]
        public async void TestAccountCanDeleteCitizen2()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/user/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password)}"),
                Method = HttpMethod.Delete
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing logging in as Citizen2 after deletion
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(19)]
        public async void TestAccountCanLoginAsDeletedCitizen2ShouldFail()
        {
            var data = $"{{'username': '{_accountFixture.Citizen2Username}', 'password': '{_accountFixture.Password}'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("InvalidCredentials", content["errorKey"].ToString());
        }

        /// <summary>
        /// Testing Citizen2's authorization after deletion
        /// Endpoint: GET:/v1/User/{id}
        /// </summary>
        [Fact, Priority(20)]
        public async void TestAccountCanUseDeletedCitizen2sToken()
        {
            await TestExtension.RegisterAccountAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password, _accountFixture.Citizen2Username, _accountFixture.GuardianUsername);
            var Citizen2Token = await TestExtension.GetTokenAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password);
            await TestExtension.DeleteAccountAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password, _accountFixture.GuardianUsername, _accountFixture.Password);

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/user/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}"),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {Citizen2Token}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("Forbidden", content["errorKey"].ToString());
        }

        /// <summary>
        /// Testing getting Citizen1's password reset token with Guardian
        /// Endpoint: GET:/v2/Account/password-reset-token/{userId}
        /// </summary>
        [Fact, Priority(21)]
        public async void TestAccountCanGetCitizen1ResetToken()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password-reset-token/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.Password)}"),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
        }

        /// <summary>
        /// Testing resetting Citizen1's password with Guardian
        /// Endpoint: POST:/v2/Account/password/{userId}
        /// </summary>
        [Fact, Priority(22)]
        public async void TestAccountCanResetCitizen1Password()
        {
            var data = $"{{'password': '{_accountFixture.BrandNewPassword}', 'token': '{await TestExtension.GetResetTokenAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.Password, _accountFixture.GuardianUsername, _accountFixture.Password)}'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.Password)}"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(content["data"].ToString());
        }

        /// <summary>
        /// Testing resetting Citizen2's password using Citizen1's token and Guardian's authorization
        /// Endpoint: POST:/v2/Account/password/{userId}
        /// </summary>
        [Fact, Priority(23)]
        public async void TestAccountCantResetCitizen2Password()
        {
            await TestExtension.RegisterAccountAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password, _accountFixture.Citizen2Username, _accountFixture.GuardianUsername);
            var citizen2Id = await TestExtension.GetUserIdAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password);
            await TestExtension.DeleteAccountAsync(_factory, _accountFixture.Citizen2Username, _accountFixture.Password, _accountFixture.GuardianUsername, _accountFixture.Password);

            var data = $"{{'password': '{_accountFixture.BrandNewPassword}', 'token': '{await TestExtension.GetResetTokenAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.BrandNewPassword, _accountFixture.GuardianUsername, _accountFixture.Password)}'}}";
            
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{citizen2Id}"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("UserNotFound", content["errorKey"].ToString());
        }

        /// <summary>
        /// Testing resetting Citizen1's password with an invalid token
        /// Endpoint: POST:/v2/Account/password/{userId}
        /// </summary>
        [Fact, Priority(24)]
        public async void TestAccountCanResetCitizen1PasswordInvalidTokenShouldFail()
        {
            var data = $"{{'password': '{_accountFixture.BrandNewPassword}', 'token': 'invalid-token'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.BrandNewPassword)}"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("InvalidProperties", content["errorKey"].ToString());
        }

        /// <summary>
        /// Testing setting Citizen1's password with an invalid token
        /// Endpoint: POST:/v2/Account/password/{userId}
        /// </summary>
        [Fact, Priority(25)]
        public async void TestAccountCanSetCitizen1PasswordInvalidTokenShouldFail()
        {
            var data = $"{{'password': '{_accountFixture.BrandNewPassword}', 'token': 'invalid-token'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.BrandNewPassword)}"),
                Method = HttpMethod.Put,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("MissingProperties", content["errorKey"].ToString());
        }

        /// <summary>
        /// Testing setting Citizen1's password with an valid token
        /// Endpoint: POST:/v2/Account/password/{userId}
        /// </summary>
        [Fact, Priority(26)]
        public async void TestAccountCanSetCitizen1PasswordValidToken()
        {
            var data = $"{{'oldPassword': '{_accountFixture.BrandNewPassword}', 'newPassword': '{_accountFixture.Password}'}}";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await TestExtension.GetUserIdAsync(_factory, _accountFixture.Citizen1Username, _accountFixture.BrandNewPassword)}"),
                Method = HttpMethod.Put,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _accountFixture.GuardianUsername, _accountFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
