from requests import get, post, put, delete
from testlib import order, BASE_URL, GIRAFTestCase


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

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestAuthorization, cls).tearDownClass()

    def setUp(self) -> None:
        """
        Setup necessary data and states before each test
        """
        pass

    def tearDown(self) -> None:
        """
        Remove or resolve necessary data and states after each test
        """
        pass

    """
    Account endpoints
    """
    @order
    def test_auth_POST_account_set_password(self):
        """
        Testing setting password

        Endpoint: POST:/v1/Account/login
        """
        response = post(f'{BASE_URL}v1/User/0/Account/password').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_POST_account_change_password(self):
        """
        Testing changing password

        Endpoint: PUT:/v1/User/{id}/Account/password
        """
        response = put(f'{BASE_URL}v1/User/0/Account/password').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_account_password_reset_token(self):
        """
        Testing getting password reset token

        Endpoint: GET:/v1/User/{id}/Account/password-reset-token
        """
        response = get(f'{BASE_URL}v1/User/0/Account/password-reset-token').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Department endpoints
    """
    @order
    def test_auth_POST_department(self):
        """
        Testing creating new department

        Endpoint: POST:/v1/Department
        """
        response = post(f'{BASE_URL}v1/Department').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_department_citizens(self):
        """
        Testing getting department citizens

        Endpoint: GET:/v1/Department/{id}/citizens
        """
        response = get(f'{BASE_URL}v1/Department/0/citizens').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_POST_department_user(self):
        """
        Testing adding user to department

        Endpoint: POST:/v1/Department/{departmentId}/user/{userId}
        """
        response = post(f'{BASE_URL}v1/Department/0/user/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Error endpoints
    """
    @order
    def test_auth_GET_error(self):
        """
        Testing GET error

        Endpoint: GET:/v1/Error
        """
        response = get(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_POST_error(self):
        """
        Testing POST error

        Endpoint: POST:/v1/Error
        """
        response = post(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Pictogram endpoints
    """
    @order
    def test_auth_GET_all_pictograms(self):
        """
        Testing getting all pictograms

        Endpoint: GET:/v1/Pictogram
        """
        response = get(f'{BASE_URL}v1/Pictogram').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_POST_pictogram(self):
        """
        Testing creating new pictogram

        Endpoint: POST:/v1/Pictogram
        """
        response = post(f'{BASE_URL}v1/Pictogram').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_pictogram_by_id(self):
        """
        Testing getting pictogram by id with

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_PUT_update_pictogram_info(self):
        """
        Testing updating pictogram info

        Endpoint: PUT:/v1/Pictogram/{id}
        """
        response = put(f'{BASE_URL}v1/Pictogram/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_DELETE_pictogram_by_id(self):
        """
        Testing deleting pictogram by id

        Endpoint: DELETE:/v1/Pictogram/{id}
        """
        response = delete(f'{BASE_URL}v1/Pictogram/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_pictogram_image_by_id(self):
        """
        Testing getting pictogram image by id

        Endpoint: GET:/v1/Pictogram/{id}/image
        """
        response = get(f'{BASE_URL}v1/Pictogram/0/image').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_PUT_pictogram_image_by_id(self):
        """
        Testing updating pictogram image by id

        Endpoint: PUT:/v1/Pictogram/{id}/image
        """
        response = put(f'{BASE_URL}v1/Pictogram/0/image').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_pictogram_image_raw_by_id(self):
        """
        Testing getting raw pictogram image by id

        Endpoint: GET:/v1/Pictogram/{id}/image/raw
        """
        response = get(f'{BASE_URL}v1/Pictogram/0/image/raw').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Status endpoints
    """
    @order
    def test_auth_GET_status(self):
        """
        Testing getting API status

        Endpoint: GET:/v1/Status
        """
        response = get(f'{BASE_URL}v1/Status').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_auth_GET_database_status(self):
        """
        Testing getting API database status

        Endpoint: GET:/v1/Status/database
        """
        response = get(f'{BASE_URL}v1/Status/database').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    """
    User endpoints
    """
    @order
    def test_auth_GET_user(self):
        """
        Testing getting current user

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_PUT_update_user(self):
        """
        Testing updating user

        Endpoint: PUT:/v1/User/{id}
        """
        response = put(f'{BASE_URL}v1/User/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_user_settings_by_user_id(self):
        """
        Testing getting user settings by user id

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/0/settings').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_PUT_update_user_settings_by_user_id(self):
        """
        Testing updating user settings by user id

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/0/settings').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_user_icon_by_user_id(self):
        """
        Testing getting user icon by user id

        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASE_URL}v1/User/0/icon').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_PUT_update_user_icon_by_user_id(self):
        """
        Testing updating user icon by user id

        Endpoint: PUT:/v1/User/{id}/icon
        """
        response = put(f'{BASE_URL}v1/User/0/icon').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_DELETE_user_icon_by_user_id(self):
        """
        Testing deleting user icon by user id

        Endpoint: DELETE:/v1/User/{id}/icon
        """
        response = delete(f'{BASE_URL}v1/User/0/icon').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_user_icon_raw_by_user_id(self):
        """
        Testing getting raw user icon by user id

        Endpoint: GET:/v1/User/{id}/icon/raw
        """
        response = get(f'{BASE_URL}v1/User/0/icon/raw').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_user_citizens_by_user_id(self):
        """
        Testing getting user citizens by user id

        Endpoint: GET:/v1/User/{id}/citizens
        """
        response = get(f'{BASE_URL}v1/User/0/citizens').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_auth_GET_user_guardians_by_user_id(self):
        """
        Testing getting user guardians by user id

        Endpoint: GET:/v1/User/{id}/guardians
        """
        response = get(f'{BASE_URL}v1/User/0/guardians').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Other tests
    """
    @order
    def test_auth_expired_authorization(self):
        """
        Testing arbitrary request with expired token

        Endpoint: GET:/v1/User
        """
        headers = {'Authorization': f'Bearer {self.EXPIRED_TOKEN}'}
        response = get(f'{BASE_URL}v1/User', headers=headers).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')
