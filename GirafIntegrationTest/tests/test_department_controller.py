from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase
from http import HTTPStatus

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


class TestDepartmentController(GIRAFTestCase):
    """
    Testing API requests on Department endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestDepartmentController, cls).setUpClass()
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
        super(TestDepartmentController, cls).tearDownClass()

    @order
    def test_department_can_login_as_super_user(self):
        """
        Testing logging in as Super User

        Endpoint: POST:/v2/Account/login
        """
        global super_user_token
        data = {'username': 'Lee', 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])
        super_user_token = response_body['data']

    @order
    def test_department_can_login_as_guardian(self):
        """
        Testing logging in as Guardian

        Endpoint: POST:/v2/Account/login
        """
        global guardian_token
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        guardian_token = response_body['data']

    @order
    def test_department_can_login_as_citizen1(self):
        """
        Testing logging in as Citizen1

        Endpoint: POST:/v2/Account/login
        """
        global citizen1_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        citizen1_token = response_body['data']

    @order
    def test_department_can_get_department_list(self):
        """
        Testing getting list of departments

        Endpoint: GET:/v1/Department
        """
        global department_count
        response = get(f'{BASE_URL}v1/Department')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        department_count = len(response_body['data'])

    @order
    def test_department_can_create_department_as_guardian_should_fail(self):
        """
        Testing trying to create new department as Guardian

        Endpoint: POST:/v1/Department
        """
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_department_can_create_department_as_citizen1_should_fail(self):
        """
        Testing trying to create new department as Citizen1

        Endpoint: POST:/v1/Department
        """
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_department_can_get_department_count(self):
        """
        Testing ensuring no department has been created

        Endpoint: GET:/v1/Department
        """
        response = get(f'{BASE_URL}v1/Department')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(department_count, len(response_body['data']))

    @order
    def test_department_can_create_department_as_super_user(self):
        """
        Testing trying to create new department as Super User

        Endpoint: POST:/v1/Department
        """
        global department_id
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

        self.assertIsNotNone(response_body['data']['id'])
        department_id = response_body['data']['id']

    @order
    def test_department_can_login_as_department(self):
        """
        Testing logging in as newly created department

        Endpoint: POST:/v2/Account/login
        """
        global department_token
        data = {'username': department_username, 'password': '0000'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        department_token = response_body['data']

    @order
    def test_department_can_get_new_department(self):
        """
        Testing getting newly created department

        Endpoint: GET:/v1/Department/{id}
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['name'])
        self.assertEqual(response_body['data']['name'], department_username)

    @order
    def test_department_can_get_nonexistent_department_should_fail(self):
        """
        Testing getting department that does not exist

        Endpoint: GET:/v1/Department/{id}
        """
        response = get(f'{BASE_URL}v1/Department/-2')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'UserNotFound')

    @order
    def test_department_can_register_citizen2_department(self):
        """
        Testing registering Citizen2 in newly created department

        Endpoint: POST:/v2/Account/register
        """
        data = {'username': citizen2_username, 'displayname': citizen2_username, 'password': 'password', 'role': 'Citizen', 'departmentId': department_id}
        response = post(f'{BASE_URL}v2/Account/register', json=data, headers=auth(super_user_token))
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

    @order
    def test_department_can_login_as_citizen2(self):
        """
        Testing logging in as Citizen1

        Endpoint: POST:/v2/Account/register
        """
        global citizen2_token
        data = {'username': citizen2_username, 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        citizen2_token = response_body['data']
        
    @order
    def test_department_can_get_citizen2_id(self):
        """
        Testing getting Citizen1's id

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
    def test_department_ensure_citizen2_is_in_department(self):
        """
        Testing ensuring Citizen1 is in department

        Endpoint: GET:/v1/Department/{id}
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['members'])
        self.assertTrue(any(x['displayName'] == citizen2_username for x in response_body['data']['members']))
        self.assertTrue(any(x['userId'] == citizen2_id for x in response_body['data']['members']))

    @order
    def test_department_can_department_add_pictogram(self):
        """
        Testing adding pictogram with department

        Endpoint: POST:/v1/Pictogram
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[0], headers=auth(department_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

        self.assertIsNotNone(response_body['data'])
        pictograms['cyclopean'] = response_body['data']

    @order
    def test_department_can_add_pictogram_to_department_with_citizen1_should_fail(self):
        """
        Testing trying to add pictogram to Department with Citizen1

        Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_department_can_add_pictogram_to_department_with_citizen2_should_fail(self):
        """
        Testing trying to add pictogram to Department with Citizen1

        Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(citizen2_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_department_can_add_new_pictogram_to_department_with_citizen1(self):
        """
        Testing trying to add new pictogram to Department with Citizen1

        Endpoint: POST:/v1/Pictogram
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[1], headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

        self.assertIsNotNone(response_body['data'])
        pictograms['rm'] = response_body['data']
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["rm"]["id"]}',
                        headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_department_can_add_new_pictogram_to_department_with_citizen2(self):
        """
        Testing trying to add new pictogram to Department with Citizen2

        Endpoint: POST:/v1/Pictogram
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[2], headers=auth(citizen2_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)
        self.assertIsNotNone(response_body['data'])
        pictograms['telnet'] = response_body['data']

        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["telnet"]["id"]}',
                        headers=auth(citizen1_token))

        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_department_ensure_no_pictograms_added_to_department(self):
        """
        Testing ensuring no pictograms have been added to Department

        Endpoint: GET:/v1/Department/{id}
        NOTE: this endpoint is deprecated
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['resources'])
        self.assertFalse(any(x['id'] in response_body['data']['resources'] for x in pictograms.values()))

    @order
    def test_department_can_add_pictogram_to_department(self):
        """
        Testing adding pictogram to Department

        Endpoint: POST:/v1/Department/{departmentId}/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{department_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(department_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_department_ensure_pictogram_added_to_department(self):
        """
        Testing ensuring pictogram is added to Department

        Endpoint: GET:/v1/Department/{id}
        NOTE: this endpoint is deprecated
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['resources'])
        self.assertTrue(any(x == pictograms['cyclopean']['id'] for x in response_body['data']['resources']))

    @order
    def test_department_can_remove_pictogram_from_department_with_department(self):
        """
        Testing removing pictogram from Department with Department

        Endpoint: POST:/v1/Department/resource/{resourceId}
        NOTE: this endpoint is deprecated
        """
        response = delete(f'{BASE_URL}v1/Department/resource/{pictograms["cyclopean"]["id"]}',
                          headers=auth(department_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_department_can_get_citizen_names(self):
        """
        Testing if superuser can get names of people in a department by department id

        Endpoint: GET:/v1/Department/{id}/citizens
        """
        response = get(f'{BASE_URL}v1/Department/{department_id}/citizens', headers=auth(super_user_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_department_can_change_name_of_department_as_superuser(self):
        """
        Testing changing name of department as superuser

        Endpoint: PUT:/v1/Department/{id}/name
        """
        x = auth(super_user_token)
        x['Content-Type'] = 'application/json-patch+json'
        data = {'id': department_id, 'name': 'DeleteMe'}
        response = put(f'{BASE_URL}v1/Department/{department_id}/name', json=data, headers=x)
        
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_department_can_delete_department_as_superuser(self):
        """
        Testing removing created department by id as superuser

        Endpoint: DELETE:/v1/Department/{id}
        """
        response = delete(f'{BASE_URL}v1/Department/{department_id}', headers=auth(super_user_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
