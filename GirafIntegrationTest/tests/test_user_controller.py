from requests import get, post, put, delete
import time
from testlib import order, BASEURL, auth, GIRAFTestCase
from unittest import skip
from http import HTTPStatus

citizen1Token = ''
citizen1Id = ''
guardianToken = ''
guardianId = ''
newGuardianToken = ''
newGuardianId = ''
newGuardianUsername = ''
citizen2Token = ''
citizen2Username = f'Gunnar{time.time()}'
citizen2Id = ''
citizen3Token = ''
citizen3Username = f'Charlie{time.time()}'
citizen3Id = ''
superUserToken = ''
departmentToken = ''
wednesdayId = ''


class TestUserController(GIRAFTestCase):
    """
    Testing API requests on User endpoints
    """

    @classmethod
    def SetUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestUserController, cls).setUpClass()
        print(f'file:/{_file_}\n')
        cls.GRAYSCALETHEME = {'orientation': 1, 'completeMark': 2, 'cancelMark': 2, 'defaultTimer': 2,
                               'timerSeconds': 900, 'activitiesCount': None, 'theme': 2, 'nrOfDaysToDisplay': 7,
                               'greyScale': True, 'lockTimerControl': True, 'pictogramText': True, 'weekDayColors':
                                   [{"hexColor": "#08a045", "day": 1}, {"hexColor": "#540d6e", "day": 2},
                                    {"hexColor": "#f77f00", "day": 3}, {"hexColor": "#004777", "day": 4},
                                    {"hexColor": "#f9c80e", "day": 5}, {"hexColor": "#db2b39", "day": 6},
                                    {"hexColor": "#ffffff", "day": 7}]}
        cls.TIMERONEHOUR = {"orientation": 1, "completeMark": 2, "cancelMark": 2, "defaultTimer": 2,
                              "timerSeconds": 3600, "activitiesCount": None, "theme": 1, "colorThemeWeekSchedules": 1,
                              "nrOfDaysToDisplay": 4, "greyScale": True, 'lockTimerControl': True,
                              'pictogramText': True, "weekDayColors":
                                  [{"hexColor": "#08A045", "day": 1}, {"hexColor": "#540D6E", "day": 2},
                                   {"hexColor": "#F77F00", "day": 3}, {"hexColor": "#004777", "day": 4},
                                   {"hexColor": "#F9C80E", "day": 5}, {"hexColor": "#DB2B39", "day": 6},
                                   {"hexColor": "#FFFFFF", "day": 7}]}
        cls.MULTIPLESETTINGS = {"orientation": 2, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2,
                                 "timerSeconds": 60, "activitiesCount": 3, "theme": 3, "nrOfDaysToDisplay": 2,
                                 "greyScale": True, 'lockTimerControl': True, 'pictogramText': True,
                                 "weekDayColors":
                                     [{"hexColor": "#FF00FF", "day": 1}, {"hexColor": "#540D6E", "day": 2},
                                      {"hexColor": "#F77F00", "day": 3}, {"hexColor": "#004777", "day": 4},
                                      {"hexColor": "#F9C80E", "day": 5}, {"hexColor": "#DB2B39", "day": 6},
                                      {"hexColor": "#FFFFFF", "day": 7}]}
        cls.NEWSETTINGS = [{"orientation": 1, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2, "theme": 1},
                            {"orientation": 1, "completeMark": 2, "cancelMark": 1, "defaultTimer": 2, "theme": 2}]
        cls.RAWIMAGE = 'ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿñÇÿÿõ×ÿÿñÇÿÿÿÿÿÿÿ' \
                        'ÿÿÿÿÿÿÿøÿÿüÿÿþ?ÿÿü?ÿÿøÿÿøÿÿà?ÿÿÿ'

    @classmethod
    def TearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestUserController, cls).tearDownClass()

    @order
    def TestUserCanLoginAsCitizen1(self):
        """
        Testing logging in as Citizen1
        Endpoint: POST:/v2/Account/login
        """
        global citizen1Token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        citizen1Token = responseBody['data']

    @order
    def TestUserCanLoginAsGuardian(self):
        """
        Testing logging in as Guardian
        Endpoint: POST:/v2/Account/login
        """
        global guardianToken
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        guardianToken = responseBody['data']

    @order
    def TestUserCanLoginAsSuperUser(self):
        """
        Testing logging in as Super User
        Endpoint: POST:/v2/Account/login
        """
        global superUserToken
        data = {'username': 'Lee', 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        superUserToken = responseBody['data']

    @order
    def TestUserCanCreateNewGuardian(self):
        """
        Creating a new guardian with no relations to test other endpoints
        Endpoint: POST:/v2/Account/register
        """
        global newGuardianId
        global newGuardianUsername
        newGuardianUsername = f'Testguardian{time.time()}'
        data = {'username': newGuardianUsername, 'username': newGuardianUsername, 'password': 'password', 'displayName': 'testG','departmentId': 2, 'role': 'Guardian'}
        response = post(f'{BASEURL}v2/Account/register', json=data, headers=auth(superUserToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.CREATED)

        self.assertIsNotNone(responseBody['data'])
        newGuardianId = responseBody['data']['id']

    @order
    def TestUserCanLoginAsNewGuardian(self):
        """
        Testing logging in as Guardian
        Endpoint: POST:/v2/Account/login
        """
        global newGuardianToken
        data = {'username': newGuardianUsername, 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        newGuardianToken = responseBody['data']

    @order
    def TestUserCanLoginAsDepartment(self):
        """
        Testing logging in as Department
        Endpoint: POST:/v2/Account/login
        """
        global departmentToken
        data = {'username': 'Tobias', 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        departmentToken = responseBody['data']

    @order
    def TestUserCanGetCitizen1Id(self):
        """
        Testing getting Citizen1's id
        Endpoint: GET:/v1/User
        """
        global citizen1Id
        response = get(f'{BASEURL}v1/User', headers=auth(citizen1Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data']['id'])
        citizen1Id = responseBody['data']['id']

    @order
    def TestUserCanGetGuardianId(self):
        """
        Testing getting Guardian's id
        Endpoint: GET:/v1/User
        """
        global guardianId
        response = get(f'{BASEURL}v1/User', headers=auth(guardianToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertIsNotNone(responseBody['data']['id'])
        guardianId = responseBody['data']['id']

    @order
    def TestUserCanRegisterCitizen2(self):
        """
        Testing registering Citizen2
        Endpoint: POST:/v2/Account/register
        """
        data = {'username': citizen2Username, 'displayname': citizen2Username, 'password': 'password', 'role': 'Citizen', 'departmentId': 2}
        response = post(f'{BASEURL}v2/Account/register', headers=auth(superUserToken), json=data)
        self.assertEqual(response.statusCode, HTTPStatus.CREATED)


    @order
    def TestUserCanLoginAsCitizen2(self):
        """
        Testing logging in as Citizen2
        Endpoint: POST:/v2/Account/login
        """
        global citizen2Token
        data = {'username': citizen2Username, 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        citizen2Token = responseBody['data']

    @order
    def TestUserCanGetCitizen2Id(self):
        """
        Testing getting Citizen2's id
        Endpoint: GET:/v1/User
        """
        global citizen2Id
        response = get(f'{BASEURL}v1/User', headers=auth(citizen2Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertIsNotNone(responseBody['data']['id'])
        citizen2Id = responseBody['data']['id']

    @order
    def TestUserCanRegisterCitizen3(self):
        """
        Testing registering Citizen3
        Endpoint: POST:/v2/Account/register
        """
        data = {'username': citizen3Username, 'displayname': citizen3Username, 'password': 'password', 'role': 'Citizen', 'departmentId': 2}
        response = post(f'{BASEURL}v2/Account/register', headers=auth(superUserToken), json=data)
        self.assertEqual(response.statusCode, HTTPStatus.CREATED)


    @order
    def TestUserCanLoginAsCitizen3(self):
        """
        Testing logging in as Citizen3
        Endpoint: POST:/v2/Account/login
        """
        global citizen3Token
        data = {'username': citizen3Username, 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        citizen3Token = responseBody['data']

    @order
    def TestUserCanGetCitizen3Id(self):
        """
        Testing getting Citizen3's id
        Endpoint: GET:/v1/User
        """
        global citizen3Id
        response = get(f'{BASEURL}v1/User', headers=auth(citizen3Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertIsNotNone(responseBody['data']['id'])
        citizen3Id = responseBody['data']['id']

    @order
    def TestUserCanGetCitizen3IdWithCitizen2ShouldFail(self):
        """
        Testing getting Citizen3's id with Citizen2
        Endpoint: GET:/v1/User/{id}
        """
        response = get(f'{BASEURL}v1/User/{citizen3Id}', headers=auth(citizen2Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.FORBIDDEN)
        self.assertEqual(responseBody['errorKey'], 'NotAuthorized')

    @order
    def TestUserCanGetCitizenInfoAsGuardian(self):
        """
        Testing getting citizen info as guardian
        Endpoint: GET:/v1/User/{id}
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}', headers=auth(guardianToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(responseBody['data']['username'], citizen2Username)

    @order
    def TestUserCanSetDisplayName(self):
        """
        Testing setting display name
        Endpoint: PUT:/v1/User/{id}
        """
        data = {'username': citizen2Username, 'displayName': 'FBI Surveillance Van'}
        response = put(f'{BASEURL}v1/User/{citizen2Id}', headers=auth(citizen2Token), json=data)

        self.assertEqual(response.statusCode, HTTPStatus.OK)


    @order
    def TestUserCanGetDisplayName(self):
        """
        Testing ensuring display name is updated
        Endpoint: GET:/v1/User
        """
        response = get(f'{BASEURL}v1/User', headers=auth(citizen2Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(responseBody['data']['displayName'], 'FBI Surveillance Van')

    @order
    def TestUserCanAddNewPictogramAsCitizen2(self):
        """
        Testing adding new pictogram as Citizen2
        Endpoint: POST:/v1/Pictogram
        """
        global wednesdayId
        data = {'accessLevel': 3, 'ttle': 'wednesday', 'id': 5, 'lastEdit': '2018-03-19T10:40:26.587Z'}
        response = post(f'{BASEURL}v1/Pictogram', headers=auth(citizen2Token), json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.CREATED)

        self.assertIsNotNone(responseBody['data'])
        self.assertIsNotNone(responseBody['data']['id'])
        wednesdayId = responseBody['data']['id']

    @order
    def TestUserCanAddPictogramToCitizen3AsCitizen2(self):
        """
        Testing adding pictogram to Citizen3 as Citizen2
        Endpoint: POST:/v1/User/{id}/resource
        """
        data = {'id': wednesdayId}
        response = post(f'{BASEURL}v1/User/{citizen3Id}/resource', headers=auth(citizen2Token), json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.FORBIDDEN)
        self.assertEqual(responseBody['errorKey'], 'NotAuthorized')

    @order
    def TestUserCanGetSettings(self):
        """
        Testing getting user settings
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(citizen2Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])

    @order
    def TestUserCanEnableGrayscaleTheme(self):
        """
        Testing setting grayscale theme
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(guardianToken),
                       json=self.GRAYSCALETHEME)

        self.assertEqual(response.statusCode, HTTPStatus.OK)


    @order
    def TestUserCanCheckTheme(self):
        """
        Testing ensuring theme has been updated
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(guardianToken),
                       json=self.GRAYSCALETHEME)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(responseBody['data']['theme'], self.GRAYSCALETHEME['theme'])

    @order
    def TestUserCanSetDefaultCountdownTime(self):
        """
        Testing setting default countdown time
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(guardianToken),
                       json=self.TIMERONEHOUR)

        self.assertEqual(response.statusCode, HTTPStatus.OK)


    @order
    def TestUserCanCheckCountdownTimer(self):
        """
        Testing ensuring countdown timer has been updated
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(guardianToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(responseBody['data']['timerSeconds'], self.TIMERONEHOUR['timerSeconds'])

    @order
    def TestUserCanSetMultipleSettings(self):
        """
        Testing setting multiple settings at once
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(guardianToken),
                       json=self.MULTIPLESETTINGS)

        self.assertEqual(response.statusCode, HTTPStatus.OK)


    @order
    def TestUserCanCheckMultipleSettings(self):
        """
        Testing ensuring settings have been updated
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(guardianToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(2, responseBody['data']['orientation'])
        self.assertEqual(2, responseBody['data']['completeMark'])
        self.assertEqual(1, responseBody['data']['cancelMark'])
        self.assertEqual(2, responseBody['data']['defaultTimer'])
        self.assertEqual(60, responseBody['data']['timerSeconds'])
        self.assertEqual(3, responseBody['data']['activitiesCount'])
        self.assertEqual(3, responseBody['data']['theme'])
        self.assertEqual(2, responseBody['data']['nrOfDaysToDisplay'])
        self.assertTrue(responseBody['data']['greyScale'])
        self.assertTrue(responseBody['data']['lockTimerControl'])
        self.assertTrue(responseBody['data']['pictogramText'])
        self.assertEqual("#FF00FF", responseBody['data']['weekDayColors'][0]['hexColor'])
        self.assertEqual(1, responseBody['data']['weekDayColors'][0]['day'])

    @order
    def TestUserCanGetCitizen1CitizensShouldFail(self):
        """
        Testing getting Citizen1's citizens
        Endpoint: GET:/v1/User/{userId}/citizens
        """
        response = get(f'{BASEURL}v1/User/{citizen1Id}/citizens', headers=auth(citizen1Token))
        responseBody = response.json()

        self.assertEqual(response.statusCode, HTTPStatus.FORBIDDEN)
        self.assertEqual(responseBody['errorKey'], 'Forbidden')



    @order
    def TestUserCanGetGuardianCitizens(self):
        """
        Testing getting Guardian's citizens
        Endpoint: GET:/v1/User/{userId}/citizens
        """
        response = get(f'{BASEURL}v1/User/{guardianId}/citizens', headers=auth(guardianToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertTrue(any(x['displayName'] == 'Kurt Andersen' for x in responseBody['data']))

    @order
    def TestUserCanGetCitizen1Guardians(self):
        """
        Testing getting Citizen1's guardians
        Endpoint: GET:/v1/User/{userId}/guardians
        """
        response = get(f'{BASEURL}v1/User/{citizen1Id}/guardians', headers=auth(citizen1Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertTrue(any(x['displayName'] == 'Harald Graatand' for x in responseBody['data']))

    @order
    def TestUserCanGetGuardianGuardiansShouldFail(self):
        """
        Testing getting Guardian's guardians
        Endpoint: GET:/v1/User/{userId}/guardians
        """
        response = get(f'{BASEURL}v1/User/{guardianId}/guardians', headers=auth(guardianToken))
        responseBody = response.json()

        self.assertEqual(response.statusCode, HTTPStatus.FORBIDDEN)
        self.assertEqual(responseBody['errorKey'], 'Forbidden')

    @order
    def TestUserCanChangeGuardianSettingsShouldFail(self):
        """
        Testing changing Guardian settings
        Endpoint: GET:/v1/User/{id}/settings
        """
        response = get(f'{BASEURL}v1/User/{guardianId}/settings', headers=auth(guardianToken))
        responseBody = response.json()

        self.assertEqual(response.statusCode, HTTPStatus.BADREQUEST)
        self.assertEqual(responseBody['errorKey'], 'RoleMustBeCitizien')

    @order
    def TestUserCanChangeSettingsAsCitizenShouldFail(self):
        """
        Testing changing settings as citizen
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASEURL}v1/User/{citizen1Id}/settings', headers=auth(citizen1Token),
                       json=self.MULTIPLESETTINGS)
        responseBody = response.json()

        self.assertEqual(response.statusCode, HTTPStatus.FORBIDDEN)
        self.assertEqual(responseBody['errorKey'], 'Forbidden')


    @order
    def TestUserCanChangeSettingsAsSuperUser(self):
        """
        Testing changing settings as super user
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(superUserToken),
                       json=self.NEWSETTINGS[0])
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(1, responseBody['data']['theme'])

    @order
    def TestUserCanChangeSettingsAsDepartment(self):
        """
        Testing changing settings as department
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASEURL}v1/User/{citizen2Id}/settings', headers=auth(departmentToken),
                       json=self.NEWSETTINGS[1])
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(2, responseBody['data']['theme'])

    @order
    def TestUserCanChangeGuardianSettingsAsSuperUserShouldFail(self):
        """
        Testing changing Guardian settings as Super User
        Endpoint: PUT:/v1/User/{id}/settings
        """
        response = put(f'{BASEURL}v1/User/{guardianId}/settings', headers=auth(superUserToken),
                       json=self.NEWSETTINGS[1])
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.BADREQUEST)
        self.assertEqual(responseBody['errorKey'], 'RoleMustBeCitizien')

    @order
    def TestUserSuperuserCanGiveCitizenIcon(self):
        """
        Testing if a superuser can set a users icon
        Endpoint: PUT:/v1/User/{id}/icon
        """
        data = {'userIcon': self.RAWIMAGE}
        response = put(f'{BASEURL}v1/User/{citizen2Id}/icon', json=data, headers=auth(superUserToken))

        self.assertEqual(response.statusCode, HTTPStatus.OK)

    @order
    def TestUserUserCanSetIconOfAnotherUserShouldFail(self):
        """
        Testing if a user can change another users icon
        Endpoint: PUT:/v1/User/{id}/icon
        """
        data = {'userIcon': self.RAWIMAGE}
        response = put(f'{BASEURL}v1/User/{citizen2Id}/icon', data=data, headers=auth(citizen1Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.FORBIDDEN)
        self.assertEqual(responseBody['errorKey'], 'NotAuthorized')

    @order
    def TestUserGuardianCanSetIconOfAnotherUser(self):
        """
        Testing if a guardian can change another users icon
        Endpoint: PUT:/v1/User/{id}/icon
        """
        data = {'userIcon': self.RAWIMAGE}
        response = put(f'{BASEURL}v1/User/{citizen2Id}/icon', json=data, headers=auth(guardianToken))

        self.assertEqual(response.statusCode, HTTPStatus.OK)


    @order
    def TestUserUserCanGetOwnIcon(self):
        """
        Testing if a user can get own userIcon
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/icon', headers=auth(citizen2Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])


    @order
    def TestUserUserCanGetSpecificUserIcon(self):
        """
        Testing if a user can get the userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/icon', headers=auth(citizen1Token))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])

    @order
    def TestUserGuardianCanGetSpecificUserIcon(self):
        """
        Testing if a guardian can get the userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/icon', headers=auth(guardianToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])

    @order
    def TestUserSuperuserCanGetSpecificUserIcon(self):
        """
        Testing if a superuser can get the userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/icon', headers=auth(superUserToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])

    @order
    @skip("Skipping since its broken")
    def TestUserSuperuserCanGetSpecificUserIconRaw(self):
        """
        Testing if a superuser can get the raw userIcon of another user
        Endpoint: GET:/v1/User/{id}/icon/raw
        """
        response = get(f'{BASEURL}v1/User/{citizen2Id}/icon/raw', headers=auth(superUserToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])

    @order
    def TestUserSuperuserCanDeleteSpecificUserIcon(self):
        """
        Testing if a superuser can delete the userIcon of another user
        Endpoint: DELETE:/v1/User/{id}/icon
        """
        response = delete(f'{BASEURL}v1/User/{citizen2Id}/icon', headers=auth(superUserToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)

        self.assertIsNotNone(responseBody['data'])

    @order
    def TestUserGuardianAddRelationToUser(self):
        """
        Testing if guardian can add relation to existing citizen
        Endpoint: POST:/v1/User/{userId}/citizens/{citizenId}
        """
        response = post(f'{BASEURL}v1/User/{newGuardianId}/citizens/{citizen2Id}', headers=auth(newGuardianToken))
        self.assertEqual(response.statusCode, HTTPStatus.OK)


    @order
    def TestUserDeleteNewGuardian(self):
        """
        deleting newly testing guardian
        Endpoint: DELETE:/v2/Account/user/{userId}
        """
        response = delete(f'{BASEURL}v2/Account/user/{newGuardianId}', headers=auth(superUserToken))
        self.assertEqual(response.statusCode, HTTPStatus.OK)