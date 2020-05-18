from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase
from unittest import skip
from http import HTTPStatus

citizen1_token = ''
citizen1_id = ''
guardian_token = ''
guardian_id = ''
new_guardian_token = ''
new_guardian_id = ''
new_guardian_username = ''
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
                               'greyScale': True, 'lockTimerControl': True, 'pictogramText': True, 'weekDayColors':
                                   [{"hexColor": "#08a045", "day": 1}, {"hexColor": "#540d6e", "day": 2},
                                    {"hexColor": "#f77f00", "day": 3}, {"hexColor": "#004777", "day": 4},
                                    {"hexColor": "#f9c80e", "day": 5}, {"hexColor": "#db2b39", "day": 6},
                                    {"hexColor": "#ffffff", "day": 7}]}
        cls.TIMER_ONE_HOUR = {"orientation": 1, "completeMark": 2, "cancelMark": 2, "defaultTimer": 2,
                              "timerSeconds": 3600, "activitiesCount": None, "theme": 1, "colorThemeWeekSchedules": 1,
                              "nrOfDaysToDisplay": 4, "greyScale": True, 'lockTimerControl': True,
                              'pictogramText': True, "weekDayColors":
                                  [{"hexColor": "#08A045", "day": 1}, {"hexColor": "#540D6E", "day": 2},
                                   {"hexColor": "#F77F00", "day": 3}, {"hexColor": "#004777", "day": 4},
                                   {"hexColor": "#F9C80E", "day": 5}, {"hexColor": "#DB2B39", "day": 6},
                                   {"hexColor": "#FFFFFF", "day": 7}]}
        cls.MULTIPLE_SETTINGS = {"orientation": 2, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2,
                                 "timerSeconds": 60, "activitiesCount": 3, "theme": 3, "nrOfDaysToDisplay": 2,
                                 "greyScale": True, 'lockTimerControl': True, 'pictogramText': True,
                                 "weekDayColors":
                                     [{"hexColor": "#FF00FF", "day": 1}, {"hexColor": "#540D6E", "day": 2},
                                      {"hexColor": "#F77F00", "day": 3}, {"hexColor": "#004777", "day": 4},
                                      {"hexColor": "#F9C80E", "day": 5}, {"hexColor": "#DB2B39", "day": 6},
                                      {"hexColor": "#FFFFFF", "day": 7}]}
        cls.NEW_SETTINGS = [{"orientation": 1, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2, "theme": 1},
                            {"orientation": 1, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2, "theme": 2}]
        cls.RAW_IMAGE = 'ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿñÇÿÿõ×ÿÿñÇÿÿÿÿÿÿÿ' \
                        'ÿÿÿÿÿÿÿøÿÿüÿÿþ?ÿÿü?ÿÿøÿÿøÿÿà?ÿÿÿ'

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestUserController, cls).tearDownClass()

    @order
    def test_user_can_login_as_citizen1(self):
        """
        Testing logging in as Citizen1
        Endpoint: POST:/v1/Account/login
        """
        global citizen1_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        citizen1_token = response_body['data']

    @order
    def test_user_can_login_as_guardian(self):
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
    def test_user_can_login_as_super_user(self):
        """
        Testing logging in as Super User
        Endpoint: POST:/v1/Account/login
        """
        global super_user_token
        data = {'username': 'Lee', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        super_user_token = response_body['data']

    @order
    def test_user_can_create_new_guardian(self):
        """
        Creating a new guardian with no relations to test other endpoints
        Endpoint: POST:/v1/Account/register
        """
        global new_guardian_id
        global new_guardian_username
        new_guardian_username = f'Testguardian{time.time()}'
        data = {'username': new_guardian_username, 'username': new_guardian_username, 'password': 'password', 'displayName': 'testG','departmentId': 2, 'role': 'Guardian'}
        response = post(f'{BASE_URL}v1/Account/register', json=data, headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

        self.assertIsNotNone(response_body['data'])
        new_guardian_id = response_body['data']['id']

    @order
    def test_user_can_login_as_new_guardian(self):
        """
        Testing logging in as Guardian
        Endpoint: POST:/v1/Account/login
        """
        global new_guardian_token
        data = {'username': new_guardian_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        new_guardian_token = response_body['data']

    @order
    def test_user_can_login_as_department(self):
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
    def test_user_can_get_citizen1_id(self):
        """
        Testing getting Citizen1's id
        Endpoint: GET:/v1/User
        """
        global citizen1_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data']['id'])
        citizen1_id = response_body['data']['id']

    @order
    def test_user_can_get_guardian_id(self):
        """
        Testing getting Guardian's id
        Endpoint: GET:/v1/User
        """
        global guardian_id
        response = get(f'{BASE_URL}v1/User', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])
        guardian_id = response_body['data']['id']

    @order
    def test_user_can_register_citizen2(self):
        """
        Testing registering Citizen2
        Endpoint: POST:/v1/Account/register
        """
        data = {'username': citizen2_username, 'displayname': citizen2_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 2}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(super_user_token), json=data)
        self.assertEqual(response.status_code, HTTPStatus.CREATED)


    @order
    def test_user_can_login_as_citizen2(self):
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
    def test_user_can_get_citizen2_id(self):
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
    def test_user_can_register_citizen3(self):
        """
        Testing registering Citizen3
        Endpoint: POST:/v1/Account/register
        """
        data = {'username': citizen3_username, 'displayname': citizen3_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 2}
        response = post(f'{BASE_URL}v1/Account/register', headers=auth(super_user_token), json=data)
        self.assertEqual(response.status_code, HTTPStatus.CREATED)


    @order
    def test_user_can_login_as_citizen3(self):
        """
        Testing logging in as Citizen3
        Endpoint: POST:/v1/Account/login
        """
        global citizen3_token
        data = {'username': citizen3_username, 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        citizen3_token = response_body['data']

    @order
    def test_user_can_get_citizen3_id(self):
        """
        Testing getting Citizen3's id
        Endpoint: GET:/v1/User
        """
        global citizen3_id
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen3_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])
        citizen3_id = response_body['data']['id']

    @order
    def test_user_can_get_citizen3_id_with_citizen2_should_fail(self):
        """
        Testing getting Citizen3's id with Citizen2
        Endpoint: GET:/v1/User/{id}
        """
        response = get(f'{BASE_URL}v1/User/{citizen3_id}', headers=auth(citizen2_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_user_can_get_citizen_info_as_guardian(self):
        """
        Testing getting citizen info as guardian
        Endpoint: GET:/v1/User/{id}
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(response_body['data']['username'], citizen2_username)

    @order
    def test_user_can_set_display_name(self):
        """
        Testing setting display name
        Endpoint: PUT:/v1/User/{id}
        """
        data = {'username': citizen2_username, 'displayName': 'FBI Surveillance Van'}
        response = put(f'{BASE_URL}v1/User/{citizen2_id}', headers=auth(citizen2_token), json=data)

        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_user_can_get_display_name(self):
        """
        Testing ensuring display name is updated
        Endpoint: GET:/v1/User
        """
        response = get(f'{BASE_URL}v1/User', headers=auth(citizen2_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(response_body['data']['displayName'], 'FBI Surveillance Van')

    @order
    def test_user_can_add_new_pictogram_as_citizen2(self):
        """
        Testing adding new pictogram as Citizen2
        Endpoint: POST:/v1/Pictogram
        """
        global wednesday_id
        data = {'accessLevel': 3, 'title': 'wednesday', 'id': 5, 'lastEdit': '2018-03-19T10:40:26.587Z'}
        response = post(f'{BASE_URL}v1/Pictogram', headers=auth(citizen2_token), json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])
        wednesday_id = response_body['data']['id']

    @order
    def test_user_can_add_pictogram_to_citizen3_as_citizen2(self):
        """
        Testing adding pictogram to Citizen3 as Citizen2
        Endpoint: POST:/v1/User/{id}/resource
        """
        data = {'id': wednesday_id}
        response = post(f'{BASE_URL}v1/User/{citizen3_id}/resource', headers=auth(citizen2_token), json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_user_can_get_settings(self):
        """
        Testing getting user settings
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(citizen2_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

    @order
    def test_user_can_enable_grayscale_theme(self):
        """
        Testing setting grayscale theme
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.GRAYSCALE_THEME)

        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_user_can_check_theme(self):
        """
        Testing ensuring theme has been updated
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.GRAYSCALE_THEME)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(response_body['data']['theme'], self.GRAYSCALE_THEME['theme'])

    @order
    def test_user_can_set_default_countdown_time(self):
        """
        Testing setting default countdown time
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.TIMER_ONE_HOUR)

        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_user_can_check_countdown_timer(self):
        """
        Testing ensuring countdown timer has been updated
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(response_body['data']['timerSeconds'], self.TIMER_ONE_HOUR['timerSeconds'])

    @order
    def test_user_can_set_multiple_settings(self):
        """
        Testing setting multiple settings at once
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token),
                       json=self.MULTIPLE_SETTINGS)

        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_user_can_check_multiple_settings(self):
        """
        Testing ensuring settings have been updated
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(2, response_body['data']['orientation'])
        self.assertEqual(2, response_body['data']['completeMark'])
        self.assertEqual(1, response_body['data']['cancelMark'])
        self.assertEqual(2, response_body['data']['defaultTimer'])
        self.assertEqual(60, response_body['data']['timerSeconds'])
        self.assertEqual(3, response_body['data']['activitiesCount'])
        self.assertEqual(3, response_body['data']['theme'])
        self.assertEqual(2, response_body['data']['nrOfDaysToDisplay'])
        self.assertTrue(response_body['data']['greyScale'])
        self.assertTrue(response_body['data']['lockTimerControl'])
        self.assertTrue(response_body['data']['pictogramText'])
        self.assertEqual("#FF00FF", response_body['data']['weekDayColors'][0]['hexColor'])
        self.assertEqual(1, response_body['data']['weekDayColors'][0]['day'])

    @order
    def test_user_can_get_citizen1_citizens_should_fail(self):
        """
        Testing getting Citizen1's citizens
        Endpoint: GET:/v1/User/{userId}/citizens
        """
        response = get(f'{BASE_URL}v1/User/{citizen1_id}/citizens', headers=auth(citizen1_token))
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'Forbidden')



    @order
    def test_user_can_get_guardian_citizens(self):
        """
        Testing getting Guardian's citizens
        Endpoint: GET:/v1/User/{userId}/citizens
        """
        response = get(f'{BASE_URL}v1/User/{guardian_id}/citizens', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertTrue(any(x['displayName'] == 'Kurt Andersen' for x in response_body['data']))

    @order
    def test_user_can_get_citizen1_guardians(self):
        """
        Testing getting Citizen1's guardians
        Endpoint: GET:/v1/User/{userId}/guardians
        """
        response = get(f'{BASE_URL}v1/User/{citizen1_id}/guardians', headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertTrue(any(x['displayName'] == 'Harald Graatand' for x in response_body['data']))

    @order
    def test_user_can_get_guardian_guardians_should_fail(self):
        """
        Testing getting Guardian's guardians
        Endpoint: GET:/v1/User/{userId}/guardians
        """
        response = get(f'{BASE_URL}v1/User/{guardian_id}/guardians', headers=auth(guardian_token))
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'Forbidden')

    @order
    def test_user_can_change_guardian_settings_should_fail(self):
        """
        Testing changing Guardian settings
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASE_URL}v1/User/{guardian_id}/settings', headers=auth(guardian_token))
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'RoleMustBeCitizien')

    @order
    def test_user_can_change_settings_as_citizen_should_fail(self):
        """
        Testing changing settings as citizen
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen1_id}/settings', headers=auth(citizen1_token),
                       json=self.MULTIPLE_SETTINGS)
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'Forbidden')


    @order
    def test_user_can_change_settings_as_super_user(self):
        """
        Testing changing settings as super user
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(super_user_token),
                       json=self.NEW_SETTINGS[0])
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(1, response_body['data']['theme'])

    @order
    def test_user_can_change_settings_as_department(self):
        """
        Testing changing settings as department
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/settings', headers=auth(department_token),
                       json=self.NEW_SETTINGS[1])
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(2, response_body['data']['theme'])

    @order
    def test_user_can_change_guardian_settings_as_super_user_should_fail(self):
        """
        Testing changing Guardian settings as Super User
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASE_URL}v1/User/{guardian_id}/settings', headers=auth(super_user_token),
                       json=self.NEW_SETTINGS[1])
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'RoleMustBeCitizien')

    @order
    def test_user_superuser_can_give_citizen_icon(self):
        """
        Testing if a superuser can set a users icon
        Endpoint: PUT:/v1/User/{id}/icon
        """
        data = {'userIcon': self.RAW_IMAGE}
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/icon', json=data, headers=auth(super_user_token))

        self.assertEqual(response.status_code, HTTPStatus.OK)

    @order
    def test_user_user_can_set_icon_of_another_user_should_fail(self):
        """
        Testing if a user can change another users icon
        Endpoint: PUT:/v1/User/{id}/icon
        """
        data = {'userIcon': self.RAW_IMAGE}
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/icon', data=data, headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_user_guardian_can_set_icon_of_another_user(self):
        """
        Testing if a guardian can change another users icon
        Endpoint: PUT:/v1/User/{id}/icon
        """
        data = {'userIcon': self.RAW_IMAGE}
        response = put(f'{BASE_URL}v1/User/{citizen2_id}/icon', json=data, headers=auth(guardian_token))

        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_user_user_can_get_own_icon(self):
        """
        Testing if a user can get own userIcon
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/icon', headers=auth(citizen2_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])


    @order
    def test_user_user_can_get_specific_user_icon(self):
        """
        Testing if a user can get the userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/icon', headers=auth(citizen1_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

    @order
    def test_user_guardian_can_get_specific_user_icon(self):
        """
        Testing if a guardian can get the userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/icon', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

    @order
    def test_user_superuser_can_get_specific_user_icon(self):
        """
        Testing if a superuser can get the userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/icon', headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

    @order
    @skip("Skipping since its broken")
    def test_user_superuser_can_get_specific_user_icon_raw(self):
        """
        Testing if a superuser can get the raw userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon/raw
        """
        response = get(f'{BASE_URL}v1/User/{citizen2_id}/icon/raw', headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

    @order
    def test_user_superuser_can_delete_specific_user_icon(self):
        """
        Testing if a superuser can delete the userIcon of another user
        Endpoint: DELETE:/v1/User/{id}/icon
        """
        response = delete(f'{BASE_URL}v1/User/{citizen2_id}/icon', headers=auth(super_user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

    @order
    def test_user_guardian_add_relation_to_user(self):
        """
        Testing if guardian can add relation to existing citizen
        Endpoint: POST:/v1/User/{userId}/citizens/{citizenId}
        """
        response = post(f'{BASE_URL}v1/User/{new_guardian_id}/citizens/{citizen2_id}', headers=auth(new_guardian_token))
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_user_delete_new_guardian(self):
        """
        deleting newly testing guardian
        Endpoint: DELETE:/v1/Account/user/{userId}
        """
        response = delete(f'{BASE_URL}v1/Account/user/{new_guardian_id}', headers=auth(super_user_token))
        self.assertEqual(response.status_code, HTTPStatus.OK)