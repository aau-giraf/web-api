from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase

kurt_token = ''
kurt_id = ''
graatand_token = ''
graatand_id = ''
gunnar_token = ''
gunnar_username = f'Gunnar{time.time()}'
gunnar_id = ''
charlie_token = ''
charlie_username = f'Charlie{time.time()}'
charlie_id = ''
lee_token = ''
tobias_token = ''
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
    def test_user_login_as_kurt(self):
        """
        Testing logging in as Kurt

        Endpoint: POST:/v1/Account/login
        """
        global kurt_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        kurt_token = response['data']

    @order
    def test_user_login_as_graatand(self):
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
    def test_user_login_as_lee(self):
        """
        Testing logging in as Lee

        Endpoint: POST:/v1/Account/login
        """
        global lee_token
        data = {'username': 'Lee', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        lee_token = response['data']

    @order
    def test_user_login_as_tobias(self):
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
    def test_user_get_kurt_id(self):
        """
        Testing getting Kurt's id

        Endpoint: GET:/v1/User
        """
        global kurt_id
        response = get(f'{BASE_URL}v1/User', headers=auth(kurt_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        kurt_id = response['data']['id']

    @order
    def test_user_get_graatand_id(self):
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
    def test_user_register_gunnar(self):
        """
        Testing registering Gunnar

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': gunnar_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(lee_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_login_as_gunnar(self):
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
    def test_user_get_gunnar_id(self):
        """
        Testing getting Gunnar's id

        Endpoint: GET:/v1/User
        """
        global gunnar_id
        response = get(f'{BASE_URL}v1/User', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        gunnar_id = response['data']['id']

    @order
    def test_user_register_charlie(self):
        """
        Testing registering Charlie

        Endpoint: POST:/v1/Account/register
        """
        data = {'username': charlie_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(lee_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_login_as_charlie(self):
        """
        Testing logging in as Charlie

        Endpoint: POST:/v1/Account/login
        """
        global charlie_token
        data = {'username': charlie_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        charlie_token = response['data']

    @order
    def test_user_get_charlie_id(self):
        """
        Testing getting Charlie's id

        Endpoint: GET:/v1/User
        """
        global charlie_id
        response = get(f'{BASE_URL}v1/User', headers=auth(charlie_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        charlie_id = response['data']['id']

    @order
    def test_user_get_charlie_id_with_gunnar(self):
        """
        Testing getting Charlie's id with Gunnar

        Endpoint: GET:/v1/User/{userId}
        """
        response = get(f'{BASE_URL}v1/User/{charlie_id}', headers=auth(gunnar_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_user_get_citizen_info_as_guardian(self):
        """
        Testing getting citizen info as guardian

        Endpoint: GET:/v1/User/{userId}
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}', headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['username'], gunnar_username)

    @order
    def test_user_set_display_name(self):
        """
        Testing setting display name

        Endpoint: PUT:/v1/User/{userId}
        """
        data = {'username': gunnar_username, 'screenName': 'FBI Surveillance Van'}
        response = put(f'{BASE_URL}v1/User/{gunnar_id}', headers=auth(gunnar_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_get_display_name(self):
        """
        Testing ensuring display name is updated

        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(response['data']['screenName'], 'FBI Surveillance Van')

    @order
    def test_user_add_new_pictogram_as_gunnar(self):
        """
        Testing adding new pictogram as Gunnar

        Endpoint: Post:/v1/Pictogram
        """
        global wednesday_id
        data = {'accessLevel': 3, 'title': 'wednesday', 'id': 5, 'lastEdit': '2018-03-19T10:40:26.587Z'}
        response = post(f'{BASE_URL}v1/Pictogram', headers=auth(gunnar_token), json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data']['id'])
        wednesday_id = response['data']['id']

    @order
    def test_user_add_pictogram_to_charlie_as_gunnar(self):
        """
        Testing adding pictogram to Charlie as Gunnar

        Endpoint: POST:/v1/User/{id}/resource
        """
        data = {'id': wednesday_id}
        response = post(f'{BASE_URL}v1/User/{charlie_id}/resource', headers=auth(gunnar_token), json=data).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotAuthorized')

    @order
    def test_user_get_settings(self):
        """
        Testing getting user settings

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(gunnar_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])

    @order
    def test_user_enable_grayscale_theme(self):
        """
        Testing setting grayscale theme

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(graatand_token),
                       json=self.GRAYSCALE_THEME).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_check_theme(self):
        """
        Testing ensuring theme has been updated

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(graatand_token),
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
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(graatand_token),
                       json=self.TIMER_ONE_HOUR).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_check_countdown_timer(self):
        """
        Testing ensuring countdown timer has been updated

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(graatand_token)).json()
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
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(graatand_token),
                       json=self.MULTIPLE_SETTINGS).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_user_check_multiple_settings(self):
        """
        Testing ensuring settings have been updated

        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(graatand_token)).json()
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
    def test_user_get_kurt_citizens(self):
        """
        Testing getting Kurt's citizens

        Endpoint: GET:/v1/User/{id}/citizens
        """
        response = get(f'{BASE_URL}v1/User/{kurt_id}/citizens', headers=auth(kurt_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_user_get_graatand_citizens(self):
        """
        Testing getting Graatand's citizens

        Endpoint: GET:/v1/User/{id}/citizens
        """
        response = get(f'{BASE_URL}v1/User/{graatand_id}/citizens', headers=auth(graatand_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x['userName'] == 'Kurt' for x in response['data']))

    @order
    def test_user_get_kurt_guardians(self):
        """
        Testing getting Kurt's guardians

        Endpoint: GET:/v1/User/{id}/guardians
        """
        response = get(f'{BASE_URL}v1/User/{kurt_id}/guardians', headers=auth(kurt_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertTrue(any(x['userName'] == 'Graatand' for x in response['data']))

    @order
    def test_user_get_graatand_guardians(self):
        """
        Testing getting Graatand's guardians

        Endpoint: GET:/v1/User/{id}/guardians
        """
        response = get(f'{BASE_URL}v1/User/{graatand_id}/guardians', headers=auth(graatand_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'Forbidden')

    @order
    def test_user_change_settings_as_citizen(self):
        """
        Testing changing settings as citizen

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{kurt_id}/settings', headers=auth(kurt_token),
                       json=self.MULTIPLE_SETTINGS).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_user_change_settings_as_super_user(self):
        """
        Testing changing settings as super user

        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(lee_token),
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
        response = put(f'{BASE_URL}v1/User/{gunnar_id}/settings', headers=auth(tobias_token),
                       json=self.NEW_SETTINGS[1]).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(2, response['data']['theme'])
