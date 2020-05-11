from requests import get, post, put, delete, patch
from testlib import order, BASE_URL, GIRAFTestCase, auth
from http import HTTPStatus

user_token = ""
user_id = ""

class TestAuthorization(GIRAFTestCase):
    """
    Testing API requests without authorization
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data and state when class is loaded
        """
        super(TestAuthorization, cls).setUpClass()
        print(f'file:/{__file__}\n')
        cls.EXPIRED_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1YmM0YzAzMC1mOGQxLTRhYTAtOTBlOC05MTNh" \
                            "MDYwOTA4YWIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltc" \
                            "y9uYW1laWRlbnRpZmllciI6Ijg0MTJkOTk1LWIzODEtNGY4My1iZDI1LWU5ODY2NzBiNTdkOSIsImV4cCI6MT" \
                            "UyNTMwMzQyNSwiaXNzIjoibm90bWUiLCJhdWQiOiJub3RtZSJ9.8KXRRqF3B5s8tUki7u5j0TqK-189QIpApd" \
                            "OC6aSxOms"
        cls.user_name = "Kurt"
        cls.weekplan_Name = 'Normal Uge'
        cls.week_Year = 0
        cls.week_Number = 0
        cls.week_Day_Number = 1
        cls.activity_Id = 1  # Might need to be existing activity id. chose 2
        cls.department_Id = 1  # Might need another number
        cls.citizen_Id = 'Jane Doe'



    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestAuthorization, cls).tearDownClass()

    """
    Account endpoints
    """

    @order 
    def test_user_kurt_login(self):
        """
        Testing logging in as Kurt
        
        Endpoint: POST:/v1/Account/login
        """
        global user_token
        data = {'username': self.user_name, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        user_token = response_body['data']        


    @order
    def test_user_kurt_get_id(self):
        """
        Testing getting Citizen2's id

        Endpoint: GET:/v1/User
        """
        global user_token
        global user_id
        response = get(f'{BASE_URL}v1/User', headers=auth(user_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])

        user_id = response_body['data']['id']

    @order
    def test_auth_POST_account_set_password_should_fail(self):
        """
        Testing setting password

        Endpoint: POST:/v1/User/{id}/Account/password
        """
        global user_id
        data = {'unrelated': 'unrelated'}
        response = post(f'{BASE_URL}v1/User/{user_id}/Account/password', json=data)
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'MissingProperties')

    @order
    def test_auth_PUT_account_change_password_should_fail(self):
        """
        Testing changing password

        Endpoint: PUT:/v1/User/{id}/Account/password
        """
        response = put(f'{BASE_URL}v1/User/{user_id}/Account/password')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_auth_GET_account_password_reset_token_should_fail(self):
        """
        Testing getting password reset token

        Endpoint: GET:/v1/User/{id}/Account/password-reset-token
        """
        response = get(f'{BASE_URL}v1/User/{user_id}/Account/password-reset-token')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_auth_POST_account_login_should_fail(self):
        """
        Testing account login

        Endpoint: POST:/v1/Account/login
        """
        response = post(f'{BASE_URL}v1/Account/login', json={})
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'MissingProperties')

    @order
    def test_auth_POST_register_account_should_fail(self):
        """
        Testing account registration without authentication token

        Endpoint: POST:/v1/Account/register
        """
        response = post(f'{BASE_URL}v1/Account/register')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_auth_DELETE_account_should_fail(self):
        """
        Testing account DELETION with authentication token

        Endpoint: DELETE:/v1/Account/user/{id}
        """
        response = delete(f'{BASE_URL}v1/Account/user/{user_id}')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

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
        response = patch(f'{BASE_URL}v2/Activity/{user_id}/update', json=data)
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
    def test_auth_POST_department_user_should_fail(self):
        """
        Testing adding user to department

        Endpoint: POST:/v1/Department/{departmentId}/user/{userId}
        """
        response = post(f'{BASE_URL}v1/Department/{self.department_Id}/user/{user_id}')
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

        Endpoint: GET:/v2/User/{userId}/week
        """
        response = get(f'{BASE_URL}v2/User/{user_id}/week')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_auth_GET_user_id_week_v1_should_fail(self):
        """
        Testing GET on user specific week v1

        Endpoint: GET:/v1/User/{userId}/week
        """
        response = get(f'{BASE_URL}v1/User/{user_id}/week')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_auth_GET_user_id_weekyear_weeknumber_should_fail(self):
        """
        Testing GET on user specific weekyear and number

        Endpoint: GET:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = get(f'{BASE_URL}v1/User/{user_id}/week/{self.week_Year}/{self.week_Number}')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_auth_PUT_user_id_weekyear_weeknumber_should_fail(self):
        """
        Testing PUT on user specific weekyear and number

        Endpoint: PUT:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = put(f'{BASE_URL}v1/User/{user_id}/week/{self.week_Year}/{self.week_Number}')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_auth_DELETE_user_id_weekyear_weeknumber_should_fail(self):
        """
        Testing DELETE on user specific weekyear and number

        Endpoint: DELETE:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = delete(f'{BASE_URL}v1/User/{user_id}/week/{self.week_Year}/{self.week_Number}')
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
