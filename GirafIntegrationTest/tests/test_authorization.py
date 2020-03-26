from unittest import TestCase
from requests import get, post, put, delete
import json
from tests import order, BASE_URL


class TestAuthorization(TestCase):
    """
    Tests for various API requests without authorization
    """
    EXPIRED_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1YmM0YzAzMC1mOGQxLTRhYTAtOTBlOC05MTNhMDYwOTA4Y" \
                    "WIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZml" \
                    "lciI6Ijg0MTJkOTk1LWIzODEtNGY4My1iZDI1LWU5ODY2NzBiNTdkOSIsImV4cCI6MTUyNTMwMzQyNSwiaXNzIjoibm90b" \
                    "WUiLCJhdWQiOiJub3RtZSJ9.8KXRRqF3B5s8tUki7u5j0TqK-189QIpApdOC6aSxOms"

    @classmethod
    def setUpClass(cls) -> None:
        """
        Pretty messages printed when class is initialized
        """
        print('\033[33m' + 'Testing API requests without authorization' + '\033[0m')
        print('file://tests/test_authorization.py\n')

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Pretty messages printed when class tests are done running
        """
        print('\n----------------------------------------------------------------------\n')

    def setUp(self) -> None:
        pass

    def tearDown(self) -> None:
        pass

    """
    Account endpoints
    """
    @order
    def test_POST_account_set_password_no_auth(self):
        """
        Testing setting password
        """
        response = json.loads(post(f'{BASE_URL}v1/Account/password').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_account_change_password_no_auth(self):
        """
        Testing changing password
        """
        response = json.loads(put(f'{BASE_URL}v1/Account/password').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_account_password_reset_token_no_auth(self):
        """
        Testing getting password reset token
        """
        response = json.loads(get(f'{BASE_URL}v1/Account/Graatand/password-reset-token').content.decode('utf-8'))
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
        response = json.loads(post(f'{BASE_URL}v1/Department').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_department_citizens_no_auth(self):
        """
        Testing getting department citizens
        """
        response = json.loads(get(f'{BASE_URL}v1/Department/0/citizens').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_department_user_no_auth(self):
        """
        Testing adding user to department
        """
        response = json.loads(post(f'{BASE_URL}v1/Department/0/user/0').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_DELETE_department_user_no_auth(self):
        """
        Testing deleting user from department
        """
        response = json.loads(delete(f'{BASE_URL}v1/Department/0/user/0').content.decode('utf-8'))
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
        response = json.loads(get(f'{BASE_URL}v1/Error').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_error_no_auth(self):
        """
        Testing POST error
        """
        response = json.loads(post(f'{BASE_URL}v1/Error').content.decode('utf-8'))
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
        response = json.loads(get(f'{BASE_URL}v1/Pictogram').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_POST_pictogram_no_auth(self):
        """
        Testing creating new pictogram
        """
        response = json.loads(post(f'{BASE_URL}v1/Pictogram').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_pictogram_by_id_no_auth(self):
        """
        Testing getting pictogram by id with
        """
        response = json.loads(get(f'{BASE_URL}v1/Pictogram/0').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_pictogram_info_no_auth(self):
        """
        Testing updating pictogram info
        """
        response = json.loads(put(f'{BASE_URL}v1/Pictogram/0').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_DELETE_pictogram_by_id_no_auth(self):
        """
        Testing deleting pictogram by id
        """
        response = json.loads(delete(f'{BASE_URL}v1/Pictogram/0').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_pictogram_image_by_id_no_auth(self):
        """
        Testing getting pictogram image by id
        """
        response = json.loads(get(f'{BASE_URL}v1/Pictogram/0/image').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_pictogram_image_by_id_no_auth(self):
        """
        Testing updating pictogram image by id
        """
        response = json.loads(put(f'{BASE_URL}v1/Pictogram/0/image').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_pictogram_image_raw_by_id_no_auth(self):
        """
        Testing getting raw pictogram image by id
        """
        response = json.loads(get(f'{BASE_URL}v1/Pictogram/0/image/raw').content.decode('utf-8'))
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
        response = json.loads(get(f'{BASE_URL}v1/Status').content.decode('utf-8'))
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_GET_database_status_no_auth(self):
        """
        Testing getting API database status
        """
        response = json.loads(get(f'{BASE_URL}v1/Status/database').content.decode('utf-8'))
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    """
    User endpoints
    """
    @order
    def test_GET_user_no_auth(self):
        """
        Testing getting user
        """
        response = json.loads(get(f'{BASE_URL}v1/User').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_user_no_auth(self):
        """
        Testing updating user
        """
        response = json.loads(put(f'{BASE_URL}v1/User/0').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_icon_by_user_id_no_auth(self):
        """
        Testing getting user icon by user id
        """
        response = json.loads(get(f'{BASE_URL}v1/User/0/icon').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_icon_raw_by_user_id_no_auth(self):
        """
        Testing getting raw user icon by user id
        """
        response = json.loads(get(f'{BASE_URL}v1/User/0/icon/raw').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_user_icon_by_user_id_no_auth(self):
        """
        Testing updating user icon by user id
        """
        response = json.loads(put(f'{BASE_URL}v1/User/0/icon').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_DELETE_user_icon_by_user_id_no_auth(self):
        """
        Testing deleting user icon by user id
        """
        response = json.loads(delete(f'{BASE_URL}v1/User/0/icon').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_citizens_by_user_id_no_auth(self):
        """
        Testing getting user citizens by user id
        """
        response = json.loads(get(f'{BASE_URL}v1/User/0/citizens').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_guardians_by_user_id_no_auth(self):
        """
        Testing getting user guardians by user id
        """
        response = json.loads(get(f'{BASE_URL}v1/User/0/guardians').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_GET_user_settings_by_user_id_no_auth(self):
        """
        Testing getting user settings by user id
        """
        response = json.loads(get(f'{BASE_URL}v1/User/0/settings').content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_PUT_update_user_settings_by_user_id_no_auth(self):
        """
        Testing updating user settings by user id
        """
        response = json.loads(put(f'{BASE_URL}v1/User/0/settings').content.decode('utf-8'))
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
        response = json.loads(get(f'{BASE_URL}v1/User', headers=headers).content.decode('utf-8'))
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')
