from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase

citizen1_token = ''
citizen1_id = ''
guardian_token = ''
guardian_id = ''
citizen2_token = ''
citizen2_username = f'Gunnar{time.time()}'
citizen2_id = ''
citizen3_token = ''
citizen3_username = f'Charlie{time.time()}'
citizen3_id = ''
super_user_token = ''
department_token = ''
wednesday_id = ''


class TestUserController(GIRAFTestCase):
    """
    Testing API requests on User endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestUserController, cls).setUpClass()
        print(f'file:/{__file__}\n')
        cls.GRAYSCALE_THEME = {'orientation': 1, 'completeMark': 2, 'cancelMark': 2, 'defaultTimer': 2,
                               'timerSeconds': 900, 'activitiesCount': None, 'theme': 2, 'nrOfDaysToDisplay': 7,
                               'greyScale': True, 'weekDayColors':
                                   [{"hexColor": "#067700", "day": 1}, {"hexColor": "#8c1086", "day": 2},
                                    {"hexColor": "#ff7f00", "day": 3}, {"hexColor": "#0017ff", "day": 4},
                                    {"hexColor": "#ffdd00", "day": 5}, {"hexColor": "#ff0102", "day": 6},
                                    {"hexColor": "#ffffff", "day": 7}]}
        cls.TIMER_ONE_HOUR = {"orientation": 1, "completeMark": 2, "cancelMark": 2, "defaultTimer": 2,
                              "timerSeconds": 3600, "activitiesCount": None, "theme": 1, "colorThemeWeekSchedules": 1,
                              "nrOfDaysToDisplay": 4, "greyScale": True, "weekDayColors":
                                  [{"hexColor": "#067700", "day": 1}, {"hexColor": "#8C1086", "day": 2},
                                   {"hexColor": "#FF7F00", "day": 3}, {"hexColor": "#0017FF", "day": 4},
                                   {"hexColor": "#FFDD00", "day": 5}, {"hexColor": "#FF0102", "day": 6},
                                   {"hexColor": "#FFFFFF", "day": 7}]}
        cls.MULTIPLE_SETTINGS = {"orientation": 2, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2,
                                 "timerSeconds": 60, "activitiesCount": 3, "theme": 3, "nrOfDaysToDisplay": 2,
                                 "greyScale": True, "weekDayColors":
                                     [{"hexColor": "#FF00FF", "day": 1}, {"hexColor": "#8C1086", "day": 2},
                                      {"hexColor": "#FF7F00", "day": 3}, {"hexColor": "#0017FF", "day": 4},
                                      {"hexColor": "#FFDD00", "day": 5}, {"hexColor": "#FF0102", "day": 6},
                                      {"hexColor": "#FFFFFF", "day": 7}]}
        cls.NEW_SETTINGS = [{"orientation": 1, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2, "theme": 1},
                            {"orientation": 1, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2, "theme": 2}]

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestUserController, cls).tearDownClass()

    @order
    def test_user_login_as_citizen1(self):
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
    def test_user_login_as_guardian(self):
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
    def test_user_login_as_super_user(self):
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
    def test_user_login_as_department(self):
        """
        Testing logging in as Department

        Endpoint: POST:/v1/Account/login
        """
        global department_token
        data = {'username': 'Tobias', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        department_token = response['data']

    @order
    def test_user_get_citizen1_id(self):
        """
        Testing getting Citizen1's id

        Endpoint: GET:/v1/User
        """
        global citizen1_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen1_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        citizen1_id = response['data']['id']

    @order
    def test_user_get_guardian_id(self):
        """
        Testing getting Guardian's id

        Endpoint: GET:/v1/User
        """
        global guardian_id
        response = get(f'{BASE_URL}v1/User', headers=auth(guardian_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        guardian_id = response['data']['id']

    @order
    def test_user_register_citizen2(self):
        """
        Testing registering Citizen2

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': citizen2_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(super_user_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_login_as_citizen2(self):
        """
        Testing logging in as Citizen2

        Endpoint: POST:/v1/Account/login
        """
        global citizen2_token
        data = {'username': citizen2_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        citizen2_token = response['data']

    @order
    def test_user_get_citizen2_id(self):
        """
        Testing getting Citizen2's id

        Endpoint: GET:/v1/User
        """
        global citizen2_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen2_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        citizen2_id = response['data']['id']

    @order
    def test_user_register_citizen3(self):
        """
        Testing registering Citizen3

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': citizen3_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(super_user_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_login_as_citizen3(self):
        """
        Testing logging in as Citizen3

        Endpoint: POST:/v1/Account/login
        """
        global citizen3_token
        data = {'username': citizen3_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        citizen3_token = response['data']

    @order
    def test_user_get_citizen3_id(self):
        """
        Testing getting Citizen3's id

        Endpoint: GET:/v1/User
        """
        global citizen3_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen3_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        citizen3_id = response['data']['id']

    @order
    def test_user_get_citizen3_id_with_citizen2(self):
        """
        Testing getting Citizen3's id with Citizen2

        Endpoint: GET:/v1/User/{userId}
        """
        response = get(f'{BASE_URL}v1/User/{citizen3_id}', headers=auth(citizen2_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_user_get_citizen_info_as_guardian(self):
        """
        Testing getting citizen info as guardian

        Endpoint: GET:/v1/User/{userId}
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}', headers=auth(guardian_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['username'], citizen2_username)

    @order
    def test_user_set_display_name(self):
        """
        Testing setting display name

        Endpoint: PUT:/v1/User/{userId}
        """
        data = {'username': citizen2_username, 'screenName': 'FBI Surveillance Van'}
        response = put(f'{BASE_URL}v1/User/{citizen2_id}', headers=auth(citizen2_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_get_display_name(self):
        """
        Testing ensuring display name is updated

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen2_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['screenName'], 'FBI Surveillance Van')

    @order
    def test_user_add_new_pictogram_as_citizen2(self):
        """
        Testing adding new pictogram as Citizen2

        Endpoint: Post:/v1/Pictogram
        """
        global wednesday_id
        data = {'accessLevel': 3, 'title': 'wednesday', 'id': 5, 'lastEdit': '2018-03-19T10:40:26.587Z'}
        response = post(f'{BASE_URL}v1/Pictogram', headers=auth(citizen2_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        wednesday_id = response['data']['id']

    @order
    def test_user_add_pictogram_to_citizen3_as_citizen2(self):
        """
        Testing adding pictogram to Citizen3 as Citizen2

        Endpoint: POST:/v1/User/{id}/resource
        """
        data = {'id': wednesday_id}
        response = post(f'{BASE_URL}v1/User/{citizen3_id}/resource', headers=auth(citizen2_token), json=data).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_user_get_settings(self):
        """
        Testing getting user settings

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(citizen2_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])

    @order
    def test_user_enable_grayscale_theme(self):
        """
        Testing setting grayscale theme

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.GRAYSCALE_THEME).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_check_theme(self):
        """
        Testing ensuring theme has been updated

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.GRAYSCALE_THEME).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['theme'], self.GRAYSCALE_THEME['theme'])

    @order
    def test_user_set_default_countdown_time(self):
        """
        Testing setting default countdown time

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.TIMER_ONE_HOUR).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_check_countdown_timer(self):
        """
        Testing ensuring countdown timer has been updated

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['timerSeconds'], self.TIMER_ONE_HOUR['timerSeconds'])

    @order
    def test_user_set_multiple_settings(self):
        """
        Testing setting multiple settings at once

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.MULTIPLE_SETTINGS).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_check_multiple_settings(self):
        """
        Testing ensuring settings have been updated

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2, response['data']['orientation'])
        self.assertEqual(2, response['data']['completeMark'])
        self.assertEqual(1, response['data']['cancelMark'])
        self.assertEqual(2, response['data']['defaultTimer'])
        self.assertEqual(60, response['data']['timerSeconds'])
        self.assertEqual(3, response['data']['activitiesCount'])
        self.assertEqual(3, response['data']['theme'])
        self.assertEqual(2, response['data']['nrOfDaysToDisplay'])
        self.assertTrue(True, response['data']['greyScale'])
        self.assertEqual("#FF00FF", response['data']['weekDayColors'][0]['hexColor'])
        self.assertEqual(1, response['data']['weekDayColors'][0]['day'])

    @order
    def test_user_get_citizen1_citizens(self):
        """
        Testing getting Citizen1's citizens

        Endpoint: GET:/v1/User/{id}/citizens
        """
        response = get(f'{BASE_URL}v1/User/{citizen1_id}/citizens', headers=auth(citizen1_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_user_get_guardian_citizens(self):
        """
        Testing getting Guardian's citizens

        Endpoint: GET:/v1/User/{id}/citizens
        """
        response = get(f'{BASE_URL}v1/User/{guardian_id}/citizens', headers=auth(guardian_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x['userName'] == 'Kurt' for x in response['data']))

    @order
    def test_user_get_citizen1_guardians(self):
        """
        Testing getting Citizen1's guardians

        Endpoint: GET:/v1/User/{id}/guardians
        """
        response = get(f'{BASE_URL}v1/User/{citizen1_id}/guardians', headers=auth(citizen1_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x['userName'] == 'Graatand' for x in response['data']))

    @order
    def test_user_get_guardian_guardians(self):
        """
        Testing getting Guardian's guardians

        Endpoint: GET:/v1/User/{id}/guardians
        """
        response = get(f'{BASE_URL}v1/User/{guardian_id}/guardians', headers=auth(guardian_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'Forbidden')

    @order
    def test_user_change_settings_as_citizen(self):
        """
        Testing changing settings as citizen

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen1_id}/settings', headers=auth(citizen1_token),
                       json=self.MULTIPLE_SETTINGS).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_user_change_settings_as_super_user(self):
        """
        Testing changing settings as super user

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(super_user_token),
                       json=self.NEW_SETTINGS[0]).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(1, response['data']['theme'])

    @order
    def test_user_change_settings_as_department(self):
        """
        Testing changing settings as department

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(department_token),
                       json=self.NEW_SETTINGS[1]).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2, response['data']['theme'])
