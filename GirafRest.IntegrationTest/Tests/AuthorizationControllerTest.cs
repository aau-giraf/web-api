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
    [Collection("Account Controller")]
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
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await AccountExtension.GetIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
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
                RequestUri = new Uri($"{BASE_URL}v2/Account/password/{await AccountExtension.GetIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
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
                RequestUri = new Uri($"{BASE_URL}v2/Account/password-reset-token/{await AccountExtension.GetIdAsync(_factory, _authorizationFixture.Username, _authorizationFixture.Password)}"),
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
                Method = HttpMethod.Post
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
            Assert.Equal("MissingProperties", content["errorKey"]);
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
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Delete
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
        public async void TestAuthPOSTWeekplannerActivityShouldFail()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v2/Account/register"),
                Method = HttpMethod.Delete
            };

            var response = await _client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }
        /*
                    """
                    Activity endpoints
                    """
                    @order
                    def test_auth_POST_weekplanner_activity_should_fail(self):
                        """
                        Testing creation of weekplanner activity without authentication token

                        Endpoint: POST:/v2/Activity/{user_id}/{weekplan_name}/{week_year}/{week_number}/{week_day_number}
                        """
                        response = post(f'{BASE_URL}v2/Activity/{user_id}/{self.weekplan_Name}/{self.week_Year}/{self.week_Number}/{self.week_Day_Number}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_DELETE_activity_should_fail(self):
                        """
                        Testing DELETION of activity without authentication token

                        Endpoint: DELETE:/v2/Activity/{user_id}/delete/{activity_id}
                        """
                        response = delete(f'{BASE_URL}v2/Activity/{user_id}/delete/{self.activity_Id}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PATCH_update_activity_should_fail(self):
                        """
                        Testing PATCHING of activity or updating it without authentication token

                        Endpoint: PATCH:/v2/Activity/{user_id}/update
                        """
                        data = {'pictogram': {'id': 6}, 'id': 1}
                        response = put(f'{BASE_URL}v2/Activity/{user_id}/update', json=data)
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    """
                    Department endpoints
                    """
                    @order
                    def test_auth_POST_department_should_fail(self):
                        """
                        Testing creating new department

                        Endpoint: POST:/v1/Department
                        """
                        response = post(f'{BASE_URL}v1/Department')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_department_citizens_should_fail(self):
                        """
                        Testing getting department citizens

                        Endpoint: GET:/v1/Department/{id}/citizens
                        """
                        response = get(f'{BASE_URL}v1/Department/{self.department_Id}/citizens')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_department_succes(self):
                        """
                        Testing GET on department

                        Endpoint: GET:/v1/Department
                        """
                        response = get(f'{BASE_URL}v1/Department')
                        self.assertEqual(response.status_code, HTTPStatus.OK)


                    @order
                    def test_auth_GET_department_id_should_fail(self):
                        """
                        Testing GET on department id

                        Endpoint: GET:/v1/Department/{id}
                        """
                        response = get(f'{BASE_URL}v1/Department/{self.department_Id}')

                        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)

                    @order
                    def test_auth_PUT_department_id_name_should_fail(self):
                        """
                        Testing PUT on department id name

                        Endpoint: PUT:/v1/Department/{id}/name
                        """
                        response = put(f'{BASE_URL}v1/Department/{self.department_Id}/name')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_DELETE_department_id_should_fail(self):
                        """
                        Testing DELETE on department id

                        Endpoint: DELETE:/v1/Department/{id}
                        """
                        response = delete(f'{BASE_URL}v1/Department/{self.department_Id}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    """
                    Error endpoints
                    """
                    @order
                    def test_auth_GET_error_should_fail(self):
                        """
                        Testing GET error

                        Endpoint: GET:/v1/Error
                        """
                        response = get(f'{BASE_URL}v1/Error')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
                        self.assertEqual(response_body['errorKey'], 'UnknownError')

                    @order
                    def test_auth_POST_error_should_fail(self):
                        """
                        Testing POST error

                        Endpoint: POST:/v1/Error
                        """
                        response = post(f'{BASE_URL}v1/Error')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
                        self.assertEqual(response_body['errorKey'], 'UnknownError')

                    @order
                    def test_auth_PUT_error_should_fail(self):
                        """
                        Testing PUT error

                        Endpoint: PUT:/v1/Error
                        """
                        response = put(f'{BASE_URL}v1/Error')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
                        self.assertEqual(response_body['errorKey'], 'UnknownError')

                    @order
                    def test_auth_DELETE_error_should_fail(self):
                        """
                        Testing DELETE error

                        Endpoint: DELETE:/v1/Error
                        """
                        response = delete(f'{BASE_URL}v1/Error')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
                        self.assertEqual(response_body['errorKey'], 'UnknownError')

                    """
                    Pictogram endpoints
                    """
                    @order
                    def test_auth_GET_all_pictograms_should_fail(self):
                        """
                        Testing getting all pictograms

                        Endpoint: GET:/v1/Pictogram
                        """
                        response = get(f'{BASE_URL}v1/Pictogram')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_POST_pictogram_should_fail(self):
                        """
                        Testing creating new pictogram

                        Endpoint: POST:/v1/Pictogram
                        """
                        response = post(f'{BASE_URL}v1/Pictogram')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_pictogram_by_id_should_fail(self):
                        """
                        Testing getting pictogram by id with

                        Endpoint: GET:/v1/Pictogram/{id}
                        """
                        response = get(f'{BASE_URL}v1/Pictogram/0')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PUT_update_pictogram_info_should_fail(self):
                        """
                        Testing updating pictogram info

                        Endpoint: PUT:/v1/Pictogram/{id}
                        """
                        response = put(f'{BASE_URL}v1/Pictogram/0')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_DELETE_pictogram_by_id_should_fail(self):
                        """
                        Testing deleting pictogram by id

                        Endpoint: DELETE:/v1/Pictogram/{id}
                        """
                        response = delete(f'{BASE_URL}v1/Pictogram/0')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_pictogram_image_by_id_should_fail(self):
                        """
                        Testing getting pictogram image by id

                        Endpoint: GET:/v1/Pictogram/{id}/image
                        """
                        response = get(f'{BASE_URL}v1/Pictogram/0/image')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PUT_pictogram_image_by_id_should_fail(self):
                        """
                        Testing updating pictogram image by id

                        Endpoint: PUT:/v1/Pictogram/{id}/image
                        """
                        response = put(f'{BASE_URL}v1/Pictogram/0/image')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_pictogram_image_raw_by_id_should_fail(self):
                        """
                        Testing getting raw pictogram image by id

                        Endpoint: GET:/v1/Pictogram/{id}/image/raw
                        """
                        response = get(f'{BASE_URL}v1/Pictogram/0/image/raw')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    """
                    Status endpoints
                    """
                    @order
                    def test_auth_GET_status(self):
                        """
                        Testing getting API status

                        Endpoint: GET:/v1/Status
                        """
                        response = get(f'{BASE_URL}v1/Status')
                        self.assertEqual(response.status_code, HTTPStatus.OK)

                    @order
                    def test_auth_GET_database_status(self):
                        """
                        Testing getting API database status

                        Endpoint: GET:/v1/Status/database
                        """
                        response = get(f'{BASE_URL}v1/Status/database')

                        self.assertEqual(response.status_code, HTTPStatus.OK)


                    @order
                    def test_auth_GET_status_version(self):
                        """
                        Testing GET status version

                        Endpoint: GET:/v1/Status/version-info
                        """
                        response = get(f'{BASE_URL}v1/Status/version-info')
                        self.assertEqual(response.status_code, HTTPStatus.OK)

                    """
                    User endpoints
                    """
                    @order
                    def test_auth_GET_user_should_fail(self):
                        """
                        Testing getting current user

                        Endpoint: GET:/v1/User
                        """
                        response = get(f'{BASE_URL}v1/User')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PUT_update_user_should_fail(self):
                        """
                        Testing updating user

                        Endpoint: PUT:/v1/User/{id}
                        """
                        response = put(f'{BASE_URL}v1/User/0')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_settings_by_user_id_should_fail(self):
                        """
                        Testing getting user settings by user id

                        Endpoint: GET:/v1/User/{id}/settings
                        """
                        response = get(f'{BASE_URL}v1/User/0/settings')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PUT_update_user_settings_by_user_id_should_fail(self):
                        """
                        Testing updating user settings by user id

                        Endpoint: PUT:/v1/User/{id}/settings
                        """
                        response = put(f'{BASE_URL}v1/User/0/settings')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_icon_by_user_id_should_fail(self):
                        """
                        Testing getting user icon by user id

                        Endpoint: GET:/v1/User/{id}/icon
                        """
                        response = get(f'{BASE_URL}v1/User/0/icon')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PUT_update_user_icon_by_user_id_should_fail(self):
                        """
                        Testing updating user icon by user id

                        Endpoint: PUT:/v1/User/{id}/icon
                        """
                        response = put(f'{BASE_URL}v1/User/0/icon')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_DELETE_user_icon_by_user_id_should_fail(self):
                        """
                        Testing deleting user icon by user id

                        Endpoint: DELETE:/v1/User/{id}/icon
                        """
                        response = delete(f'{BASE_URL}v1/User/0/icon')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_icon_raw_by_user_id_should_fail(self):
                        """
                        Testing getting raw user icon by user id

                        Endpoint: GET:/v1/User/{id}/icon/raw
                        """
                        response = get(f'{BASE_URL}v1/User/0/icon/raw')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_citizens_by_user_id_should_fail(self):
                        """
                        Testing getting user citizens by user id

                        Endpoint: GET:/v1/User/{userId}/citizens
                        """
                        response = get(f'{BASE_URL}v1/User/0/citizens')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_guardians_by_user_id_should_fail(self):
                        """
                        Testing getting user guardians by user id

                        Endpoint: GET:/v1/User/{userId}/guardians
                        """
                        response = get(f'{BASE_URL}v1/User/0/guardians')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_id_should_fail(self):
                        """
                        Testing GET user Id

                        Endpoint: GET:/v1/User/{id}
                        """
                        response = get(f'{BASE_URL}v1/User/{user_id}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_POST_user_id_citizens_citizenid_should_fail(self):
                        """
                        Testing POST as guardian for citizen

                        Endpoint: POST:/v1/User/{userId}/citizens/{citizenId}
                        """
                        response = post(f'{BASE_URL}v1/User/{user_id}/citizens/{self.citizen_Id}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    """
                    Week endpoints
                    """
                    @order
                    def test_auth_GET_user_id_week_v2_should_fail(self):
                        """
                        Testing GET on user specific week v2

                        Endpoint: GET:/v1/Week/{userId}/week
                        """
                        response = get(f'{BASE_URL}v1/Week/{user_id}/week')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_id_week_v1_should_fail(self):
                        """
                        Testing GET on user specific week v1

                        Endpoint: GET:/v1/Week/{userId}/weekName
                        """
                        response = get(f'{BASE_URL}v1/Week/{user_id}/weekName')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_user_id_weekyear_weeknumber_should_fail(self):
                        """
                        Testing GET on user specific weekyear and number

                        Endpoint: GET:/v1/Week/{userId}/{weekYear}/{weekNumber}
                        """
                        response = get(f'{BASE_URL}v1/Week/{user_id}/{self.week_Year}/{self.week_Number}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PUT_user_id_weekyear_weeknumber_should_fail(self):
                        """
                        Testing PUT on user specific weekyear and number

                        Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
                        """
                        response = put(f'{BASE_URL}v1/Week/{user_id}/{self.week_Year}/{self.week_Number}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_DELETE_user_id_weekyear_weeknumber_should_fail(self):
                        """
                        Testing DELETE on user specific weekyear and number

                        Endpoint: DELETE:/v1/Week/{userId}/{weekYear}/{weekNumber}
                        """
                        response = delete(f'{BASE_URL}v1/Week/{user_id}/{self.week_Year}/{self.week_Number}')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    """
                    WeekTemplate endpoints
                    """
                    @order
                    def test_auth_GET_weektemplate_should_fail(self):
                        """
                        Testing GET on weektemplate

                        Endpoint: GET:/v1/WeekTemplate
                        """
                        response = get(f'{BASE_URL}v1/WeekTemplate')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_POST_weektemplate_should_fail(self):
                        """
                        Testing POST on weektemplate

                        Endpoint: POST:/v1/WeekTemplate
                        """
                        response = post(f'{BASE_URL}v1/WeekTemplate')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_GET_weektemplate_id_should_fail(self):
                        """
                        Testing GET on weektemplate id

                        Endpoint: GET:/v1/WeekTemplate/{id}
                        """
                        response = get(f'{BASE_URL}v1/WeekTemplate')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_PUT_weektemplate_id_should_fail(self):
                        """
                        Testing PUT on weektemplate id

                        Endpoint: PUT:/v1/WeekTemplate/{id}
                        """
                        response = put(f'{BASE_URL}v1/WeekTemplate/123')
                        response_body = response.json()

                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    @order
                    def test_auth_DELETE_weektemplate_id_should_fail(self):
                        """
                        Testing DELETE on weektemplate id

                        Endpoint: DELETE:/v1/WeekTemplate/{id}
                        """
                        response = delete(f'{BASE_URL}v1/WeekTemplate/123')
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                    """
                    Other tests
                    """
                    @order
                    def test_auth_expired_authorization_should_fail(self):
                        """
                        Testing arbitrary request with expired token

                        Endpoint: GET:/v1/User
                        """
                        headers = {'Authorization': f'Bearer {self.EXPIRED_TOKEN}'}
                        response = get(f'{BASE_URL}v1/User', headers=headers)
                        response_body = response.json()
                        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
                        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

                */
    }
}
