from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, is_sequence, GIRAFTestCase

citizen_username = f'Gunnar{time.time()}'
citizen_id = ''
citizen_token = ''
week_year = 0
week_number = 0
super_user_token = ''


class TestWeekController(GIRAFTestCase):
    """
    Testing API requests on Week endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestWeekController, cls).setUpClass()
        print(f'file:/{__file__}\n')

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestWeekController, cls).tearDownClass()

    def setUp(self) -> None:
        """
        Setup necessary data and states before each test
        """
        self.correct_week = self.week([self.day(i) for i in range(1, 8)])
        self.different_correct_week = self.week([self.different_day(i) for i in range(1, 8)])
        self.too_many_days_week = self.week([self.day(i) for i in range(1, 8)] + [self.day(3)])
        self.bad_enum_value_week = self.week([self.day(i * 10) for i in range(1, 8)])

    @staticmethod
    def day(day_number: int) -> dict:
        return {'day': day_number, 'activities': [{'pictogram': {'title': 'sig', 'id': 4, 'state': 1,
                                                                 'lastEdit': '2018-03-28T10:47:51.628333',
                                                                 'accessLevel': 0}, 'order': 0}]}

    @staticmethod
    def different_day(day_number: int) -> dict:
        return {'day': day_number, 'activities': [{'pictogram': {'title': 'JUNK', 'id': 2, 'state': 3,
                                                                 'lastEdit': '2017-03-28T10:47:51.628333',
                                                                 'accessLevel': 0}, 'order': 0}]}

    @staticmethod
    def week(days: list) -> dict:
        return {'thumbnail': {'title': 'simpelt', 'id': 5, 'lastEdit': '2018-04-20T13:17:51.033Z', 'accessLevel': 0},
                'name': 'Coronabots roll out', 'id': 0, 'days': days}

    @order
    def test_week_can_login_as_super_user(self):
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
    def test_week_can_register_citizen(self):
        """
        Testing registering Citizen

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': citizen_username, 'displayname': citizen_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(super_user_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_week_can_login_as_citizen(self):
        """
        Testing logging in as Citizen

        Endpoint: POST:/v1/Account/login
        """
        global citizen_token
        data = {'username': citizen_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        citizen_token = response['data']

    @order
    def test_week_can_get_citizen_id(self):
        """
        Testing getting Citizen's id

        Endpoint: GET:/v1/User
        """
        global citizen_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        citizen_id = response['data']['id']

    @order
    def test_week_can_get_no_weeks(self):
        """
        Testing getting empty list of weeks

        Endpoint: GET:/v1/User/{userId}/week
        """
        response = get(f'{BASE_URL}v1/User/{citizen_id}/week', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertFalse(response['data'])
        self.assertTrue(is_sequence(response['data']))

    @order
    def test_week_can_add_week(self):
        """
        Testing adding week

        Endpoint: PUT:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        global week_year, week_number
        response = put(f'{BASE_URL}v1/User/{citizen_id}/week/2018/11', headers=auth(super_user_token),
                       json=self.correct_week).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2018, response['data']['weekYear'])
        self.assertEqual(11, response['data']['weekNumber'])
        week_year = response['data']['weekYear']
        week_number = response['data']['weekNumber']

    @order
    def test_week_can_get_new_weeks(self):
        """
        Testing getting list containing new week

        Endpoint: GET:/v1/User/{userId}/week
        """
        response = get(f'{BASE_URL}v1/User/{citizen_id}/week', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2018, response['data'][0]['weekYear'])
        self.assertEqual(11, response['data'][0]['weekNumber'])

    @order
    def test_week_can_get_new_weeks_new_v2_endpoint(self):
        """
        Testing getting list containing new week v2 endpoint

        Endpoint: GET:/v2/User/{userId}/week
        """
        response = get(f'{BASE_URL}v2/User/{citizen_id}/week', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2018, response['data'][0]['weekYear'])
        self.assertEqual(11, response['data'][0]['weekNumber'])

    @order
    def test_week_can_add_week_with_too_many_days_should_fail(self):
        """
        Testing adding week containing too many days

        Endpoint: PUT:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = put(f'{BASE_URL}v1/User/{citizen_id}/week/2018/12', headers=auth(super_user_token),
                       json=self.too_many_days_week).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidAmountOfWeekdays')

    @order
    def test_week_ensure_week_with_too_many_days_not_added(self):
        """
        Testing ensuring week containing too many days was not added

        Endpoint: GET:/v1/User/{userId}/week
        """
        response = get(f'{BASE_URL}v1/User/{citizen_id}/week', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(1, len(response['data']))

    @order
    def test_week_can_add_week_with_invalid_enums_should_fail(self):
        """
        Testing adding new week with invalid enums

        Endpoint: PUT:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = put(f'{BASE_URL}v1/User/{citizen_id}/week/2018/13', headers=auth(super_user_token),
                       json=self.bad_enum_value_week).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidDay')

    @order
    def test_week_ensure_week_with_invalid_enums_not_added(self):
        """
        Testing ensuring week with invalid enums was not added

        Endpoint: GET:/v1/User/{userId}/week
        """
        response = get(f'{BASE_URL}v1/User/{citizen_id}/week', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(1, len(response['data']))

    @order
    def test_week_can_update_week(self):
        """
        Testing updating week

        Endpoint: PUT:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = put(f'{BASE_URL}v1/User/{citizen_id}/week/{week_year}/{week_number}', headers=auth(super_user_token),
                       json=self.different_correct_week).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])

    @order
    def test_week_ensure_week_is_updated(self):
        """
        Testing ensuring week has been updated

        Endpoint: GET:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = get(f'{BASE_URL}v1/User/{citizen_id}/week/{week_year}/{week_number}',
                       headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(response['data']['days'][i]['activities'][0]['pictogram']['id'] == 2 for i in range(1, 6))

    @order
    def test_week_can_delete_week(self):
        """
        Testing deleting week

        Endpoint: DELETE:/v1/User/{userId}/week/{weekYear}/{weekNumber}
        """
        response = delete(f'{BASE_URL}v1/User/{citizen_id}/week/{week_year}/{week_number}',
                          headers=auth(super_user_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_week_ensure_week_deleted(self):
        """
        Testing ensuring week has been deleted

        Endpoint: GET:/v1/User/{userId}/week
        """
        response = get(f'{BASE_URL}v1/User/{citizen_id}/week', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertFalse(response['data'])
        self.assertTrue(is_sequence(response['data']))
