from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase

lee_token = ''
graatand_token = ''
kurt_token = ''
gunnar_token = ''
gunnar_username = f'Gunnar{time.time()}'
gunnar_id = ''
dalgaard_username = f'Dalgardsholstuen{time.time()}'
dalgaard_id = ''
dalgaard_token = ''
department_count = 0
pictograms = {}


class TestAccountController(GIRAFTestCase):
    """
    Testing API requests on Department endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestAccountController, cls).setUpClass()
        print(f'file:/{__file__}\n')
        cls.DEPARTMENT = {'id': 0, 'name': dalgaard_username, 'members': [], 'resources': []}
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
    def test_login_as_lee(self):
        """
        Testing logging in as Lee
        """
        global lee_token
        data = {'username': 'Lee', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        lee_token = response['data']

    @order
    def test_login_as_graatand(self):
        """
        Testing logging in as Graatand
        """
        global graatand_token
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        graatand_token = response['data']

    @order
    def test_login_as_kurt(self):
        """
        Testing logging in as Kurt
        """
        global kurt_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        kurt_token = response['data']

    @order
    def test_get_department_list(self):
        """
        Testing getting list of departments
        """
        global department_count
        response = get(f'{BASE_URL}v1/Department').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        department_count = len(response['data'])

    @order
    def test_create_department_as_graatand(self):
        """
        Testing trying to create new department as Graatand
        """
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(graatand_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_create_department_as_kurt(self):
        """
        Testing trying to create new department as Kurt
        """
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(kurt_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_get_department_count(self):
        """
        Testing ensuring no department has been created
        """
        response = get(f'{BASE_URL}v1/Department').json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(department_count, len(response['data']))

    @order
    def test_create_department_as_lee(self):
        """
        Testing trying to create new department as Lee
        """
        global dalgaard_id
        response = post(f'{BASE_URL}v1/Department', json=self.DEPARTMENT, headers=auth(lee_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertIsNotNone(response['data']['id'])
        dalgaard_id = response['data']['id']

    @order
    def test_login_as_department(self):
        """
        Testing logging in as newly created department
        """
        global dalgaard_token
        data = {'username': dalgaard_username, 'password': '0000'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        dalgaard_token = response['data']

    @order
    def test_get_department(self):
        """
        Testing getting newly created department
        """
        response = get(f'{BASE_URL}v1/Department/{dalgaard_id}', headers=auth(lee_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertIsNotNone(response['data']['name'])
        self.assertEqual(response['data']['name'], dalgaard_username)

    @order
    def test_get_nonexistent_department(self):
        """
        Testing getting department that does not exist
        """
        response = get(f'{BASE_URL}v1/Department/-2').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'UserNotFound')

    @order
    def test_register_gunnar_department(self):
        """
        Testing registering Gunnar in newly created department
        """
        data = {'username': gunnar_username, 'password': 'password', 'role': 'Citizen', 'departmentId': dalgaard_id}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(lee_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_login_as_gunnar_department(self):
        """
        Testing logging in as Gunnar
        """
        global gunnar_token
        data = {'username': gunnar_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        gunnar_token = response['data']

    @order
    def test_get_gunnar_id_department(self):
        """
        Testing getting Gunnar's id
        """
        global gunnar_id
        response = get(f'{BASE_URL}v1/User', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertIsNotNone(response['data']['id'])
        gunnar_id = response['data']['id']

    @order
    def test_ensure_gunnar_in_department(self):
        """
        Testing ensuring Gunnar is in department
        """
        response = get(f'{BASE_URL}v1/Department/{dalgaard_id}', headers=auth(lee_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x['userName'] == gunnar_username for x in response['data']['members']))
        self.assertTrue(any(x['userId'] == gunnar_id for x in response['data']['members']))

    @order
    def test_department_add_pictogram(self):
        """
        Testing adding pictogram with department
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[0], headers=auth(dalgaard_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        pictograms['cyclopean'] = response['data']

    @order
    def test_add_pictogram_to_dalgaard_with_kurt(self):
        """
        Testing trying to add pictogram to Dalgaardsholmstuen with Kurt

        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{dalgaard_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(kurt_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_add_pictogram_to_dalgaard_with_gunnar(self):
        """
        Testing trying to add pictogram to Dalgaardsholmstuen with Gunnar

        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{dalgaard_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(gunnar_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_add_new_pictogram_to_dalgaard_with_kurt(self):
        """
        Testing trying to add new pictogram to Dalgaardsholmstuen with Kurt
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[1], headers=auth(kurt_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        pictograms['rm'] = response['data']
        response = post(f'{BASE_URL}v1/Department/{dalgaard_id}/resource/{pictograms["rm"]["id"]}',
                        headers=auth(kurt_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_add_new_pictogram_to_dalgaard_with_gunnar(self):
        """
        Testing trying to add new pictogram to Dalgaardsholmstuen with Gunnar
        """
        global pictograms
        response = post(f'{BASE_URL}v1/Pictogram', json=self.NEW_PICTOGRAMS[2], headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        pictograms['telnet'] = response['data']
        response = post(f'{BASE_URL}v1/Department/{dalgaard_id}/resource/{pictograms["telnet"]["id"]}',
                        headers=auth(kurt_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_ensure_no_pictograms_added_to_department(self):
        """
        Testing ensuring no pictograms have been added to Dalgaardsholmstuen

        NOTE: this endpoint is deprecated
        """
        response = get(f'{BASE_URL}v1/Department/{dalgaard_id}', headers=auth(lee_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertFalse(any(x['id'] in response['data']['resources'] for x in pictograms.values()))

    @order
    def test_add_pictogram_to_dalgaard(self):
        """
        Testing adding pictogram to Dalgaardsholmstuen

        NOTE: this endpoint is deprecated
        """
        response = post(f'{BASE_URL}v1/Department/{dalgaard_id}/resource/{pictograms["cyclopean"]["id"]}',
                        headers=auth(dalgaard_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_ensure_pictogram_added_to_dalgaard(self):
        """
        Testing ensuring pictogram is added to Dalgaardsholmstuen

        NOTE: this endpoint is deprecated
        """
        response = get(f'{BASE_URL}v1/Department/{dalgaard_id}', headers=auth(lee_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x == pictograms['cyclopean']['id'] for x in response['data']['resources']))

    @order
    def test_remove_pictogram_from_department_with_dalgaard(self):
        """
        Testing removing pictogram from Dalgaardsholmstuen with Dalgaardsholmstuen

        NOTE: this endpoint is deprecated
        """
        response = delete(f'{BASE_URL}v1/Department/resource/{pictograms["cyclopean"]["id"]}',
                          headers=auth(dalgaard_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
