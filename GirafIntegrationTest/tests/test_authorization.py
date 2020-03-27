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
    def test_POST_account_set_password_no_auth(self):
        """
        Testing setting password
        """
        response = post(f'{BASE_URL}v1/User/0/Account/password').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_account_change_password_no_auth(self):
        """
        Testing changing password
        """
        response = put(f'{BASE_URL}v1/User/0/Account/password').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_account_password_reset_token_no_auth(self):
        """
        Testing getting password reset token
        """
        response = get(f'{BASE_URL}v1/User/0/Account/password-reset-token').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Department endpoints
    """
    @order
    def test_POST_department_no_auth(self):
        """
        Testing creating new department
        """
        response = post(f'{BASE_URL}v1/Department').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_department_citizens_no_auth(self):
        """
        Testing getting department citizens
        """
        response = get(f'{BASE_URL}v1/Department/0/citizens').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_department_user_no_auth(self):
        """
        Testing adding user to department
        """
        response = post(f'{BASE_URL}v1/Department/0/user/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Error endpoints
    """
    @order
    def test_GET_error_no_auth(self):
        """
        Testing GET error
        """
        response = get(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_error_no_auth(self):
        """
        Testing POST error
        """
        response = post(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Pictogram endpoints
    """
    @order
    def test_GET_all_pictograms_no_auth(self):
        """
        Testing getting all pictograms
        """
        response = get(f'{BASE_URL}v1/Pictogram').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_pictogram_no_auth(self):
        """
        Testing creating new pictogram
        """
        response = post(f'{BASE_URL}v1/Pictogram').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_pictogram_by_id_no_auth(self):
        """
        Testing getting pictogram by id with
        """
        response = get(f'{BASE_URL}v1/Pictogram/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_pictogram_info_no_auth(self):
        """
        Testing updating pictogram info
        """
        response = put(f'{BASE_URL}v1/Pictogram/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_DELETE_pictogram_by_id_no_auth(self):
        """
        Testing deleting pictogram by id
        """
        response = delete(f'{BASE_URL}v1/Pictogram/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_pictogram_image_by_id_no_auth(self):
        """
        Testing getting pictogram image by id
        """
        response = get(f'{BASE_URL}v1/Pictogram/0/image').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_pictogram_image_by_id_no_auth(self):
        """
        Testing updating pictogram image by id
        """
        response = put(f'{BASE_URL}v1/Pictogram/0/image').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_pictogram_image_raw_by_id_no_auth(self):
        """
        Testing getting raw pictogram image by id
        """
        response = get(f'{BASE_URL}v1/Pictogram/0/image/raw').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Status endpoints
    """
    @order
    def test_GET_status_no_auth(self):
        """
        Testing getting API status
        """
        response = get(f'{BASE_URL}v1/Status').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_GET_database_status_no_auth(self):
        """
        Testing getting API database status
        """
        response = get(f'{BASE_URL}v1/Status/database').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    """
    User endpoints
    """
    @order
    def test_GET_user_no_auth(self):
        """
        Testing getting current user
        """
        response = get(f'{BASE_URL}v1/User').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_user_no_auth(self):
        """
        Testing updating user
        """
        response = put(f'{BASE_URL}v1/User/0').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_settings_by_user_id_no_auth(self):
        """
        Testing getting user settings by user id
        """
        response = get(f'{BASE_URL}v1/User/0/settings').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_user_settings_by_user_id_no_auth(self):
        """
        Testing updating user settings by user id
        """
        response = put(f'{BASE_URL}v1/User/0/settings').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_icon_by_user_id_no_auth(self):
        """
        Testing getting user icon by user id
        """
        response = get(f'{BASE_URL}v1/User/0/icon').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_user_icon_by_user_id_no_auth(self):
        """
        Testing updating user icon by user id
        """
        response = put(f'{BASE_URL}v1/User/0/icon').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_DELETE_user_icon_by_user_id_no_auth(self):
        """
        Testing deleting user icon by user id
        """
        response = delete(f'{BASE_URL}v1/User/0/icon').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_icon_raw_by_user_id_no_auth(self):
        """
        Testing getting raw user icon by user id
        """
        response = get(f'{BASE_URL}v1/User/0/icon/raw').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_citizens_by_user_id_no_auth(self):
        """
        Testing getting user citizens by user id
        """
        response = get(f'{BASE_URL}v1/User/0/citizens').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_guardians_by_user_id_no_auth(self):
        """
        Testing getting user guardians by user id
        """
        response = get(f'{BASE_URL}v1/User/0/guardians').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    """
    Other tests
    """
    @order
    def test_expired_authorization(self):
        """
        Testing arbitrary request with expired token
        """
        headers = {'Authorization': f'Bearer {self.EXPIRED_TOKEN}'}
        response = get(f'{BASE_URL}v1/User', headers=headers).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')
