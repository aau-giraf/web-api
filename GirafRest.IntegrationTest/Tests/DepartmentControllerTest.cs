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
    [Collection("Department Controller")]
    public class DepartmentControllerTest
    : IClassFixture<CustomWebApplicationFactory>, IClassFixture<DepartmentFixture>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly DepartmentFixture _departmentFixture;
        private const string BASE_URL = "https://localhost:5000/";
        private readonly HttpClient _client;
        public DepartmentControllerTest(CustomWebApplicationFactory factory, DepartmentFixture departmentFixture)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _departmentFixture = departmentFixture;
        }

        /// <summary>
        /// Testing getting list of departments
        /// Endpoint: GET:/v1/Department
        /// </summary>
        [Fact, Priority(0)]
        public async void TestDepartmentCanGetDepartmentList()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department"),
                Method = HttpMethod.Get,
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing trying to create new department as Guardian
        /// Endpoint: POST:/v1/Department
        /// </summary>
        [Fact, Priority(1)]
        public async void TestDepartmentCanCreateDepartmentAsGuardianShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.Department, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.GuardianUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing trying to create new department as Citizen1
        /// Endpoint: POST:/v1/Department
        /// </summary>
        [Fact, Priority(2)]
        public async void TestDepartmentCanCreateDepartmentAsCitizen1ShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.Department, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen1Username, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing trying to create new department as Super User
        /// Endpoint: POST:/v1/Department
        /// </summary>
        [Fact, Priority(3)]
        public async void TestDepartmentCanCreateDepartmentAsSuperUser()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.Department, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.SuperUserUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("NotAuthorized", content["data"]["id"]);
        }

        /// <summary>
        /// Testing logging in as newly created department
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(4)]
        public async void TestDepartmentCanLoginAsDepartment()
        {
            var data = $"{{'username': '{_departmentFixture.DepartmentUsername}', 'password': {_departmentFixture.DepartmentPassword}}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing getting newly created department
        /// Endpoint: GET:/v1/Department/{id}
        /// </summary>
        [Fact, Priority(5)]
        public async void TestDepartmentCanGetNewDepartment()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.SuperUserUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Equal(_departmentFixture.DepartmentUsername, content["data"]["name"]);
        }

        /// <summary>
        /// Testing getting department that does not exist
        /// Endpoint: GET:/v1/Department/{id}
        /// </summary>
        [Fact, Priority(6)]
        public async void TestDepartmentCanGetNonexistentDepartmentShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/-2"),
                Method = HttpMethod.Get,
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("UserNotFound", content["errorKey"]);
        }

        /// <summary>
        /// Testing registering Citizen2 in newly created department
        /// Endpoint: POST:/v2/Account/register
        /// </summary>
        [Fact, Priority(7)]
        public async void TestDepartmentCanRegisterCitizen2Department()
        {
            string data = $"{{ 'username': '{_departmentFixture.Citizen2Username}', 'displayname': '{_departmentFixture.Citizen2Username}', 'password': '{_departmentFixture.Password}', 'role': 'Citizen', 'departmentId': {await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.SuperUserUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        /// <summary>
        /// Testing logging in as Citizen1
        /// Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact, Priority(8)]
        public async void TestDepartmentCanLoginAsCitizen2()
        {
            string data = $"{{ 'username': '{_departmentFixture.Citizen2Username}', 'password': '{_departmentFixture.Password}'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing getting Citizen1's id
        /// Endpoint: GET:/v1/User
        /// </summary>
        [Fact, Priority(9)]
        public async void TestDepartmentCanGetCitizen2Id()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/User"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen2Username, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.NotNull(content["data"]["id"]);
        }

        /// <summary>
        /// Testing ensuring Citizen1 is in department
        /// Endpoint: GET:/v1/Department/{id}
        /// </summary>
        [Fact, Priority(10)]
        public async void TestDepartmentEnsureCitizen2IsInDepartment()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.SuperUserUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.Contains(content["data"]["members"], x => x["displayName"].ToString() == _departmentFixture.Citizen2Username);
            Assert.Contains(content["data"]["members"], x => x["userId"].ToString() == TestExtension.GetUserIdAsync(_factory, _departmentFixture.Citizen2Username, _departmentFixture.Password).Result);
        }

        /// <summary>
        /// Testing adding pictogram with department
        /// Endpoint: POST:/v1/Pictogram
        /// </summary>
        [Fact, Priority(11)]
        public async void TestDepartmentCanDepartmentAddPictogram()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.NewPictograms[0], Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(content["data"]);
        }

        /// <summary>
        /// Testing trying to add pictogram to Department with Citizen1
        /// Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        /// NOTE: this endpoint is deprecated
        /// </summary>
        [Fact, Priority(12)]
        public async void TestDepartmentCanAddPictogramToDepartmentWithCitizen1ShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}/resource/{await TestExtension.GetPictogramIdAsync(_factory, _departmentFixture.NewPictogramTitles[0], _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}"),
                Method = HttpMethod.Post,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen1Username, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing trying to add pictogram to Department with Citizen2
        /// Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        /// NOTE: this endpoint is deprecated
        /// </summary>
        [Fact, Priority(13)]
        public async void TestDepartmentCanAddPictogramToDepartmentWithCitizen2ShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}/resource/{await TestExtension.GetPictogramIdAsync(_factory, _departmentFixture.NewPictogramTitles[0], _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}"),
                Method = HttpMethod.Post,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen2Username, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing trying to add new pictogram to Department with Citizen1
        /// Endpoint: POST:/v1/Pictogram
        /// </summary>
        [Fact, Priority(14)]
        public async void TestDepartmentCanAddNewPictogramToDepartmentWithCitizen1()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.NewPictograms[1], Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen1Username, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(content["data"]);

            request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}/resource/{content["data"]["id"]}"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.NewPictograms[1], Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen1Username, _departmentFixture.Password)}");

            response = await _client.SendAsync(request);
            content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing trying to add new pictogram to Department with Citizen2
        /// Endpoint: POST:/v1/Pictogram
        /// </summary>
        [Fact, Priority(15)]
        public async void TestDepartmentCanAddNewPictogramToDepartmentWithCitizen2()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Pictogram"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.NewPictograms[2], Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen2Username, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(content["data"]);

            request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}/resource/{content["data"]["id"]}"),
                Method = HttpMethod.Post,
                Content = new StringContent(_departmentFixture.NewPictograms[1], Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen2Username, _departmentFixture.Password)}");

            response = await _client.SendAsync(request);
            content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing ensuring no pictograms have been added to Department
        /// Endpoint: GET:/v1/Department/{id}
        /// NOTE: this endpoint is deprecated
        /// </summary>
        [Fact, Priority(16)]
        public async void TestDepartmentEnsureNoPictogramsAddedToDepartment()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.Citizen2Username, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.NotNull(content["data"]["resources"]);
            Assert.DoesNotContain(content["data"]["resources"], x => x["id"] != null);
        }

        /// <summary>
        /// Testing adding pictogram to Department
        /// Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        /// NOTE: this endpoint is deprecated
        /// </summary>
        [Fact, Priority(17)]
        public async void TestDepartmentCanAddPictogramToDepartment()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}/resource/{await TestExtension.GetPictogramIdAsync(_factory, _departmentFixture.NewPictogramTitles[0], _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}"),
                Method = HttpMethod.Post,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing ensuring pictogram is added to Department
        /// Endpoint: GET:/v1/Department/{id
        /// NOTE: this endpoint is deprecated
        /// </summary>
        [Fact, Priority(18)]
        public async void TestDepartmentEnsurePictogramAddedToDepartment()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.SuperUserUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content["data"]);
            Assert.NotNull(content["data"]["resources"]);
            Assert.Contains(content["data"]["resources"], x => x.ToObject<long>() == TestExtension.GetPictogramIdAsync(_factory, _departmentFixture.NewPictogramTitles[0], _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword).Result);
        }

        /// <summary>
        /// Testing removing pictogram from Department with Department
        /// Endpoint: POST:/v1/Department/resource/{resourceId}
        /// NOTE: this endpoint is deprecated
        /// </summary>
        [Fact, Priority(19)]
        public async void TestDepartmentCanRemovePictogramFromDepartmentWithDepartment()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/resource/{await TestExtension.GetPictogramIdAsync(_factory, _departmentFixture.NewPictogramTitles[0], _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}"),
                Method = HttpMethod.Delete,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing if superuser can get names of people in a department by department id
        /// Endpoint: GET:/v1/Department/{id}/citizens
        /// </summary>
        [Fact, Priority(20)]
        public async void TestDepartmentCanGetCitizenNames()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}/citizens"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.DepartmentUsername, _departmentFixture.DepartmentPassword)}");

            var response = await _client.SendAsync(request);
            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing changing name of department as superuser
        /// Endpoint: PUT:/v1/Department/{id}/name
        /// </summary>
        [Fact, Priority(21)]
        public async void TestDepartmentCanChangeNameOfDepartmentAsSuperuser()
        {
            var data = $"{{'id': '{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}', 'name': '{_departmentFixture.DepartmentRename}'}}";
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentUsername)}/name"),
                Method = HttpMethod.Put,
                Content = new StringContent(data, Encoding.UTF8, "application/json-patch+json")
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.SuperUserUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Testing removing created department by id as superuser
        /// Endpoint: DELETE:/v1/Department/{id}
        /// </summary>
        [Fact, Priority(22)]
        public async void TestDepartmentCanDeleteDepartmentAsSuperuser()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Department/{await TestExtension.GetDepartmentIdAsync(_factory, _departmentFixture.DepartmentRename)}"),
                Method = HttpMethod.Delete,
            };
            request.Headers.Add("Authorization", $"Bearer {await TestExtension.GetTokenAsync(_factory, _departmentFixture.SuperUserUsername, _departmentFixture.Password)}");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        // Content = new StringContent(_departmentFixture.NewPictograms[2], Encoding.UTF8, "application/json")
    }
}
