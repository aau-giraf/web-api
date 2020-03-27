from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase

graatand_token = ''
graatand_id = ''
gunnar_token = ''
gunnar_reset_token = ''
gunnar_username = ''
gunnar_id = ''
grundenberger_token = ''
grundenberger_id = ''
grundenberger_username = ''
tobias_token = ''


class TestAccountController(GIRAFTestCase):
    """
    Testing API requests on Account endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestAccountController, cls).setUpClass()
        print(f'file:/{__file__}\n')

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestAccountController, cls).tearDownClass()

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

    @order
    def test_account_login_as_graatand(self):
        """
        Testing logging in as Graatand

        Endpoint: POST:/v1/Account/login
        """
        global graatand_token
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        graatand_token = response['data']

    @order
    def test_account_get_graatand_id(self):
        """
        Testing getting Graatand's id

        Endpoint: GET:/v1/User
        """
        global graatand_id
        response = get(f'{BASE_URL}v1/User', headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        graatand_id = response['data']['id']

    @order
    def test_account_register_grundenberger(self):
        """
        Testing registering Grundenberger

        Endpoint: POST:/v1/Account/register
        """
        global grundenberger_username
        grundenberger_username = f'Grundenberger{time.time()}'
        data = {'username': grundenberger_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_account_login_as_grundenberger(self):
        """
        Testing logging in as Grundenberger

        Endpoint: POST:/v1/Account/login
        """
        global grundenberger_token
        data = {'username': grundenberger_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        grundenberger_token = response['data']

    @order
    def test_account_get_grundenberger_id(self):
        """
        Testing getting Grundenberger's id

        Endpoint: GET:/v1/User
        """
        global grundenberger_id
        response = get(f'{BASE_URL}v1/User', headers=auth(grundenberger_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        grundenberger_id = response['data']['id']

    @order
    def test_account_get_username_with_auth(self):
        """
        Testing getting username using authorization

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['username'])
        self.assertEqual(response['data']['username'], 'Graatand')

    @order
    def test_account_login_invalid_password(self):
        """
        Testing logging in with invalid password

        Endpoint: POST:/v1/Account/login
        """
        data = {'username': 'Graatand', 'password': 'this-wont-work'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidCredentials')
        self.assertIsNone(response['data'])

    @order
    def test_account_login_invalid_username(self):
        """
        Testing logging in with invalid username

        Endpoint: POST:/v1/Account/login
        """
        data = {'username': 'this-wont-work-either', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidCredentials')
        self.assertIsNone(response['data'])

    @order
    def test_account_register_gunnar_no_auth(self):
        """
        Testing registering Gunnar with no authorization

        Endpoint: POST:/v1/Account/register
        """
        global gunnar_username
        gunnar_username = f'Gunnar{time.time()}'
        data = {'username': gunnar_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_account_register_gunnar(self):
        """
        Testing registering Gunnar

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': gunnar_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_account_login_as_gunnar(self):
        """
        Testing logging in as Gunnar

        Endpoint: POST:/v1/Account/login
        """
        global gunnar_token
        data = {'username': gunnar_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        gunnar_token = response['data']

    @order
    def test_account_get_gunnar_id(self):
        """
        Testing getting Gunnar's id

        Endpoint: GET:/v1/User
        """
        global gunnar_id
        response = get(f'{BASE_URL}v1/User', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        gunnar_id = response['data']['id']

    @order
    def test_account_get_gunnar_username(self):
        """
        Testing getting Gunnar's username

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['username'], gunnar_username)

    @order
    def test_account_get_gunnar_role(self):
        """
        Testing getting Gunnar's role

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['roleName'], 'Citizen')

    @order
    def test_account_login_as_tobias(self):
        """
        Testing logging in as Tobias

        Endpoint: POST:/v1/Account/login
        """
        global tobias_token
        data = {'username': 'Tobias', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        tobias_token = response['data']

    @order
    def test_account_delete_graatand_with_grundenberger(self):
        """
        Testing deleting Graatand with Grundenberger

        Endpoint: DELETE:/v1/Account/user/{id}
        """
        response = delete(f'{BASE_URL}v1/Account/user/{graatand_id}', headers=auth(grundenberger_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_account_delete_grundenberger(self):
        """
        Testing deleting Grundenberger

        Endpoint: DELETE:/v1/Account/user/{id}
        """
        response = delete(f'{BASE_URL}v1/Account/user/{grundenberger_id}', headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_account_login_as_deleted_grundenberger(self):
        """
        Testing logging in as Grundenberger after deletion

        Endpoint: POST:/v1/Account/login
        """
        data = {'username': grundenberger_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidCredentials')
        self.assertIsNone(response['data'])

    @order
    def test_account_deleted_grundenberger_auth(self):
        """
        Testing Grundenberger's authorization after deletion

        Endpoint: GET:/v1/User/{id}
        """
        response = get(f'{BASE_URL}v1/User/{graatand_id}', headers=auth(grundenberger_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')
        self.assertIsNone(response['data'])

    @order
    def test_account_get_gunnar_reset_token(self):
        """
        Testing getting Gunnar's password reset token with Graatand

        Endpoint: GET:/v1/User/{id}/Account/password-reset-token
        """
        global gunnar_reset_token
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/Account/password-reset-token',
                       headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        gunnar_reset_token = response['data']

    @order
    def test_account_reset_gunnar_password(self):
        """
        Testing resetting Gunnar's password with Graatand

        Endpoint: POST:/v1/User/{id}/Account/password
        """
        data = {'password': 'brand-new-password', 'token': gunnar_reset_token}
        response = post(f'{BASE_URL}v1/User/{gunnar_id}/Account/password', json=data,
                        headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_account_reset_grundenberger_password(self):
        """
        Testing resetting Grundenberger's password using Gunnar's token and Graatand's authorization

        Endpoint: POST:/v1/User/{id}/Account/password
        """
        data = {'password': 'brand-new-password', 'token': gunnar_reset_token}
        response = post(f'{BASE_URL}v1/User/{grundenberger_id}/Account/password', json=data,
                        headers=auth(graatand_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'UserNotFound')

    @order
    def test_account_reset_gunnar_password_invalid_token(self):
        """
        Testing resetting Gunnar's password with an invalid token

        Endpoint: POST:/v1/User/{id}/Account/password
        """
        data = {'password': 'brand-new-password', 'token': 'invalid-token'}
        response = post(f'{BASE_URL}v1/User/{gunnar_id}/Account/password', json=data,
                        headers=auth(graatand_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidProperties')
