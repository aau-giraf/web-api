from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase
from http import HTTPStatus

guardian_token = ''
guardian_id = ''
citizen1_token = ''
citizen1_reset_token = ''
citizen1_username = ''
citizen1_id = ''
citizen2_token = ''
citizen2_id = ''
citizen2_username = ''
department_token = ''


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

    @order
    def test_account_can_login_as_guardian(self):
        """
        Testing logging in as Guardian

        Endpoint: POST:/v1/Account/login
        """
        global guardian_token
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        
        guardian_token = response_body['data']

    @order
    def test_account_can_get_guardian_id(self):
        """
        Testing getting Guardian's id

        Endpoint: GET:/v1/User
        """
        global guardian_id
        response = get(f'{BASE_URL}v1/User/', headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])

        guardian_id = response_body['data']['id']

    @order
    def test_account_cannot_register_citizen_without_displayName(self):
        """
        Testing registering a citizen fails without displayName

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': 'myUsername', 'password': 'password',
                'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(guardian_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'MissingProperties')

    @order
    def test_account_cannot_register_citizen_with_empty_displayName(self):
        """
        Testing registering a citizen fails with empty displayName

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': 'myUsername', 'displayName': '', 'password': 'password',
                'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(guardian_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'MissingProperties')

    @order
    def test_account_can_register_citizen2(self):
        """
        Testing registering Citizen2

        Endpoint: POST:/v1/Account/register
        """
        global citizen2_username
        citizen2_username = f'Grundenberger{time.time()}'
        data = {'username': citizen2_username, 'displayName': citizen2_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.CREATED)
        self.assertIsNotNone(response_body['data'])

    @order
    def test_account_can_login_as_citizen2(self):
        """
        Testing logging in as Citizen2

        Endpoint: POST:/v1/Account/login
        """
        global citizen2_token
        data = {'username': citizen2_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        citizen2_token = response_body['data']

    @order
    def test_account_can_get_citizen2_id(self):
        """
        Testing getting Citizen2's id

        Endpoint: GET:/v1/User
        """
        global citizen2_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen2_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])

        citizen2_id = response_body['data']['id']

    @order
    def test_account_can_get_username_with_auth(self):
        """
        Testing getting username using authorization

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])

    @order
    def test_account_can_login_invalid_password_should_fail(self):
        """
        Testing logging in with invalid password

        Endpoint: POST:/v1/Account/login
        """
        data = {'username': 'Graatand', 'password': 'this-wont-work'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
   
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['message'], 'Invalid Credentials')

    @order
    def test_account_can_login_invalid_username_should_fail(self):
        """
        Testing logging in with invalid username

        Endpoint: POST:/v1/Account/login
        """
        data = {'username': 'this-wont-work-either', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['message'], 'Invalid credentials')
        

    @order
    def test_account_can_register_citizen1_should_fail(self):
        """
        Testing registering Citizen1 with no authorization

        Endpoint: POST:/v1/Account/register
        """
        global citizen1_username
        citizen1_username = f'Gunnar{time.time()}'
        data = {'username': citizen1_username, 'displayname': citizen1_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_account_can_register_citizen1(self):
        """
        Testing registering Citizen1

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': citizen1_username, 'displayname': citizen1_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.CREATED)
        self.assertIsNotNone(response_body['data'])

    @order
    def test_account_can_login_as_citizen1(self):
        """
        Testing logging in as Citizen1

        Endpoint: POST:/v1/Account/login
        """
        global citizen1_token
        data = {'username': citizen1_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        citizen1_token = response_body['data']

    @order
    def test_account_can_get_citizen1_id(self):
        """
        Testing getting Citizen1's id

        Endpoint: GET:/v1/User
        """
        global citizen1_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen1_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])
        citizen1_id = response_body['data']['id']

    @order
    def test_account_can_get_citizen1_username(self):
        """
        Testing getting Citizen1's username

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen1_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])

    @order
    def test_account_can_get_citizen1_role(self):
        """
        Testing getting Citizen1's role

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen1_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])

    @order
    def test_account_can_login_as_department(self):
        """
        Testing logging in as Department

        Endpoint: POST:/v1/Account/login
        """
        global department_token
        data = {'username': 'Tobias', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        department_token = response_body['data']

    @order
    def test_account_can_delete_guardian_with_citizen2_should_fail(self):
        """
        Testing deleting Guardian with Citizen2

        Endpoint: DELETE:/v1/Account/user/{id}
        """
        response = delete(f'{BASE_URL}v1/Account/user/{guardian_id}', headers=auth(citizen2_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'Forbidden')

    @order
    def test_account_can_delete_citizen2(self):
        """
        Testing deleting Citizen2

        Endpoint: DELETE:/v1/Account/user/{id}
        """
        response = delete(f'{BASE_URL}v1/Account/user/{citizen2_id}', headers=auth(guardian_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)

    @order
    def test_account_can_login_as_deleted_citizen2_should_fail(self):
        """
        Testing logging in as Citizen2 after deletion

        Endpoint: POST:/v1/Account/login
        """
        data = {'username': citizen2_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED) 
        self.assertEqual(response_body['errorKey'], 'InvalidCredentials')

    @order
    def test_account_can_use_deleted_citizen2s_token(self):
        """
        Testing Citizen2's authorization after deletion

        Endpoint: GET:/v1/User/{id}
        """
        response = get(f'{BASE_URL}v1/User/{guardian_id}', headers=auth(citizen2_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_account_can_get_citizen1_reset_token(self):
        """
        Testing getting Citizen1's password reset token with Guardian

        Endpoint: GET:/v1/User/{id}/Account/password-reset-token
        """
        global citizen1_reset_token
        response = get(f'{BASE_URL}v1/User/{citizen1_id}/Account/password-reset-token',
                       headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        citizen1_reset_token = response_body['data']

    @order
    def test_account_can_reset_citizen1_password(self):
        """
        Testing resetting Citizen1's password with Guardian

        Endpoint: POST:/v1/User/{id}/Account/password
        """
        data = {'password': 'brand-new-password', 'token': citizen1_reset_token}
        response = post(f'{BASE_URL}v1/User/{citizen1_id}/Account/password', json=data,
                        headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])

    @order
    def test_account_can_reset_citizen2_password(self):
        """
        Testing resetting Citizen2's password using Citizen1's token and Guardian's authorization

        Endpoint: POST:/v1/User/{id}/Account/password
        """
        data = {'password': 'brand-new-password', 'token': citizen1_reset_token}
        response = post(f'{BASE_URL}v1/User/{citizen2_id}/Account/password', json=data,
                        headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'UserNotFound')

    @order
    def test_account_can_reset_citizen1_password_invalid_token_should_fail(self):
        """
        Testing resetting Citizen1's password with an invalid token

        Endpoint: POST:/v1/User/{id}/Account/password
        """
        data = {'password': 'brand-new-password', 'token': 'invalid-token'}
        response = post(f'{BASE_URL}v1/User/{citizen1_id}/Account/password', json=data,
                        headers=auth(guardian_token))
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'InvalidProperties')

    @order
    def test_account_can_set_citizen1_password_invalid_token_should_fail(self):
        """
        Testing setting Citizen1's password with an invalid token

        Endpoint: PUT:/v1/User/{id}/Account/password
        """
        data = {'password': 'brand-new-password', 'token': 'invalid-token'}
        response = put(f'{BASE_URL}v1/User/{citizen1_id}/Account/password', json=data,
                       headers=auth(guardian_token))
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'MissingProperties')

    @order
    def test_account_can_set_citizen1_password_valid_token(self):
        """
        Testing setting Citizen1's password with an invalid token

        Endpoint: PUT:/v1/User/{id}/Account/password
        """
        data = {'oldPassword': 'brand-new-password', 'newPassword': citizen1_reset_token}
        response = put(f'{BASE_URL}v1/User/{citizen1_id}/Account/password', json=data,
                       headers=auth(department_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
