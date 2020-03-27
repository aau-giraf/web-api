from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase, parse_image

gunnar_username = f'Gunnar{time.time()}'
gunnar_id = ''
gunnar_token = ''
week_year = 0
week_number = 0
lee_token = ''


class TestWeekController(GIRAFTestCase):
    """
    Testing API requests on Pictogram endpoints
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

    def tearDown(self) -> None:
        """
        Remove or resolve necessary data and states after each test
        """
        pass

    def day(self, day_number: int) -> dict:
        return {'day': day_number, 'activities': [{'pictogram': {'title': 'sig', 'id': 4, 'state': 1,
                                                                 'lastEdit': '2018-03-28T10:47:51.628333',
                                                                 'accessLevel': 0}, 'order': 0}]}

    def different_day(self, day_number: int) -> dict:
        return {'day': day_number, 'activities': [{'pictogram': {'title': 'JUNK', 'id': 2, 'state': 3,
                                                                 'lastEdit': '2017-03-28T10:47:51.628333',
                                                                 'accessLevel': 0}, 'order': 0}]}

    def week(self, days: list) -> dict:
        return {'thumbnail': {'title': 'simpelt', 'id': 5, 'lastEdit': '2018-04-20T13:17:51.033Z', 'accessLevel': 0},
                'name': 'Coronabots roll out', 'id': 0, 'days': days}

    @order
    def test_week_login_as_lee(self):
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
    def test_week_register_gunnar(self):
        """
        Testing registering Gunnar
        """
        data = {'username': gunnar_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(lee_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_week_login_as_gunnar(self):
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
    def test_week_get_gunnar_id(self):
        """
        Testing getting Gunnar's id
        """
        global gunnar_id
        response = get(f'{BASE_URL}v1/User', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        gunnar_id = response['data']['id']

    @order
    def test_week_get_no_weeks(self):
        """
        Testing getting empty list of weeks
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/week', headers=auth(gunnar_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NoWeekScheduleFound')

    @order
    def test_week_add_week(self):
        """
        Testing adding week
        """
        global week_year, week_number
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/week/2018/11', headers=auth(lee_token),
                       json=self.correct_week).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2018, response['data']['weekYear'])
        self.assertEqual(11, response['data']['weekNumber'])
        week_year = response['data']['weekYear']
        week_number = response['data']['weekNumber']

    @order
    def test_week_get_new_weeks(self):
        """
        Testing getting list containing new week
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/week', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2018, response['data'][0]['weekYear'])
        self.assertEqual(11, response['data'][0]['weekNumber'])

    @order
    def test_week_adding_week_too_many_days(self):
        """
        Testing adding week containing too many days
        """
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/week/2018/12', headers=auth(lee_token),
                       json=self.too_many_days_week).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidAmountOfWeekdays')

    @order
    def test_week_ensure_week_too_many_days_not_added(self):
        """
        Testing ensuring week containing too many days was not added
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/week', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(1, len(response['data']))

    @order
    def test_week_add_week_invalid_enums(self):
        """
        Testing adding new week with invalid enums
        """
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/week/2018/13', headers=auth(lee_token),
                       json=self.bad_enum_value_week).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'InvalidDay')

    @order
    def test_week_ensure_week_invalid_enums_not_added(self):
        """
        Testing ensuring week with invalid enums was not added
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/week', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(1, len(response['data']))

    @order
    def test_week_update_week(self):
        """
        Testing updating week
        """
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/week/{week_year}/{week_number}', headers=auth(lee_token),
                       json=self.different_correct_week).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])

    @order
    def test_week_ensure_updated_week(self):
        """
        Testing ensuring week has been updated
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/week/{week_year}/{week_number}',
                       headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(response['data']['days'][i]['activities'][0]['pictogram']['id'] == 2 for i in range(1, 6))

    @order
    def test_week_delete_week(self):
        """
        Testing deleting week
        """
        response = delete(f'{BASE_URL}v1/User/{gunnar_id}/week/{week_year}/{week_number}',
                          headers=auth(lee_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_week_ensure_week_deleted(self):
        """
        Testing ensuring week has been deleted
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/week/', headers=auth(gunnar_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NoWeekScheduleFound')
