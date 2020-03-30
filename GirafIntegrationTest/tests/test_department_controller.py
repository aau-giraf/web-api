from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase

super_user_token = ''
guardian_token = ''
citizen1_token = ''
citizen2_token = ''
citizen2_username = f'Gunnar{time.time()}'
citizen2_id = ''
department_username = f'Dalgaardsholmstuen{time.time()}'
department_id = ''
department_token = ''
department_count = 0
pictograms = {}


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
        cls.DEPARTMENT = {'id': 0, 'name': department_username, 'members': [], 'resources': []}
        cls.NEW_PICTOGRAMS = [{'accessLevel': 3, 'title': 'Cyclopean', 'id': -1,
                               'lastEdit': '2018-03-19T10:40:26.587Z'},
                              {'accessLevel': 1, 'title': '$ sudo rm -rf', 'id': -1,
                               'lastEdit': '2018-03-19T10:40:26.587Z'},
                              {'accessLevel': 1, 'title': '$ telnet nsa.gov', 'id': -1,
                               'lastEdit': '2018-03-19T10:40:26.587Z'}]

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestAccountController, cls).tearDownClass()

    @order
    def test_department_login_as_super_user(self):
        """
        Testing logging in as Super User

        Endpoint: POST:/v1/Account/login
        """
        global super_user_token
        data = {'username': 'Lee', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        super_user_token = response['data']

    @order
    def test_department_login_as_guardian(self):
        """
        Testing logging in as Guardian

        Endpoint: POST:/v1/Account/login
        """
        global guardian_token
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        guardian_token = response['data']

    @order
    def test_department_login_as_citizen1(self):
        """
        Testing logging in as Citizen1

        Endpoint: POST:/v1/Account/login
        """
        global citizen1_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        citizen1_token = response['data']

    @order
    def test_department_get_department_list(self):
        """
        Testing getting list of departments

        Endpoint: GET:/v1/Department
        """
        global department_count
        response = get(f'{BASE_URL}v1/Department').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        department_count = len(response['data'])

    @order
    def test_department_create_department_as_guardian(self):
        """
        Testing trying to create new department as Guardian

        Endpoint: POST:/v1/Department
        """
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(guardian_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_department_create_department_as_citizen1(self):
        """
        Testing trying to create new department as Citizen1

        Endpoint: POST:/v1/Department
        """
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(citizen1_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_department_get_department_count(self):
        """
        Testing ensuring no department has been created

        Endpoint: GET:/v1/Department
        """
        response = get(f'{BASE_URL}v1/Department').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(department_count, len(response['data']))

    @order
    def test_department_create_department_as_super_user(self):
        """
        Testing trying to create new department as Super User

        Endpoint: POST:/v1/Department
        """
        global department_id
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(super_user_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertIsNotNone(response['data']['id'])
        department_id = response['data']['id']

    @order
    def test_department_login_as_department(self):
        """
        Testing logging in as newly created department

        Endpoint: POST:/v1/Account/login
        """
        global department_token
        data = {'username': department_username, 'password': '0000'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        department_token = response['data']

    @order
    def test_department_get_department(self):
        """
        Testing getting newly created department

        Endpoint: GET:/v1/Department/{id}
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertIsNotNone(response['data']['name'])
        self.assertEqual(response['data']['name'], department_username)

    @order
    def test_department_get_nonexistent_department(self):
        """
        Testing getting department that does not exist

        Endpoint: GET:/v1/Department/{id}
        """
        response = get(f'{BASE_URL}v1/Department/-2').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'UserNotFound')

    @order
    def test_department_register_citizen2_department(self):
        """
        Testing registering Citizen1 in newly created department

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': citizen2_username, 'password': 'password', 'role': 'Citizen', 'departmentId': department_id}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(super_user_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_department_login_as_citizen2_department(self):
        """
        Testing logging in as Citizen1

        Endpoint: POST:/v1/Account/register
        """
        global citizen2_token
        data = {'username': citizen2_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        citizen2_token = response['data']

    @order
    def test_department_get_citizen2_id_department(self):
        """
        Testing getting Citizen1's id

        Endpoint: GET:/v1/User
        """
        global citizen2_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen2_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertIsNotNone(response['data']['id'])
        citizen2_id = response['data']['id']

    @order
    def test_department_ensure_citizen2_in_department(self):
        """
        Testing ensuring Citizen1 is in department

        Endpoint: GET:/v1/Department/{id}
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x['userName'] == citizen2_username for x in response['data']['members']))
        self.assertTrue(any(x['userId'] == citizen2_id for x in response['data']['members']))

    @order
    def test_department_department_add_pictogram(self):
        """
        Testing adding pictogram with department

        Endpoint: POST:/v1/Pictogram
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[0], headers=auth(department_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        pictograms['cyclopean'] = response['data']

    @order
    def test_department_add_pictogram_to_department_with_citizen1(self):
        """
        Testing trying to add pictogram to Department with Citizen1

        Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(citizen1_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_department_add_pictogram_to_department_with_citizen2(self):
        """
        Testing trying to add pictogram to Department with Citizen1

        Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(citizen2_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_department_add_new_pictogram_to_department_with_citizen1(self):
        """
        Testing trying to add new pictogram to Department with Citizen1

        Endpoint: POST:/v1/Pictogram
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[1], headers=auth(citizen1_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        pictograms['rm'] = response['data']
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["rm"]["id"]}',
                        headers=auth(citizen1_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_department_add_new_pictogram_to_department_with_citizen2(self):
        """
        Testing trying to add new pictogram to Department with Citizen1

        Endpoint: POST:/v1/Pictogram
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[2], headers=auth(citizen2_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        pictograms['telnet'] = response['data']
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["telnet"]["id"]}',
                        headers=auth(citizen1_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_department_ensure_no_pictograms_added_to_department(self):
        """
        Testing ensuring no pictograms have been added to Department

        Endpoint: GET:/v1/Department/{id}
        NOTE: this endpoint is deprecated
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertFalse(any(x['id'] in response['data']['resources'] for x in pictograms.values()))

    @order
    def test_department_add_pictogram_to_department(self):
        """
        Testing adding pictogram to Department

        Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(department_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_department_ensure_pictogram_added_to_department(self):
        """
        Testing ensuring pictogram is added to Department

        Endpoint: GET:/v1/Department/{id}
        NOTE: this endpoint is deprecated
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x == pictograms['cyclopean']['id'] for x in response['data']['resources']))

    @order
    def test_department_remove_pictogram_from_department_with_department(self):
        """
        Testing removing pictogram from Department with Department

        Endpoint: POST:/v1/Department/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = delete(f'{BASE_URL}v1/Department/resource/{pictograms["cyclopean"]["id"]}',
                          headers=auth(department_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
